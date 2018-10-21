using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.ParserComponents
{
    class ParseConfig
    {
        //Заполняется только при помощи запроса к командной строке!
        public string serversHost;
        //Путь до приложения, создающего лог-файл Aveva
        public string avevasLogWasDeleteStr;
        public List<LogAndHisLastEntry> logFiles;
        public string connectionString;
    }
}
