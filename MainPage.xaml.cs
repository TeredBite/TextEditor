using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace TextEditor;

public partial class MainPage : ContentPage
{
    private const string EditorHtml = @"<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            font-size: 14px;
            line-height: 1.6;
            margin: 20px;
            padding: 0;
            background: white;
        }
        #editor {
            min-height: 400px;
            outline: none;
        }
        b, strong { font-weight: bold; }
        i, em { font-style: italic; }
        u { text-decoration: underline; }
        s, strike { text-decoration: line-through; }
        sub { vertical-align: sub; font-size: smaller; }
        sup { vertical-align: super; font-size: smaller; }
        ul { padding-left: 20px; }
        ol { padding-left: 20px; }
    </style>
</head>
<body>
    <div id='editor' contenteditable='true'>
        <p>Start typing your document here...</p>
    </div>
    <script>
        const editor = document.getElementById('editor');
        
        // Notify C# about selection changes
        document.addEventListener('selectionchange', function() {
            const selection = window.getSelection();
            if (selection.rangeCount > 0) {
                const text = selection.toString();
                window.location.href = 'selectionchanged:' + encodeURIComponent(text);
            }
        });
        
        // Document changed notification
        editor.addEventListener('input', function() {
            window.location.href = 'contentchanged:' + encodeURIComponent(editor.innerHTML);
        });
        
        // Format commands
        function execCommand(command, value = null) {
            document.execCommand(command, false, value);
            editor.focus();
            return true;
        }
        
        function queryCommandState(command) {
            return document.queryCommandState(command);
        }
        
        function queryCommandValue(command) {
            return document.queryCommandValue(command);
        }
        
        function getSelectionInfo() {
            const selection = window.getSelection();
            const text = selection.toString();
            return {
                text: text,
                length: text.length,
                isBold: queryCommandState('bold'),
                isItalic: queryCommandState('italic'),
                isUnderline: queryCommandState('underline'),
                isStrikethrough: queryCommandState('strikeThrough'),
                fontName: queryCommandValue('fontName'),
                fontSize: queryCommandValue('fontSize'),
                alignment: queryCommandValue('justify')
            };
        }
        
        function getContent() {
            return editor.innerHTML;
        }
        
        function setContent(html) {
            editor.innerHTML = html;
        }
        
        function getText() {
            return editor.innerText;
        }
    </script>
</body>
</html>";

    private string _currentSelection = "";
    private bool _isBold = false;
    private bool _isItalic = false;
    private bool _isUnderline = false;

    public MainPage()
    {
        InitializeComponent();
        LoadEditor();
        InitializePickers();
    }

    private void InitializePickers()
    {
        FontSizePicker.SelectedIndex = 4;
    }

    private void LoadEditor()
    {
        EditorWebView.Source = new HtmlWebViewSource
        {
            Html = EditorHtml
        };
    }

    private void OnWebViewNavigating(object sender, WebNavigatingEventArgs e)
    {
        var url = e.Url;
        
        if (url.StartsWith("selectionchanged:"))
        {
            e.Cancel = true;
            var data = Uri.UnescapeDataString(url.Substring("selectionchanged:".Length));
            OnSelectionChanged(data);
        }
        else if (url.StartsWith("contentchanged:"))
        {
            e.Cancel = true;
            var html = Uri.UnescapeDataString(url.Substring("contentchanged:".Length));
            OnContentChanged(html);
        }
    }

    private void OnWebViewNavigated(object sender, WebNavigatedEventArgs e)
    {
        if (e.Result == WebNavigationResult.Success)
        {
            StatusLabel.Text = "Ready";
            UpdateWordCount();
        }
    }

    private void OnSelectionChanged(string text)
    {
        try
        {
            _currentSelection = text;
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UpdateFormatButtons();
            });
        }
        catch { }
    }

    private void OnContentChanged(string html)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            UpdateWordCount();
        });
    }

    private void UpdateSelectionInfo()
    {
        // Removed - Quick Format Bar was deleted
    }

    private void UpdateFormatButtons()
    {
        RunJavaScript("queryCommandState('bold')", result =>
        {
            bool isBold = result?.ToString() == "true";
            MainThread.BeginInvokeOnMainThread(() =>
            {
                BoldButton.BackgroundColor = isBold ? Color.FromArgb("#2980b9") : Color.FromArgb("#ecf0f1");
                BoldButton.TextColor = isBold ? Colors.White : Color.FromArgb("#2c3e50");
            });
        });

        RunJavaScript("queryCommandState('italic')", result =>
        {
            bool isItalic = result?.ToString() == "true";
            MainThread.BeginInvokeOnMainThread(() =>
            {
                ItalicButton.BackgroundColor = isItalic ? Color.FromArgb("#2980b9") : Color.FromArgb("#ecf0f1");
                ItalicButton.TextColor = isItalic ? Colors.White : Color.FromArgb("#2c3e50");
            });
        });

        RunJavaScript("queryCommandState('underline')", result =>
        {
            bool isUnderline = result?.ToString() == "true";
            MainThread.BeginInvokeOnMainThread(() =>
            {
                UnderlineButton.BackgroundColor = isUnderline ? Color.FromArgb("#2980b9") : Color.FromArgb("#ecf0f1");
                UnderlineButton.TextColor = isUnderline ? Colors.White : Color.FromArgb("#2c3e50");
            });
        });
    }

    private void UpdateWordCount()
    {
        RunJavaScript("getText()", result =>
        {
            if (result != null)
            {
                var text = result.ToString() ?? "";
                var wordCount = string.IsNullOrWhiteSpace(text) ? 0 : text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
                var charCount = text.Length;
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    WordCountLabel.Text = $"{wordCount} words";
                    CharCountLabel.Text = $"{charCount} chars";
                });
            }
        });
    }

    private async void RunJavaScript(string script, Action<object> callback = null)
    {
        try
        {
            var result = await EditorWebView.EvaluateJavaScriptAsync(script);
            callback?.Invoke(result);
        }
        catch { }
    }

    private void OnBoldClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('bold')");
        StatusLabel.Text = "Bold formatting applied";
        UpdateFormatButtons();
    }

    private void OnItalicClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('italic')");
        StatusLabel.Text = "Italic formatting applied";
        UpdateFormatButtons();
    }

    private void OnUnderlineClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('underline')");
        StatusLabel.Text = "Underline formatting applied";
        UpdateFormatButtons();
    }

    private void OnStrikethroughClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('strikeThrough')");
        StatusLabel.Text = "Strikethrough formatting applied";
    }

    private void OnSubscriptClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('subscript')");
        StatusLabel.Text = "Subscript formatting applied";
    }

    private void OnSuperscriptClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('superscript')");
        StatusLabel.Text = "Superscript formatting applied";
    }

    private void OnAlignLeftClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('justifyLeft')");
        StatusLabel.Text = "Left alignment applied";
    }

    private void OnAlignCenterClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('justifyCenter')");
        StatusLabel.Text = "Center alignment applied";
    }

    private void OnAlignRightClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('justifyRight')");
        StatusLabel.Text = "Right alignment applied";
    }

    private void OnAlignJustifyClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('justifyFull')");
        StatusLabel.Text = "Justify alignment applied";
    }

    private void OnBulletListClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('insertUnorderedList')");
        StatusLabel.Text = "Bullet list inserted";
    }

    private void OnNumberListClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('insertOrderedList')");
        StatusLabel.Text = "Numbered list inserted";
    }

    private void OnFontChanged(object sender, EventArgs e)
    {
        if (FontPicker.SelectedItem != null)
        {
            var fontName = FontPicker.SelectedItem.ToString();
            RunJavaScript($"execCommand('fontName', '{fontName}')");
            StatusLabel.Text = $"Font changed to {fontName}";
        }
    }

    private void OnFontSizeChanged(object sender, EventArgs e)
    {
        if (FontSizePicker.SelectedItem != null)
        {
            var fontSize = FontSizePicker.SelectedItem.ToString();
            RunJavaScript($"execCommand('fontSize', '{fontSize}')");
            StatusLabel.Text = $"Font size changed to {fontSize}pt";
        }
    }

    private void OnTextColorClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('foreColor', '#2c3e50')");
        StatusLabel.Text = "Text color changed";
    }

    private void OnHighlightClicked(object sender, EventArgs e)
    {
        RunJavaScript("execCommand('hiliteColor', '#f1c40f')");
        StatusLabel.Text = "Highlight applied";
    }

    private async void OnCutClicked(object sender, EventArgs e)
    {
        RunJavaScript("getSelectionInfo()", async result =>
        {
            if (result != null)
            {
                var text = result.ToString();
                await Clipboard.Default.SetTextAsync(text);
                RunJavaScript("execCommand('delete')");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.Text = "Selection cut to clipboard";
                });
            }
        });
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        RunJavaScript("getSelectionInfo()", async result =>
        {
            if (result != null)
            {
                var text = result.ToString();
                await Clipboard.Default.SetTextAsync(text);
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StatusLabel.Text = "Selection copied to clipboard";
                });
            }
        });
    }

    private async void OnPasteClicked(object sender, EventArgs e)
    {
        var text = await Clipboard.Default.GetTextAsync();
        if (!string.IsNullOrEmpty(text))
        {
            RunJavaScript($"execCommand('insertHTML', '{text.Replace("'", "\\'")}')");
            StatusLabel.Text = "Content pasted from clipboard";
        }
    }

    private void OnFileMenuTapped(object sender, EventArgs e)
    {
        ShowFileMenu();
    }

    private async void ShowFileMenu()
    {
        var action = await DisplayActionSheet("File", "Cancel", null, 
            "New", "Open...", "Save", "Save As...", "Exit");
        
        switch (action)
        {
            case "New":
                await NewDocument();
                break;
            case "Open...":
                await OpenDocument();
                break;
            case "Save":
                await SaveDocument();
                break;
            case "Save As...":
                await SaveDocumentAs();
                break;
            case "Exit":
                Application.Current?.Quit();
                break;
        }
    }

    private async Task NewDocument()
    {
        RunJavaScript("setContent('<p>Start typing your document here...</p>')");
        _currentFilePath = null;
        StatusLabel.Text = "New document created";
    }

    private string _currentFilePath;

    private async Task OpenDocument()
    {
        var fileBrowser = new FileBrowserPage();
        fileBrowser.Disappearing += (s, e) =>
        {
            var selectedPath = fileBrowser.GetSelectedFilePath();
            if (!string.IsNullOrEmpty(selectedPath) && File.Exists(selectedPath))
            {
                try
                {
                    var content = File.ReadAllText(selectedPath);
                    var escapedContent = content.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\n", "\\n").Replace("\r", "\\r");
                    RunJavaScript($"setContent('{escapedContent}')");
                    _currentFilePath = selectedPath;
                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusLabel.Text = $"Opened: {Path.GetFileName(selectedPath)}";
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Error", $"Failed to open file: {ex.Message}", "OK");
                    });
                }
            }
        };
        
        await Navigation.PushAsync(fileBrowser);
    }

    private async Task SaveDocument()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            await SaveDocumentAs();
        }
        else
        {
            await SaveToFile(_currentFilePath);
        }
    }

    private async Task SaveDocumentAs()
    {
        var fileName = await DisplayPromptAsync("Save As", "Enter file name:", initialValue: "document.txt");
        if (!string.IsNullOrWhiteSpace(fileName))
        {
            var savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            await SaveToFile(savePath);
        }
    }

    private async Task SaveToFile(string filePath)
    {
        RunJavaScript("getContent()", async result =>
        {
            if (result != null)
            {
                try
                {
                    var html = result.ToString();
                    File.WriteAllText(filePath, html);
                    _currentFilePath = filePath;
                    
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StatusLabel.Text = $"Saved: {Path.GetFileName(filePath)}";
                    });
                }
                catch (Exception ex)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await DisplayAlert("Error", $"Failed to save file: {ex.Message}", "OK");
                    });
                }
            }
        });
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        await OpenDocument();
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        await SaveDocument();
    }

    private async Task OpenFileBrowser()
    {
        // Removed - File Browser button deleted
    }

    private void OnEditMenuTapped(object sender, EventArgs e)
    {
        // Removed - Edit menu deleted
    }

    private async void ShowEditMenu()
    {
        // Removed - Edit menu deleted
    }

    private void OnFormatMenuTapped(object sender, EventArgs e)
    {
        // Removed - Format menu deleted
    }

    private async void ShowFormatMenu()
    {
        // Removed - Format menu deleted
    }

    private void OnHelpMenuTapped(object sender, EventArgs e)
    {
        // Removed - Help menu deleted
    }
}
