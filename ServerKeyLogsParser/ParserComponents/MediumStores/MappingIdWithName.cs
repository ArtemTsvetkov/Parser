using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.ParserComponents.DataConverters
{
    class MappingIdWithName
    {
        private int id;
        private string name;
        private string host;

        public MappingIdWithName(int id, string name, string host)
        {
            this.id = id;
            this.name = name;
            this.host = host;
        }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }

        public string getHost()
        {
            return host;
        }
    }
}
