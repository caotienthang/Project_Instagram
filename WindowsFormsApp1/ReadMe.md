# Instagram WinForms OAuth Flow

## 1. Flow tổng

```
User Login → Instagram OAuth → Redirect (code)
→ Exchange Code → Access Token → Get User Info → Dashboard
```

---

## 2. Cấu trúc project

* Forms

  * Login: xử lý login
  * Dashboard: container chính
* Views (UserControl)

  * HomeView
  * AccountView
* Services

  * InstagramAuthService: xử lý API
* Models

  * AuthResult
  * InstagramUser
* Config

  * AppConfig (clientId, redirectUri)

---

## 3. Các bước chính

### Step 1: Login

* Mở WebView2
* Điều hướng đến URL OAuth
* User đăng nhập

---

### Step 2: Lấy code

* Instagram redirect về:

```
https://localhost/?code=XXXX
```

* Parse:

```
?code=...
```

---

### Step 3: Đổi token

POST:

```
https://api.instagram.com/oauth/access_token
```

Body:

```
client_id
client_secret
grant_type=authorization_code
redirect_uri
code
```

---

### Step 4: Lấy user

GET:

```
https://graph.instagram.com/me
```

Params:

```
fields=id,username,account_type
access_token=...
```

---

## 4. Output

```json
{
  "id": "...",
  "username": "...",
  "account_type": "BUSINESS"
}
```

---

## 5. UI Flow (Dashboard)

```
Dashboard (Form)
   └── panelMain
         ├── HomeView
         ├── AccountView
```

### Cách hoạt động

* Dashboard giữ dữ liệu user
* Menu click → load View
* View nhận user qua constructor

---

## 6. Truyền dữ liệu giữa các màn

### Dashboard

```
private InstagramUser _user;
```

### Load View

```
LoadView(new HomeView(_user));
LoadView(new AccountView(_user));
```

### View nhận dữ liệu

```
public HomeView(InstagramUser user)
```

---

## 7. Lưu account (khuyến nghị)

Sau khi login thành công:

```json
{
  "id": "...",
  "username": "...",
  "accessToken": "..."
}
```

### Mục đích

* Không cần login lại
* Dùng multi account
* Dùng automation

---

## 8. Bảo mật

### Không nên

* Hard-code `clientSecret`
* Lưu `clientSecret` trong WinForms

### Nên

```
WinForms → Backend → Instagram API
```

---

## 9. Backend (Production)

### API cần có

```
POST /api/auth/exchange
```

### Flow

```
WinForms → gửi code → Backend
Backend → gọi Instagram
Backend → trả access_token
```

---

## 10. Các lỗi thường gặp

| Lỗi                         | Nguyên nhân            |
| --------------------------- | ---------------------- |
| Invalid redirect_uri        | Sai URL config         |
| Insufficient Developer Role | chưa add tester        |
| Page isn't available        | sai app type           |
| Không lấy được code         | redirectUri không khớp |
| Null user                   | chưa parse JSON        |

---

## 11. Mở rộng (MMO / Automation)

Có thể phát triển thêm:

### Account

* Multi account
* Switch account
* Lưu JSON / DB

### Automation

* Auto login bằng cookie
* Comment / inbox
* Đăng bài

### Hạ tầng

* Proxy từng account
* Anti-detect browser
* Session management

---

## 12. Best Practice

* Tách Service khỏi UI
* Không gọi API trực tiếp trong Form
* Dùng config JSON
* Dùng UserControl thay vì nhiều Form
* Cache View để tránh reload

---

## 13. Roadmap đề xuất

```
1. Login + lấy user
2. Lưu account
3. Multi account UI
4. Dashboard hoàn chỉnh
5. Backend (bảo mật)
6. Automation
```

---

## 14. Ghi chú

* OAuth chỉ dùng để login ban đầu
* Tool MMO thường dùng cookie/session thay vì OAuth
* WebView2 chỉ là bước đầu

---
