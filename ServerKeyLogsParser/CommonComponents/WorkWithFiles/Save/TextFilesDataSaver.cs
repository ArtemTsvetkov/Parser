using ServerKeyLogsParser.CommonComponents.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.WorkWithFiles.Save
{
    class TextFilesDataSaver : DataWorker<TextFilesConfigFieldsOnSave, bool>
    {
        private TextFilesConfigFieldsOnSave config;
        private bool resultStorage;

        public bool connect()
        {
            return true;
        }

        public void execute()
        {
            resultStorage = ReadWriteTextFile.Write_to_file(config.getData(), config.getFilePath(),
            config.getAction());
        }

        public bool getResult()
        {
            return resultStorage;
        }

        public void setConfig(TextFilesConfigFieldsOnSave config)
        {
            this.config = config;
        }
    }
}
