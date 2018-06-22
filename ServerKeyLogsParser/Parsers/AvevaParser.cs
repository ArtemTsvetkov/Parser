using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class AvevaParser
    {
        public List<ResultTableRows> go_parsing(List<string> buf_of_lines, string server_host)//основная функция
        {
            string user_name_example = @".*User name\W.*";
            string host_name_example = @".*Host name\W.*";
            string status_example = @".*Status.*";
            DateTime empty_date = new DateTime(1, 1, 1, 1, 1, 1);
            List<ResultTableRows> result = new List<ResultTableRows>();
            LogsRows lr = new LogsRows(empty_date, "", "", "Aveva", "Aveva", server_host);


            try
            {
                for (int i = 0; i < buf_of_lines.Count; i++)
                {
                    if (Regex.IsMatch(buf_of_lines.ElementAt(i), user_name_example))//поиск имени пользователя
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        lr.user = words[4];
                        continue;
                    }
                    if (Regex.IsMatch(buf_of_lines.ElementAt(i), host_name_example))//поиск имени хоста пользователя
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        lr.host = words[4];
                        continue;
                    }
                    if (Regex.IsMatch(buf_of_lines.ElementAt(i), status_example))//поиск даты подключения
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        //удаление лишнего нуля перед числом(день)
                        string a = words[7];
                        if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))
                        {
                            words[7] = words[7].Remove(0, 1);
                        }
                        //парсинг времени
                        string[] time = words[8].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int h = 0; h < time.Count(); h++)
                        {
                            a = time[h];
                            if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))
                            {
                                time[h] = time[h].Remove(0, 1);
                            }
                        }
                        DateTime connection_date = new DateTime(int.Parse(words[9]), month_converter(words[6]), int.Parse(words[7]), int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
                        ResultTableRows rtr = new ResultTableRows(connection_date, empty_date, lr.user, lr.host, lr.vendor, lr.po, lr.servers_host);
                        result.Add(rtr);
                        continue;
                    }
                }
            }
            catch (Exception ex)
            {
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: AutoDeskParser");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                buf.Add("Rows:");
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                ReadWriteTextFile.Write_to_file(buf_of_lines, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                result.Clear();
            }
            return result;
        }


        private int month_converter(string month)//перевод символьного обозначения месяца в числовое
        {
            string[] conveter_month = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            for (int i = 0; i < 12; i++)
            {
                if (conveter_month[i] == month)
                {
                    i++;
                    return i;
                }
            }
            return -1;
        }
    }
}
