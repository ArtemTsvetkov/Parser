using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces;
using ServerKeyLogsParser.CommonComponents.ExceptionHandler.TextJornalist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Exceptions
{
    class AlreadyExistException : Exception, ConcreteException
    {
        public AlreadyExistException() : base() { }

        public AlreadyExistException(string message) : base(message) { }

        public void processing(Exception ex)
        {
            TextJornalistConfig jornalistConfig =
                new TextJornalistConfig(ex.Message, ex.StackTrace, ex.Source);
            TextFilesJornalist jornalist = new TextFilesJornalist();
            jornalist.setConfig(jornalistConfig);
            jornalist.write();
        }
    }
}
