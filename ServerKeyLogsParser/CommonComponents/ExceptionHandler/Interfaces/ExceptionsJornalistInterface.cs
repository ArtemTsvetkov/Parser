using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces
{
    interface ExceptionsJornalistInterface<TConfigType>
    {
        //Установка конфигурации
        void setConfig(TConfigType config);
        //Запись в журнал информации о исключении
        void write();
    }
}
