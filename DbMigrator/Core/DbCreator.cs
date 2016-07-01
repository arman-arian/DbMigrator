using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;

namespace DbMigrator.Core
{
    public static class DbCreator
    {
        public class Database
        {
            public string NAME { get; set; }

            public string COLLATION { get; set; }
        }

        public abstract class DatabaseFile
        {
            public string NAME { get; set; }

            public string FILENAME { get; set; }

            public decimal SIZE { get; set; }

            public decimal MAXSIZE { get; set; }

            public decimal FILEGROWTH { get; set; }
        }

        public sealed class LogFile : DatabaseFile
        {


        }

        public abstract class DataFile : DatabaseFile
        {

        }

        public sealed class MasterDataFile : DataFile
        {

        }

        public sealed class NextDataFile : DataFile
        {

        }

        public static bool IsDatabaseExists<T>() where T : DbContext, new()
        {
            var ctx = new T();
            return ctx.Database.Exists();
        }

        public static string GenerateTablesScript<T>() where T : DbMigrationsConfiguration, new()
        {
            var configuration = new T();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            var scriptor = new MigratorScriptingDecorator(migrator);
            var script = scriptor.ScriptUpdate(sourceMigration: null, targetMigration: null);
            return script;
        }

        public static string GenerateCreateDatabaseScript(Database database, MasterDataFile masterDataFile, LogFile logFile)
        {
            return GenerateCreateDatabaseScript(database, masterDataFile, new List<NextDataFile>(),
                new List<LogFile>() { logFile });
        }

        public static string GenerateCreateDatabaseScript(Database database, MasterDataFile masterDataFile, List<NextDataFile> dataFiles, List<LogFile> logFiles)
        {
            ArgumentValidation(database, masterDataFile, dataFiles, logFiles);

            var script = string.Format("CREATE DATABASE [{0}]", database.NAME) + Environment.NewLine;

            script += "ON PRIMARY" + Environment.NewLine;
            script += GenerateMasterDataFileScript(masterDataFile) + Environment.NewLine;

            if (dataFiles.Any())
                script += GenerateDataFilesScript(dataFiles) + Environment.NewLine;

            script += "LOG ON" + Environment.NewLine;
            script += GenerateLogFilesScript(logFiles) + Environment.NewLine;

            if (!string.IsNullOrEmpty(database.COLLATION))
                script += string.Format("COLLATE {0}", database.COLLATION) + Environment.NewLine;

            return script;
        }

        private static string AddLine(this string str, string line)
        {
            return str + line + Environment.NewLine;
        }

        private static string GenerateDatabaseFileScript(DatabaseFile databaseFile)
        {
            return string.Format("(NAME = {0}, FILENAME = '{1}', SIZE = {2}MB, MAXSIZE = {3}MB, FILEGROWTH = {4}MB)",
                databaseFile.NAME, databaseFile.FILENAME, databaseFile.SIZE, databaseFile.MAXSIZE,
                databaseFile.FILEGROWTH);
        }

        private static string GenerateMasterDataFileScript(MasterDataFile masterDataFile)
        {
            return GenerateDatabaseFileScript(masterDataFile);
        }

        private static string GenerateDataFilesScript(List<NextDataFile> dataFiles)
        {
            var script = string.Empty;
            foreach (var dataFile in dataFiles)
            {
                script += "," + Environment.NewLine;
                script += GenerateDatabaseFileScript(dataFile);
            }
            return script;
        }

        private static string GenerateLogFilesScript(List<LogFile> logFiles)
        {
            var script = string.Empty;
            foreach (var logFile in logFiles)
            {
                script += GenerateDatabaseFileScript(logFile);
                script += "," + Environment.NewLine;
            }
            script = script.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',');
            return script;
        }

        private static void ArgumentValidation(Database database, MasterDataFile masterDataFile, List<NextDataFile> dataFiles, List<LogFile> logFiles)
        {
            if (database == null)
                throw new ArgumentException("Database object is null.");

            if (string.IsNullOrEmpty(database.NAME))
                throw new ArgumentException("Database name is invalid.");

            if (masterDataFile == null)
                throw new ArgumentException("MasterDataFile object is null.");

            if (string.IsNullOrEmpty(masterDataFile.NAME))
                throw new ArgumentException("MasterDataFile name is invalid.");

            if (logFiles.Any() == false)
                throw new ArgumentException("Database must have one log file at least.");
        }
    }
}
