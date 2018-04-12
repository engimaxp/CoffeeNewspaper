using System;
using CoffeeNewspaper_CLI;
using NUnit.Framework;

namespace CoffeeNewspaper_UnitTest.CLITest
{
    [TestFixture]
    public class ArgumentParserTest
    {
        [Test]
        public void SingleCommandOnly()
        {
            var parser = new ArgumentParser("task");
            Assert.AreEqual("task",parser.CommandLine );
        }
        [Test]
        public void SingleCommand_SingleParameter()
        {
            var parser = new ArgumentParser("task -s \"8:00:00\"");
            Assert.AreEqual("task", parser.CommandLine);
            Assert.AreEqual("8:00:00", parser.GetSingle<string>("s"));
        }

        [Test]
        public void MutiCommand_SingleParameter()
        {
            var parser = new ArgumentParser("task memo -s \"2018-4-7 8:00:00\"");
            Assert.AreEqual("task", parser.CommandLine);
            Assert.AreEqual("2018-4-7 8:00:00", parser.GetSingle<string>("s"));
        }

        [Test]
        public void MutiCommand_MutiParameter()
        {
            var parser = new ArgumentParser("task memo -s \"2018-4-7 8:00:00\" -m elody opera");
            Assert.AreEqual("task", parser.CommandLine);
            Assert.AreEqual("2018-4-7 8:00:00", parser.GetSingle<string>("s"));
            Assert.AreEqual(new []{ "elody" , "opera" }, parser.Get<string>("m"));
        }
        [Test]
        public void SingleCommandOnly_WithMutiBlankSpace_andAparameter()
        {
            var parser = new ArgumentParser(" task   87878");
            Assert.AreEqual("task", parser.CommandLine);
            Assert.IsFalse(parser.Defined("87878"));
        }
        [Test]
        public void MutiCommand_MutiParameter_WithMutiBlankSpace_andAparameter()
        {
            var parser = new ArgumentParser("task memo -s \"2018-4-7 8:00:00\" -m elody opera -t 1 2 3 -f \"2018-4-7 7:00:00\" \"2018-4-7 8:00:00\" \"2018-4-7 9:00:00\"");
            Assert.AreEqual("task", parser.CommandLine);
            Assert.IsFalse(parser.Defined("memo"));
            Assert.AreEqual("2018-4-7 8:00:00", parser.GetSingle<string>("s"));
            Assert.AreEqual(new[] { "elody", "opera" }, parser.Get<string>("m"));
            Assert.AreEqual(new[] { 1,2,3 }, parser.Get<int>("t"));
            Assert.AreEqual(new[] { Convert.ToDateTime("2018-4-7 7:00:00"), Convert.ToDateTime("2018-4-7 8:00:00"), Convert.ToDateTime("2018-4-7 9:00:00") }, parser.Get<DateTime>("f"));
        }
    }
}
