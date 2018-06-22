using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    public class LogAndHisLastEntry
    {//для хранения информации о логе и дате последней записи
        public string path;
        public string last_entry;
        public LogAndHisLastEntry()
        {
            path = "";
            last_entry = "";
        }
    }
}
