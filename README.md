# Text Editor - Advanced MAUI Rich Text Editor

A professional Windows text editor built with .NET MAUI featuring rich text editing capabilities similar to Microsoft Word.

## Features

### Core Editing Features
- **Rich Text Editor**: WebView-based editor with full HTML formatting support
- **Selective Formatting**: Apply formatting to selected text only (like Microsoft Word)
- **Live Word Count**: Real-time word and character counting
- **Selection Info**: Display current selection and formatting state

### Text Formatting
- **Bold**: Apply bold formatting to selected text
- **Italic**: Apply italic formatting to selected text  
- **Underline**: Apply underline to selected text
- **Strikethrough**: Cross out selected text
- **Subscript**: Lower text below baseline (H₂O)
- **Superscript**: Raise text above baseline (E=mc²)

### Font & Typography
- **Font Family**: Arial, Times New Roman, Calibri, Georgia, Verdana, Courier New, Consolas
- **Font Size**: 8pt to 72pt with standard document sizes (8, 9, 10, 11, 12, 14, 16, 18, 20, 24, 28, 32, 36, 48, 72)
- **Proportional Fonts**: Arial, Times New Roman, Calibri, Georgia, Verdana
- **Monospace Fonts**: Courier New, Consolas

### Paragraph Formatting
- **Left Align**: Align text to left margin
- **Center Align**: Center text horizontally
- **Right Align**: Align text to right margin
- **Justify**: Full justification with even word spacing

### Lists
- **Bullet Lists**: Unordered lists with bullet points
- **Numbered Lists**: Ordered lists with sequential numbers

### Colors & Highlighting
- **Text Color**: Change foreground text color
- **Highlighting**: Apply background color highlighting to text

### Clipboard Operations
- **Cut**: Remove selected text and copy to clipboard
- **Copy**: Copy selected text to clipboard
- **Paste**: Insert clipboard content at cursor position

## Modern UI Design

### Menu Bar
Professional menu bar with File, Edit, Format, and Help menus.

### Toolbar Groups
Organized toolbars with visual grouping:
- **Clipboard**: Cut, Copy, Paste operations
- **Typography**: Font family and size selection
- **Formatting**: Bold, Italic, Underline, Strikethrough
- **Scripts**: Subscript and Superscript
- **Alignment**: Left, Center, Right, Justify
- **Lists**: Bullet and numbered lists
- **Colors**: Text color and highlighting

### Editor Area
- Clean white editing surface with subtle shadow
- HTML-based rich text editor (contenteditable)
- Responsive layout that adapts to window size

### Quick Format Bar
Real-time display of:
- Current selection info
- Active formatting state
- Cursor position (Line, Column)

### Status Bar
Professional status bar showing:
- Current operation status
- Word count
- Character count

## Technical Implementation

### WebView Rich Text Editor
- Uses HTML5 `contenteditable` for rich text editing
- JavaScript interop for C# to JavaScript communication
- document.execCommand API for formatting operations
- Real-time selection change monitoring

### JavaScript Interop
- `execCommand()`: Execute formatting commands
- `queryCommandState()`: Check current formatting state
- `getSelectionInfo()`: Get selected text info
- Event handling for selection and content changes

### Formatting Architecture
All formatting applies to **selected text only**:
1. User selects text in editor
2. Clicks formatting button
3. JavaScript `execCommand` applies formatting to selection
4. Formatting buttons highlight when active
5. Status bar shows applied formatting

## Requirements

- .NET 8.0 SDK
- Visual Studio 2022 with MAUI workload
- Windows 10 version 17763 or higher
- Windows App SDK

## Building and Running

1. Open `TextEditor.sln` in Visual Studio 2022
2. Select Windows Machine as target
3. Press F5 or click "Run" to build and launch

## Project Structure

```
TextEditor/
├── MainPage.xaml              # Modern UI with toolbar groups
├── MainPage.xaml.cs           # WebView editor and JavaScript interop
├── App.xaml                   # Application styles
├── App.xaml.cs                # Application entry
├── MauiProgram.cs             # MAUI configuration
├── TextEditor.csproj          # Project configuration
├── Platforms/
│   └── Windows/               # Windows-specific files
│       ├── App.xaml           # WinUI app definition
│       ├── App.xaml.cs        # WinUI app logic
│       └── Program.cs         # Entry point
└── Resources/
    ├── Fonts/
    └── Images/
```

## Usage Guide

### Basic Text Editing
1. Type or paste text into the editor
2. Select text by clicking and dragging
3. Apply formatting using toolbar buttons

### Formatting Selected Text
1. Select the text you want to format
2. Click Bold (B), Italic (I), or Underline (U) button
3. Formatting is applied only to the selection
4. Button highlights to show active formatting

### Changing Fonts
1. Select text or place cursor
2. Choose font from Font dropdown
3. Choose size from Size dropdown
4. Changes apply to selection or new text

### Paragraph Alignment
1. Click in paragraph or select multiple paragraphs
2. Click Left, Center, Right, or Justify button
3. Alignment applies to current paragraph(s)

### Creating Lists
1. Click where you want list to start
2. Click • List for bullets or 1. List for numbers
3. Type list items, press Enter for new items

### Clipboard Operations
1. Select text to cut or copy
2. Click Cut or Copy button
3. Click where you want to paste
4. Click Paste button

## Keyboard Shortcuts

While full shortcut support is planned, the editor currently supports:
- Standard text editing shortcuts (Ctrl+C, Ctrl+V, Ctrl+X)
- Arrow keys for navigation
- Shift+Arrow keys for selection

## Future Enhancements

- **File Operations**: Open/Save documents
- **Undo/Redo**: Full history management  
- **Find/Replace**: Search and replace functionality
- **Tables**: Insert and edit tables
- **Images**: Insert images into documents
- **Print**: Print document support
- **Export**: Export to PDF, DOCX formats
- **Keyboard Shortcuts**: Full shortcut implementation

## License

This project is provided as an educational example for MAUI development with rich text editing capabilities.
