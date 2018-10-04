using ServerKeyLogsParser.ParserComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface Model
    {
        void parseFiles();//Распарсить файлы логов
        void setConfig(ParseConfig config);//Загрузка в модель файла с конфигурацией
        //void unloadIntoDB();//Выгрузить результаты в БД
        //void updateConfig(string pathToFileConfig);//Выгрузить файл с конфигурацией (обновить)
        ModelsState copySelf();
        void recoverySelf(ModelsState state);
    }
}
