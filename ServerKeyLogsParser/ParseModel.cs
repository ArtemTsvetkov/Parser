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
            for (int h = 0; h < state.logFiles.Count; h++)//последовательно разбираем файлы
            {
                //читаем лог-файл
                LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                lahle = state.logFiles.ElementAt(h);
                try
                {
                    string last_date = "";//записываю для перезаписи файла настроек 
                    state.bufOfLines = ReadWriteTextFile.Read_from_file(lahle.path, lahle.last_entry);
                    state.bufOfLines.Add("");//вставил пустую строку, если не сделать, ты вылетит исключение, если все строки проверяемого файла по времени не попадают в рассмотрение
                                         //парсим лог-файл
                    if (state.bufOfLines.ElementAt(0) == "Aveva")
                    {
                        state.result = state.aveva_parser.go_parsing(state.bufOfLines, state.serverHost);
                        File.Delete(state.logFiles.ElementAt(h).path);
                        state.avevasLogWasDelete = true;
                    }
                    else
                    {
                        state.result = state.autodesk_parser.go_parsing(state.bufOfLines, state.serverHost, ref last_date);
                    }

                    //запись ответа в БД
                    MSAccessProxy accessProxy = new MSAccessProxy();
                    //получение значения id
                    accessProxy.setConfig(state.pathOfDataBase, "SELECT COUNT(*) FROM " + state.tableOfDataBase);
                    DataSet ds = accessProxy.execute();
                    //DataSet ds = wwmsa.Run_query(state.pathOfDataBase, "SELECT COUNT(*) FROM " + state.tableOfDataBase);
                    int id = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                    //формирование массива запросов 
                    List<string> buf = new List<string>();
                    for (int i = 0; i < state.result.Count; i++)
                    {
                        if (state.result.ElementAt(i).vendor == "Aveva")//если это логи aveva, то нужно каждый раз проверять, есть ли такие же записи в бд
                        {
                            accessProxy.setConfig(state.pathOfDataBase, "SELECT * from " + state.tableOfDataBase + " where server_host='" + state.result.ElementAt(i).servers_host + "' and vendor='" + state.result.ElementAt(i).vendor + "' and user_name='" + state.result.ElementAt(i).user + "' and user_host='" + state.result.ElementAt(i).host + "' and year_in=" + state.result.ElementAt(i).star_time.Year + " and month_in=" + state.result.ElementAt(i).star_time.Month + " and day_in=" + state.result.ElementAt(i).star_time.Day + " and hours_in=" + state.result.ElementAt(i).star_time.Hour + " and minute_in=" + state.result.ElementAt(i).star_time.Minute + " and second_in=" + state.result.ElementAt(i).star_time.Second + "");
                            ds = accessProxy.execute();
                            //ds = wwmsa.Run_query();
                            int count = ds.Tables[0].Rows.Count;
                            if (count == 0)//значит нет такой строки и ее можно записать
                            {
                                accessProxy.setConfig(state.pathOfDataBase, "INSERT INTO " + state.tableOfDataBase + " VALUES(" + id + ",'" + state.serverHost + "','" + state.result.ElementAt(i).vendor + "','" + state.result.ElementAt(i).po + "','" + state.result.ElementAt(i).user + "','" + state.result.ElementAt(i).host + "'," + state.result.ElementAt(i).star_time.Year + "," + state.result.ElementAt(i).star_time.Month + "," + state.result.ElementAt(i).star_time.Day + "," + state.result.ElementAt(i).star_time.Hour + "," + state.result.ElementAt(i).star_time.Minute + "," + state.result.ElementAt(i).star_time.Second + "," + state.result.ElementAt(i).finish_time.Year + "," + state.result.ElementAt(i).finish_time.Month + "," + state.result.ElementAt(i).finish_time.Day + "," + state.result.ElementAt(i).finish_time.Hour + "," + state.result.ElementAt(i).finish_time.Minute + "," + state.result.ElementAt(i).finish_time.Second + ")");
                                accessProxy.execute();
                                //wwmsa.Run_query_without_answer(state.pathOfDataBase, "INSERT INTO " + state.tableOfDataBase + " VALUES(" + id + ",'" + state.serverHost + "','" + state.result.ElementAt(i).vendor + "','" + state.result.ElementAt(i).po + "','" + state.result.ElementAt(i).user + "','" + state.result.ElementAt(i).host + "'," + state.result.ElementAt(i).star_time.Year + "," + state.result.ElementAt(i).star_time.Month + "," + state.result.ElementAt(i).star_time.Day + "," + state.result.ElementAt(i).star_time.Hour + "," + state.result.ElementAt(i).star_time.Minute + "," + state.result.ElementAt(i).star_time.Second + "," + state.result.ElementAt(i).finish_time.Year + "," + state.result.ElementAt(i).finish_time.Month + "," + state.result.ElementAt(i).finish_time.Day + "," + state.result.ElementAt(i).finish_time.Hour + "," + state.result.ElementAt(i).finish_time.Minute + "," + state.result.ElementAt(i).finish_time.Second + ")");
                                id++;
                            }
                            continue;
                        }
                        if (state.result.ElementAt(i).star_time.Year == 1)//если дата не известна, то вместо нее везде стоят единицы, но чтобы все не проверять, достаточно проверить толлько год, он при известной дате точно не может быть равен 1
                        {
                            buf.Add("INSERT INTO " + state.tableOfDataBase + " VALUES(" + id + ",'" + state.serverHost + "','" + state.result.ElementAt(i).vendor + "','" + state.result.ElementAt(i).po + "','" + state.result.ElementAt(i).user + "','" + state.result.ElementAt(i).host + "'," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + state.result.ElementAt(i).finish_time.Year + "," + state.result.ElementAt(i).finish_time.Month + "," + state.result.ElementAt(i).finish_time.Day + "," + state.result.ElementAt(i).finish_time.Hour + "," + state.result.ElementAt(i).finish_time.Minute + "," + state.result.ElementAt(i).finish_time.Second + ")");
                            id++;
                            continue;
                        }
                        if (state.result.ElementAt(i).finish_time.Year == 1)//если дата не известна, то вместо нее везде стоят единицы, но чтобы все не проверять, достаточно проверить толлько год, он при известной дате точно не может быть равен 1
                        {
                            buf.Add("INSERT INTO " + state.tableOfDataBase + " VALUES(" + id + ",'" + state.serverHost + "','" + state.result.ElementAt(i).vendor + "','" + state.result.ElementAt(i).po + "','" + state.result.ElementAt(i).user + "','" + state.result.ElementAt(i).host + "'," + state.result.ElementAt(i).star_time.Year + "," + state.result.ElementAt(i).star_time.Month + "," + state.result.ElementAt(i).star_time.Day + "," + state.result.ElementAt(i).star_time.Hour + "," + state.result.ElementAt(i).star_time.Minute + "," + state.result.ElementAt(i).star_time.Second + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + ")");
                            id++;
                            continue;
                        }
                        if ((state.result.ElementAt(i).finish_time.Year != 1) & (state.result.ElementAt(i).star_time.Year != 1))
                        {
                            buf.Add("INSERT INTO " + state.tableOfDataBase + " VALUES(" + id + ",'" + state.serverHost + "','" + state.result.ElementAt(i).vendor + "','" + state.result.ElementAt(i).po + "','" + state.result.ElementAt(i).user + "','" + state.result.ElementAt(i).host + "'," + state.result.ElementAt(i).star_time.Year + "," + state.result.ElementAt(i).star_time.Month + "," + state.result.ElementAt(i).star_time.Day + "," + state.result.ElementAt(i).star_time.Hour + "," + state.result.ElementAt(i).star_time.Minute + "," + state.result.ElementAt(i).star_time.Second + "," + state.result.ElementAt(i).finish_time.Year + "," + state.result.ElementAt(i).finish_time.Month + "," + state.result.ElementAt(i).finish_time.Day + "," + state.result.ElementAt(i).finish_time.Hour + "," + state.result.ElementAt(i).finish_time.Minute + "," + state.result.ElementAt(i).finish_time.Second + ")");
                            id++;
                        }
                    }
                    accessProxy.setConfig(state.pathOfDataBase, buf);
                    accessProxy.execute();
                    //wwmsa.Run_query_without_answer_buf(state.pathOfDataBase, buf);
                    //перезапись последней даты
                    List<string> new_buf_of_lines = new List<string>();
                    state.bufOfLines = ReadWriteTextFile.Read_from_file(Directory.GetCurrentDirectory() + "\\settings.txt");
                    for (int i = 0; i < state.bufOfLines.Count; i++)
                    {
                        if (state.bufOfLines.ElementAt(i) != "")
                        {
                            string[] words = state.bufOfLines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (words[1] == "path_of_log_file")//если это путь к логу, то записываю в него новую дату
                            {
                                if (words[0] == state.logFiles.ElementAt(h).path)
                                {
                                    string new_line = "";
                                    if (last_date != "")
                                    {
                                        new_line = words[0] + " " + words[1] + " " + last_date;
                                    }
                                    else
                                    {
                                        new_line = words[0] + " " + words[1] + " " + words[2];
                                    }
                                    new_buf_of_lines.Add(new_line);
                                    continue;
                                }
                                else
                                {
                                    new_buf_of_lines.Add(state.bufOfLines.ElementAt(i));
                                    continue;
                                }
                            }
                            else//иначе просто копирую строку
                            {
                                new_buf_of_lines.Add(state.bufOfLines.ElementAt(i));
                                continue;
                            }
                        }
                    }
                    ReadWriteTextFile.Write_to_file(new_buf_of_lines, (Directory.GetCurrentDirectory() + "\\settings.txt"), 1);
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
                    ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                    ReadWriteTextFile.Write_to_file(state.bufOfLines, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                }
            }
            if (state.avevasLogWasDelete == false)
            {
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: Form1");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Ошибка: файл " + state.avevasLogWasDeleteStr + " пуст. Произошла ошибка при создании лога aveva.");
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                File.Delete(state.avevasLogWasDeleteStr);
            }
        }

        public void recoverySelf(ModelsState state)
        {
            this.state = (ConcreteModelsState)state;
        }

        public void setConfig(string pathToFileConfig)
        {
            List<string> buf_of_lines = ReadWriteTextFile.Read_from_file(Directory.GetCurrentDirectory() + "\\settings.txt");
            for (int i = 0; i < buf_of_lines.Count; i++)
            {
                if (buf_of_lines.ElementAt(i) != "")
                {
                    string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (words.Count() > 1)//исключение ошибок неправильного заполнения файла настроек
                    {
                        if ((words[1] == "path_of_log_file") & (words.Count() > 2))//если это путь к логу
                        {
                            LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                            lahle.path = words[0];
                            lahle.last_entry = words[2];
                            state.logFiles.Add(lahle);
                            continue;
                        }
                        if (words[1] == "server's_host")//если это название сервера
                        {
                            state.serverHost = words[0];
                            continue;
                        }
                        if (words[1] == "path_of_data_base")//если это путь к базе данных
                        {
                            state.pathOfDataBase = words[0];
                            continue;
                        }
                        if (words[1] == "table")//если это название таблицы
                        {
                            state.tableOfDataBase = words[0];
                            continue;
                        }
                        if (words[1] == "PathAvevasParser")//если необходимо парсить логи aveva
                        {
                            state.avevasLogWasDelete = false;
                            LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                            lahle.path = Directory.GetCurrentDirectory() + "\\output.txt";
                            lahle.last_entry = "1.1.1970_12:0:0";//просто так, чтобы не переделывать парсер для случая пустого времени. Для логов Aveva это не важно и одинаковые строки исключаются другим способом - по запросу к БД.
                            state.logFiles.Add(lahle);
                            state.avevasLogWasDeleteStr = lahle.path;

                            //запуск утилиты создания лога Aveva
                            string command = @"/C " + Directory.GetCurrentDirectory() + "\\CreateAvevasLog.bat";
                            WorkWithWindowsCommandLine wwwcl = new WorkWithWindowsCommandLine();
                            state.serverHost = wwwcl.Run_command(command);//в переменную server_host записываю значение только чтобы не создавать нувую переменную, здесь просто лежит ответ командной строки
                            while (File.Exists(Directory.GetCurrentDirectory() + "\\output.txt") == false)//ожидание создания файла
                            {

                            }
                            continue;
                        }
                    }
                }
            }
        }
    }
}
