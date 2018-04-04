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
        public void MemosListEqualTest()
        {
            var memo1 = new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() {""}
            };
            var memo2 = new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            };
            
            Assert.AreEqual(1, new List<CNMemo>() { memo1 }.Intersect(new List<CNMemo>() { memo2 },CNMemo.CnMemoComparer).Count());
            Assert.AreEqual(0,new List<CNMemo>() { memo1 }.Except(new List<CNMemo>() { memo2 }, CNMemo.CnMemoComparer).Count()); 
        }

        [Test]
        public void MemosAreEqual()
        {
            Assert.AreEqual(new CNMemo()
            {
                MemoId ="1",
                Content = "Start with writing tests!",
                Tags = new List<string>(){ "" }
            }, new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            });
        }
        [Test]
        public void MemosAreNotEqual()
        {
            Assert.AreNotEqual(new CNMemo()
            {
                MemoId = "1",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            }, new CNMemo()
            {
                MemoId = "2",
                Content = "Start with writing tests!",
                Tags = new List<string>() { "" }
            });
        }

    }
}
