using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface DataSaver
    {
        void setConfig(string host, string query);
        void setConfig(string host, List<string> querys);
        object execute();
        bool connect();
    }
}
/*
 * Позволяет выгрузить данные, например, в БД.
 */