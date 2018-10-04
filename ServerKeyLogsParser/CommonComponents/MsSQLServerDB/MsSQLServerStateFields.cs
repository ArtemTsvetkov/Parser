using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.MsSQLServerDB
{
    class MsSQLServerStateFields
    {
        private string connectionString;
        private List<string> query;

        public MsSQLServerStateFields(List<string> query, string connectionString)
        {
            this.connectionString = connectionString;
            this.query = query;
        }

        public string getConnectionString()
        {
            return connectionString;
        }

        public List<string> getQuery()
        {
            return query;
        }
    }
}
