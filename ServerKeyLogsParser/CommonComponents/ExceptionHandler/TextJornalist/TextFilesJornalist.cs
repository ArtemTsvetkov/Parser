using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces;
using ServerKeyLogsParser.CommonComponents.Exceptions;
using ServerKeyLogsParser.CommonComponents.Interfaces.Data;
using ServerKeyLogsParser.CommonComponents.WorkWithFiles.Save;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.TextJornalist
{
    class TextFilesJornalist : ExceptionsJornalistInterface<TextJornalistConfig>
    {
        private TextJornalistConfig config;

        public void setConfig(TextJornalistConfig config)
        {
            this.config = config;
        }

        public void write()
        {
            try
            {
                if (config == null)
                {
                    throw new NoConfigurationSpecified("No configuration specified");
                }
                else
                {
                    List<string> buf = new List<string>();
                    buf.Add("-----------------------------------------------");
                    buf.Add("Module: " + config.getExeptionsSourse());
                    buf.Add("Trace: " + config.getExceptionTrace());
                    DateTime thisDay = DateTime.Now;
                    buf.Add("Time: " + thisDay.ToString());
                    buf.Add("Exception: " + config.getExceptionMessage());

                    TextFilesConfigFieldsOnSave proxyConfig =
                        new TextFilesConfigFieldsOnSave(buf, Directory.GetCurrentDirectory() +
                        "\\Errors.txt", 0);


                    DataWorker<TextFilesConfigFieldsOnSave, bool> saver = new TextFilesDataSaver();
                    saver.setConfig(proxyConfig);
                    saver.connect();
                    saver.execute();
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Concrete.ExceptionHandler.getInstance().processing(ex);
            }
        }
    }
}

