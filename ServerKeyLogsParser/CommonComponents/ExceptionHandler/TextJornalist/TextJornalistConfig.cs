using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.TextJornalist
{
    class TextJornalistConfig
    {
        private string exceptionMessage;
        private string exceptionTrace;
        private string exeptionsSourse;

        public TextJornalistConfig(string exceptionMessage, string exceptionTrace,
            string exeptionsSourse)
        {
            this.exceptionMessage = exceptionMessage;
            this.exceptionTrace = exceptionTrace;
            this.exeptionsSourse = exeptionsSourse;
        }

        public string getExceptionMessage()
        {
            return exceptionMessage;
        }

        public string getExceptionTrace()
        {
            return exceptionTrace;
        }

        public string getExeptionsSourse()
        {
            return exeptionsSourse;
        }
    }
}
