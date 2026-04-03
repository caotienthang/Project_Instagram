using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Data
{
    public static class AccountRepository
    {
        // ================= GET ALL =================
        public static List<AccountInfo> GetAll()
        {
            var list = new List<AccountInfo>();

            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Accounts";

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));
                    }
                }
            }

            return list;
        }

        // ================= INSERT =================
        public static void Insert(AccountInfo acc)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Accounts (FbAccountId, PhoneAccountId, Username, FullName, Email, Phone, Avatar, Birthday, Status)
                VALUES (@FbAccountId, @PhoneAccountId, @Username, @FullName, @Email, @Phone, @Avatar, @Birthday, @Status)";

                BindParams(cmd, acc, includeId: false);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= UPDATE =================
        public static void Update(AccountInfo acc)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                UPDATE Accounts SET
                    FbAccountId=@FbAccountId,
                    PhoneAccountId=@PhoneAccountId,
                    Username=@Username,
                    FullName=@FullName,
                    Email=@Email,
                    Phone=@Phone,
                    Avatar=@Avatar,
                    Birthday=@Birthday,
                    Status=@Status
                WHERE Id=@Id";

                BindParams(cmd, acc, includeId: true);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= UPSERT (THAY SaveAccount) =================
        public static void Upsert(AccountInfo acc)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO Accounts (Id, FbAccountId, PhoneAccountId, Username, FullName, Email, Phone, Avatar, Birthday, Status)
                VALUES (@Id, @FbAccountId, @PhoneAccountId, @Username, @FullName, @Email, @Phone, @Avatar, @Birthday, @Status)
                ON CONFLICT(Id) DO UPDATE SET
                    FbAccountId=excluded.FbAccountId,
                    PhoneAccountId=excluded.PhoneAccountId,
                    Username=excluded.Username,
                    FullName=excluded.FullName,
                    Email=excluded.Email,
                    Phone=excluded.Phone,
                    Avatar=excluded.Avatar,
                    Birthday=excluded.Birthday,
                    Status=excluded.Status;
                ";

                BindParams(cmd, acc, includeId: true);

                cmd.ExecuteNonQuery();
            }
        }

        // ================= GET BY USERNAME =================
        public static AccountInfo GetByUsername(string username)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT * FROM Accounts WHERE Username=@Username";
                cmd.Parameters.AddWithValue("@Username", username);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }

            return null;
        }

        // ================= GET BY FB ACCOUNT ID OR PHONE ACCOUNT ID =================
        public static AccountInfo GetByAccountIds(string fbAccountId, string phoneAccountId)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                    SELECT * FROM Accounts 
                    WHERE (FbAccountId=@FbAccountId AND FbAccountId IS NOT NULL AND FbAccountId != '') 
                       OR (PhoneAccountId=@PhoneAccountId AND PhoneAccountId IS NOT NULL AND PhoneAccountId != '')
                    LIMIT 1";
                cmd.Parameters.AddWithValue("@FbAccountId", fbAccountId ?? "");
                cmd.Parameters.AddWithValue("@PhoneAccountId", phoneAccountId ?? "");

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return Map(reader);
                }
            }

            return null;
        }

        // ================= UPDATE AVATAR =================
        public static void UpdateAvatar(int accountId, string avatarPath)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Accounts SET Avatar=@Avatar WHERE Id=@Id";

                cmd.Parameters.AddWithValue("@Id", accountId);
                cmd.Parameters.AddWithValue("@Avatar", avatarPath ?? "");

                cmd.ExecuteNonQuery();
            }
        }

        // ================= UPDATE PASSWORD =================
        public static void UpdatePassword(int accountId, string newPassword)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = "UPDATE Accounts SET Password=@Password WHERE Id=@Id";

                cmd.Parameters.AddWithValue("@Id", accountId);
                cmd.Parameters.AddWithValue("@Password", newPassword ?? "");

                cmd.ExecuteNonQuery();
            }
        }

        // ================= HELPER =================
        private static AccountInfo Map(SqliteDataReader reader)
        {
            return new AccountInfo
            {
                Id             = Convert.ToInt32(reader["Id"]),
                FbAccountId    = reader["FbAccountId"]?.ToString(),
                PhoneAccountId = reader["PhoneAccountId"]?.ToString(),
                Username       = reader["Username"]?.ToString(),
                FullName       = reader["FullName"]?.ToString(),
                Email          = reader["Email"]?.ToString(),
                Phone          = reader["Phone"]?.ToString(),
                Avatar         = reader["Avatar"]?.ToString(),
                Birthday       = reader["Birthday"]?.ToString(),
                Status         = reader["Status"]?.ToString(),
                Password       = reader["Password"]?.ToString()
            };
        }

        private static void BindParams(SqliteCommand cmd, AccountInfo acc, bool includeId)
        {
            if (includeId)
                cmd.Parameters.AddWithValue("@Id", acc.Id);

            cmd.Parameters.AddWithValue("@FbAccountId",    acc.FbAccountId    ?? "");
            cmd.Parameters.AddWithValue("@PhoneAccountId", acc.PhoneAccountId ?? "");
            cmd.Parameters.AddWithValue("@Username",       acc.Username       ?? "");
            cmd.Parameters.AddWithValue("@FullName",       acc.FullName       ?? "");
            cmd.Parameters.AddWithValue("@Email",          acc.Email          ?? "");
            cmd.Parameters.AddWithValue("@Phone",          acc.Phone          ?? "");
            cmd.Parameters.AddWithValue("@Avatar",         acc.Avatar         ?? "");
            cmd.Parameters.AddWithValue("@Birthday",       acc.Birthday       ?? "");
            cmd.Parameters.AddWithValue("@Status",         acc.Status         ?? "");
        }
    }
}