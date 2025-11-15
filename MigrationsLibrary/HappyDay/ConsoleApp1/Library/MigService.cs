using System;
using System.Reflection;

namespace Library
{
    public class MigrationService
    {
        private readonly Conection db;
        private readonly MigrationRepository repo;
        private readonly Assembly models;

        public MigrationService(Conection conection, Assembly asm)
        {
            db = conection;
            repo = new MigrationRepository(conection);
            models = asm;
        }

        public MigrationResponse Create()
        {
            var result = MigrationGenerator.Generate(models);

            if (string.IsNullOrEmpty(result.up))
            {
                return new MigrationResponse
                {
                    Name = "",
                    Status = "no_changes"
                };
            }

            string name = "Migration" + DateTime.Now.ToString("yyyyMMddHHmmss");

            repo.Save(name, result.up, result.down);

            return new MigrationResponse
            {
                Name = name,
                Status = "created"
            };
        }

        public MigrationResponse Apply()
        {
            var mig = repo.GetLastPending();

            if (mig == null)
            {
                return new MigrationResponse
                {
                    Name = "",
                    Status = "nothing_to_apply"
                };
            }

            var conn = db.Conn();
            conn.Open();
            var tx = conn.BeginTransaction();

            try
            {
                var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = mig.Value.up;
                cmd.ExecuteNonQuery();

                tx.Commit();
            }
            catch
            {
                tx.Rollback();
                conn.Close();

                return new MigrationResponse
                {
                    Name = mig.Value.name,
                    Status = "error"
                };
            }

            conn.Close();
            repo.MarkApplied(mig.Value.id);

            return new MigrationResponse
            {
                Name = mig.Value.name,
                Status = "applied"
            };
        }
    }
}
