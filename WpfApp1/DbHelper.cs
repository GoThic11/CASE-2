using System;
using System.Collections.Generic;
using Npgsql;
using BCrypt.Net;

namespace WpfApp1
{
    public static class DbHelper
    {
        private static string connString = "Host=localhost;Port=5432;Username=postgres;Password=aa22aa;Database=postgres";

        public static bool RegisterUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count > 0) return false;
                }

                string hash = BCrypt.Net.BCrypt.HashPassword(password);

                using (var cmd = new NpgsqlCommand("INSERT INTO Users (Username, PasswordHash) VALUES (@username, @hash)", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("hash", hash);
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }

        public static bool ValidateUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT PasswordHash FROM Users WHERE Username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    var result = cmd.ExecuteScalar();

                    if (result == null) return false;

                    string storedHash = result.ToString();
                    return BCrypt.Net.BCrypt.Verify(password, storedHash);
                }
            }
        }
    }
}