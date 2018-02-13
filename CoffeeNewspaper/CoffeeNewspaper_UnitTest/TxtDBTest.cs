using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleTxtDB;
namespace CoffeeNewspaper_UnitTest
{
#region test resource
    public class TestObj:IEquatable<TestObj>
    {
        public string property1{ get; set; }
        public List<TestChildObj> ChildObjs { get; set; }

        public static TestObj GetTestObj()
        {
            return new TestObj()
            {
                property1 = "hello",
                ChildObjs = new List<TestChildObj>()
                {
                    new TestChildObj(){property2 = "boo"},
                    new TestChildObj(){property2 = "bar"}
                }
            };
        }

        public bool Equals(TestObj other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(property1, other.property1) && ChildObjs.Count == other.ChildObjs.Count && !ChildObjs.Except(other.ChildObjs).Any();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestObj) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((property1 != null ? property1.GetHashCode() : 0)*397) ^ (ChildObjs != null ? ChildObjs.GetHashCode() : 0);
            }
        }
    }

    public class TestChildObj : IEquatable<TestChildObj>
    {
        public bool Equals(TestChildObj other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(property2, other.property2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TestChildObj) obj);
        }

        public override int GetHashCode()
        {
            return (property2 != null ? property2.GetHashCode() : 0);
        }

        public string property2 { get; set; }
    }
#endregion
    [TestFixture]
    public class TxtDBTest
    {
        [Test]
        public void CreateOrWriteToTxt()
        {
            TxtDB txtDb = new TxtDB("info");
            string teststring = "Testing" + Guid.NewGuid().ToString("N");
            txtDb.AppendAndSave(teststring);
            string lastline = txtDb.ReadLastLine();
            Assert.AreEqual(lastline, teststring);
        }

        [Test]
        public void ReadFromTxt_NotExist_ReturnEmpty()
        {
            TxtDB txtDb = new TxtDB("testEmpty");
            Assert.IsEmpty(txtDb.ReadLastLine());
            txtDb.DumpFile();
        }

        [Test]
        public void ReadAllAndWriteAll()
        {
            TxtDB txtDb = new TxtDB("info");
            string alltext = txtDb.ReadAll();
            string testString = "Addtest" + Guid.NewGuid().ToString("N");
            if (!alltext.EndsWith(Environment.NewLine))
            {
                alltext += Environment.NewLine;
            }
            alltext += testString;
            txtDb.OverWrite(alltext);
            Assert.AreEqual(testString, txtDb.ReadLastLine());
        }

        [Test]
        public void WriteAndReadJsonData()
        {
            TxtDB txtDb = new TxtDB("jsonTest");
            txtDb.OverWrite(JsonConvert.SerializeObject(TestObj.GetTestObj(),Formatting.Indented));

            string jsontxt = txtDb.ReadAll();
            Assert.AreEqual(JsonConvert.DeserializeObject<TestObj>(jsontxt), TestObj.GetTestObj());

            txtDb.DumpFile();
        }
    }
}
