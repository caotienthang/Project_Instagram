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
                FullName TEXT,
                Username TEXT NOT NULL UNIQUE,
                Email TEXT,
                Phone TEXT,
                Avatar TEXT,
                Birthday TEXT,
                Status TEXT
            );";
            cmd.ExecuteNonQuery();

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
                FOREIGN KEY (AccountId) REFERENCES Accounts(Id) ON DELETE CASCADE
            );";
            cmd.ExecuteNonQuery();

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
