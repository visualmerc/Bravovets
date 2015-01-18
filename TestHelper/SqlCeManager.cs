using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestHelper
{
    using System.Configuration;
    using System.Data.SqlServerCe;
    using System.IO;

    public class SqlCeManager
    {

        public void CreateBravoVetsCeDatabase()
        {
            string connectionString;
            string fileName = @"C:\CeShare\bravovets.sdf";
            string password = "C0mpactRul3z";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            connectionString = string.Format("DataSource=\"{0}\"; Password='{1}'", fileName, password);
            var en = new SqlCeEngine(connectionString);
            en.CreateDatabase();

            this.PopulateCeDatabase(connectionString);
        }

        private void PopulateCeDatabase(string connectionString)
        {
            var cmdReturn = 0;

            using (var connection = new SqlCeConnection(connectionString))
            {

                string[] commands;
                string[] commands2;

                // Script is embedded as an application resource.  Retrieve
                // script from resource

                commands = Properties.Resources.bvsqlce
                    .Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                commands2 = Properties.Resources.bvsqlce_data
                    .Split(new string[] { "GO" }, StringSplitOptions.RemoveEmptyEntries);
                var cmd = new SqlCeCommand();

                cmd.Connection = connection;
                connection.Open();

                // Loop through the command strings

                foreach (string command in from command in commands let trimmed = command.Trim() where !string.IsNullOrEmpty(trimmed) select command)
                {
                    cmd.CommandText = command;
                    cmdReturn = cmd.ExecuteNonQuery();
                }

                foreach (string command in from command in commands2 let trimmed = command.Trim() where !string.IsNullOrEmpty(trimmed) select command)
                {
                    cmd.CommandText = command;
                    cmdReturn = cmd.ExecuteNonQuery();
                }

                connection.Close();
            }

        }

    }
}
