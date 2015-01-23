using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shiwch.game
{
    class DataStorage
    {
        private Dictionary<string, byte[]> dataStorage = new Dictionary<string, byte[]>();
        private static DataStorage instance = new DataStorage();

        public static DataStorage Instance
        {
            get { return instance; }
        }

        public IDictionary<string, byte[]> Dict
        {
            get { return dataStorage; }
        }
    }
}
