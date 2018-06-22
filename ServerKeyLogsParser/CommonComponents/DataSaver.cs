using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface DataSaver
    {
        void setConfig(string host, string query);
        void setConfig(string host, List<string> querys);
        DataSet execute();
        bool connect();
    }
}
/*
 * Позволяет выгрузить данные, например, в БД.
 */