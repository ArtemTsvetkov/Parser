using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.WorkWithFiles.Load
{
    class TextFilesConfigFieldsOnLoad
    {
        private string filePath;

        public TextFilesConfigFieldsOnLoad(string filePath)
        {
            this.filePath = filePath;
        }

        public string getFilePath()
        {
            return filePath;
        }
    }
}
