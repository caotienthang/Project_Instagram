using Microsoft.Data.Sqlite;
using System;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Data
{
    public static class InstagramSessionRepository
    {
        // ================= UPSERT (web session) =================
        public static void Upsert(InstagramSession s)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO InstagramSessions (AccountId, Cookie, FbDtsg, Lsd, CreatedAt,
                    SessionIdPhone, DsUserIdPhone, CsrfTokenPhone, AuthorizationPhone)
                VALUES (@AccountId, @Cookie, @FbDtsg, @Lsd, @CreatedAt,
                    @SessionIdPhone, @DsUserIdPhone, @CsrfTokenPhone, @AuthorizationPhone)
                ON CONFLICT(AccountId) DO UPDATE SET
                    Cookie=excluded.Cookie,
                    FbDtsg=excluded.FbDtsg,
                    Lsd=excluded.Lsd,
                    CreatedAt=excluded.CreatedAt,
                    SessionIdPhone=excluded.SessionIdPhone,
                    DsUserIdPhone=excluded.DsUserIdPhone,
                    CsrfTokenPhone=excluded.CsrfTokenPhone,
                    AuthorizationPhone=excluded.AuthorizationPhone;
                ";

                BindParams(cmd, s);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= UPSERT PHONE SESSION =================
        // Tìm hoặc tạo account dựa vào FbAccountId hoặc PhoneAccountId, sau đó upsert session
        public static void UpsertPhone(string fbAccountId, string phoneAccountId, 
            string username, string fullName, string avatar, string phone,
            string sessionIdPhone, string dsUserIdPhone, string csrfTokenPhone, string authorizationPhone)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                // Tìm account dựa vào FbAccountId hoặc PhoneAccountId
                int accountId;
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT Id FROM Accounts 
                    WHERE (FbAccountId=@FbAccountId AND FbAccountId IS NOT NULL AND FbAccountId != '') 
                       OR (PhoneAccountId=@PhoneAccountId AND PhoneAccountId IS NOT NULL AND PhoneAccountId != '')
                    LIMIT 1";
                cmd.Parameters.AddWithValue("@FbAccountId", fbAccountId ?? "");
                cmd.Parameters.AddWithValue("@PhoneAccountId", phoneAccountId ?? "");

                var result = cmd.ExecuteScalar();

                if (result != null)
                {
                    // Account đã tồn tại - update thông tin
                    accountId = Convert.ToInt32(result);

                    var updateCmd = conn.CreateCommand();
                    updateCmd.CommandText = @"
                        UPDATE Accounts SET
                            FbAccountId=@FbAccountId,
                            PhoneAccountId=@PhoneAccountId,
                            Username=@Username,
                            FullName=@FullName,
                            Avatar=@Avatar,
                            Phone=@Phone
                        WHERE Id=@Id";
                    updateCmd.Parameters.AddWithValue("@Id", accountId);
                    updateCmd.Parameters.AddWithValue("@FbAccountId", fbAccountId ?? "");
                    updateCmd.Parameters.AddWithValue("@PhoneAccountId", phoneAccountId ?? "");
                    updateCmd.Parameters.AddWithValue("@Username", username ?? "");
                    updateCmd.Parameters.AddWithValue("@FullName", fullName ?? "");
                    updateCmd.Parameters.AddWithValue("@Avatar", avatar ?? "");
                    updateCmd.Parameters.AddWithValue("@Phone", phone ?? "");
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    // Account chưa tồn tại - tạo mới
                    var insertCmd = conn.CreateCommand();
                    insertCmd.CommandText = @"
                        INSERT INTO Accounts (FbAccountId, PhoneAccountId, Username, FullName, Avatar, Phone, Status)
                        VALUES (@FbAccountId, @PhoneAccountId, @Username, @FullName, @Avatar, @Phone, 'active');
                        SELECT last_insert_rowid();";
                    insertCmd.Parameters.AddWithValue("@FbAccountId", fbAccountId ?? "");
                    insertCmd.Parameters.AddWithValue("@PhoneAccountId", phoneAccountId ?? "");
                    insertCmd.Parameters.AddWithValue("@Username", username ?? "");
                    insertCmd.Parameters.AddWithValue("@FullName", fullName ?? "");
                    insertCmd.Parameters.AddWithValue("@Avatar", avatar ?? "");
                    insertCmd.Parameters.AddWithValue("@Phone", phone ?? "");

                    accountId = Convert.ToInt32(insertCmd.ExecuteScalar());
                }

                // Upsert session
                var sessionCmd = conn.CreateCommand();
                sessionCmd.CommandText = @"
                    INSERT INTO InstagramSessions (AccountId, SessionIdPhone, DsUserIdPhone, CsrfTokenPhone, AuthorizationPhone, CreatedAt)
                    VALUES (@AccountId, @SessionIdPhone, @DsUserIdPhone, @CsrfTokenPhone, @AuthorizationPhone, @CreatedAt)
                    ON CONFLICT(AccountId) DO UPDATE SET
                        SessionIdPhone=excluded.SessionIdPhone,
                        DsUserIdPhone=excluded.DsUserIdPhone,
                        CsrfTokenPhone=excluded.CsrfTokenPhone,
                        AuthorizationPhone=excluded.AuthorizationPhone;
                    ";

                sessionCmd.Parameters.AddWithValue("@AccountId", accountId);
                sessionCmd.Parameters.AddWithValue("@SessionIdPhone", sessionIdPhone ?? "");
                sessionCmd.Parameters.AddWithValue("@DsUserIdPhone", dsUserIdPhone ?? "");
                sessionCmd.Parameters.AddWithValue("@CsrfTokenPhone", csrfTokenPhone ?? "");
                sessionCmd.Parameters.AddWithValue("@AuthorizationPhone", authorizationPhone ?? "");
                sessionCmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                sessionCmd.ExecuteNonQuery();
            }
        }

        // ================= GET BY ACCOUNT =================
        public static InstagramSession GetByAccountId(int accountId)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                SELECT * FROM InstagramSessions 
                WHERE AccountId = @AccountId 
                LIMIT 1;
                ";

                cmd.Parameters.AddWithValue("@AccountId", accountId);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }

            return null;
        }

        // ================= DELETE =================
        public static void DeleteByAccountId(int accountId)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "DELETE FROM InstagramSessions WHERE AccountId=@AccountId";

                cmd.Parameters.AddWithValue("@AccountId", accountId);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= HELPER =================
        private static InstagramSession Map(SqliteDataReader reader)
        {
            return new InstagramSession
            {
                Id        = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                AccountId = Convert.ToInt32(reader["AccountId"]),
                Cookie    = reader["Cookie"]?.ToString(),
                FbDtsg    = reader["FbDtsg"]?.ToString(),
                Lsd       = reader["Lsd"]?.ToString(),
                CreatedAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var dt)
                    ? dt
                    : DateTime.Now,
                SessionIdPhone     = reader["SessionIdPhone"]?.ToString(),
                DsUserIdPhone      = reader["DsUserIdPhone"]?.ToString(),
                CsrfTokenPhone     = reader["CsrfTokenPhone"]?.ToString(),
                AuthorizationPhone = reader["AuthorizationPhone"]?.ToString()
            };
        }

        private static void BindParams(SqliteCommand cmd, InstagramSession s)
        {
            cmd.Parameters.AddWithValue("@AccountId",          s.AccountId);
            cmd.Parameters.AddWithValue("@Cookie",             s.Cookie             ?? "");
            cmd.Parameters.AddWithValue("@FbDtsg",             s.FbDtsg             ?? "");
            cmd.Parameters.AddWithValue("@Lsd",                s.Lsd                ?? "");
            cmd.Parameters.AddWithValue("@CreatedAt",          s.CreatedAt);
            cmd.Parameters.AddWithValue("@SessionIdPhone",     s.SessionIdPhone     ?? "");
            cmd.Parameters.AddWithValue("@DsUserIdPhone",      s.DsUserIdPhone      ?? "");
            cmd.Parameters.AddWithValue("@CsrfTokenPhone",     s.CsrfTokenPhone     ?? "");
            cmd.Parameters.AddWithValue("@AuthorizationPhone", s.AuthorizationPhone ?? "");
        }
    }
}