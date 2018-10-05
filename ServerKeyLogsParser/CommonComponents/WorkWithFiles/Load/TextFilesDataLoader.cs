using ServerKeyLogsParser.CommonComponents.Exceptions;
using ServerKeyLogsParser.CommonComponents.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.WorkWithFiles.Load
{
    class TextFilesDataLoader : DataWorker<TextFilesConfigFieldsOnLoad, List<string>>
    {
        private TextFilesConfigFieldsOnLoad config;
        private List<string> resultStorage = new List<string>();


        public bool connect()
        {
            return ReadWriteTextFile.testExistFile(config.getFilePath());
        }

        public void execute()
        {
            if (connect())
            {
                resultStorage = ReadWriteTextFile.Read_from_file(config.getFilePath());
            }
            else
            {
                throw new ConnectionErrorToFile("Connection error to file");
            }
        }

        public List<string> getResult()
        {
            return resultStorage;
        }

        public void setConfig(TextFilesConfigFieldsOnLoad config)
        {
            this.config = config;
        }
    }
}
