# Two-Factor Authentication Phone API - Complete Implementation

## Overview
Hoàn thành implementation đầy đủ 2FA management qua Phone API, đảm bảo consistency khi user chọn phone để check 2FA thì tất cả operations (add, remove, refresh) đều dùng phone API.

## API Endpoints Implemented

### 1. Get Status (Check 2FA)
- **Endpoint**: `POST https://i.instagram.com/api/v1/bloks/apps/com.bloks.www.fx.settings.security.two_factor.totp.settings/`
- **Response Path**: `$.layout.bloks_payload.data[]` 
- **Device List**: Tìm object có `type == "gs"` và `data.key == "FX_TWO_FACTOR_TOTP_SEEDS:totp_seeds"`
- **Method**: `TwoFactorPhoneService.GetStatusByPhoneAsync()`

### 2. Generate Key (Add Device)
- **Endpoint**: `POST https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.generate_key/`
- **Payload**:
  ```json
  {
    "client_input_params": {
      "family_device_id": "<dynamic>",
      "device_id": "<dynamic>", 
      "machine_id": "<dynamic>",
      "key_nickname": "<optional - for additional devices>"
    },
    "server_params": {
      "requested_screen_component_type": null,
      "account_type": 1,
      "machine_id": null,
      "INTERNAL__latency_qpl_marker_id": 36707139,
      "INTERNAL__latency_qpl_instance_id": "<timestamp>",
      "account_id": "<fbAccountId>"
    }
  }
  ```
- **Response**: Returns `key_text`, `key_id`, `qr_code_uri` in action string
- **Method**: `TwoFactorPhoneService.AddTotpByPhoneAsync()`

### 3. Enable TOTP (Confirm Code)
- **Endpoint**: `POST https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.enable/`
- **Payload**: Same device IDs + `verification_code` (6 digits)
- **Success Indicators**:
  - `"FX_TWO_FACTOR_INFO:totp_enabled" true`
  - `"FX_TWO_FACTOR_STATUS:is_enabled" true`
  - `"ENABLE_TOTP_SUCCESS"`
- **Method**: `TwoFactorPhoneService.ConfirmTotpByPhoneAsync()`

### 4. Remove Key (Remove Device)
- **Endpoint**: `POST https://i.instagram.com/api/v1/bloks/async_action/com.bloks.www.fx.settings.security.two_factor.totp.remove_key/`
- **Payload**: Same device IDs + `totp_key_id` (device ID to remove)
- **Success Indicators**:
  - `"REMOVE_TOTP_KEY_SUCCESS"`
  - `"Device removed"`
- **Method**: `TwoFactorPhoneService.RemoveTotpByPhoneAsync()`

## Dynamic Device ID Generation

All device IDs are generated dynamically using cryptographic random:

```csharp
// 1. Family Device ID - UUID
private string GenerateFamilyDeviceId() 
{
    return Guid.NewGuid().ToString();
}

// 2. Android Device ID - "android-{16 hex chars}"
private string GenerateAndroidDeviceId()
{
    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] randomBytes = new byte[8];
        rng.GetBytes(randomBytes);
        return $"android-{BitConverter.ToString(randomBytes).Replace("-", "").ToLower()}";
    }
}

// 3. Machine ID - Base64 encoded 20 random bytes
private string GenerateMachineId()
{
    using (var rng = new RNGCryptoServiceProvider())
    {
        byte[] randomBytes = new byte[20];
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }
}
```

## Consistency Logic - TwoFactorManagerDialog

### Architecture
- **Field**: `private readonly bool _usePhoneApi;`
- **Initialized in Constructor**: Based on `session.AuthorizationPhone`
- **Principle**: Once determined, all operations use the same API

### Flow
```
User clicks "2FA" in Dashboard
    ↓
Dashboard checks session type & asks user (if both available)
    ↓
Calls appropriate GetStatus API (Phone or Computer)
    ↓
Opens TwoFactorManagerDialog with session
    ↓
Dialog detects: _usePhoneApi = !string.IsNullOrWhiteSpace(session.AuthorizationPhone)
    ↓
All operations in dialog use consistent service:
    - Add Device    → Uses _usePhoneApi flag
    - Remove Device → Uses _usePhoneApi flag  
    - Refresh       → Uses _usePhoneApi flag
```

## Key Features

### 1. Add Device with Nickname
- First device: No nickname required
- Additional devices: Prompt for nickname
- Nickname sent in `client_input_params.key_nickname`

### 2. Response Parsing
- **Get Status**: Parse from `$.layout.bloks_payload.data`
- **Other APIs**: Parse from action string with regex patterns

### 3. Error Handling
- Pattern: `dq8 "<key>" "<error_message>"`
- Extracted via regex: `dq8\s+"[^"]*"\s+"([^"]*)"`

### 4. Logging
All responses saved to `logs/` folder:
- `twofactor_status_{timestamp}.json`
- `twofactor_generate_{timestamp}.json`
- `twofactor_enable_{timestamp}.json`
- `twofactor_remove_{timestamp}.json`

## Code Changes

### TwoFactorPhoneService.cs
- ✅ Added `key_nickname` support in AddTotpByPhoneAsync
- ✅ Updated timestamp to use `DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()`
- ✅ Fixed response path for GetStatusByPhoneAsync
- ✅ All device IDs generated dynamically

### TwoFactorManagerDialog.cs
- ✅ Added `_usePhoneApi` flag
- ✅ Added `_phoneService` instance
- ✅ Updated all operations to use consistent service:
  - `RemoveDevice_ClickAsync()`
  - `btnAdd_Click()`
  - `RefreshStatusAsync()`
- ✅ Removed device selection dialog (uses flag instead)

### Dashboard.cs
- ✅ Asks user to choose phone/computer when checking 2FA
- ✅ Auto-selects if only one session type available
- ✅ Logs which method is being used

## Usage Example

### Scenario: Account has Phone Session
1. User clicks "2FA" button
2. Dashboard detects: `session.AuthorizationPhone` exists
3. Auto-select or ask: "Phone API"
4. Call: `TwoFactorPhoneService.GetStatusByPhoneAsync()`
5. Open: `TwoFactorManagerDialog` with session
6. Dialog sets: `_usePhoneApi = true`
7. All operations in dialog:
   - Add → `_phoneService.AddTotpByPhoneAsync()`
   - Remove → `_phoneService.RemoveTotpByPhoneAsync()`
   - Refresh → `_phoneService.GetStatusByPhoneAsync()`

### Scenario: Account has Computer Session Only
1. User clicks "2FA" button
2. Dashboard detects: Only `session.Cookie` exists
3. Auto-select: "Computer API"
4. Call: `TwoFactorService.GetStatusAsync()`
5. Open: `TwoFactorManagerDialog` with session
6. Dialog sets: `_usePhoneApi = false`
7. All operations in dialog:
   - Add → `_service.GenerateTotpKeyAsync()` + WebView2
   - Remove → `_service.DisableTotpAsync()`
   - Refresh → `_service.GetStatusAsync()`

## Benefits

1. **Consistency**: Once user chooses method, all operations use same API
2. **No confusion**: No mixing phone and computer APIs in single session
3. **Clean code**: Single source of truth via `_usePhoneApi` flag
4. **Better UX**: User doesn't need to choose method for each operation
5. **Production ready**: Dynamic device IDs, proper error handling, logging

## Testing Checklist

- [ ] Check 2FA status via phone
- [ ] Check 2FA status via computer
- [ ] Add first device via phone
- [ ] Add additional device via phone (with nickname)
- [ ] Remove device via phone
- [ ] Add device via computer
- [ ] Remove device via computer
- [ ] Account with both sessions - choose phone
- [ ] Account with both sessions - choose computer
- [ ] Verify all logs are saved correctly
- [ ] Verify device IDs are unique each time
- [ ] Test error cases (invalid code, network error)

## Next Steps

Consider adding:
- [ ] SMS 2FA support via phone API
- [ ] Backup codes management
- [ ] 2FA disable functionality via phone API
- [ ] Batch 2FA operations for multiple accounts
