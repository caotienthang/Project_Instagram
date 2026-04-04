using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1.Helpers
{
    public static class SqliteHelper
    {
        private static string _dbPath = Path.Combine(Application.StartupPath, "SQLite", "data.db");

        public static string DbPath => _dbPath;

        public static void EnsureDatabase()
        {
            string folder = Path.Combine(Application.StartupPath, "SQLite");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            if (!File.Exists(_dbPath))
            {
                File.Create(_dbPath).Dispose();
                Console.WriteLine("SQLite database created at " + _dbPath);
            }

            CreateTables();
        }

        private static void CreateTables()
        {
            var connection = new SqliteConnection($"Data Source={_dbPath}");
            connection.Open();

            var cmd = connection.CreateCommand();

            // 1. Accounts
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS Accounts (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AccountId TEXT,
                FbAccountId TEXT,
                PhoneAccountId TEXT,
                FullName TEXT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT,
                Phone TEXT,
                Avatar TEXT,
                LinkAvatar TEXT,
                Birthday TEXT,
                Status TEXT
            );";
            cmd.ExecuteNonQuery();

            // Migration: add AccountId column to existing databases (legacy)
            try
            {
                cmd.CommandText = "ALTER TABLE Accounts ADD COLUMN AccountId TEXT;";
                cmd.ExecuteNonQuery();
            }
            catch { /* column already exists — ignore */ }

            // Migration: add FbAccountId column to existing databases
            try
            {
                cmd.CommandText = "ALTER TABLE Accounts ADD COLUMN FbAccountId TEXT;";
                cmd.ExecuteNonQuery();
            }
            catch { /* column already exists — ignore */ }

            // Migration: add PhoneAccountId column to existing databases
            try
            {
                cmd.CommandText = "ALTER TABLE Accounts ADD COLUMN PhoneAccountId TEXT;";
                cmd.ExecuteNonQuery();
            }
            catch { /* column already exists — ignore */ }

            // Migration: add Password column to existing databases
            try
            {
                cmd.CommandText = "ALTER TABLE Accounts ADD COLUMN Password TEXT;";
                cmd.ExecuteNonQuery();
            }
            catch { /* column already exists — ignore */ }

            // Migration: add LinkAvatar column to existing databases
            try
            {
                cmd.CommandText = "ALTER TABLE Accounts ADD COLUMN LinkAvatar TEXT;";
                cmd.ExecuteNonQuery();
            }
            catch { /* column already exists — ignore */ }

            // 2. AuthResults
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS AuthResults (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AccountId INTEGER NOT NULL,
                Code TEXT,
                AccessToken TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
            );";
            cmd.ExecuteNonQuery();

            // 3. InstagramSessions
            cmd.CommandText = @"
            CREATE TABLE IF NOT EXISTS InstagramSessions (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                AccountId INTEGER NOT NULL UNIQUE,
                Cookie TEXT,
                FbDtsg TEXT,
                Lsd TEXT,
                CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
                SessionIdPhone TEXT,
                DsUserIdPhone TEXT,
                CsrfTokenPhone TEXT,
                AuthorizationPhone TEXT,
                FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
            );";
            cmd.ExecuteNonQuery();

            // Migration: add phone session columns to existing databases
            try { cmd.CommandText = "ALTER TABLE InstagramSessions ADD COLUMN SessionIdPhone TEXT;";    cmd.ExecuteNonQuery(); } catch { /* column already exists — ignore */ }
            try { cmd.CommandText = "ALTER TABLE InstagramSessions ADD COLUMN DsUserIdPhone TEXT;";     cmd.ExecuteNonQuery(); } catch { /* column already exists — ignore */ }
            try { cmd.CommandText = "ALTER TABLE InstagramSessions ADD COLUMN CsrfTokenPhone TEXT;";    cmd.ExecuteNonQuery(); } catch { /* column already exists — ignore */ }
            try { cmd.CommandText = "ALTER TABLE InstagramSessions ADD COLUMN AuthorizationPhone TEXT;"; cmd.ExecuteNonQuery(); } catch { /* column already exists — ignore */ }

            connection.Close();
            Console.WriteLine("Tables created or already exist.");
        }

        public static SqliteConnection GetConnection()
        {
            var conn = new SqliteConnection($"Data Source={_dbPath}");
            conn.Open();
            return conn;
        }
    }
}
