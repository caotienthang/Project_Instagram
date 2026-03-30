# 📸 Instagram Automation Tool

A Windows desktop application for managing multiple Instagram accounts with features for posting content, changing avatars, and managing sessions with proxy support.

## 🌟 Features

### 📊 Multi-Account Management
- **Dashboard View**: Manage multiple Instagram accounts from a single interface
- **Real-time Status Updates**: Track account activity with dynamic log updates
- **Account Details**: View and edit account information (username, email, phone, birthday)
- **Cookie Management**: Extract and store Instagram session cookies for automation

### 🖼️ Avatar Management
- **Change Avatars**: Update profile pictures for single or multiple accounts
- **Bulk Operations**: Select multiple accounts and change avatars in batch
- **Flexible Upload**: Choose images from file or folder
- **Local Caching**: Downloaded avatars are cached locally for faster access

### 📝 Instagram Posting
- **Single Image Posts**: Post individual photos with captions
- **Carousel Posts**: Create multi-image posts (automatically switches between ConfigureSingle and ConfigureSidecar)
- **Bulk Content Creation**: Add multiple content variations and post them sequentially
- **Multi-Account Posting**: Post the same content to multiple accounts
- **Async Operations**: Non-blocking UI with background posting
- **Real-time Progress**: Live updates showing current posting status

### 🔒 Security & Privacy
- **Proxy Support**: Configure HTTP/HTTPS proxies for all operations
- **Session Management**: Secure storage of Instagram session tokens
- **SQLite Database**: Local encrypted storage for account data
- **Cookie-based Authentication**: No password storage required

## 🏗️ Architecture

### Project Structure

```
WindowsFormsApp1/
├── Forms/                          # Windows Forms UI
│   ├── DashBoard.cs               # Main application dashboard
│   ├── AccountsCenterForm.cs     # Account management center
│   └── AccountDetailForm.cs      # Individual account details
│
├── Views/                         # Custom UserControls
│   ├── PostPanel.cs              # Instagram posting interface
│   └── ImageSourceDialog.cs      # Custom dialog for image source selection
│
├── Services/                      # Business logic layer
│   ├── InstagramPostService.cs   # Instagram posting operations
│   ├── AvatarService.cs          # Avatar upload and management
│   ├── AccountsCenterService.cs  # Account center operations
│   └── HttpClientFactory.cs      # Centralized HTTP client with proxy support
│
├── Data/                          # Data access layer
│   ├── AccountRepository.cs      # Account CRUD operations
│   └── InstagramSessionRepository.cs  # Session CRUD operations
│
├── Models/                        # Data models
│   ├── AccountInfo.cs            # Account information model
│   └── InstagramSession.cs       # Instagram session model
│
├── Helpers/                       # Utility classes
│   └── SqliteHelper.cs           # SQLite database helper
│
└── Program.cs                     # Application entry point
```

### Design Patterns

- **Repository Pattern**: Abstraction of data access logic
- **Service Layer**: Separation of business logic from UI
- **Designer Pattern**: UI/UX separation for maintainability
- **Event-Driven Architecture**: Loose coupling with events (OnLog, OnStatusUpdate, OnClose)
- **Factory Pattern**: HttpClientFactory for consistent HTTP client creation
- **Async/Await**: Non-blocking operations for better user experience

## 🛠️ Technology Stack

- **Framework**: .NET Framework 4.8
- **Language**: C# 7.3
- **UI Framework**: Windows Forms
- **Database**: SQLite
- **HTTP Client**: HttpClient with custom proxy support
- **API Integration**: Instagram Web API (unofficial)

## 📦 Installation

### Prerequisites

- Windows 7/8/10/11
- .NET Framework 4.8 Runtime
- Visual Studio 2019 or later (for development)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/caotienthang/Project_Instagram.git
   cd Project_Instagram
   ```

2. **Open the solution**
   - Open `WindowsFormsApp1.sln` in Visual Studio
   - Restore NuGet packages (if any)

3. **Build the project**
   ```bash
   # In Visual Studio: Build > Build Solution
   # Or use command line:
   msbuild WindowsFormsApp1.sln /p:Configuration=Release
   ```

4. **Run the application**
   - Press F5 in Visual Studio
   - Or run the executable from `bin/Release/WindowsFormsApp1.exe`

## 🚀 Usage

### Getting Started

1. **Launch the Application**
   - Run `WindowsFormsApp1.exe`
   - The DashBoard will open showing your accounts

2. **Add Accounts**
   - Click "Add Account" or navigate to Account Center
   - Enter account details (username, email, phone, etc.)
   - Click "Get Cookie" to extract Instagram session cookies

### Posting Content

1. **Select Accounts**
   - Check the accounts you want to post to in the DashBoard

2. **Click "Post" Button**
   - A PostPanel will appear

3. **Select Images**
   - Click "Select Image" button
   - Choose "Folder" to select multiple images from a directory
   - Or choose "File" to select specific image files
   - Preview appears in the FlowLayoutPanel

4. **Add Content**
   - Enter caption in the content textbox
   - Click "Add Content" to create multiple content variations
   - Each content will be posted sequentially

5. **Post**
   - Click "Post" button
   - The panel closes immediately
   - Posting continues in background
   - Real-time status updates appear in the Log column

### Changing Avatars

1. **Select Accounts**
   - Check one or multiple accounts

2. **Click "Change Avatar" Button**
   - Choose "File" for single image
   - Or "Folder" to randomly select from folder

3. **Monitor Progress**
   - Watch the Log column for status updates
   - Success/error messages appear in real-time

### Getting Cookies

1. **Open Instagram in Browser**
   - Log in to your Instagram account
   - Open Developer Tools (F12)
   - Go to Network tab

2. **Extract Cookies**
   - Copy the cookie string
   - In the application, click "Get Cookie" button in the account row
   - Paste the cookie string

3. **Verify Session**
   - The application will validate the cookie
   - Status updates to show if session is valid

## 🔧 Configuration

### Proxy Settings

Configure proxy in `HttpClientFactory.cs`:

```csharp
HttpClientFactory.SetProxy("http://proxy-server:port");
```

Or set proxy per service call for more granular control.

### Database Location

SQLite database is created at:
- Development: `{AppDirectory}/instagram.db`
- Modify path in `SqliteHelper.cs` if needed

## 📝 API Integration

### Instagram Web API

The application uses unofficial Instagram Web API endpoints:

- **ConfigureSingle**: For single image posts
- **ConfigureSidecar**: For carousel (multi-image) posts
- **Avatar Upload**: Profile picture updates
- **Session Validation**: Cookie-based authentication

**Note**: This uses Instagram's private API which may change. Use responsibly and comply with Instagram's Terms of Service.

## 🐛 Troubleshooting

### Common Issues

1. **"Cookie Invalid" Error**
   - Re-extract cookies from browser
   - Ensure you're logged in to Instagram
   - Check if session hasn't expired

2. **Proxy Connection Failed**
   - Verify proxy server is running
   - Check proxy URL format (http://host:port)
   - Test proxy connection separately

3. **Image Upload Failed**
   - Verify image format (JPG, PNG supported)
   - Check image file size (< 8MB recommended)
   - Ensure image is not corrupted

4. **Database Locked**
   - Close other instances of the application
   - Check file permissions on database file
   - Restart the application

## 🤝 Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Development Guidelines

- Follow C# naming conventions
- Use Designer pattern for UI components
- Add XML comments for public methods
- Write async methods for I/O operations
- Test with multiple accounts before submitting

## 📄 License

This project is for educational purposes only. Please comply with Instagram's Terms of Service and API usage policies.

## ⚠️ Disclaimer

This tool is provided "as is" without any warranties. The authors are not responsible for any misuse or violation of Instagram's terms of service. Use at your own risk.

**Important Notes:**
- Instagram may ban accounts that violate their automation policies
- Use reasonable delays between actions
- Do not spam or abuse the platform
- Respect rate limits and API usage guidelines
- This is an unofficial tool and not affiliated with Instagram/Meta

## 📞 Contact

- **Repository**: [https://github.com/caotienthang/Project_Instagram](https://github.com/caotienthang/Project_Instagram)
- **Issues**: Please report bugs via GitHub Issues

## 🎯 Roadmap

Future enhancements:
- [ ] Story posting support
- [ ] Comment automation
- [ ] Follow/Unfollow management
- [ ] Analytics dashboard
- [ ] Scheduled posting
- [ ] Multi-language support
- [ ] Dark mode UI

---

**Built with ❤️ using .NET Framework 4.8 and Windows Forms**
