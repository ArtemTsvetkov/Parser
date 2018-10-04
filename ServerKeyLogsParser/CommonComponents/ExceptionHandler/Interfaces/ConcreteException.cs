using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces
{
    interface ConcreteException
    {
        //Обработка исключения, используя данные базового
        void processing(Exception basic);
    }
}
