using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces
{
    interface ExceptionHandlerInterface
    {
        //Обработка исключения
        void processing(Exception exception);
        //Добавление нового исключения
        void addException(ConcreteException exception);
    }
}
