using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace ServerKeyLogsParser
{
    class ReadWriteTextFile
    {
        public bool Write_to_file(List<String> buf_of_file_lines, string file_path, int action)
        {
            try
            {
                FileMode fm = FileMode.Create;
                if (action == 0)
                {
                    fm = FileMode.Append;
                }
                if (action == 1)
                {
                    fm = FileMode.Create;
                }
                if (action == 2)
                {
                    fm = FileMode.CreateNew;
                }
                if (action == 3)
                {
                    fm = FileMode.Open;
                }
                if (action == 4)
                {
                    fm = FileMode.OpenOrCreate;
                }
                if (action == 5)
                {
                    fm = FileMode.Truncate;
                }
                FileStream file1 = new FileStream(file_path, fm); //создаем файловый поток
                StreamWriter writer = new StreamWriter(file1, Encoding.UTF8); //создаем «потоковый писатель» и связываем его с файловым потоком 
                for (int i = 0; i < buf_of_file_lines.Count; i++)
                {
                    writer.WriteLine(buf_of_file_lines.ElementAt(i)); //записываем в файл
                }
                writer.Close(); //закрываем поток. Не закрыв поток, в файл ничего не запишется 
                return true;
            }
            catch(Exception ex)
            {
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: ReadWriteTextFile");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                return false;
            }
        }



        public List<String> Read_from_file(string file_path, string last_records_time)
        {
            List<String> buf_of_file_lines = new List<string>();
            int i = 0;
            try
            {
                string aveva_log_example = " [Contacting Sentinel RMS Development Kit server on host \"aveva\"]";
                string four_file_rows = "";//сюда прочитаем 4 строку из файла, если она равна aveva_log_file, то это лог-файл aveva
                int previous_time_hour = 0;//0 чтобы не ругался компилятор
                int found_the_last_date = 0;
                bool found_the_last_time = false;
                string[] time_plus_date = last_records_time.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);//отделение времени и даты от комментария, пример 25.6.2017_22:31:26 last_record's_time
                string[] onli_time_plus_date = time_plus_date[0].Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);//отделение даты от времени, пример 25.6.2017_22:31:26
                FileStream file1 = new FileStream(file_path, FileMode.Open); //создаем файловый поток
                StreamReader reader = new StreamReader(file1, Encoding.UTF8); // создаем «потоковый читатель» и связываем его с файловым потоком 


                //читаем первые 4 строки для идентификации лога авевы
                for (int j = 0; j < 4; j++)
                {
                    four_file_rows = reader.ReadLine();
                }
                if (four_file_rows == aveva_log_example)//это aveva-log
                {//если это aveva, то просто читаем весь файл
                    string row = "Aveva";//делаю отметку о том, что это лог-файл aveva
                    buf_of_file_lines.Add(row);
                    while (reader.EndOfStream == false)
                    {
                        row = reader.ReadLine();
                        buf_of_file_lines.Add(row); //считываем все данные с потока
                    }
                }
                else//это autodesk-лог
                {
                    //сразу запишу в ответ строку с датой, иначе ее может и не быть
                    string[] new_date = onli_time_plus_date[0].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                    buf_of_file_lines.Add(onli_time_plus_date[1] + " (adskflex) (@adskflex-SLOG@) Time: Tue " + month_converter(int.Parse(new_date.ElementAt(1))) + " " + new_date.ElementAt(0) + " " + new_date.ElementAt(2) + " " + onli_time_plus_date[1] + " Калининградское время (зима)");
                    while (reader.EndOfStream == false)
                    {
                        string row = reader.ReadLine();
                        if (found_the_last_date == 0)//новая дата  не найдена, в логе вначале указывается дата, а потом время(оно на нижних строках)
                        {//третий аргумент для случая, если будет найден пердыдущий день и нужно будет запомнить час
                            found_the_last_date = Check_rows_on_date(row, onli_time_plus_date[0], ref previous_time_hour);
                            if (found_the_last_date == 1)
                            {
                                buf_of_file_lines.Add(row); //записываю найденную дату, иначе ее не будет вообще
                                                            //потому что далее алгоритм ищет только новые строки, а в них может не быть даты
                            }
                        }
                        if (found_the_last_date == 2)//ищем место, в логе, где время как бы обнуляется
                        {//то есть время на следующей строке меньше, чем на предыдущей и тогда found_the_last_date == 1
                            if (check_on_new_day(row, ref previous_time_hour) == true)
                            {
                                found_the_last_date = 1;
                            }
                        }
                        if (found_the_last_date == 1)//новая дата найдена, но не найдено еще новое время, в логе вначале указывается дата, а потом время(оно на нижних строках)
                        {
                            found_the_last_time = Check_rows_on_time(row, onli_time_plus_date[1], ref previous_time_hour);
                        }
                        //если true, то все строки, которые ниже-новые
                        if (found_the_last_time == true)//для возврата к исходному классу оставить содержание текущего if, а все условия на этом уровне удалить
                        {
                            buf_of_file_lines.Add(row); //считываем все данные с потока
                            found_the_last_date = 4;//чтобы работало только это условие
                        }
                        
                        
                        
                        /*if (i == 8255)
                        {
                            int gdfg = 0;
                        }
                        i++;*/
                    }
                }
                reader.Close(); //закрываем поток
                return buf_of_file_lines;
            }
            catch (Exception ex)
            {
                buf_of_file_lines.Clear();
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: ReadWriteTextFile");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                return buf_of_file_lines;
            }
        }


        public List<String> Read_from_file(string file_path)//обычное чтение файла
        {
            List<String> buf_of_file_lines = new List<string>();
            try
            {
                FileStream file1 = new FileStream(file_path, FileMode.Open); //создаем файловый поток
                StreamReader reader = new StreamReader(file1, Encoding.UTF8); // создаем «потоковый читатель» и связываем его с файловым потоком 
                while (reader.EndOfStream == false)
                {
                        buf_of_file_lines.Add(reader.ReadLine()); //считываем все данные с потока
                }
                reader.Close(); //закрываем поток
                return buf_of_file_lines;
            }
            catch (Exception ex)
            {
                buf_of_file_lines.Clear();
                ReadWriteTextFile rwtf = new ReadWriteTextFile();
                List<string> buf = new List<string>();
                buf.Add("-----------------------------------------------");
                buf.Add("Module: ReadWriteTextFile");
                DateTime thisDay = DateTime.Now;
                buf.Add("Time: " + thisDay.ToString());
                buf.Add("Exception: " + ex.Message);
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                return buf_of_file_lines;
            }
        }


        private bool Check_rows_on_time(string row, string last_time_non_parsing, ref int previous_time)//проверка на уже проверенные строки, когда вернет true можно дальше не проверять
        {
            string in_example = @".*IN\W.*";
            string out_example = @".*OUT\W.*";
            string time_example = @".*Time:.*";
            string time_example2 = @".*Start-Date:.*";
            string[] last_time = last_time_non_parsing.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            if (Regex.IsMatch(row, out_example) | Regex.IsMatch(row, in_example) | (Regex.IsMatch(row, time_example)) | (Regex.IsMatch(row, time_example2)))//проверке поддаются только строки нужного формата(в них указаны даты входа и выхода)
            {
                string[] words = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string[] time_string = words[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                //удаление лишних нулей, например, 4 часа до этого значились как 04
                for(int i=0;i<3;i++)
                {
                    string a = time_string[i];
                    if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count()>1))
                    {
                        time_string[i] = time_string[i].Remove(0, 1);
                    }
                }
                if (previous_time > int.Parse(time_string[0]))//
                {//некоторые дни заканчиваются в 22-00, некоторые позже и дата, например, позднее, чем 23-00 может и не встретится, а начнется новый день
                    return true;
                }
                else
                {
                    if (int.Parse(time_string[0]) == int.Parse(last_time[0]))//проверка часа
                    {
                        if (int.Parse(time_string[1]) == int.Parse(last_time[1]))//проверка минуты
                        {
                            if (int.Parse(time_string[2]) > int.Parse(last_time[2]))//проверка секунды
                            {
                                return true;//если все больше, значит, что это новые строки
                            }
                            //ЕЩЕ ОДИН IF, И В НЕМ НУЖНО ПРОВЕРИТЬ ОСТАЛЬНУЮ ЧАСТЬ СТРОКИ
                        }
                        if (int.Parse(time_string[1]) > int.Parse(last_time[1]))//проверка минуты
                        {
                            return true;//если все больше, значит, что это новые строки
                        }
                    }
                    if (int.Parse(time_string[0]) > int.Parse(last_time[0]))//проверка часа
                    {
                        return true;//если все больше, значит, что это новые строки
                    }
                    previous_time = int.Parse(time_string[0]);
                }
            }
            return false;
        }



        private int Check_rows_on_date(string row, string last_date_non_parsing, ref int previous_time_hour)//проверка на уже проверенные строки, когда вернет true можно дальше проверять функцией Check_rows_on_time
        {//0-не новая дата, 1-новая дата, 2-за день до новой даты. 2 нужна, потому что новая дата пишется не в 24:00, а на несколько часов позже
            string time_example = @".*Time:.*";
            string time_example2 = @".*Start-Date:.*";
            string[] last_date = last_date_non_parsing.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);

            if ((Regex.IsMatch(row, time_example)) | (Regex.IsMatch(row, time_example2)))//обновление даты//проверке поддаются только строки нужного формата(в них указана дата)
            {
                string[] words = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                //удаление лишних нулей, например, 4 число до этого значилось как 04
                for (int i = 0; i < words.Count(); i++)
                {
                    string a = words[i];
                    if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))
                    {
                        words[i] = words[i].Remove(0, 1);
                    }
                }
                if (words[3] == "System")//этот и следующий if из-за разных форматов строки с текущем временем
                {
                    if (int.Parse(words[10]) == int.Parse(last_date[2]))//проверка года
                    {
                        if (month_converter(words[8]) == int.Parse(last_date[1]))//проверка месяца
                        {
                            if (int.Parse(words[9]) >= int.Parse(last_date[0]))//проверка дня
                            {
                                return 1;//если все больше, значит, что это новые строки
                            }
                            if (int.Parse(last_date[0]) - int.Parse(words[9]) == 1)//проверка на предыдущий день
                            {
                                string[] times = words[11].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                string a = times[0];
                                if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))//удаление лишнего нуля вначале
                                {
                                    times[0] = times[0].Remove(0, 1);
                                }
                                previous_time_hour = int.Parse(times[0]);
                                
                                return 2;
                            }
                        }
                        if (month_converter(words[8]) > int.Parse(last_date[1]))//проверка месяца
                        {
                            return 1;//если все больше, значит, что это новые строки
                        }
                    }
                    if (int.Parse(words[10]) > int.Parse(last_date[2]))//проверка года
                    {
                        return 1;//если все больше, значит, что это новые строки
                    }
                }
                else
                {
                    if (int.Parse(words[7]) == int.Parse(last_date[2]))//проверка года
                    {
                        if (month_converter(words[5]) == int.Parse(last_date[1]))//проверка месяца
                        {
                            if (int.Parse(words[6]) >= int.Parse(last_date[0]))//проверка дня
                            {
                                return 1;//если все больше, значит, что это новые строки
                            }
                            if (int.Parse(last_date[0]) - int.Parse(words[6]) == 1)//проверка на предыдущий день
                            {
                                string[] times = words[8].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                string a = times[0];
                                if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))//удаление лишнего нуля вначале
                                {
                                    times[0] = times[0].Remove(0, 1);
                                }
                                previous_time_hour = int.Parse(times[0]);

                                return 2;
                            }
                        }
                        if (month_converter(words[5]) > int.Parse(last_date[1]))//проверка месяца
                        {
                            return 1;//если все больше, значит, что это новые строки
                        }
                    }
                    if (int.Parse(words[7]) > int.Parse(last_date[2]))//проверка года
                    {
                        return 1;//если все больше, значит, что это новые строки
                    }
                }
            }
            return 0;
        }



        private bool check_on_new_day(string row, ref int previous_time_hour)
        {
            string[] words = row.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] time_string = words[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            //удаление лишних нулей, например, 4 часа до этого значились как 04
            for (int i = 0; i < 3; i++)
            {
                string a = time_string[i];
                if ((a.ElementAt(0).ToString().Equals("0")) & (a.Count() > 1))
                {
                    time_string[i] = time_string[i].Remove(0, 1);
                }
            }
            if (int.Parse(time_string[0]) < previous_time_hour)//проверка часа
            {
                previous_time_hour = int.Parse(time_string[0]);//обновляю
                return true;//если все меньше, значит, что это новый день
            }
            previous_time_hour = int.Parse(time_string[0]);//обновляю
            return false;
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



        private string month_converter(int month)//перевод числового обозначения месяца в символьное
        {
            string[] conveter_month = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            return conveter_month.ElementAt((month-1));
        }
    }
}
/*
 * Это модифицированный модуль, в функцию чтения файла добавлена проверка на уже прочтенные строки
 * Изначально тут только две функции Write_to_file и Read_from_file
 * Модуль для работы с файлами, для корректной работы файлы должны быть в кодировке utf8!
 * При успешной записи в файл функция Write_to_file вернет "true", иначе "false".
 * При успешном чтении из файла функция вернет список строк, иначе вернет список, в котором будет одна строка
 * со значением:"Ошибка чтения, файл не существует или не доступен!".
 * При записи в файл, одним из параметров является пареметр action, который принимает значение от 0 до 5 и задает режим записи и создания файла
 */
