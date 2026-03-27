using Microsoft.Data.Sqlite;
using System;
using WindowsFormsApp1.Helpers;
using WindowsFormsApp1.Models;

namespace WindowsFormsApp1.Data
{
    public static class InstagramSessionRepository
    {
        // ================= UPSERT =================
        public static void Upsert(InstagramSession s)
        {
            using (var conn = new SqliteConnection($"Data Source={SqliteHelper.DbPath}"))
            {
                conn.Open();

                var cmd = conn.CreateCommand();
                cmd.CommandText = @"
                INSERT INTO InstagramSessions (AccountId, Cookie, FbDtsg, Lsd, CreatedAt)
                VALUES (@AccountId, @Cookie, @FbDtsg, @Lsd, @CreatedAt)
                ON CONFLICT(AccountId) DO UPDATE SET
                    Cookie=excluded.Cookie,
                    FbDtsg=excluded.FbDtsg,
                    Lsd=excluded.Lsd,
                    CreatedAt=excluded.CreatedAt;
                ";

                BindParams(cmd, s);

                cmd.ExecuteNonQuery();
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
                Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                AccountId = Convert.ToInt32(reader["AccountId"]),
                Cookie = reader["Cookie"]?.ToString(),
                FbDtsg = reader["FbDtsg"]?.ToString(),
                Lsd = reader["Lsd"]?.ToString(),
                CreatedAt = DateTime.TryParse(reader["CreatedAt"]?.ToString(), out var dt)
                    ? dt
                    : DateTime.Now
            };
        }

        private static void BindParams(SqliteCommand cmd, InstagramSession s)
        {
            cmd.Parameters.AddWithValue("@AccountId", s.AccountId);
            cmd.Parameters.AddWithValue("@Cookie", s.Cookie ?? "");
            cmd.Parameters.AddWithValue("@FbDtsg", s.FbDtsg ?? "");
            cmd.Parameters.AddWithValue("@Lsd", s.Lsd ?? "");
            cmd.Parameters.AddWithValue("@CreatedAt", s.CreatedAt);
        }
    }
}