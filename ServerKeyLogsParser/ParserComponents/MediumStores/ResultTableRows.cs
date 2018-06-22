using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ResultTableRows//класс для хранения строк результирующей таблицы
    {
        public DateTime star_time;
        public DateTime finish_time;
        public string user;
        public string host;
        public string vendor;
        public string po;
        public string servers_host;
        public ResultTableRows(DateTime star_time, DateTime finish_time, string user, string host, string vendor, string po, string servers_host)
        {
            this.star_time = star_time;
            this.finish_time = finish_time;
            this.user = user;
            this.host = host;
            this.vendor = vendor;
            this.po = po;
            this.servers_host = servers_host;
    }
    }
}
