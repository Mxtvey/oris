using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Microsoft.Data.SqlClient;
using Npgsql;

public class ORMContext
{
    private readonly string _connectionString;
 
    public ORMContext(string connectionString)
    {
        _connectionString = connectionString;
    }
 
    public T Create<T>(T entity, string tableName) where T : class
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();
        
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

     
        var columns = props.Where(p => p.Name.ToLower() != "id").ToArray();

        var columnNames = string.Join(", ", columns.Select(p => p.Name.ToLower()));
        var paramNames = string.Join(", ", columns.Select(p => "@" + p.Name.ToLower()));

        var sql = $"INSERT INTO {tableName} ({columnNames}) VALUES ({paramNames}) RETURNING *;";

        using var cmd = new NpgsqlCommand(sql, connection);

      
        foreach (var prop in columns)
        {
            var value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@" + prop.Name.ToLower(), value);
        }

      
        using var reader = cmd.ExecuteReader();

        if (reader.Read())
        {
            var newEntity = Activator.CreateInstance<T>();
            foreach (var prop in props)
            {
                if (!reader.HasColumn(prop.Name.ToLower())) continue;

                var val = reader[prop.Name.ToLower()];
                if (val != DBNull.Value)
                    prop.SetValue(newEntity, val);
            }

            return newEntity;
        }

        return entity;
    }
    
   
 
    public T? ReadById<T>(int id, string tableName) where T : class, new()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        string sql = $"SELECT * FROM {tableName} WHERE id = @id";
        using var cmd = new NpgsqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", id);

        using var reader = cmd.ExecuteReader();

        if (!reader.Read())
            return null;

        var entity = new T();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        var columns = new HashSet<string>();
        for (int i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i).ToLower());

       
        foreach (var prop in props)
        {
            var colName = prop.Name.ToLower();
            if (!columns.Contains(colName)) continue;

            var value = reader[colName];
            if (value != DBNull.Value)
                prop.SetValue(entity, value);
        }

        return entity;
    }
 
    public List<T> ReadAll<T>(string tableName) where T : class, new()
    {
        var result = new List<T>();

        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        string sql = $"SELECT * FROM {tableName}";
        using var cmd = new NpgsqlCommand(sql, connection);
        using var reader = cmd.ExecuteReader();

        if (!reader.HasRows)
            return result;
        
        var columns = new HashSet<string>();
        for (int i = 0; i < reader.FieldCount; i++)
            columns.Add(reader.GetName(i).ToLower());

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        while (reader.Read())
        {
            var entity = new T();

            foreach (var prop in props)
            {
                var colName = prop.Name.ToLower();
                if (!columns.Contains(colName)) continue;
                var value = reader[colName];
                if (value != DBNull.Value)
                    prop.SetValue(entity, value);
            }

            result.Add(entity);
        }
        return result;
    }

    public void Update<T>(int id, T entity, string tableName)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        connection.Open();

        // Получаем все публичные свойства объекта
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        // Исключаем поле Id, т.к. оно ключевое
        var columns = props.Where(p => !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase)).ToArray();

        // Формируем SQL строку с параметрами
        var setClause = string.Join(", ", columns.Select(p => $"{p.Name.ToLower()} = @{p.Name.ToLower()}"));
        var sql = $"UPDATE {tableName} SET {setClause} WHERE id = @id;";

        using var cmd = new NpgsqlCommand(sql, connection);

        // Добавляем параметры для всех свойств
        foreach (var prop in columns)
        {
            var value = prop.GetValue(entity) ?? DBNull.Value;
            cmd.Parameters.AddWithValue("@" + prop.Name.ToLower(), value);
        }

        // Добавляем параметр для id
        cmd.Parameters.AddWithValue("@id", id);

        // Выполняем запрос
        cmd.ExecuteNonQuery();
    }


    public bool Delete(int id, string tableName)
    {
            // ВАЖНО: tableName нельзя параметризовать — проверь/белый список!
            using var connection = new NpgsqlConnection(_connectionString);
            connection.Open();

            var sql = $"DELETE FROM {tableName} WHERE id = @id;";
            using var cmd = new NpgsqlCommand(sql, connection);
            cmd.Parameters.AddWithValue("@id", id);

            var affected = cmd.ExecuteNonQuery();
            return affected > 0; // true, если строка удалена
    }

}