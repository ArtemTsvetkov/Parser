using ServerKeyLogsParser.CommonComponents.Interfaces.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.MsSQLServerDB
{
    class MsSQLServerDataWorker : DataWorker<MsSQLServerStateFields, DataSet>
    {
        private MsSQLServerStateFields config;
        private DataSet resultStorage;

        public void execute()
        {
            string connStr = string.Format(config.getConnectionString());
            resultStorage = runQuery(connStr, config.getQuery());
        }

        //Тестирует возможность подключения к БД, в этом классе(он не прокси) всегда есть
        //подключение
        public bool connect()
        {
            return true;
        }

        public void setConfig(MsSQLServerStateFields config)
        {
            this.config = config;
        }

        //функция поиска названия таблицы базы данных
        private string selectTableNameFromQuery(string query)
        {
            string[] buf_of_substrings = query.Split(new char[] { ' ' }, StringSplitOptions.
                RemoveEmptyEntries);
            if (buf_of_substrings[0].Equals("SELECT", StringComparison.CurrentCultureIgnoreCase) ==
                true)
            {
                return buf_of_substrings[3];
            }
            if (buf_of_substrings[0].Equals("INSERT", StringComparison.CurrentCultureIgnoreCase) ==
                true)
            {
                return buf_of_substrings[2];
            }
            if (buf_of_substrings[0].Equals("UPDATE", StringComparison.CurrentCultureIgnoreCase) ==
                true)
            {
                return buf_of_substrings[1];
            }
            if (buf_of_substrings[0].Equals("DELETE", StringComparison.CurrentCultureIgnoreCase) ==
                true)
            {
                return buf_of_substrings[2];
            }
            return "null";
        }

        private DataSet runQuery(string connection_string, List<string> query)
        {
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
                        adapter.Fill(dataSet, selectTableNameFromQuery(query.ElementAt(i)));
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
                        ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() +
                            "\\Errors.txt", 0);
                    }
                }
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
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt",
                    0);
                return null;
            }
            finally
            {
                if (conn != null) conn.Close();
            }
        }

        public DataSet getResult()
        {
            return resultStorage;
        }
    }
}
