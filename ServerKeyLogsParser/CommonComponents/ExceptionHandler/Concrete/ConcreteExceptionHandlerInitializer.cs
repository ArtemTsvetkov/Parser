using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces;
using ServerKeyLogsParser.CommonComponents.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Concrete
{
    class ConcreteExceptionHandlerInitializer
    {
        public static void initThisExceptionHandler(ExceptionHandlerInterface handler)
        {
            try
            {
                handler.addException(new NoConfigurationSpecified());
                handler.addException(new ConnectionErrorToFile());
                handler.addException(new DatabaseQueryError());
                handler.addException(new NoDataBaseConnection());
            }
            catch (Exception ex)
            {
                ExceptionHandler.getInstance().processing(ex);
            }
        }
    }
}
