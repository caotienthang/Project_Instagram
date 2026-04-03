# ⚙️ Settings System - Hướng Dẫn Sử Dụng

## 📖 Tổng Quan

Hệ thống Settings cho phép bạn tùy chỉnh và quản lý cấu hình ứng dụng Instagram MMO Tool một cách dễ dàng thông qua giao diện trực quan và file JSON.

---

## 🎯 Tính Năng

### 1. **Instagram API Settings**
Cấu hình các thông số kết nối với Instagram API:

- **Encryption Key ID** - ID của khóa mã hóa (mặc định: 228)
- **Public Key (Base64)** - Khóa công khai RSA để mã hóa mật khẩu
- **User-Agent** - User agent của thiết bị Android giả lập
- **Bloks Versioning ID** - ID phiên bản Bloks của Instagram
- **Request Timeout** - Thời gian chờ tối đa cho mỗi request (giây)

### 2. **Application Settings**
Cài đặt chức năng ứng dụng:

- **Auto Save Accounts** - Tự động lưu tài khoản sau khi login thành công
- **Download Avatars** - Tự động tải ảnh đại diện về máy
- **Avatar Cache Folder** - Thư mục lưu trữ avatar (mặc định: "Avatars")
- **Max Login Retries** - Số lần retry tối đa khi login thất bại
- **Retry Delay (ms)** - Thời gian chờ giữa các lần retry
- **Enable Debug Logging** - Bật chế độ ghi log chi tiết (cho developer)

### 3. **Proxy Settings** (Nâng cao)
Cấu hình proxy server:

- **Enable Proxy** - Bật/tắt sử dụng proxy
- **Proxy Server** - Địa chỉ proxy server
- **Port** - Cổng proxy
- **Proxy Type** - Loại proxy (HTTP, HTTPS, SOCKS4, SOCKS5)
- **Username/Password** - Thông tin xác thực proxy (nếu cần)

---

## 🚀 Cách Sử Dụng

### Mở Settings Dialog

1. Nhấn nút **⚙️ Settings** trên toolbar của Dashboard
2. Hoặc sử dụng shortcut key (nếu có cấu hình)

### Chỉnh Sửa Cài Đặt

1. **Tab Instagram API**: 
   - Thay đổi Encryption Key ID nếu Instagram cập nhật
   - Cập nhật Public Key khi Instagram thay đổi khóa mã hóa
   - Thay đổi User-Agent để giả lập thiết bị khác

2. **Tab Ứng dụng**:
   - Bật/tắt tự động lưu accounts
   - Cấu hình retry logic
   - Thay đổi thư mục lưu avatar

3. **Tab Proxy**:
   - Cấu hình proxy nếu cần ẩn IP
   - Hữu ích khi chạy nhiều tài khoản hoặc ở vùng bị chặn

### Lưu Cài Đặt

- Nhấn **💾 Lưu** để lưu thay đổi vào file `appsettings.json`
- Nhấn **❌ Hủy** để bỏ qua thay đổi

---

## 📁 File Cấu Hình

### Vị Trí File

```
[Thư mục ứng dụng]/appsettings.json
```

### Cấu Trúc JSON

```json
{
  "InstagramApi": {
    "EncryptionKeyId": 228,
    "PublicKeyBase64": "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0K...",
    "UserAgent": "Instagram 423.0.0.47.66 Android...",
    "BloksVersioningId": "899adff463607d5f13a547f7417a9de4a8b4add115ddebc553c1bc5b8d48a28a",
    "RequestTimeoutSeconds": 30
  },
  "Application": {
    "AutoSaveAccounts": true,
    "DownloadAvatars": true,
    "AvatarCacheFolder": "Avatars",
    "MaxLoginRetries": 3,
    "RetryDelayMs": 2000,
    "EnableDebugLogging": false
  },
  "Proxy": {
    "Enabled": false,
    "Host": "",
    "Port": 8080,
    "Username": "",
    "Password": "",
    "ProxyType": "HTTP"
  }
}
```

---

## 🔧 Các Tính Năng Nâng Cao

### 1. Export/Import Settings

#### Export Settings
- Nhấn nút **📤 Export** trong Settings Dialog
- Chọn vị trí lưu file backup
- File sẽ được lưu với tên: `appsettings_backup_YYYYMMDD_HHmmss.json`

**Khi nào cần Export:**
- Trước khi thử nghiệm cài đặt mới
- Sao lưu cài đặt hoạt động tốt
- Chia sẻ cài đặt với người khác

#### Import Settings
- Nhấn nút **📥 Import** trong Settings Dialog
- Chọn file JSON cần import
- Xác nhận để áp dụng cài đặt mới

**Lưu ý:** File import phải có cấu trúc JSON hợp lệ

### 2. Reset to Default

- Nhấn nút **🔄 Reset** để khôi phục cài đặt mặc định
- Xác nhận khi được hỏi
- **Cảnh báo:** Tất cả cài đặt hiện tại sẽ bị xóa!

### 3. Chỉnh Sửa Trực Tiếp File JSON

Bạn có thể chỉnh sửa file `appsettings.json` bằng text editor:

```bash
# Mở bằng Notepad
notepad appsettings.json

# Hoặc VS Code
code appsettings.json
```

**Lưu ý:** Đảm bảo cú pháp JSON đúng để tránh lỗi!

---

## 🎨 Tùy Chỉnh Nâng Cao

### Thay Đổi Encryption Key (Khi Instagram Cập Nhật)

Nếu Instagram thay đổi khóa mã hóa:

1. Mở Settings Dialog
2. Tab **Instagram API**
3. Cập nhật **Encryption Key ID** mới
4. Cập nhật **Public Key (Base64)** mới
5. Lưu lại

### Thêm User-Agent Mới

Để giả lập thiết bị khác:

```
Instagram [VERSION] Android ([SDK]/[VERSION]; [DPI]dpi; [RESOLUTION]; [MANUFACTURER]; [MODEL]; [DEVICE]; [CPU]; [LOCALE]; [BUILD_ID])
```

Ví dụ:
```
Instagram 423.0.0.47.66 Android (28/9; 480dpi; 1080x1920; Redmi; 22127RK46C; marlin; qcom; en_US; 923309183)
```

### Cấu Hình Proxy Chi Tiết

#### HTTP Proxy
```json
{
  "Enabled": true,
  "Host": "proxy.example.com",
  "Port": 8080,
  "ProxyType": "HTTP"
}
```

#### SOCKS5 Proxy với Authentication
```json
{
  "Enabled": true,
  "Host": "socks5.example.com",
  "Port": 1080,
  "Username": "your_username",
  "Password": "your_password",
  "ProxyType": "SOCKS5"
}
```

---

## 🛡️ Bảo Mật

### Mật Khẩu Proxy

- Mật khẩu proxy được lưu dưới dạng plain text trong file JSON
- **Khuyến nghị:** Sử dụng proxy không yêu cầu password hoặc dùng VPN
- Không chia sẻ file `appsettings.json` chứa thông tin nhạy cảm

### Public Key

- Public Key được mã hóa Base64
- Không cần bảo mật vì đây là khóa công khai
- Chỉ cập nhật khi Instagram thay đổi chính thức

---

## ⚠️ Xử Lý Lỗi

### Lỗi File Cấu Hình Không Hợp Lệ

**Triệu chứng:**
```
Lỗi khi đọc file cấu hình: [Error message]
Sử dụng cấu hình mặc định.
```

**Giải pháp:**
1. Kiểm tra cú pháp JSON bằng [JSONLint](https://jsonlint.com/)
2. Xóa file `appsettings.json` - ứng dụng sẽ tự tạo file mới
3. Import lại từ file backup

### Lỗi Validation

**Triệu chứng:**
```
Cấu hình không hợp lệ. Vui lòng kiểm tra lại.
```

**Giải pháp:**
1. Kiểm tra các trường bắt buộc không được để trống:
   - Encryption Key ID > 0
   - Public Key không rỗng
   - User-Agent không rỗng
   - Bloks Versioning ID không rỗng
2. Kiểm tra giá trị hợp lệ:
   - Request Timeout: 5-300 giây
   - Proxy Port: 1-65535

### Settings Không Được Áp Dụng

**Triệu chứng:** Thay đổi settings nhưng ứng dụng vẫn dùng giá trị cũ

**Giải pháp:**
1. Nhấn **💾 Lưu** trong Settings Dialog
2. Kiểm tra file `appsettings.json` đã được cập nhật
3. Khởi động lại ứng dụng nếu cần

---

## 📊 Ví Dụ Sử Dụng

### Scenario 1: Instagram Cập Nhật API

```
1. Instagram thông báo thay đổi Encryption Key ID từ 228 → 229
2. Mở Settings Dialog
3. Tab Instagram API → Encryption Key ID: 229
4. Lưu lại
5. Test login → Thành công!
```

### Scenario 2: Tối Ưu Performance

```
1. Mở Settings Dialog
2. Tab Ứng dụng:
   - Tắt "Download Avatars" → Tăng tốc độ login
   - Giảm "Retry Delay" → 1000ms → Login nhanh hơn
   - Tăng "Max Retries" → 5 → Tăng tỷ lệ thành công
3. Lưu lại
4. Test với nhiều accounts
```

### Scenario 3: Sử Dụng Proxy

```
1. Mua proxy từ nhà cung cấp
2. Mở Settings Dialog → Tab Proxy:
   - Enable Proxy: ✓
   - Host: 123.45.67.89
   - Port: 8080
   - Proxy Type: HTTP
3. Lưu lại
4. Test login → IP đã thay đổi!
```

---

## 🔍 Debug & Troubleshooting

### Bật Debug Logging

1. Settings Dialog → Tab Ứng dụng
2. Bật **Enable Debug Logging**
3. Lưu lại
4. Check console output hoặc log files

### Kiểm Tra Settings Hiện Tại

```csharp
// Trong code
var settings = SettingsManager.LoadSettings();
Console.WriteLine($"Encryption Key ID: {settings.InstagramApi.EncryptionKeyId}");
Console.WriteLine($"User Agent: {settings.InstagramApi.UserAgent}");
```

### Log File Location

```
[Thư mục ứng dụng]/Logs/debug_YYYYMMDD.log
```

---

## 🚀 Tips & Tricks

1. **Backup thường xuyên**
   - Export settings trước khi thử nghiệm
   - Lưu nhiều versions khác nhau

2. **Tối ưu theo use case**
   - Account cá nhân: Tắt proxy, bật avatar
   - MMO nhiều accounts: Bật proxy, tắt avatar để tăng tốc

3. **Testing**
   - Test trên 1-2 accounts trước khi áp dụng hàng loạt
   - Giữ settings backup hoạt động tốt

4. **Security**
   - Không chia sẻ Public Key nếu là custom
   - Đổi proxy password thường xuyên nếu dùng

---

## 📞 Hỗ Trợ

Nếu gặp vấn đề với Settings:

1. Kiểm tra log files
2. Reset to default settings
3. Import từ backup
4. Liên hệ support với file `appsettings.json` (xóa thông tin nhạy cảm trước)

---

## 📝 Changelog

### Version 2.0 (Current)
- ✅ Settings Dialog với UI trực quan
- ✅ JSON-based configuration
- ✅ Export/Import settings
- ✅ Reset to default
- ✅ Validation & error handling
- ✅ Support cho Proxy settings
- ✅ Real-time settings reload

### Future Plans
- 🔮 Multiple profile support
- 🔮 Cloud sync settings
- 🔮 Settings version control
- 🔮 Advanced proxy rotation

---

**Được phát triển với ❤️ cho Instagram MMO Tool**

*Last updated: 2025*
