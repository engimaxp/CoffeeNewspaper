using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CN_Model;

namespace CoffeeNewspaper_UnitTest.DomainTest
{
    [TestFixture]
    public class MemoTest
    {

        [Test]
        public void MemosAreEqual()
        {
            Assert.AreEqual(new CNMemo()
            {
                MemoId ="1",
                Content = "Start with writing tests!",
                Tag = ""
            }, new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tag = ""
            });
        }
        [Test]
        public void MemosAreNotEqual()
        {
            Assert.AreNotEqual(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tag = ""
            }, new CNMemo()
            {
                MemoId = "2",
                Content = "Start with writing tests!",
                Tag = ""
            });
        }

    }
}
