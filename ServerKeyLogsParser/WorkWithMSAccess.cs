using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace ServerKeyLogsParser
{
    class WorkWithMSAccess
    {
        public DataSet Run_query(string host, string password, string query)//если у БД есть пароль и запрос возвращает данные
        {
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;Jet OLEDB:Database Password=" + password + ";");
            return Run_query_private(connStr, query);
        }


        public DataSet Run_query(string host, string query)//если у БД нет пароля и запрос возвращает данные
        {
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;");
            return Run_query_private(connStr, query);
        }


        private DataSet Run_query_private(string connection_string, string query)//внутреняя функция, нужна для исключения повторения кусков кода в функциях Run_query(они перегружаемые,их несколько)
        {
            DataSet dataSet = new DataSet();
            OleDbConnection conn;
            conn = null;

            try
            {
                conn = new OleDbConnection(connection_string);
                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                adapter.Fill(dataSet, select_table_name_from_query(query));
                return dataSet;
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
                buf.Add("Query:" + query);
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                return dataSet;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }






        public void Run_query_without_answer(string host, string password, string query)//усли у БД есть пароль и запрос не возвращает данные
        {
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;Jet OLEDB:Database Password=" + password + ";");
            Run_query_without_answer_private(connStr, query);
        }


        public void Run_query_without_answer_buf(string host, string password, List<string> query)//усли у БД есть пароль и запрос не возвращает данные
        {//для большого числа запросов, экономит время
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;Jet OLEDB:Database Password=" + password + ";");
            Run_query_without_answer_private_buf(connStr, query);
        }


        public void Run_query_without_answer(string host, string query)//если у БД нет пароля и запрос не возвращает данные
        {
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;");
            Run_query_without_answer_private(connStr, query);
        }


        public void Run_query_without_answer_buf(string host, List<string> query)//если у БД нет пароля и запрос не возвращает данные
        {//для большого числа запросов, экономит время
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;");
            Run_query_without_answer_private_buf(connStr, query);
        }


        private void Run_query_without_answer_private(string connection_string, string query)//внутреняя функция, нужна для исключения повторения кусков кода в функциях Run_query_without_answer(они перегружаемые,их несколько)
        {
            DataSet dataSet = new DataSet();
            OleDbConnection conn;
            conn = null;

            try
            {
                conn = new OleDbConnection(connection_string);
                conn.Open();

                OleDbDataAdapter adapter = new OleDbDataAdapter(query, conn);
                adapter.Fill(dataSet, select_table_name_from_query(query));
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
                buf.Add("Query:" + query);
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }


        private void Run_query_without_answer_private_buf(string connection_string, List<string> query)//внутреняя функция, нужна для исключения повторения кусков кода в функциях Run_query_without_answer(они перегружаемые,их несколько)
        {//для большого количества запросов, экономит время
            DataSet dataSet = new DataSet();
            OleDbConnection conn;
            conn = null;

            try
            {
                conn = new OleDbConnection(connection_string);
                conn.Open();
                for (int i = 0; i < query.Count; i++)
                {
                    try
                    {
                        OleDbDataAdapter adapter = new OleDbDataAdapter(query.ElementAt(i), conn);
                        adapter.Fill(dataSet, select_table_name_from_query(query.ElementAt(i)));
                    }
                    catch(Exception ex)
                    {
                        ReadWriteTextFile rwtf = new ReadWriteTextFile();
                        List<string> buf = new List<string>();
                        buf.Add("-----------------------------------------------");
                        buf.Add("Module: Form1");
                        DateTime thisDay = DateTime.Now;
                        buf.Add("Time: " + thisDay.ToString());
                        buf.Add("Exception: " + ex.Message);
                        buf.Add("Query:" + query);
                        rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                    }
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
                rwtf.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }


        private string select_table_name_from_query(string query)//функция поиска названия таблицы базы данных
        {
            String[] buf_of_substrings = query.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (buf_of_substrings[0].Equals("SELECT", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return buf_of_substrings[3];
            }
            if (buf_of_substrings[0].Equals("INSERT", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return buf_of_substrings[2];
            }
            if (buf_of_substrings[0].Equals("UPDATE", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return buf_of_substrings[1];
            }
            if (buf_of_substrings[0].Equals("DELETE", StringComparison.CurrentCultureIgnoreCase) == true)
            {
                return buf_of_substrings[2];
            }
            return "null";
        }
    }
}
/*
Модуль по работе с базой данных microsoft office access
пример пароля:1234
пример хоста:C:\\Users\\Artem\\Documents\\Database3.accdb
пример запроса:SELECT * FROM testtable
пример названия таблицы базы данных:testtable
примеры работы с возвращаемыми данными
    dataSet.Tables[0].Rows[0][1].ToString();-Rows[0][1] первый параметр-индекс строки, второй-индекс столбца
    dataSet.Tables[0].Columns.Count.ToString();-кол-во столбцов
    dataSet.Tables[0].Rows.Count.ToString();-кол-во строк
    dataSet.Tables[0].Columns[0].Caption;-название первого столбца
*/
