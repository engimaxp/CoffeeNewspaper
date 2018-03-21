using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CN_Model;
using Newtonsoft.Json;
using SimpleTxtDB;

namespace CN_Repository
{
    public class RootDataProvider:IRootDataProvider
    {
        private static RootDataProvider _rootDataProvider;

        private static readonly object lockobject = new object();

        public string Today { get; set; }

        private const string persistenceFileName = "tasksMemo";

        public static RootDataProvider GetProvider()
        {
            if (_rootDataProvider == null)
            {
                lock (lockobject)
                {
                    if (_rootDataProvider == null)
                    {

                        lock (lockobject)
                        {
                            _rootDataProvider = new RootDataProvider();
                        }
                    }
                }
            }
            return _rootDataProvider;
        }

        public void Persistence(CNRoot rootInfo)
        {
            TxtDB fileDb = new TxtDB(persistenceFileName);
            fileDb.OverWrite(JsonConvert.SerializeObject(rootInfo, Formatting.Indented));
        }

        public CNRoot GetRootData()
        {
            TxtDB fileDb = new TxtDB(persistenceFileName);
            CNRoot processData = null;
            try
            {
                string datasource = fileDb.ReadAll();
                processData = string.IsNullOrEmpty(datasource)
                    ? new CNRoot()
                    : JsonConvert.DeserializeObject<CNRoot>(datasource);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return processData;
        }
    }
}
