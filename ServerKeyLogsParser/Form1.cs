using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerKeyLogsParser
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //InitializeComponent();
            /*MSAccessProxy testDS = new MSAccessProxy();
            testDS.setConfig("D:\\Files\\MsVisualProjects\\Diplom\\Логи\\testlogs\\Database3.accdb","SELECT * FROM Information");
            testDS.execute();*/
            Model model = new ParseModel();
            ConcreteCommandStore commandsStore = new ConcreteCommandStore();


            List<string> buf_of_lines = new List<string>();
            try
            {
                List<ResultTableRows> result = new List<ResultTableRows>();
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                AutoDeskParser autodesk_parser = new AutoDeskParser();
                AvevaParser aveva_parser = new AvevaParser();
                List<LogAndHisLastEntry> log_files = new List<LogAndHisLastEntry>();
                //List<string> log_files = new List<string>();
                string path_of_data_base = "";
                string password_of_data_base = "";
                string table_of_data_base = "";
                //string last_records_time = "";
                string server_host = "";
                string avevas_log_was_dalete_str = "";
                bool avevas_log_was_dalete = true;//на случай, если включена функция парсинга логов Aveva.
                //при ошибке в чтении или создании этого лога создается пустой лог-файл. В этом случае программа его не удалит и это будет означат ошибку, по стандарту в случае ошибки консоль ничего не выводит и в случае успеха тоже.


                //читаем файл настроек
                commandsStore.executeCommand(new ConfigModelCommand(model));
                /*buf_of_lines = rwtf.Read_from_file(Directory.GetCurrentDirectory() + "\\settings.txt");
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
                                log_files.Add(lahle);
                                continue;
                            }
                            if (words[1] == "server's_host")//если это название сервера
                            {
                                server_host = words[0];
                                continue;
                            }
                            if (words[1] == "path_of_data_base")//если это путь к базе данных
                            {
                                path_of_data_base = words[0];
                                continue;
                            }
                            if (words[1] == "password")//если это пароль
                            {
                                password_of_data_base = words[0];
                                continue;
                            }
                            if (words[1] == "table")//если это пароль
                            {
                                table_of_data_base = words[0];
                                continue;
                            }
                            if (words[1] == "PathAvevasParser")//если необходимо парсить логи aveva
                            {
                                avevas_log_was_dalete = false;
                                LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                                lahle.path = Directory.GetCurrentDirectory() + "\\output.txt";
                                lahle.last_entry = "1.1.1970_12:0:0";//просто так, чтобы не переделывать парсер для случая пустого времени. Для логов Aveva это не важно и одинаковые строки исключаются другим способом - по запросу к БД.
                                log_files.Add(lahle);
                                avevas_log_was_dalete_str = lahle.path;

                                //запуск утилиты создания лога Aveva
                                string command = @"/C " + Directory.GetCurrentDirectory() + "\\CreateAvevasLog.bat";
                                WorkWithWindowsCommandLine wwwcl = new WorkWithWindowsCommandLine();
                                server_host = wwwcl.Run_command(command);//в переменную server_host записываю значение только чтобы не создавать нувую переменную, здесь просто лежит ответ командной строки
                                while(File.Exists(Directory.GetCurrentDirectory() + "\\output.txt") == false)//ожидание создания файла
                                {

                                }
                                continue;
                            }
                        }
                    }
                }
                */


                for (int h = 0; h < log_files.Count; h++)//последовательно разбираем файлы
                {
                    //читаем лог-файл
                    LogAndHisLastEntry lahle = new LogAndHisLastEntry();
                    lahle = log_files.ElementAt(h);
                    try
                    {
                        string last_date = "";//записываю для перезаписи файла настроек 
                        buf_of_lines = rwtf.Read_from_file(lahle.path, lahle.last_entry);
                        buf_of_lines.Add("");//вставил пустую строку, если не сделать, ты вылетит исключение, если все строки проверяемого файла по времени не попадают в рассмотрение
                        //парсим лог-файл
                        if (buf_of_lines.ElementAt(0) == "Aveva")
                        {
                            result = aveva_parser.go_parsing(buf_of_lines, server_host);
                            File.Delete(log_files.ElementAt(h).path);
                            avevas_log_was_dalete = true;
                        }
                        else
                        {
                            result = autodesk_parser.go_parsing(buf_of_lines, server_host, ref last_date);
                        }

                        //запись ответа в БД
                        WorkWithMSAccess wwmsa = new WorkWithMSAccess();
                        //получение значения id
                        
                        DataSet ds = wwmsa.Run_query(path_of_data_base, "SELECT COUNT(*) FROM " + table_of_data_base);
                        int id = int.Parse(ds.Tables[0].Rows[0][0].ToString());
                        //формирование массива запросов 
                        List<string> buf = new List<string>();
                        for (int i = 0; i < result.Count; i++)
                        {
                            if (result.ElementAt(i).vendor == "Aveva")//если это логи aveva, то нужно каждый раз проверять, есть ли такие же записи в бд
                            {
                                ds = wwmsa.Run_query(path_of_data_base, "SELECT * from " + table_of_data_base + " where server_host='" + result.ElementAt(i).servers_host + "' and vendor='" + result.ElementAt(i).vendor + "' and user_name='" + result.ElementAt(i).user + "' and user_host='" + result.ElementAt(i).host + "' and year_in=" + result.ElementAt(i).star_time.Year + " and month_in=" + result.ElementAt(i).star_time.Month + " and day_in=" + result.ElementAt(i).star_time.Day + " and hours_in=" + result.ElementAt(i).star_time.Hour + " and minute_in=" + result.ElementAt(i).star_time.Minute + " and second_in=" + result.ElementAt(i).star_time.Second + "");
                                int count = ds.Tables[0].Rows.Count;
                                if (count == 0)//значит нет такой строки и ее можно записать
                                {
                                    wwmsa.Run_query_without_answer(path_of_data_base, password_of_data_base, "INSERT INTO " + table_of_data_base + " VALUES(" + id + ",'" + server_host + "','" + result.ElementAt(i).vendor + "','" + result.ElementAt(i).po + "','" + result.ElementAt(i).user + "','" + result.ElementAt(i).host + "'," + result.ElementAt(i).star_time.Year + "," + result.ElementAt(i).star_time.Month + "," + result.ElementAt(i).star_time.Day + "," + result.ElementAt(i).star_time.Hour + "," + result.ElementAt(i).star_time.Minute + "," + result.ElementAt(i).star_time.Second + "," + result.ElementAt(i).finish_time.Year + "," + result.ElementAt(i).finish_time.Month + "," + result.ElementAt(i).finish_time.Day + "," + result.ElementAt(i).finish_time.Hour + "," + result.ElementAt(i).finish_time.Minute + "," + result.ElementAt(i).finish_time.Second + ")");
                                    id++;
                                }
                                continue;
                            }
                            if (result.ElementAt(i).star_time.Year == 1)//если дата не известна, то вместо нее везде стоят единицы, но чтобы все не проверять, достаточно проверить толлько год, он при известной дате точно не может быть равен 1
                            {
                                buf.Add("INSERT INTO " + table_of_data_base + " VALUES(" + id + ",'" + server_host + "','" + result.ElementAt(i).vendor + "','" + result.ElementAt(i).po + "','" + result.ElementAt(i).user + "','" + result.ElementAt(i).host + "'," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + result.ElementAt(i).finish_time.Year + "," + result.ElementAt(i).finish_time.Month + "," + result.ElementAt(i).finish_time.Day + "," + result.ElementAt(i).finish_time.Hour + "," + result.ElementAt(i).finish_time.Minute + "," + result.ElementAt(i).finish_time.Second + ")");
                                id++;
                                continue;
                            }
                            if (result.ElementAt(i).finish_time.Year == 1)//если дата не известна, то вместо нее везде стоят единицы, но чтобы все не проверять, достаточно проверить толлько год, он при известной дате точно не может быть равен 1
                            {
                                buf.Add("INSERT INTO " + table_of_data_base + " VALUES(" + id + ",'" + server_host + "','" + result.ElementAt(i).vendor + "','" + result.ElementAt(i).po + "','" + result.ElementAt(i).user + "','" + result.ElementAt(i).host + "'," + result.ElementAt(i).star_time.Year + "," + result.ElementAt(i).star_time.Month + "," + result.ElementAt(i).star_time.Day + "," + result.ElementAt(i).star_time.Hour + "," + result.ElementAt(i).star_time.Minute + "," + result.ElementAt(i).star_time.Second + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + "," + "null" + ")");
                                id++;
                                continue;
                            }
                            if ((result.ElementAt(i).finish_time.Year != 1) & (result.ElementAt(i).star_time.Year != 1))
                            {
                                buf.Add("INSERT INTO " + table_of_data_base + " VALUES(" + id + ",'" + server_host + "','" + result.ElementAt(i).vendor + "','" + result.ElementAt(i).po + "','" + result.ElementAt(i).user + "','" + result.ElementAt(i).host + "'," + result.ElementAt(i).star_time.Year + "," + result.ElementAt(i).star_time.Month + "," + result.ElementAt(i).star_time.Day + "," + result.ElementAt(i).star_time.Hour + "," + result.ElementAt(i).star_time.Minute + "," + result.ElementAt(i).star_time.Second + "," + result.ElementAt(i).finish_time.Year + "," + result.ElementAt(i).finish_time.Month + "," + result.ElementAt(i).finish_time.Day + "," + result.ElementAt(i).finish_time.Hour + "," + result.ElementAt(i).finish_time.Minute + "," + result.ElementAt(i).finish_time.Second + ")");
                                id++;
                            }
                        }
                        wwmsa.Run_query_without_answer_buf(path_of_data_base, buf);
                        


                        //перезапись последней даты
                        List<string> new_buf_of_lines = new List<string>();
                        buf_of_lines = rwtf.Read_from_file(Directory.GetCurrentDirectory() + "\\settings.txt");
                        for (int i = 0; i < buf_of_lines.Count; i++)
                        {
                            if(buf_of_lines.ElementAt(i) != "")
                            {
                                string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (words[1] == "path_of_log_file")//если это путь к логу, то записываю в него новую дату
                                {
                                    if (words[0] == log_files.ElementAt(h).path)
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
                                        new_buf_of_lines.Add(buf_of_lines.ElementAt(i));
                                        continue;
                                    }
                                }
                                else//иначе просто копирую строку
                                {
                                    new_buf_of_lines.Add(buf_of_lines.ElementAt(i));
                                    continue;
                                }
                            }
                        }
                        rwtf.Write_to_file(new_buf_of_lines, (Directory.GetCurrentDirectory() + "\\settings.txt"), 1);
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
                        rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                        rwtf.Write_to_file(buf_of_lines, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                    }
                }
                if(avevas_log_was_dalete == false)
                {
                    List<string> buf = new List<string>();
                    buf.Add("-----------------------------------------------");
                    buf.Add("Module: Form1");
                    DateTime thisDay = DateTime.Now;
                    buf.Add("Time: " + thisDay.ToString());
                    buf.Add("Ошибка: файл " + avevas_log_was_dalete_str + " пуст. Произошла ошибка при создании лога aveva.");
                    rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                    File.Delete(avevas_log_was_dalete_str);
                }
            }
            catch (Exception ex)
            {
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: Form1");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                buf.Add("Rows:");
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                rwtf.Write_to_file(buf_of_lines, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
            }
            finally
            {
                Environment.Exit(0);//в конце завершаю работу приложения
            }
        }
    }
}