using System;

namespace Library
{
    public class MigrationRepository
    {
        private readonly Conection con;

        public MigrationRepository(Conection connection)
        {
            con = connection;
            con.Table(); 
        }

        public void Save(string name, string upSql, string downSql)
        {
            var conn = con.Conn();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "INSERT INTO migrations (name, applied_at, up_sql, down_sql) " +
                "VALUES (@name, NULL, @up, @down)";

            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("name", name));
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("up", upSql));
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("down", downSql));

            cmd.ExecuteNonQuery();
            conn.Close();
        }

        public (int id, string name, string up)? GetLastPending()
        {
            var conn = con.Conn();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT id, name, up_sql FROM migrations " +
                "WHERE applied_at IS NULL ORDER BY id DESC LIMIT 1;";

            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var up = reader.GetString(2);

                conn.Close();
                return (id, name, up);
            }

            conn.Close();
            return null;
        }

        public void MarkApplied(int id)
        {
            var conn = con.Conn();
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText =
                "UPDATE migrations SET applied_at = NOW() WHERE id=@id;";

            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("id", id));
            cmd.ExecuteNonQuery();
            conn.Close();
        }
    }
}
