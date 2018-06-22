using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.AccessDataBase
{
    class MSAccessStateFields
    {
        private string host;
        //пример хоста:C:\\Users\\Artem\\Documents\\Database3.accdb
        private List<string> query;

        public MSAccessStateFields(string host, List<string> query)
        {
            this.host = host;
            this.query = query;
        }

        public string getHost()
        {
            return host;
        }

        public List<string> getQuery()
        {
            return query;
        }
    }
}
