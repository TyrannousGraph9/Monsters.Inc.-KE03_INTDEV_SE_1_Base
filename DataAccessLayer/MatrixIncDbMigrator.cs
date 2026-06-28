using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DataAccessLayer
{
    public static class MatrixIncDbMigrator
    {
        public static void Apply(MatrixIncDbContext context)
        {
            if (ColumnExists(context, "Orders", "Status"))
            {
                return;
            }

            context.Database.ExecuteSqlRaw("ALTER TABLE Orders ADD COLUMN Status INTEGER NOT NULL DEFAULT 0");

            context.Database.ExecuteSqlRaw("UPDATE Orders SET Status = 3 WHERE Id = 1");
            context.Database.ExecuteSqlRaw("UPDATE Orders SET Status = 2 WHERE Id = 2");
            context.Database.ExecuteSqlRaw("UPDATE Orders SET Status = 1 WHERE Id = 3");
        }

        private static bool ColumnExists(MatrixIncDbContext context, string tableName, string columnName)
        {
            var connection = context.Database.GetDbConnection();
            var wasOpen = connection.State == ConnectionState.Open;

            if (!wasOpen)
            {
                connection.Open();
            }

            try
            {
                using var command = connection.CreateCommand();
                command.CommandText = $"PRAGMA table_info(\"{tableName}\")";

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }

                return false;
            }
            finally
            {
                if (!wasOpen)
                {
                    connection.Close();
                }
            }
        }
    }
}
