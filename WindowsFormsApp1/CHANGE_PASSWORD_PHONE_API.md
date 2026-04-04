# 🔑 Change Password via Phone API - Implementation Guide

## 🎯 Tổng Quan

Implement tính năng đổi mật khẩu Instagram qua **Phone API** endpoint thay vì chỉ update local database.

---

## ✅ Tính Năng Đã Implement

### 1. **ChangePasswordService.cs** - Service Layer

**File:** `WindowsFormsApp1/Services/ChangePasswordService.cs`

**Chức năng:**
- ✅ Call API Instagram để đổi password thật
- ✅ Encrypt password theo format `#PWD_INSTAGRAM:4:timestamp:encrypted_data`
- ✅ Parse response để lấy success/error message
- ✅ Sử dụng settings từ `SettingsManager` (UserAgent, BloksVersioningId)
- ✅ Sử dụng proxy từ `HttpClientFactory`

**API Endpoint:**
```
POST https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.change_password.submit_password/
```

**Headers:**
- `User-Agent`: Từ settings
- `Content-Type`: `application/x-www-form-urlencoded`
- `Authorization`: Từ `InstagramSession.AuthorizationPhone`

**Request Payload:**
```
params={url_encoded_json}&bloks_versioning_id={id}
```

**JSON Structure:**
```json
{
  "client_input_params": {
    "new_password_confirm": "#PWD_INSTAGRAM:4:...",
    "account_type": 1,
    "account_id": "17841438130065598",
    "account_name": "username",
    "new_password": "#PWD_INSTAGRAM:4:...",
    "should_logout": 0,
    "current_password": "#PWD_INSTAGRAM:4:..."
  },
  "server_params": {
    "profile_picture_url": null,
    "is_standalone_bottom_sheet": 0
  }
}
```

**Response Parsing:**

**Success:**
```
action: dhv "You changed your Instagram password for {username}"
```

**Error:**
```
action: dq8 "FX_CHANGE_PASSWORD:new_password_error_message" "Create a new password..."
```

---

### 2. **ChangePasswordDialog.cs** - UI Layer

**File:** `WindowsFormsApp1/Forms/ChangePasswordDialog.cs`

**Cập nhật:**
- ✅ Thêm `ChangePasswordService` instance
- ✅ Chuyển `btnConfirm_Click` thành `async`
- ✅ Validate session & account info trước khi call API
- ✅ Disable UI during API call (show "⏳ Changing...")
- ✅ Hiển thị error/success message từ API response
- ✅ Update password trong DB nếu thành công

**Validation Checks:**
1. Current password không empty
2. New password không empty
3. New password == Confirm password
4. Session tồn tại và có `AuthorizationPhone`
5. Account có `FbAccountId`

**UI Flow:**
```
User nhập mật khẩu
    ↓
Click "Confirm"
    ↓
Validate inputs
    ↓
Disable UI + Show "⏳ Changing..."
    ↓
Call ChangePasswordService.ChangePasswordAsync()
    ↓
[Success] → Update DB → Show success → Close dialog
[Error]   → Show error → Re-enable UI
```

---

## 🔧 Technical Details

### Password Encryption

Sử dụng **BouncyCastle** encryption giống như login:

**Format:**
```
#PWD_INSTAGRAM:4:{timestamp}:{base64_encrypted_data}
```

**Encryption Steps:**
1. Generate AES-256 key (32 bytes)
2. Generate GCM IV (12 bytes)
3. Encrypt password with AES-GCM
4. Encrypt AES key with RSA (PKCS1_v1_5)
5. Combine: `version + keyId + IV + encryptedKey + tag + ciphertext`
6. Base64 encode và thêm prefix

**Code:**
```csharp
_loginService.EncryptPassword("myPassword123")
// → "#PWD_INSTAGRAM:4:1775211465:AeWP3g8Ep..."
```

---

### Response Parsing

Sử dụng **Regex** để parse action string từ response:

**Success Pattern:**
```regex
dhv\s+"([^"]*)"
```
Match: `You changed your Instagram password for username`

**Error Pattern:**
```regex
dq8\s+"[^"]*"\s+"([^"]*)"
```
Match: `Create a new password you haven't used before.`

**Code:**
```csharp
var successMatch = Regex.Match(action, @"dhv\s+""([^""]*)""");
if (successMatch.Success)
{
    string message = successMatch.Groups[1].Value;
    return (true, message);
}
```

---

## 📋 Yêu Cầu

### Database
Account phải có:
- ✅ `FbAccountId` (Facebook Account ID)
- ✅ `Username`

Session phải có:
- ✅ `AuthorizationPhone` (Bearer token)

### Settings
Trong `appsettings.json`:
- ✅ `InstagramApi.UserAgent`
- ✅ `InstagramApi.BloksVersioningId`
- ✅ `InstagramApi.EncryptionKeyId`
- ✅ `InstagramApi.PublicKeyBase64`

---

## 🚀 Usage Example

### Từ Dashboard

```csharp
// User click "Change Password" button
// → Device selection dialog
var deviceType = DeviceSelectionDialog.ShowChangePasswordDialog(this);

if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
{
    using (var dlg = new ChangePasswordDialog(account))
    {
        if (dlg.ShowDialog(this) == DialogResult.OK)
        {
            AppendLog($"[{account.Username}] 🔑 Password updated successfully.");
        }
    }
}
```

### Trực Tiếp Từ Code

```csharp
var service = new ChangePasswordService();
var session = InstagramSessionRepository.GetByAccountId(accountId);

var (success, message) = await service.ChangePasswordAsync(
    session,
    account.FbAccountId,
    account.Username,
    "oldPassword123",
    "newPassword456"
);

if (success)
{
    Console.WriteLine($"✅ {message}");
}
else
{
    Console.WriteLine($"❌ {message}");
}
```

---

## ⚠️ Lưu Ý Quan Trọng

### 1. **Authorization Token**
- Token lấy từ `InstagramSession.AuthorizationPhone`
- Chỉ có khi login qua **Phone API** (PhoneLoginDialog)
- Nếu login qua **WebView2** → không có token → phải dùng change password qua web

### 2. **Profile Picture URL**
- Theo yêu cầu: set `null` thay vì URL thật
- API vẫn chấp nhận và không cần avatar

### 3. **Password Format**
- PHẢI encrypt theo format `#PWD_INSTAGRAM:4:...`
- Không thể gửi plain text password
- Timestamp phải là Unix seconds

### 4. **Error Handling**
- Instagram có thể reject password nếu:
  - Đã dùng password này trước đó
  - Password quá yếu
  - Current password sai
  - Account bị lock/restrict

### 5. **Session Timeout**
- Authorization token có thể expire
- Nếu call API trả 401/403 → cần login lại

---

## 🐛 Error Messages Từ Instagram

### Common Errors

| Error Message | Meaning | Solution |
|--------------|---------|----------|
| `Create a new password you haven't used before.` | Password đã dùng rồi | Đổi password khác |
| `Your password must be at least 6 characters.` | Password quá ngắn | Tối thiểu 6 ký tự |
| `The password you entered is incorrect.` | Current password sai | Kiểm tra lại |
| `Authorization token is required` | Không có session | Login qua Phone trước |
| `Account ID is required` | Chưa get FbAccountId | Get Cookie trước |

---

## 📂 Files Liên Quan

### Mới Tạo
- `WindowsFormsApp1/Services/ChangePasswordService.cs` ← Service layer

### Đã Sửa
- `WindowsFormsApp1/Forms/ChangePasswordDialog.cs` ← UI layer

### Dependencies
- `WindowsFormsApp1/Services/InstagramPhoneLoginService.cs` ← Reuse EncryptPassword()
- `WindowsFormsApp1/Services/HttpClientFactory.cs` ← HTTP client with proxy
- `WindowsFormsApp1/Managers/SettingsManager.cs` ← Settings
- `WindowsFormsApp1/Models/InstagramSession.cs` ← Session model
- `WindowsFormsApp1/Data/InstagramSessionRepository.cs` ← Session DB

---

## ✅ Testing Checklist

- [ ] Login account qua PhoneLoginDialog (để có AuthorizationPhone)
- [ ] Mở ChangePasswordDialog từ Dashboard
- [ ] Nhập current password đúng
- [ ] Nhập new password + confirm
- [ ] Click Confirm → hiện "⏳ Changing..."
- [ ] Đợi API response
- [ ] Verify success message
- [ ] Verify password đã update trong DB
- [ ] Thử login với password mới
- [ ] Test error case: password đã dùng
- [ ] Test error case: current password sai
- [ ] Test error case: không có session

---

## 🔮 Mở Rộng Sau Này

### 1. **Retry Mechanism**
```csharp
for (int i = 0; i < 3; i++)
{
    var result = await ChangePasswordAsync(...);
    if (result.success) break;
    await Task.Delay(2000);
}
```

### 2. **Password Strength Validator**
```csharp
if (newPassword.Length < 8)
    return (false, "Password must be at least 8 characters");

if (!HasUpperCase(newPassword) || !HasNumber(newPassword))
    return (false, "Password must contain uppercase and numbers");
```

### 3. **Logout Option**
```csharp
// Change should_logout from 0 to 1
["should_logout"] = 1  // Logout khỏi các devices khác
```

### 4. **Logging**
```csharp
if (Settings.Application.EnableDebugLogging)
{
    Console.WriteLine($"[ChangePassword] User: {username}");
    Console.WriteLine($"[ChangePassword] Response: {responseBody}");
}
```

---

## 📞 Support

Nếu gặp lỗi khi đổi password:
1. Kiểm tra `AuthorizationPhone` có tồn tại không
2. Kiểm tra `FbAccountId` có đúng không
3. Verify current password bằng cách thử login
4. Check log để xem API response thực tế
5. Thử login lại qua PhoneLoginDialog

---

## 🎉 Build Status

```
✅ Build successful
✅ ChangePasswordService.cs compiled
✅ ChangePasswordDialog.cs updated
✅ No breaking changes
✅ Ready to use
```
