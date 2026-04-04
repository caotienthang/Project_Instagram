# 📱🖥️ Device Selection Dialog - Implementation Guide

## 🎯 Mục Đích

Tạo một **dialog chung** (reusable) để hỏi người dùng chọn "Máy tính" hoặc "Điện thoại" thay vì nhiều dialog riêng lẻ. Dễ dàng mở rộng cho các tùy chọn khác sau này.

---

## ✅ Thay Đổi Đã Thực Hiện

### 1. **Tạo DeviceSelectionDialog.cs** (Dialog Chung)

**File:** `WindowsFormsApp1/Forms/DeviceSelectionDialog.cs`

**Tính năng:**
- ✅ Dialog chung có thể tái sử dụng cho nhiều mục đích
- ✅ Enum `DeviceType` để dễ mở rộng (Computer, Phone, hoặc thêm sau)
- ✅ Constructor linh hoạt với custom title, message, button labels
- ✅ Static helper methods để sử dụng nhanh:
  - `ShowAddAccountDialog()` - Thêm tài khoản
  - `ShowChangePasswordDialog()` - Đổi mật khẩu
  - `ShowVerifyAccountDialog()` - Xác minh tài khoản
  - `ShowCustomDialog()` - Custom dialog tùy ý

**Usage Example:**
```csharp
// Cách 1: Dùng static helper (khuyến nghị)
var deviceType = DeviceSelectionDialog.ShowAddAccountDialog(this);
if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
{
    // Xử lý Computer
}
else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
{
    // Xử lý Phone
}

// Cách 2: Tạo custom dialog
var deviceType = DeviceSelectionDialog.ShowCustomDialog(
    title: "Chọn Phương Thức",
    message: "Bạn muốn làm gì?",
    computerLabel: "🖥️ Desktop",
    phoneLabel: "📱 Mobile",
    owner: this
);

// Cách 3: Tạo instance với full control
using (var dlg = new DeviceSelectionDialog(
    title: "Custom Title",
    message: "Custom message",
    computerLabel: "Option A",
    phoneLabel: "Option B",
    showCancelButton: true))
{
    if (dlg.ShowDialog(this) == DialogResult.OK)
    {
        var selected = dlg.SelectedDevice;
        // Process...
    }
}
```

---

### 2. **Cập Nhật DashBoard.cs**

**Thay đổi:**

#### ✅ Button "Đổi Mật Khẩu" (`btnChangePassword_Click`)
**Trước:**
```csharp
using (var methodDlg = new PasswordMethodDialog())
{
    if (methodDlg.ShowDialog(this) != DialogResult.OK) return;
    
    if (methodDlg.SelectedMethod == PasswordMethodDialog.ChangeMethod.Computer)
    {
        // ...
    }
}
```

**Sau:**
```csharp
var deviceType = DeviceSelectionDialog.ShowChangePasswordDialog(this);

if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
{
    // ...
}
else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
{
    // ...
}
```

#### ✅ Button "Thêm Tài Khoản" (`btnAddAccount_Click`)
**Trước:**
```csharp
using (var dlg = new AddAccountDialog())
{
    if (dlg.ShowDialog(this) != DialogResult.OK) return;
    
    if (dlg.SelectedMethod == AddAccountDialog.LoginMethod.Computer)
    {
        // ...
    }
}
```

**Sau:**
```csharp
var deviceType = DeviceSelectionDialog.ShowAddAccountDialog(this);

if (deviceType == DeviceSelectionDialog.DeviceType.Computer)
{
    // ...
}
else if (deviceType == DeviceSelectionDialog.DeviceType.Phone)
{
    // ...
}
```

---

## 🚀 Lợi Ích

### 1. **Code Gọn Hơn**
- Không cần `using` statement
- Không cần check `DialogResult.OK`
- Trả về trực tiếp `DeviceType`

### 2. **Dễ Mở Rộng**
Nếu sau này cần thêm tùy chọn mới (VD: Tablet, API, etc.):

```csharp
// Chỉ cần thêm vào enum
public enum DeviceType
{
    None,
    Computer,
    Phone,
    Tablet,    // ← Thêm mới
    API        // ← Thêm mới
}

// Tạo static helper mới
public static DeviceType ShowVerifyAccountDialog(IWin32Window owner = null)
{
    using (var dlg = new DeviceSelectionDialog(
        title: "Xác Minh",
        message: "Chọn phương thức xác minh",
        computerLabel: "🖥️ WebView2",
        phoneLabel: "📱 OTP"
    ))
    {
        return dlg.ShowDialog(owner) == DialogResult.OK ? dlg.SelectedDevice : DeviceType.None;
    }
}
```

### 3. **Tái Sử Dụng**
Một dialog có thể dùng cho:
- ✅ Thêm tài khoản
- ✅ Đổi mật khẩu
- ✅ Xác minh tài khoản
- ✅ Bất kỳ chức năng nào cần chọn Computer/Phone

### 4. **Consistent UI**
- Tất cả dialog có giao diện giống nhau
- Màu sắc, font, layout thống nhất
- Hover effects, keyboard navigation

---

## 📝 Cách Sử Dụng Cho Features Mới

Khi thêm feature mới cần hỏi Computer/Phone:

```csharp
// Trong event handler của button mới
private void btnNewFeature_Click(object sender, EventArgs e)
{
    // Option 1: Tạo static helper riêng (khuyến nghị)
    var deviceType = DeviceSelectionDialog.ShowCustomDialog(
        title: "Feature Mới",
        message: "Chọn phương thức thực hiện",
        computerLabel: "🖥️ Tự động",
        phoneLabel: "📱 Thủ công",
        owner: this
    );
    
    // Option 2: Hoặc thêm vào DeviceSelectionDialog.cs
    // public static DeviceType ShowNewFeatureDialog(IWin32Window owner = null) { ... }
    
    // Xử lý
    switch (deviceType)
    {
        case DeviceSelectionDialog.DeviceType.Computer:
            // Computer logic
            break;
            
        case DeviceSelectionDialog.DeviceType.Phone:
            // Phone logic
            break;
            
        case DeviceSelectionDialog.DeviceType.None:
            // User cancelled
            return;
    }
}
```

---

## 🔧 Customization

### Thay Đổi Màu Sắc
Trong `DeviceSelectionDialog.cs`:

```csharp
// Computer button - Màu xanh dương
var btnComputer = CreateStyledButton(_computerLabel, Color.FromArgb(52, 152, 219));

// Phone button - Màu xanh lá
var btnPhone = CreateStyledButton(_phoneLabel, Color.FromArgb(46, 204, 113));

// Cancel button - Màu xám
var btnCancel = CreateStyledButton("❌ Hủy", Color.FromArgb(149, 165, 166));
```

### Thêm Button Thứ 3
Sửa enum và constructor:

```csharp
public enum DeviceType
{
    None,
    Computer,
    Phone,
    Cloud  // ← New
}

// Trong InitializeComponent()
var btnCloud = CreateStyledButton("☁️ Cloud", Color.FromArgb(155, 89, 182));
btnCloud.Click += (s, e) =>
{
    SelectedDevice = DeviceType.Cloud;
    DialogResult = DialogResult.OK;
    Close();
};
buttonPanel.Controls.Add(btnCloud);
```

---

## 📦 Files Liên Quan

### Mới Tạo
- `WindowsFormsApp1/Forms/DeviceSelectionDialog.cs` ← **Dialog chung**

### Đã Sửa
- `WindowsFormsApp1/Forms/DashBoard.cs`
  - `btnChangePassword_Click()` - Dùng `ShowChangePasswordDialog()`
  - `btnAddAccount_Click()` - Dùng `ShowAddAccountDialog()`

### Có Thể Xóa (Nếu Muốn)
Sau khi migrate toàn bộ code sang `DeviceSelectionDialog`:
- `WindowsFormsApp1/Forms/PasswordMethodDialog.cs` (cũ)
- `WindowsFormsApp1/Forms/AddAccountDialog.cs` (cũ)

---

## ✅ Build Status

```
Build successful ✅
All refactoring complete
No breaking changes
```

---

## 🎨 UI Preview

```
┌─────────────────────────────────────────────┐
│  Chọn Phương Thức                      [×]  │
├─────────────────────────────────────────────┤
│                                             │
│   Bạn muốn sử dụng phương thức nào?        │
│                                             │
│  ┌─────────┐  ┌──────────┐  ┌─────────┐   │
│  │🖥️ Máy   │  │📱 Điện   │  │❌ Hủy   │   │
│  │  tính   │  │  thoại   │  │         │   │
│  └─────────┘  └──────────┘  └─────────┘   │
│                                             │
└─────────────────────────────────────────────┘
```

**Features:**
- ✅ Hover effect (button sáng lên khi hover)
- ✅ Keyboard support (Tab để di chuyển, Enter/Esc)
- ✅ Center parent window
- ✅ Modern flat design với màu sắc đẹp
- ✅ Icon emoji cho dễ nhận biết

---

## 🔮 Mở Rộng Sau Này

### Thêm Tùy Chọn "Remember My Choice"
```csharp
private CheckBox chkRemember;

// Trong InitializeComponent()
chkRemember = new CheckBox
{
    Text = "Nhớ lựa chọn của tôi",
    ForeColor = Color.White,
    Dock = DockStyle.Bottom,
    Padding = new Padding(30, 5, 30, 10)
};
this.Controls.Add(chkRemember);

// Khi user click button
if (chkRemember.Checked)
{
    // Lưu vào settings
    SettingsManager.SavePreferredDevice(SelectedDevice);
}
```

### Thêm Description Cho Mỗi Option
```csharp
var lblComputerDesc = new Label
{
    Text = "Tự động qua WebView2, nhanh và an toàn",
    Font = new Font("Segoe UI", 8F),
    ForeColor = Color.LightGray
};
```

---

## 📞 Hỗ Trợ

Nếu cần thêm dialog mới hoặc custom cho case đặc biệt, chỉ cần:
1. Gọi `DeviceSelectionDialog.ShowCustomDialog()` với tham số phù hợp
2. Hoặc thêm static helper method mới vào `DeviceSelectionDialog.cs`

Không cần tạo dialog class mới! 🎉
