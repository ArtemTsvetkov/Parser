using ServerKeyLogsParser.CommonComponents.DataConverters;
using ServerKeyLogsParser.CommonComponents.InitialyzerComponent.ReadConfig;
using ServerKeyLogsParser.CommonComponents.Interfaces.Data;
using ServerKeyLogsParser.CommonComponents.MsSQLServerDB;
using ServerKeyLogsParser.CommonComponents.WorkWithFiles.Load;
using ServerKeyLogsParser.ParserComponents;
using ServerKeyLogsParser.ParserComponents.DataConverters;
using ServerKeyLogsParser.ParserComponents.MediumStores;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ParseModel : Model
    {
        ConcreteModelsState state = new ConcreteModelsState();

        public ModelsState copySelf()
        {
            return state;
        }

        public void parseFiles()
        {
            //Получение id пользователей
            List<MappingIdWithNameWithHost> usersInfo = getUsersIDAndNames();
            //Получение id программного обеспечения
            List<MappingIdWithName> softwareInfo = getSoftwaresIdAndNames();

            for (int h = 0; h < state.logFiles.Count; h++)//последовательно разбираем файлы
            {
                //читаем лог-файл
                LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                lahle = state.logFiles.ElementAt(h);
                try
                {
                    string last_date = "";//записываю для перезаписи файла настроек 
                    //Так как чтение из файла с проверкой на last_entry является специфической
                    //функцией, то не стал делать через dataWorker, так как потребуется
                    //создание большого числа новых классов
                    state.bufOfLines = ReadWriteTextFile.Read_from_file(lahle.path,
                        lahle.last_entry);
                    //вставил пустую строку, если не сделать, ты вылетит 
                    //исключение, если все строки проверяемого файла по времени не 
                    //попадают в рассмотрение 
                    //парсим лог-файл
                    state.bufOfLines.Add("");
                    if (state.bufOfLines.ElementAt(0) == "Aveva")
                    {
                        state.result = state.aveva_parser.go_parsing(state.bufOfLines,
                            state.serverHost);
                        File.Delete(state.logFiles.ElementAt(h).path);
                        state.avevasLogWasDelete = true;
                    }
                    else
                    {
                        state.result = state.autodesk_parser.go_parsing(state.bufOfLines,
                            state.serverHost, ref last_date);
                    }

                    //формирование массива запросов 
                    List<string> buf = new List<string>();
                    for (int i = 0; i < state.result.Count; i++)
                    {
                        int userID;
                        int softwareID;
                        //Получение id пользователя
                        try
                        {
                            userID = checkExistUserInDB(
                                usersInfo, state.result.ElementAt(i).user,
                                state.result.ElementAt(i).host);
                        }
                        catch (Exception ex)
                        {
                            userID = addUserIntoDB(state.result.ElementAt(i).user,
                                state.result.ElementAt(i).host);
                            usersInfo = getUsersIDAndNames();
                        }
                        //Получение id ПО
                        softwareID = checkExistSoftWareInDB(
                            softwareInfo, state.result.ElementAt(i).po);
                        //если это логи aveva, то нужно каждый раз проверять, есть ли 
                        //такие же записи в бд
                        if (state.result.ElementAt(i).vendor == "Aveva")
                        {
                            string query = "SELECT COUNT(*) FROM History WHERE UserID=" + userID +
                                " AND SoftwareID = " + softwareID + " AND DateIN = '" +
                                state.result.ElementAt(i).star_time.Date.ToString("yyyy-MM-dd") + 
                                "' AND DateExit = '" +
                                state.result.ElementAt(i).finish_time.Date.ToString("yyyy-MM-dd") + 
                                "' AND TimeIn = '" +
                                state.result.ElementAt(i).star_time.ToLongTimeString() + 
                                "' AND TimeExit='" +
                                state.result.ElementAt(i).finish_time.ToLongTimeString() + "'";
                            DataSet ds = configProxyForLoadDataFromBDAndExecute(query);
                            int count = ds.Tables[0].Rows.Count;



                            if (count == 0)//значит нет такой строки и ее можно записать
                            {
                                string newLine = "INSERT INTO History VALUES(" + userID + "," + 
                                    softwareID + ",'" +
                                    state.result.ElementAt(i).star_time.Date.ToString("yyyy-MM-dd") + 
                                    "','" +
                                    state.result.ElementAt(i).finish_time.Date.
                                    ToString("yyyy-MM-dd") + "','" +
                                    state.result.ElementAt(i).star_time.ToLongTimeString() + 
                                    "','" +
                                    state.result.ElementAt(i).finish_time.ToLongTimeString() + 
                                    "')";
                                configProxyForLoadDataFromBDAndExecute(newLine);
                            }
                            continue;
                        }
                        //Иначе это не aveva
                        //если дата не известна, то вместо нее везде стоят единицы, но чтобы 
                        //все не проверять, достаточно проверить толлько год, он при 
                        //известной дате точно не может быть равен 1
                        if (state.result.ElementAt(i).star_time.Year == 1)
                        {
                            string newLine = "INSERT INTO History VALUES(" + userID + "," +
                                    softwareID + "," +
                                    "null" +
                                    ",'" +
                                    state.result.ElementAt(i).finish_time.Date.ToString("yyyy-MM-dd") +
                                    "'," +
                                    "null" +
                                    ",'" +
                                    state.result.ElementAt(i).finish_time.ToLongTimeString() + "')";
                            buf.Add(newLine);
                            continue;
                        }
                        //если дата не известна, то вместо нее везде стоят единицы, но 
                        //чтобы все не проверять, достаточно проверить толлько год, он при 
                        //известной дате точно не может быть равен 1
                        if (state.result.ElementAt(i).finish_time.Year == 1)
                        {
                            string newLine = "INSERT INTO History VALUES(" + userID + "," +
                                    softwareID + ",'" +
                                    state.result.ElementAt(i).star_time.Date.ToString("yyyy-MM-dd") +
                                    "'," +
                                    "null" +
                                    ",'" +
                                    state.result.ElementAt(i).star_time.ToLongTimeString() +
                                    "'," +
                                    "null" + ")";
                            buf.Add(newLine);
                            continue;
                        }
                        if ((state.result.ElementAt(i).finish_time.Year != 1) &
                            (state.result.ElementAt(i).star_time.Year != 1))
                        {
                            string newLine = "INSERT INTO History VALUES(" + userID + "," +
                                    softwareID + ",'" +
                                    state.result.ElementAt(i).star_time.Date.ToString("yyyy-MM-dd") +
                                    "','" +
                                    state.result.ElementAt(i).finish_time.Date.
                                    ToString("yyyy-MM-dd") + "','" +
                                    state.result.ElementAt(i).star_time.ToLongTimeString() +
                                    "','" +
                                    state.result.ElementAt(i).finish_time.ToLongTimeString() +
                                    "')";
                            buf.Add(newLine);
                        }
                    }
                    configProxyForLoadDataFromBDAndExecute(buf);
                    //перезапись последней даты
                    List<string> new_buf_of_lines = new List<string>();
                    IniFiles INI = new IniFiles("config.ini");
                    if (h == 0)
                    {
                        if (last_date != "")
                        {
                            INI.Write("Settings", "lastDateOfLogFile", last_date);
                        }
                    }
                    else
                    {
                        if (last_date != "")
                        {
                            INI.Write("Settings", "lastDateOfLogFile"+h.ToString(), "last_date");
                        }
                    }
                }
                catch (Exception ex)
                {
                    List<string> buf = new List<string>();
                    buf.Add("-----------------------------------------------");
                    buf.Add("Module: Form1");
                    DateTime thisDay = DateTime.Now;
                    buf.Add("Time: " + thisDay.ToString());
                    buf.Add("Exception: " + ex.Message);
                    buf.Add("Rows:");
                    ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() +
                        "\\Errors.txt", 0);
                    ReadWriteTextFile.Write_to_file(state.bufOfLines, Directory.
                        GetCurrentDirectory() + "\\Errors.txt", 0);
                }
            }
            if (state.avevasLogWasDelete == false)
            {
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: Form1");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Ошибка: файл " + state.avevasLogWasDeleteStr +
                    " пуст. Произошла ошибка при создании лога aveva.");
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() +
                    "\\Errors.txt", 0);
                File.Delete(state.avevasLogWasDeleteStr);
            }
        }

        //Функция получения id и имен пользователей
        private List<MappingIdWithNameWithHost> getUsersIDAndNames()
        {
            MappingIDWithNameWithHostConverter converter = new MappingIDWithNameWithHostConverter();
            return converter.convert(configProxyForLoadDataFromBDAndExecute(
                "SELECT UserID, name, host FROM Users"));
        }
        //Функция получения всех id и названий ПО
        private List<MappingIdWithName> getSoftwaresIdAndNames()
        {
            MappingIDWithNameConverter converter = new MappingIDWithNameConverter();
            return converter.convert(configProxyForLoadDataFromBDAndExecute(
                "SELECT SoftwareID, Code FROM Software"));
        }

        //Функция получения id пользователя по его имени
        private int checkExistUserInDB(List<MappingIdWithNameWithHost> userInfo, 
            string userName, string host)
        {
            for(int i=0;i< userInfo.Count;i++)
            {
                if(userName.Equals(userInfo.ElementAt(i).getName()))
                {
                    return userInfo.ElementAt(i).getId();
                }
            }
            //ДОБАВИТЬ ИКЛЮЧЕНИЕ-ПОЛЬЗОВАТЕЛЬ НЕ НАЙДЕН
            throw new Exception();
        }

        //Функция создания пользователя, если он не найден функцией 
        //checkExistUserInDB(вылет исключения) вернется id пользователя
        private int addUserIntoDB(string userName, string host)
        {
            configProxyForLoadDataFromBDAndExecute("INSERT INTO Users VALUES('" + userName +
                "','" + host + "')");
            ConverterSingleColumnFromDataSet converter = new ConverterSingleColumnFromDataSet();
            return int.Parse(converter.convert(configProxyForLoadDataFromBDAndExecute(
                "SELECT UserID FROM Users WHERE name='" + userName + "' AND host='" + host + 
                "'"))[0]);
        }

        //Функция получения id ПО по его имени
        private int checkExistSoftWareInDB(List<MappingIdWithName> softwareInfo,
            string softwareName)
        {
            for (int i = 0; i < softwareInfo.Count; i++)
            {
                if (softwareName.Equals(softwareInfo.ElementAt(i).getName()))
                {
                    return softwareInfo.ElementAt(i).getId();
                }
            }
            //ДОБАВИТЬ ИКЛЮЧЕНИЕ-ПО НЕ НАЙДЕНО(его необходимо добавить вручную)
            throw new Exception();
        }

        public void recoverySelf(ModelsState state)
        {
            this.state = (ConcreteModelsState)state;
        }

        private DataSet configProxyForLoadDataFromBDAndExecute(string query)
        {
            DataWorker<MsSQLServerStateFields, DataSet> msSQLServerProxy = new MsSQLServerProxy();
            List<string> list = new List<string>();
            list.Add(query);
            MsSQLServerStateFields configProxy = new MsSQLServerStateFields(
                list, state.connectionString);
            msSQLServerProxy.setConfig(configProxy);
            msSQLServerProxy.execute();
            list.Clear();
            return msSQLServerProxy.getResult();
        }

        private DataSet configProxyForLoadDataFromBDAndExecute(List<string> list)
        {
            DataWorker<MsSQLServerStateFields, DataSet> msSQLServerProxy = new MsSQLServerProxy();
            MsSQLServerStateFields configProxy = new MsSQLServerStateFields(
                list, state.connectionString);
            msSQLServerProxy.setConfig(configProxy);
            msSQLServerProxy.execute();
            list.Clear();
            return msSQLServerProxy.getResult();
        }

        public void setConfig(ParseConfig config)
        {
            state.serverHost = config.serversHost;
            state.avevasLogWasDeleteStr = config.avevasLogWasDeleteStr;
            state.logFiles = config.logFiles;
            state.connectionString = config.connectionString;

            if (state.avevasLogWasDeleteStr!=null)
            {
                state.avevasLogWasDelete = false;
                //запуск утилиты создания лога Aveva
                string command = @"/C " + Directory.GetCurrentDirectory() +
                    "\\CreateAvevasLog.bat";
                WorkWithWindowsCommandLine wwwcl = new WorkWithWindowsCommandLine();
                wwwcl.Run_command(command);
                while (File.Exists(Directory.GetCurrentDirectory() + "\\output.txt")
                    == false)//ожидание создания файла
                {

                }
            }
            
        }
    }
}
