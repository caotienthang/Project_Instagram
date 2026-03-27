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
                INSERT INTO Accounts (Username, FullName, Email, Phone, Avatar, Birthday, Status)
                VALUES (@Username, @FullName, @Email, @Phone, @Avatar, @Birthday, @Status)";

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
                INSERT INTO Accounts (Id, Username, FullName, Email, Phone, Avatar, Birthday, Status)
                VALUES (@Id, @Username, @FullName, @Email, @Phone, @Avatar, @Birthday, @Status)
                ON CONFLICT(Id) DO UPDATE SET
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

        // ================= HELPER =================
        private static AccountInfo Map(SqliteDataReader reader)
        {
            return new AccountInfo
            {
                Id = Convert.ToInt32(reader["Id"]),
                Username = reader["Username"]?.ToString(),
                FullName = reader["FullName"]?.ToString(),
                Email = reader["Email"]?.ToString(),
                Phone = reader["Phone"]?.ToString(),
                Avatar = reader["Avatar"]?.ToString(),
                Birthday = reader["Birthday"]?.ToString(),
                Status = reader["Status"]?.ToString()
            };
        }

        private static void BindParams(SqliteCommand cmd, AccountInfo acc, bool includeId)
        {
            if (includeId)
                cmd.Parameters.AddWithValue("@Id", acc.Id);

            cmd.Parameters.AddWithValue("@Username", acc.Username ?? "");
            cmd.Parameters.AddWithValue("@FullName", acc.FullName ?? "");
            cmd.Parameters.AddWithValue("@Email", acc.Email ?? "");
            cmd.Parameters.AddWithValue("@Phone", acc.Phone ?? "");
            cmd.Parameters.AddWithValue("@Avatar", acc.Avatar ?? "");
            cmd.Parameters.AddWithValue("@Birthday", acc.Birthday ?? "");
            cmd.Parameters.AddWithValue("@Status", acc.Status ?? "");
        }
    }
}