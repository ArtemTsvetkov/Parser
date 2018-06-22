using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class LogsRows//класс для промежуточных шагов парсинга
    {
        public DateTime date;//дата, указанная в строке
        public string user;//имя пользователя
        public string host;//хост пользователя
        public string vendor;//вендор
        public string po;//ПО
        public string servers_host;//хост сервера


        public LogsRows(DateTime date, string user, string host, string vendor, string po, string servers_host)
        {
            this.date = date;
            this.user = user;
            this.host = host;
            this.vendor = vendor;
            this.po = po;
            this.servers_host = servers_host;
        }
    }
}
