using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleTxtDB;

namespace CoffeeNewspaper_UnitTest.CLITest
{
    [TestFixture]
    public class TaskCommand
    {
        private TxtDB txtDb { get; set; }
        [SetUp]
        public void Setup()
        {
            txtDb = new TxtDB("taskcommandtest");
        }

        [Test]
        public void AddATask()
        {

        }

        [TearDown]
        public void TearDown()
        {
            txtDb.DumpFile();
        }
    }
}
