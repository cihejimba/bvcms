using Consolas.Core;
using DbGenerator.Args;
using DbGenerator.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Text;

namespace DbGenerator.Commands
{
    public class CreateCommand : Command
    {
        public object Execute(CreateArgs args)
        {
            var cmsDbManifestPath = Path.Combine(args.ScriptDirectory, "allscripts.txt");
            var imageDbManifestPath = Path.Combine(args.ScriptDirectory, "BuildImageDatabase.sql");
            var migrationDirectoryPath = Path.Combine(args.ScriptDirectory, "migration");
            var schemaDirectoryPath = Path.Combine(args.ScriptDirectory, "schema");
            var seedDirectoryPath = Path.Combine(args.ScriptDirectory, "seed");
            var cmsDatabaseName = $"CMS_{args.DatabaseName}";
            var imageDatabaseName = $"CMSi_{args.DatabaseName}";

            var scriptFiles = new List<string>(File.ReadLines(cmsDbManifestPath));
            scriptFiles.AddRange(Directory.EnumerateFiles(seedDirectoryPath));
            scriptFiles.AddRange(Directory.EnumerateFiles(migrationDirectoryPath));

            var queryBuilder = new StringBuilder();

            queryBuilder.AppendLine($"create database {cmsDatabaseName}");
            queryBuilder.AppendLine("go");
            queryBuilder.AppendLine($"create database {imageDatabaseName}");
            queryBuilder.AppendLine("go");
            queryBuilder.AppendLine($"use {cmsDatabaseName}");
            queryBuilder.AppendLine("go");

            foreach (var file in scriptFiles)
            {
                var script = Path.Combine(schemaDirectoryPath, file);
                var sql = File.ReadAllText(script);

                queryBuilder.AppendLine(sql);
                queryBuilder.AppendLine("go");
            }

            queryBuilder.AppendLine($"use {imageDatabaseName}");
            queryBuilder.AppendLine("go");
            queryBuilder.AppendLine(File.ReadAllText(imageDbManifestPath));
            queryBuilder.AppendLine("go");

            var sqlText = queryBuilder.ToString().Replace("ro-CMS_StarterDb", $"ro-{cmsDatabaseName}").Replace("p@ssw0rd", "Micah123");

            using (var connection = new SqlConnection(args.ConnectionString))
            {
                connection.Open();

                foreach (var sqlBatch in sqlText.Split(new[] { "\r\nGO\r\n", "\r\ngo\r\n", "\r\nGo\r\n", "\r\ngO\r\n" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = sqlBatch;
                        command.CommandTimeout = 60;
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }

            return View("CreateResultsView", new CreateDatabaseResultsViewModel());
        }
    }
}
