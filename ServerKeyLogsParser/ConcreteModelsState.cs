using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ConcreteModelsState : ModelsState
    {
        public string pathOfDataBase = "";
        public string tableOfDataBase = "";
        public string serverHost = "";
        public string avevasLogWasDeleteStr = "";
        public bool avevasLogWasDelete = true;//на случай, если включена функция парсинга логов Aveva.
                                              //при ошибке в чтении или создании этого лога создается пустой лог-файл.
                                              //В этом случае программа его не удалит и это будет означат ошибку, по 
                                              //стандарту в случае ошибки консоль ничего не выводит и в случае успеха тоже.
        public List<LogAndHisLastEntry> logFiles = new List<LogAndHisLastEntry>();
        public List<string> bufOfLines = new List<string>();
        public List<ResultTableRows> result = new List<ResultTableRows>();
        public AutoDeskParser autodesk_parser = new AutoDeskParser();
        public AvevaParser aveva_parser = new AvevaParser();
    }
}
