using DbMigrator.Core;
using DbMigrator.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DbMigrator.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            if (DbCreator.IsDatabaseExists<BankContext>() == false)
            {
                var script = DbCreator.GenerateCreateDatabaseScript(
                    new DbCreator.Database()
                    {
                        NAME = "DbTest",
                        COLLATION = "Arabic_CI_AS"
                    },
                    new DbCreator.MasterDataFile()
                    {
                        NAME = "Arch1",
                        FILENAME = @"D:\SalesData\archdat1.mdf",
                        SIZE = 50,
                        MAXSIZE = 100,
                        FILEGROWTH = 50
                    },
                    new DbCreator.LogFile()
                    {
                        NAME = "Arch1",
                        FILENAME = @"D:\SalesData\archdat1.mdf",
                        SIZE = 50,
                        MAXSIZE = 100,
                        FILEGROWTH = 50
                    }
                    );

                script += DbCreator.GenerateTablesScript<BankContextContextConfiguration>();
            }

        }

        
    }
}
