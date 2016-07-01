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
    }



    public sealed partial class Database<TContext, TConfiguration> 
        where TContext : DbContext, new() 
        where TConfiguration : DbMigrationsConfiguration, new()
    {
        /// <summary>
        /// Specifies the name of the new database
        /// </summary>
        public string NAME { get; private set; }

        /// <summary>
        /// Specifies the default collation for the database. Collation name can be either a Windows collation name or a SQL collation name. 
        /// If not specified, the database is assigned the default collation of the instance of SQL Server. 
        /// </summary>
        public string COLLATION { get; private set; }

        /// <summary>
        /// Specifies the containment status of the database. NONE = non-contained database. PARTIAL = partially contained database.
        /// </summary>
        public CONTAINMENT CONTAINMENT { get; private set; }

        public PrimaryFileGroup PrimaryFileGroup { get; private set; }

        public List<FileGroup> FileGroups { get; private set; }

        public List<LogFile> LogFiles { get; private set; }
    }

    public sealed partial class Database<TContext, TConfiguration>
        where TContext : DbContext, new()
        where TConfiguration : DbMigrationsConfiguration, new()
    {
        private Database()
        {
            this.FileGroups = new List<FileGroup>();
            this.LogFiles = new List<LogFile>();
        }

        public Database(string name, PrimaryFileGroup primaryFileGroup, LogFile logFile) : this()
        {
            this.NAME = name;
            this.PrimaryFileGroup = primaryFileGroup;
            this.LogFiles.Add(logFile);
        }

        public Database(string name, string collation, PrimaryFileGroup primaryFileGroup, LogFile logFile)
            : this(name, primaryFileGroup, logFile)
        {
            this.COLLATION = collation;
        }

        public void SetCollation(string Collation)
        {
            
        }

        public void AddLogFile(LogFile logFile)
        {
            this.LogFiles.Add(logFile);
        }

        public void AddFileGroup(FileGroup fileGroup)
        {
            this.FileGroups.Add(fileGroup);
        }

        public static bool IsDatabaseExists()
        {
            var ctx = new TContext();
            return ctx.Database.Exists();
        }

        public string GenerateTablesScript()
        {
            var configuration = new TConfiguration();
            var migrator = new System.Data.Entity.Migrations.DbMigrator(configuration);
            var scriptor = new MigratorScriptingDecorator(migrator);
            var script = scriptor.ScriptUpdate(sourceMigration: null, targetMigration: null);
            return script;
        }

        public string GenerateScript()
        {
            ValidateDatabase();

            var script = string.Format("CREATE DATABASE [{0}]", this.NAME) + Environment.NewLine;

            script += "ON PRIMARY" + Environment.NewLine;
            script += GenerateDatabaseFileScript(this.PrimaryFileGroup.MasterDataFile) + "," + Environment.NewLine;

            if (this.PrimaryFileGroup.NextDataFiles.Any())
                script += GenerateDatabaseFileScript(this.PrimaryFileGroup.NextDataFiles) + Environment.NewLine;

            if (this.FileGroups.Any())
            {
                foreach (var fileGroup in this.FileGroups)
                {
                    script += string.Format("FILEGROUP {0}", fileGroup.Name) + Environment.NewLine;
                    script += GenerateDatabaseFileScript(fileGroup.NextDataFiles) + Environment.NewLine;
                }
            }

            script += "LOG ON" + Environment.NewLine;
            script += GenerateDatabaseFileScript(this.LogFiles) + Environment.NewLine;

            if (!string.IsNullOrEmpty(this.COLLATION))
                script += string.Format("COLLATE {0}", this.COLLATION) + Environment.NewLine;

            return script;
        }

        private static string GenerateDatabaseFileScript(DatabaseFile databaseFile)
        {
            return string.Format("(NAME = {0}, FILENAME = '{1}', SIZE = {2}MB, MAXSIZE = {3}MB, FILEGROWTH = {4}MB)",
                databaseFile.NAME, databaseFile.FILENAME, databaseFile.SIZE, databaseFile.MAXSIZE,
                databaseFile.FILEGROWTH);
        }

        private static string GenerateDatabaseFileScript(IEnumerable<DatabaseFile> databaseFiles)
        {
            var script = string.Empty;
            foreach (var databaseFile in databaseFiles)
            {
                script += GenerateDatabaseFileScript(databaseFile);
                script += "," + Environment.NewLine;
            }
            script = script.TrimEnd(Environment.NewLine.ToCharArray()).TrimEnd(',');
            return script;
        }

        private void ValidateDatabase()
        {
            if (string.IsNullOrEmpty(this.NAME))
                throw new ArgumentException("Database name is invalid.");

            if(this.PrimaryFileGroup == null)
                throw new ArgumentException("Primary File Group is invalid.");

            if (this.PrimaryFileGroup.MasterDataFile == null)
                throw new ArgumentException("Primary File Group Master Data File is invalid.");
        }
    }

    public enum CONTAINMENT : byte
    {
        NONE,
        PARTIAL
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

    public sealed class FileGroup
    {
        public string Name { get; set; }

        public List<NextDataFile> NextDataFiles { get; set; }
    }

    public sealed class PrimaryFileGroup
    {
        public MasterDataFile MasterDataFile { get; set; }

        public List<NextDataFile> NextDataFiles { get; set; }
    }
}
