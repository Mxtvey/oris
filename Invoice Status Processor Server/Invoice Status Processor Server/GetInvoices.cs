using Invoice_Status_Processor_Server;
using Microsoft.Data.SqlClient;
using Npgsql;

class GetInvoices
{
        string connectionString = "Host=localhost;Port=5432;Database=exmpl;Username=postgres;Password=1234";
        List<Invoice> invoices;
        public async Task GetPedding()
        {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {   
                        await connection.OpenAsync();
                        SqlCommand command = new SqlCommand();
                        command.CommandText= "SELECT * FROM invoice WHERE status = 'pending';";
                        command.Connection = connection;
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                                invoices.Add(new Invoice {
                                        id = reader.GetInt32(0),
                                        bankname = reader.GetString(1),
                                        amount = reader.GetDecimal(2),
                                        status = reader.GetString(3)
                                });    
                        }
                        Console.WriteLine("peding получен");
                }
        }

        public void Random()
        {
                Random random = new Random();
                foreach (var inv in invoices)
                {
                       var rnd =  random.Next(0, 100);
                       if (rnd < 30)
                       {
                               inv.status = "success";
                       }
                       else
                       {
                               inv.status = "error";
                       }
                       inv.updateat = DateTime.Now;
                }
                Console.WriteLine("status изменен");
        }

        public async Task UpdateInvoicesAsync()
        {
                string sql = "UPDATE invoice SET status = @status, updated_at = NOW() WHERE id = @id;";

                using var connection = new NpgsqlConnection(connectionString);
                await connection.OpenAsync();

                using var command = new NpgsqlCommand(sql, connection);
                command.Parameters.Add("@status");
                command.Parameters.Add("@id");
                command.Parameters.Add("@updated_at");

                foreach (var inv in invoices)
                {
                        command.Parameters["@status"].Value = inv.status;
                        command.Parameters["@id"].Value = inv.id;
                        command.Parameters["@updateat"].Value= inv.updateat;

                        await command.ExecuteNonQueryAsync();
                }

                Console.WriteLine("Статусы сохранены в БД");
        }

}
