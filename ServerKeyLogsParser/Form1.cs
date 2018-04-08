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
            Model model = new ParseModel();
            ConcreteCommandStore commandsStore = new ConcreteCommandStore();


            List<string> buf_of_lines = new List<string>();
            try
            {
                //читаем файл настроек
                commandsStore.executeCommand(new ConfigModelCommand(model));
                //разбираем файлы логов
                commandsStore.executeCommand(new ParseCommand(model));
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
                ReadWriteTextFile.Write_to_file(buf, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
                ReadWriteTextFile.Write_to_file(buf_of_lines, Directory.GetCurrentDirectory() + "\\Errors.txt", 0);
            }
            finally
            {
                Environment.Exit(0);//в конце завершаю работу приложения
            }
        }
    }
}