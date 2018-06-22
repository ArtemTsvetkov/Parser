using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class AutoDeskParser
    {
        public List<ResultTableRows> go_parsing(List<string> buf_of_lines, 
            string server_host, ref string last_date)//основная функция
        {
            
            string in_example = @".*IN\W.*";
            string out_example = @".*OUT\W.*";
            string time_example = @".*Time:.*";
            string time_example2 = @".*Start-Date:.*";
            string vendor = "AutoDesk";
            DateTime current_date_time = new DateTime(1, 1, 1);
            List<LogsRows> buf_of_inputs = new List<LogsRows>();
            List<ResultTableRows> result = new List<ResultTableRows>();

            try
            {
                for (int i = 0; i < buf_of_lines.Count(); i++)
                {
                    //обновление даты
                    if ((Regex.IsMatch(buf_of_lines.ElementAt(i), time_example)) | 
                        (Regex.IsMatch(buf_of_lines.ElementAt(i), time_example2)))
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        //этот и следующий if из-за разных форматов строки с текущем временем
                        if (words[3] == "System")
                        {
                            current_date_time = new DateTime(int.Parse(words[10]), 
                                month_converter(words[8]), int.Parse(words[9]));
                        }
                        else
                        {
                            current_date_time = new DateTime(int.Parse(words[7]), 
                                month_converter(words[5]), int.Parse(words[6]));
                        }
                    }
                    if (Regex.IsMatch(buf_of_lines.ElementAt(i), out_example))
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        //иногда попадаются такие строки, их нужно исключить
                        if (words[2] != "SERVER-OUT:")
                        {
                            string[] time = words[0].Split(new char[] { ':' }, 
                                StringSplitOptions.RemoveEmptyEntries);
                            DateTime date = new DateTime(current_date_time.Year, 
                                current_date_time.Month, current_date_time.Day, 
                                int.Parse(time[0]), int.Parse(time[1]), int.Parse(time[2]));
                            string[] user_and_host = words[4].Split(new char[] { '@' }, 
                                StringSplitOptions.RemoveEmptyEntries);
                            //удаление лишних символов вначале и в конце вендора и типа лицензии
                            words[3] = words[3].Remove(0, 1);
                            words[1] = words[1].Remove(0, 1);
                            words[3] = words[3].Remove((words[3].Count() - 1), 1);
                            words[1] = words[1].Remove((words[1].Count() - 1), 1);

                            LogsRows lr = new LogsRows(date, user_and_host[0], 
                                user_and_host[1], words[1], words[3], server_host);

                            //перезапись последней даты, она будет содержать дату 
                            //последней строки с in или out
                            last_date = date.Day.ToString() + "." + date.Month.ToString() + 
                                "." + date.Year.ToString() + "_" + date.Hour.ToString() + 
                                ":" + date.Minute.ToString() + ":" + date.Second.ToString();


                            //поиск "своего in"
                            //out может встретится и до первого in, его нужно все равно 
                            //записать в ответ
                            if (buf_of_inputs.Count() == 0)
                            {
                                //обработка out без in
                                DateTime date2 = new DateTime(1, 1, 1, 1, 1, 1);
                                ResultTableRows rtr = new ResultTableRows(date2, lr.date, 
                                    lr.user, lr.host, vendor, lr.po, lr.servers_host);
                                result.Add(rtr);
                            }
                            for (int j = 0; j < buf_of_inputs.Count(); j++)
                            {
                                if (buf_of_inputs.ElementAt(j).user == lr.user & buf_of_inputs.
                                    ElementAt(j).host == lr.host & buf_of_inputs.ElementAt(j).
                                    po == lr.po)
                                {
                                    ResultTableRows rtr = new ResultTableRows(
                                        buf_of_inputs.ElementAt(j).date, lr.date, lr.user, 
                                        lr.host, vendor, lr.po, lr.servers_host);
                                    result.Add(rtr);
                                    buf_of_inputs.RemoveAt(j);
                                    break;
                                }
                                if (j == buf_of_inputs.Count() - 1)
                                {
                                    //обработка out без in
                                    DateTime date2 = new DateTime(1, 1, 1, 1, 1, 1);
                                    ResultTableRows rtr = new ResultTableRows(date2, lr.date, 
                                        lr.user, lr.host, vendor, lr.po, lr.servers_host);
                                    result.Add(rtr);
                                }
                            }
                        }
                    }
                    if (Regex.IsMatch(buf_of_lines.ElementAt(i), in_example))
                    {
                        string[] words = buf_of_lines.ElementAt(i).Split(new char[] { ' ' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        string[] time = words[0].Split(new char[] { ':' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        DateTime date = new DateTime(current_date_time.Year, 
                            current_date_time.Month, current_date_time.Day, int.Parse(time[0]), 
                            int.Parse(time[1]), int.Parse(time[2]));
                        string[] user_and_host = words[4].Split(new char[] { '@' }, 
                            StringSplitOptions.RemoveEmptyEntries);
                        LogsRows lr = new LogsRows(date, user_and_host[0], 
                            user_and_host[1], words[1], words[3], server_host);
                        lr.po = lr.po.Remove(0, 1);
                        lr.po = lr.po.Remove((lr.po.Count() - 1), 1);
                        buf_of_inputs.Add(lr);


                        //перезапись последней даты, она будет содержать дату последней 
                        //строки с in или out
                        last_date = date.Day.ToString() + "." + date.Month.ToString() + 
                            "." + date.Year.ToString() + "_" + date.Hour.ToString() + ":" + 
                            date.Minute.ToString() + ":" + date.Second.ToString();
                    }
                }

                for (int i = 0; i < buf_of_inputs.Count; i++)//вывод незавершенных соединений
                {
                    DateTime date = new DateTime(1, 1, 1, 1, 1, 1);
                    ResultTableRows rtr = new ResultTableRows(buf_of_inputs.ElementAt(i).date, 
                        date, buf_of_inputs.ElementAt(i).user, buf_of_inputs.ElementAt(i).host, 
                        vendor, buf_of_inputs.ElementAt(i).po, buf_of_inputs.ElementAt(i).
                        servers_host);
                    result.Add(rtr);
                }

                return result;
            }
            catch(Exception ex)
            {
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: AutoDeskParser");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: "+thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                buf.Add("Rows:");
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + 
                    "\\Errors.txt", 0);
                ReadWriteTextFile.Write_to_file(buf_of_lines, 
                    Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                result.Clear();
                return result;
            }
        }

        //перевод символьного обозначения месяца в числовое
        private int month_converter(string month)
        {
                string[] conveter_month = { "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
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
