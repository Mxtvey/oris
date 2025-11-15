using Npgsql;
using System;
using System.Data;

namespace Library
{
    
    public class Conection
    {
        private string connectionString = "host=localhost;port=5432;username=postgres;password=123456;";

        public Conection(string cs) => connectionString = cs;

        public IDbConnection Conn() => new NpgsqlConnection(connectionString);

        public void Table()
        {
            using var connection = Conn();
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
        CREATE TABLE IF NOT EXISTS migrations(
            id SERIAL PRIMARY KEY,
            name TEXT,
            applied_at TIMESTAMP,
            up_sql TEXT,
            down_sql TEXT
        );";
            cmd.ExecuteNonQuery();
        }
    }
}