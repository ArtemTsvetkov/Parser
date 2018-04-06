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
    class MSAccessProxy : DataSaver
    {
        private MSAccessDataSaver saver = new MSAccessDataSaver();
        private string host;//пример хоста:C:\\Users\\Artem\\Documents\\Database3.accdb
        private string query;//Для выполненения 1 запроса
        private List<string> querys;//Для выполнения сразу нескольких запросов

        public void setConfig(string host, List<string> querys)
        {
            this.host = host;
            this.querys = querys;
            this.query = null;
            saver.setConfig(host, querys);
        }

        public void setConfig(string host, string query)
        {
            this.host = host;
            this.query = query;
            this.querys = null;
            saver.setConfig(host, query);
        }

        public object execute()
        {
            if(connect())
            {
                return saver.execute();
            }
            else
            {
                return null;
            }
        }

        public bool connect()//Пока для простоты, будем считать, что если запрос выполнился, значит подключение есть
        {
            List<string> currentQuerys = new List<string>();
            currentQuerys.Add("SELECT count(*) FROM Information");            
            string connStr = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + host + ";Persist Security Info=True;");
            DataSet dataSet = new DataSet();
            OleDbConnection conn;
            conn = null;


            try
            {
                conn = new OleDbConnection(connStr);
                conn.Open();
                for (int i = 0; i < currentQuerys.Count; i++)
                {
                    OleDbDataAdapter adapter = new OleDbDataAdapter(currentQuerys.ElementAt(i), conn);
                    adapter.Fill(dataSet, selectTableNameFromQuery(currentQuerys.ElementAt(i)));
                }
                return true;
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
                return false;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        private string selectTableNameFromQuery(string query)//функция поиска названия таблицы базы данных
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
            return null;
        }
    }
}
/*
 * Контролирует возможность доступа к БД. Реализация паттерна "Посредник"
 */