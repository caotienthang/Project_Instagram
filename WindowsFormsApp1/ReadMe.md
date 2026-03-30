

# Project\_Instagram (WinForms)

Ứng dụng **Windows Forms (.NET Framework 4.8)** quản lý tài khoản Instagram thông qua AccountsCenter API, hỗ trợ đăng bài, quản lý xác thực 2 yếu tố (2FA/TOTP), lưu trữ session cục bộ bằng SQLite.

---

## Yêu cầu

| Thành phần | Phiên bản |
|---|---|
| Windows | 10 trở lên |
| .NET Framework | 4.8 |
| Visual Studio | 2019 trở lên (khuyến nghị) |
| Microsoft Edge WebView2 Runtime | Bất kỳ (dùng cho luồng đăng nhập) |

---

## Chạy nhanh

1. Clone repository về máy.
2. Mở `WindowsFormsApp1.sln` bằng Visual Studio.
3. Restore NuGet packages (chuột phải Solution → **Restore NuGet Packages**).
4. Build và chạy (`F5`).

> File SQLite được tạo tự động tại `<thư mục chạy>\SQLite\data.db` khi khởi động lần đầu.

---

## Tính năng chính

### 1. Quản lý tài khoản (AccountsCenterForm)

- Mở WebView2 điều hướng đến `https://www.instagram.com/accounts/login/`.
- Sau khi đăng nhập, tự động redirect sang `https://accountscenter.instagram.com/`.
- `AccountsCenterService` trích xuất từ WebView2:
  - Token `fb_dtsg`, `lsd` (từ JS runtime).
  - Cookie phiên làm việc.
  - Thông tin tài khoản (username, full name, email, phone, avatar, birthday, account ID).
- Lưu `AccountInfo` và `InstagramSession` vào SQLite.
- Hỗ trợ thêm mới hoặc cập nhật tài khoản đã tồn tại.

### 2. Dashboard (DashBoard)

- Hiển thị danh sách tài khoản dạng DataGridView với avatar, thông tin cơ bản, trạng thái.
- Checkbox chọn nhiều tài khoản đồng thời.
- Tìm kiếm/lọc tài khoản theo tên.
- Cột **Get Cookie** lấy cookie mới cho tài khoản đã chọn.
- Load avatar không đồng bộ qua `AvatarService`.

### 3. Đăng bài (InstagramPostService)

- Upload ảnh đơn hoặc album (sidecar) lên Instagram.
- Tự động phân nhánh: 1 ảnh → `configure_content_publish`, nhiều ảnh → `configure_sidecar`.
- Upload ảnh binary trực tiếp qua `rupload_igphoto`.
- Nhận `caption` tùy chỉnh.

### 4. Quản lý 2FA/TOTP (TwoFactorService)

Toàn bộ giao tiếp qua `https://accountscenter.instagram.com/api/graphql/`.

| Method | Mô tả | doc\_id |
|---|---|---|
| `GetStatusAsync` | Lấy trạng thái 2FA, danh sách thiết bị TOTP | `25960508096941189` |
| `GenerateTotpKeyAsync` | Tạo TOTP key mới (trả về QR code URI, key text) | `9837172312995248` |
| `ConfirmTotpAsync` | Xác nhận mã 6 chữ số để bật 2FA | `29164158613231327` |
| `DisableTotpAsync` | Gỡ thiết bị TOTP theo `method_id` | `29616201561327421` |
| `GenerateTotpCode` | Tạo mã TOTP hiện tại từ Base32 secret (RFC 6238) | — |
| `PreloadConfirmDialogAsync` | Pre-flight query UI strings trước khi xác nhận | `26277751381877270` |

**Luồng thêm thiết bị 2FA:**

```
[Nhập tên thiết bị (nếu đã có thiết bị)]
        ↓
GenerateTotpKeyAsync()        — lấy QR code + key text
        ↓
PreloadConfirmDialogAsync()   — pre-flight query (bỏ qua response)
        ↓
TwoFactorAddDeviceDialog      — hiện QR, tải ảnh QR từ Facebook CDN
        ↓
User quét QR → nhập mã 6 số
        ↓
ConfirmTotpAsync()            — bật 2FA
```

**Luồng gỡ thiết bị:**

```
[Chọn thiết bị cần gỡ — nút chỉ active khi có ≥ 2 thiết bị]
        ↓
DisableTotpAsync(seedId)      — gỡ thiết bị theo method_id
        ↓
Refresh danh sách local
```

### 5. Hỗ trợ Proxy

`HttpClientFactory` hỗ trợ proxy toàn cục cho tất cả HTTP request:

```csharp
// Thiết lập proxy cho toàn bộ service
HttpClientFactory.SetProxy("http://user:pass@host:port");

// Hoặc qua từng service
var svc = new AccountsCenterService(coreWebView2);
svc.SetProxy("http://127.0.0.1:8888");

var postSvc = new InstagramPostService();
postSvc.SetProxy("http://127.0.0.1:8888");
```

Format hỗ trợ:
- `http://host:port`
- `http://username:password@host:port`

Nếu không gọi `SetProxy`, tất cả request đi thẳng (không qua proxy).

---

## Cấu trúc dự án

```
WindowsFormsApp1/
├── Forms/
│   ├── DashBoard.cs              # Form chính, danh sách tài khoản
│   └── AccountsCenterForm.cs     # WebView2 đăng nhập & lấy session
├── Views/
│   ├── TwoFactorManagerDialog.cs # Quản lý thiết bị 2FA
│   ├── TwoFactorAddDeviceDialog.cs # Thêm thiết bị TOTP (QR + xác nhận)
│   ├── PostPanel.cs              # Panel đăng bài
│   └── ImageSourceDialog.cs      # Chọn nguồn ảnh
├── Services/
│   ├── AccountsCenterService.cs  # Lấy session từ WebView2
│   ├── InstagramPostService.cs   # Upload & đăng bài
│   ├── TwoFactorService.cs       # Toàn bộ logic 2FA/TOTP
│   ├── AvatarService.cs          # Tải avatar async
│   └── HttpClientFactory.cs     # HttpClient với proxy support
├── Data/
│   ├── AccountRepository.cs      # CRUD bảng Accounts
│   └── InstagramSessionRepository.cs # CRUD bảng Sessions
├── Models/
│   ├── AccountInfo.cs            # Thông tin tài khoản
│   └── InstagramSession.cs       # Cookie + fb_dtsg + lsd
└── Helpers/
    └── SqliteHelper.cs           # Tạo DB & migrate schema
```

---

## Cơ sở dữ liệu SQLite

File: `<thư mục chạy>\SQLite\data.db`

| Bảng | Mô tả |
|---|---|
| `Accounts` | Thông tin tài khoản Instagram |
| `InstagramSessions` | Cookie, fb\_dtsg, lsd theo từng account |

Schema được tạo tự động khi khởi động. Migration `ADD COLUMN` chạy an toàn (bỏ qua nếu cột đã tồn tại).

---

## NuGet packages chính

| Package | Mục đích |
|---|---|
| `Microsoft.Data.Sqlite` | SQLite ORM-free |
| `SQLitePCLRaw.bundle_e_sqlite3` | SQLite native provider |
| `Newtonsoft.Json` | JSON serialize/deserialize |
| `Microsoft.Web.WebView2` | Nhúng trình duyệt Chromium |

---

## Ghi chú quan trọng — SQLite provider

Nếu gặp lỗi: *"You need to call SQLitePCL.raw.SetProvider()..."*

Thêm vào `Program.Main()` **trước** `SqliteHelper.EnsureDatabase()`:

```csharp
SQLitePCL.Batteries.Init();
SqliteHelper.EnsureDatabase();
```

---

## Troubleshooting

| Triệu chứng | Nguyên nhân | Giải pháp |
|---|---|---|
| Lỗi provider SQLite | Chưa khởi tạo | Gọi `SQLitePCL.Batteries.Init()` |
| WebView2 không load | Chưa cài runtime | Cài [Edge WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) |
| 2FA: "This code isn't right" | Mã TOTP hết hạn | Nhập lại mã mới trong vòng 30 giây |
| Upload ảnh thất bại | Session hết hạn | Dùng **Get Cookie** để lấy session mới |
- `Invalid redirect_uri`: kiểm tra `redirect_uri` trong Instagram app và `config.json`.
- WebView2 không load: cài WebView2 runtime.

## Contributing

Mở PR cho bugfix hoặc tính năng. Không commit secrets.

## License

Theo file license ở root (nếu có).

