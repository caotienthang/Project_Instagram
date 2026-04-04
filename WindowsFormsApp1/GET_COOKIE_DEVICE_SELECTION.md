# 🍪 Get Cookie with Device Selection - Implementation Guide

## 🎯 Mục Đích

Cập nhật nút **Get Cookie** trong Dashboard để hỏi user chọn "Máy tính" (WebView2) hoặc "Điện thoại" (Phone API) trước khi lấy cookie/session.

---

## ✅ Thay Đổi Đã Thực Hiện

### 1. **PhoneLoginDialog.cs** - Thêm Update Session Mode

**File:** `WindowsFormsApp1/Views/PhoneLoginDialog.cs`

**Thay đổi:**

#### ✅ Thêm Constructor Overload
```csharp
// Constructor cũ - Add Account mode
public PhoneLoginDialog()

// Constructor mới - Update Session mode
public PhoneLoginDialog(AccountInfo existingAccount)
{
    _isUpdateMode = true;
    _existingAccount = existingAccount;
    
    // Pre-fill username & password
    txtUsername.Text = existingAccount.Username;
    txtPassword.Text = existingAccount.Password;
    
    btnConfirm.Text = "🔄 Update Session";
    this.Text = $"Get Cookie - @{existingAccount.Username}";
}
```

#### ✅ Skip Validation Trong Update Mode
```csharp
// Chỉ check account tồn tại khi Add mode
if (!_isUpdateMode)
{
    var existing = AccountRepository.GetByUsername(username);
    if (existing != null)
    {
        SetStatus("⚠️ Tài khoản đã tồn tại...");
        return;
    }
}
```

#### ✅ Phân Biệt Success Message
```csharp
if (_isUpdateMode)
{
    SavedAccount = _existingAccount;
    SetStatus("✅ Session updated thành công!");
}
else
{
    SavedAccount = AccountRepository.GetByAccountIds(...);
    SetStatus("✅ Đăng nhập thành công!");
}
```

---

### 2. **DeviceSelectionDialog.cs** - Thêm Helper Method

**File:** `WindowsFormsApp1/Forms/DeviceSelectionDialog.cs`

**Thêm:**
```csharp
/// <summary>
/// Hiện dialog chọn phương thức get cookie
/// </summary>
public static DeviceType ShowGetCookieDialog(string username, IWin32Window owner = null)
{
    using (var dlg = new DeviceSelectionDialog(
        title: "Get Cookie",
        message: $"Lấy cookie cho @{username} bằng phương thức nào?",
        computerLabel: "🖥️ WebView2 (Tự động)",
        phoneLabel: "📱 Login Phone (API)"
    ))
    {
        return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
    }
}
```

---

### 3. **DashBoard.cs** - Cập Nhật Get Cookie Button

**File:** `WindowsFormsApp1/Forms/DashBoard.cs`

**Trước:**
```csharp
// Get-Cookie button - mở trực tiếp AccountsCenterForm
if (e.ColumnIndex == grid.Columns["GetCookie"].Index)
{
    AppendLog($"🍪 Getting cookie for: {account.Username}");
    
    using (var form = new AccountsCenterForm(account))
        form.ShowDialog();
    
    LoadData();
}
```

**Sau:**
```csharp
// Get-Cookie button - hỏi Computer hay Phone
if (e.ColumnIndex == grid.Columns["GetCookie"].Index)
{
    var deviceType = DeviceSelectionDialog.ShowGetCookieDialog(account.Username, this);
    
    if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
    {
        // WebView2 (existing logic)
        using (var form = new AccountsCenterForm(account))
            form.ShowDialog();
    }
    else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
    {
        // Phone API - update session
        using (var form = new PhoneLoginDialog(account))
        {
            if (form.ShowDialog(this) == DialogResult.OK)
            {
                LoadData();
                AppendLog($"✅ Session updated: {account.Username}");
            }
        }
    }
}
```

---

## 🔧 Cách Hoạt Động

### Flow: Get Cookie - Computer (WebView2)

```
User click "🍪 Get Cookie" trên grid row
    ↓
DeviceSelectionDialog hiện lên
    ↓
User chọn "🖥️ WebView2 (Tự động)"
    ↓
Mở AccountsCenterForm với account info
    ↓
User login trên WebView2
    ↓
Lấy cookie + FbDtsg + Lsd
    ↓
Save vào InstagramSession (web session)
    ↓
Refresh grid
```

### Flow: Get Cookie - Phone (API)

```
User click "🍪 Get Cookie" trên grid row
    ↓
DeviceSelectionDialog hiện lên
    ↓
User chọn "📱 Login Phone (API)"
    ↓
Mở PhoneLoginDialog với account info
    ↓
Username & Password đã pre-fill
    ↓
User click "🔄 Update Session"
    ↓
Call Instagram Phone Login API
    ↓
Nếu cần 2FA → nhập code
    ↓
Lấy session (SessionId, Authorization, etc.)
    ↓
Update InstagramSession (phone session)
    ↓
Refresh grid
```

---

## 📋 So Sánh 2 Modes

| Feature | Add Account Mode | Update Session Mode |
|---------|------------------|---------------------|
| Constructor | `PhoneLoginDialog()` | `PhoneLoginDialog(account)` |
| Title | "Đăng Nhập Instagram" | "Get Cookie - @username" |
| Button Text | "✓ Confirm" | "🔄 Update Session" |
| Username Field | Empty | Pre-filled |
| Password Field | Empty | Pre-filled (if exists) |
| Validation | Check tồn tại → reject | Skip check |
| After Success | Create new account | Update existing account |
| Success Message | "Đăng nhập thành công" | "Session updated thành công" |

---

## 🎯 Use Cases

### 1. **Get Cookie cho Account Đã Tồn Tại**
```csharp
// Từ Dashboard grid button
var account = _accounts.First();
var deviceType = DeviceSelectionDialog.ShowGetCookieDialog(account.Username, this);

if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
{
    using (var form = new PhoneLoginDialog(account))
    {
        if (form.ShowDialog() == DialogResult.OK)
        {
            // Session updated
        }
    }
}
```

### 2. **Refresh Session Khi Expired**
```csharp
// Session expired → yêu cầu login lại
if (session.IsExpired())
{
    var deviceType = DeviceSelectionDialog.ShowGetCookieDialog(account.Username);
    
    if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
    {
        using (var dlg = new PhoneLoginDialog(account))
        {
            dlg.ShowDialog();
        }
    }
}
```

### 3. **Switch Session Type**
```csharp
// Account có Web session → muốn thêm Phone session
var account = AccountRepository.GetById(id);

using (var dlg = new PhoneLoginDialog(account))
{
    if (dlg.ShowDialog() == DialogResult.OK)
    {
        // Bây giờ account có cả 2 session types
    }
}
```

---

## ⚠️ Lưu Ý

### 1. **Pre-filled Password**
- Nếu account có `Password` trong DB → auto-fill
- Nếu không có → user phải nhập manual
- Password được update sau khi login thành công

### 2. **Session Update**
- `UpsertPhone()` tự động create hoặc update session
- Không tạo duplicate account
- Update info: Username, FullName, Avatar, Phone

### 3. **Validation**
- Update mode **skip** check account tồn tại
- Vẫn validate username/password không empty
- Vẫn validate 2FA nếu cần

### 4. **UX Improvements**
- Button text đổi thành "🔄 Update Session"
- Title hiện username: "Get Cookie - @username"
- Success message khác: "Session updated thành công"

---

## 🐛 Error Handling

### Trường Hợp 1: Account Không Có Password
```csharp
// Pre-fill sẽ empty → user nhập manual
txtPassword.Text = existingAccount.Password ?? "";
```

### Trường Hợp 2: Username Thay Đổi
```csharp
// Nếu Instagram trả về username khác
result.Username ?? username  // Fallback to input
```

### Trường Hợp 3: Login Fail
```csharp
// Re-enable UI để user thử lại
btnConfirm.Enabled = true;
txtUsername.Enabled = true;
txtPassword.Enabled = true;
```

---

## 📂 Files Đã Sửa

### Modified
- ✅ `WindowsFormsApp1/Views/PhoneLoginDialog.cs`
  - Thêm constructor `PhoneLoginDialog(AccountInfo)`
  - Thêm `_isUpdateMode`, `_existingAccount` fields
  - Skip validation trong update mode
  - Pre-fill username/password
  - Change button text & title

- ✅ `WindowsFormsApp1/Forms/DeviceSelectionDialog.cs`
  - Thêm `ShowGetCookieDialog(username, owner)` static method

- ✅ `WindowsFormsApp1/Forms/DashBoard.cs`
  - Cập nhật `grid_CellContentClick` Get Cookie logic
  - Thêm device selection dialog
  - Handle cả Computer và Phone flows

---

## ✅ Testing Checklist

### Get Cookie - Computer (WebView2)
- [ ] Click "🍪 Get Cookie" trên grid
- [ ] Dialog hiện với username trong message
- [ ] Chọn "🖥️ WebView2"
- [ ] AccountsCenterForm mở
- [ ] Login thành công
- [ ] Cookie saved vào DB
- [ ] Grid refresh

### Get Cookie - Phone (API)
- [ ] Click "🍪 Get Cookie" trên grid
- [ ] Dialog hiện với username
- [ ] Chọn "📱 Login Phone"
- [ ] PhoneLoginDialog mở với:
  - [ ] Title: "Get Cookie - @username"
  - [ ] Username pre-filled
  - [ ] Password pre-filled (nếu có)
  - [ ] Button text: "🔄 Update Session"
- [ ] Click Update Session
- [ ] Login thành công (hoặc 2FA)
- [ ] Session saved vào DB
- [ ] Grid refresh
- [ ] Success message: "Session updated thành công"

### Edge Cases
- [ ] Account không có password → nhập manual
- [ ] Login fail → UI re-enable
- [ ] User cancel dialog → không làm gì
- [ ] 2FA required → nhập code
- [ ] Session expired → update thành công

---

## 🔮 Mở Rộng Sau Này

### 1. **Auto-Select Based on Existing Session**
```csharp
// Nếu có web session → default Computer
// Nếu có phone session → default Phone
if (session.Cookie != null)
    defaultDevice = DeviceType.Computer;
else if (session.AuthorizationPhone != null)
    defaultDevice = DeviceType.Phone;
```

### 2. **Quick Refresh Button**
```csharp
// Thêm nút refresh session nhanh (không hỏi)
private void btnQuickRefresh_Click(object sender, EventArgs e)
{
    // Auto detect và dùng same method như lần trước
}
```

### 3. **Session Status Indicator**
```csharp
// Hiện icon cho biết có session type nào
🖥️ - Có Web session
📱 - Có Phone session
🔄 - Có cả 2
❌ - Không có session nào
```

---

## 📞 Support

**Khi nào dùng Computer vs Phone?**

| Scenario | Recommended | Reason |
|----------|------------|--------|
| First time login | Computer (WebView2) | Dễ dàng, visual, ít lỗi |
| Update expired session | Phone (API) | Nhanh, không cần browser |
| Account bị checkpoint | Computer (WebView2) | Cần xử lý captcha/verify |
| Bulk update sessions | Phone (API) | Tự động hóa được |
| Account có 2FA | Either | Cả 2 đều support |

---

## 🎉 Build Status

```
✅ Build successful
✅ PhoneLoginDialog constructor added
✅ DeviceSelectionDialog helper added
✅ Dashboard Get Cookie updated
✅ No breaking changes
✅ Ready to use
```
