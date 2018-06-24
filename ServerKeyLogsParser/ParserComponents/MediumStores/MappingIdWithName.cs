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

        public MappingIdWithName(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        public int getId()
        {
            return id;
        }

        public string getName()
        {
            return name;
        }
    }
}
