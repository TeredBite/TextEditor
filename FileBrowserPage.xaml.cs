using System.Collections.ObjectModel;
using System.ComponentModel;
using Microsoft.Maui.Controls;

namespace TextEditor;

public partial class FileBrowserPage : ContentPage
{
    private string _currentPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    private FileItem _selectedItem;
    public ObservableCollection<FileItem> FileItems { get; set; } = new();

    public FileBrowserPage()
    {
        InitializeComponent();
        FileListView.ItemsSource = FileItems;
        LoadDirectory(_currentPath);
    }

    private void LoadDirectory(string path)
    {
        try
        {
            FileItems.Clear();
            _currentPath = path;
            PathEntry.Text = path;

            // Add parent directory entry
            var parentDir = Directory.GetParent(path);
            if (parentDir != null)
            {
                FileItems.Add(new FileItem
                {
                    Name = "..",
                    FullPath = parentDir.FullName,
                    Icon = "⬆️",
                    Type = "Parent Folder",
                    IsDirectory = true
                });
            }

            // Load directories
            var directories = Directory.GetDirectories(path);
            foreach (var dir in directories)
            {
                var dirInfo = new DirectoryInfo(dir);
                FileItems.Add(new FileItem
                {
                    Name = dirInfo.Name,
                    FullPath = dir,
                    Icon = "📁",
                    ModifiedDate = dirInfo.LastWriteTime.ToString("MM/dd/yyyy"),
                    Type = "Folder",
                    IsDirectory = true
                });
            }

            // Load files
            var files = Directory.GetFiles(path);
            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);
                FileItems.Add(new FileItem
                {
                    Name = fileInfo.Name,
                    FullPath = file,
                    Icon = GetFileIcon(fileInfo.Extension),
                    ModifiedDate = fileInfo.LastWriteTime.ToString("MM/dd/yyyy"),
                    Type = fileInfo.Extension.ToUpper() + " File",
                    Size = FormatFileSize(fileInfo.Length),
                    IsDirectory = false
                });
            }

            StatusLabel.Text = $"{directories.Length} folders, {files.Length} files";
        }
        catch (Exception ex)
        {
            StatusLabel.Text = $"Error: {ex.Message}";
        }
    }

    private string GetFileIcon(string extension)
    {
        return extension.ToLower() switch
        {
            ".txt" => "📄",
            ".doc" or ".docx" => "📝",
            ".pdf" => "📕",
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" => "🖼️",
            ".mp3" or ".wav" or ".ogg" => "🎵",
            ".mp4" or ".avi" or ".mkv" => "🎬",
            ".exe" or ".dll" => "⚙️",
            ".zip" or ".rar" or ".7z" => "📦",
            ".cs" or ".java" or ".cpp" or ".py" or ".js" or ".html" => "💻",
            _ => "📄"
        };
    }

    private string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }

    private void OnFileSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is FileItem item)
        {
            _selectedItem = item;
            if (item.IsDirectory)
            {
                LoadDirectory(item.FullPath);
            }
            else
            {
                StatusLabel.Text = $"Selected: {item.Name}";
            }
        }
    }

    private void OnBackClicked(object sender, EventArgs e)
    {
        var parent = Directory.GetParent(_currentPath);
        if (parent != null)
        {
            LoadDirectory(parent.FullName);
        }
    }

    private void OnHomeClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
    }

    private void OnDesktopClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
    }

    private void OnDocumentsClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
    }

    private void OnDownloadsClicked(object sender, EventArgs e)
    {
        LoadDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads"));
    }

    private void OnPicturesClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));
    }

    private void OnMusicClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));
    }

    private void OnVideosClicked(object sender, EventArgs e)
    {
        LoadDirectory(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
    }

    private void OnDriveCClicked(object sender, EventArgs e)
    {
        LoadDirectory("C:\\");
    }

    private void OnGoClicked(object sender, EventArgs e)
    {
        var path = PathEntry.Text;
        if (Directory.Exists(path))
        {
            LoadDirectory(path);
        }
        else
        {
            StatusLabel.Text = "Invalid path";
        }
    }

    private void OnPathChanged(object sender, TextChangedEventArgs e)
    {
        // Path can be edited manually
    }

    private async void OnNewFolderClicked(object sender, EventArgs e)
    {
        var result = await DisplayPromptAsync("New Folder", "Enter folder name:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            try
            {
                var newPath = Path.Combine(_currentPath, result);
                Directory.CreateDirectory(newPath);
                LoadDirectory(_currentPath);
                StatusLabel.Text = $"Folder '{result}' created";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async void OnNewFileClicked(object sender, EventArgs e)
    {
        var result = await DisplayPromptAsync("New File", "Enter file name:");
        if (!string.IsNullOrWhiteSpace(result))
        {
            try
            {
                var newPath = Path.Combine(_currentPath, result);
                File.Create(newPath).Dispose();
                LoadDirectory(_currentPath);
                StatusLabel.Text = $"File '{result}' created";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (_selectedItem == null)
        {
            await DisplayAlert("No Selection", "Please select a file or folder to delete", "OK");
            return;
        }

        var confirm = await DisplayAlert("Confirm Delete", 
            $"Are you sure you want to delete '{_selectedItem.Name}'?", 
            "Delete", "Cancel");
        
        if (confirm)
        {
            try
            {
                if (_selectedItem.IsDirectory)
                {
                    Directory.Delete(_selectedItem.FullPath, true);
                }
                else
                {
                    File.Delete(_selectedItem.FullPath);
                }
                LoadDirectory(_currentPath);
                StatusLabel.Text = $"'{_selectedItem.Name}' deleted";
                _selectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async void OnRenameClicked(object sender, EventArgs e)
    {
        if (_selectedItem == null)
        {
            await DisplayAlert("No Selection", "Please select a file or folder to rename", "OK");
            return;
        }

        var newName = await DisplayPromptAsync("Rename", "Enter new name:", initialValue: _selectedItem.Name);
        if (!string.IsNullOrWhiteSpace(newName) && newName != _selectedItem.Name)
        {
            try
            {
                var newPath = Path.Combine(_currentPath, newName);
                if (_selectedItem.IsDirectory)
                {
                    Directory.Move(_selectedItem.FullPath, newPath);
                }
                else
                {
                    File.Move(_selectedItem.FullPath, newPath);
                }
                LoadDirectory(_currentPath);
                StatusLabel.Text = $"Renamed to '{newName}'";
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    public string GetSelectedFilePath()
    {
        return _selectedItem?.FullPath;
    }
}

public class FileItem : INotifyPropertyChanged
{
    public string Name { get; set; }
    public string FullPath { get; set; }
    public string Icon { get; set; } = "📄";
    public string ModifiedDate { get; set; } = "";
    public string Type { get; set; } = "File";
    public string Size { get; set; } = "";
    public bool IsDirectory { get; set; } = false;

    public event PropertyChangedEventHandler PropertyChanged;
}
