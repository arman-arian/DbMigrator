using System.Collections.Generic;
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
                var database = new Database<BankContext, BankContextContextConfiguration>(
                    "DbTest",
                    "Arabic_CI_AS",
                    new PrimaryFileGroup
                    {
                        MasterDataFile = new MasterDataFile
                        {
                            NAME = "Arch1",
                            FILENAME = @"D:\SalesData\archdat1.mdf",
                            SIZE = 50,
                            MAXSIZE = 100,
                            FILEGROWTH = 50
                        }
                    },
                    new LogFile
                    {
                        NAME = "Arch1",
                        FILENAME = @"D:\SalesData\archdat1.mdf",
                        SIZE = 50,
                        MAXSIZE = 100,
                        FILEGROWTH = 50
                    }
                    );


                
                var script = database.GenerateScript();
                script += DbCreator.GenerateTablesScript<BankContextContextConfiguration>();
            }

        }

        
    }
}
