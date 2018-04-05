using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface DataConverter
    {
        object convert();
    }
}
/*
 * Конвертирует данные до определенного формата
 */