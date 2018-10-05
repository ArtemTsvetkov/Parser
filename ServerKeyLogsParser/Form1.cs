using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Concrete;
using ServerKeyLogsParser.CommonComponents.InitialyzerComponent;
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
            try
            {
                //Создание конфига из ini-файла
                InitComponents initComponents = new InitComponents();
                Initialyzer initialyzer = new Initialyzer(initComponents);
                initialyzer.init();
                //Конфигурирование модели из конфига
                initComponents.commandsStore.executeCommand(
                    new ConfigModelCommand(initComponents.model, initComponents.config));
                //разбираем файлы логов
                initComponents.commandsStore.executeCommand(
                    new ParseCommand(initComponents.model));
            }
            catch (Exception ex)
            {
                ExceptionHandler.getInstance().processing(ex);
            }
            finally
            {
                Environment.Exit(0);//в конце завершаю работу приложения
            }
        }
    }
}