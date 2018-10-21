using ServerKeyLogsParser.CommonComponents.Exceptions;
using ServerKeyLogsParser.ParserComponents;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.InitialyzerComponent.ReadConfig
{
    class ConfigReader
    {
        private ParseConfig config = new ParseConfig();
        private IniFiles INI = new IniFiles("config.ini");

        public void read()
        {
            //Для работы необходим хотя бы 1 файл с логами
            if ((INI.KeyExists("pathOfLogFile", "Settings") || 
                INI.KeyExists("pathAvevasParser", "Settings")) &
                INI.KeyExists("connectionString", "Settings"))
            {
                config.logFiles = new List<LogAndHisLastEntry>();
                if (INI.KeyExists("pathOfLogFile", "Settings"))
                {
                    LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                    lahle.path = INI.ReadINI("Settings", "pathOfLogFile");
                    if (INI.KeyExists("lastDateOfLogFile", "Settings"))
                    {
                        lahle.last_entry = INI.ReadINI("Settings", "lastDateOfLogFile");
                        config.logFiles.Add(lahle);
                    }
                    else
                    {
                        throw new NoConfigurationSpecified("No configuration specified, check ini-files");
                    }

                    //Если файлов с логами больше 1, то они имеют ключ: pathOfLogFile+<i>
                    //Предполагается, что i-это целое число, i отсчитывается от числа "2"
                    bool allFilesWasFound = false;
                    int i = 2;
                    while (!allFilesWasFound)
                    {
                        if (INI.KeyExists("pathOfLogFile" + i.ToString(), "Settings"))
                        {
                            LogAndHisLastEntry lahle2 = new LogAndHisLastEntry();
                            lahle2.path = INI.ReadINI("Settings", "pathOfLogFile" + i.ToString());
                            if (INI.KeyExists("lastDateOfLogFile" + i.ToString(), "Settings"))
                            {
                                lahle2.last_entry = INI.ReadINI("Settings", "lastDateOfLogFile"
                                    + i.ToString());
                                config.logFiles.Add(lahle2);
                            }
                            else
                            {
                                throw new NoConfigurationSpecified("No configuration "+
                                    "specified, check ini-files");
                            }
                        }
                        else
                        {
                            allFilesWasFound = true;
                        }
                    }
                }


                if(INI.KeyExists("pathAvevasParser", "Settings"))
                {
                    config.avevasLogWasDeleteStr = INI.ReadINI("Settings", "pathAvevasParser");
                    LogAndHisLastEntry lahle2 = new LogAndHisLastEntry();
                    lahle2.path = Directory.GetCurrentDirectory() + "\\output.txt";
                    //просто так, чтобы не переделывать парсер для случая пустого 
                    //времени. Для логов Aveva это не важно и одинаковые строки 
                    //исключаются другим способом - по запросу к БД.
                    lahle2.last_entry = "1.1.1970_12:0:0";
                    config.logFiles.Add(lahle2);
                }


                config.connectionString = INI.ReadINI("Settings", "connectionString");
            }
            else
            {
                throw new NoConfigurationSpecified("No configuration specified, check ini-files");
            }
        }

        public ParseConfig getConfig()
        {
            return config;
        }
    }
}

