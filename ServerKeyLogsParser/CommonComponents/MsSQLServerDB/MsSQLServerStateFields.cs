using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.MsSQLServerDB
{
    class MsSQLServerStateFields
    {
        //ВРЕМЕННО
        private string connectionString;
        private List<string> query;

        public MsSQLServerStateFields(List<string> query)
        {
            connectionString = "Provider=SQLNCLI11;Data Source=DESKTOP-CG8MSKG;Integrated Security=SSPI;Initial Catalog=LicenseInformationSystem";
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
