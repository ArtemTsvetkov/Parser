using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class WorkWithWindowsCommandLine
    {
        public string Run_command(string command)
        {
            ProcessStartInfo psiOpt = new ProcessStartInfo(@"cmd.exe", command);
            // скрываем окно запущенного процесса
            psiOpt.WindowStyle = ProcessWindowStyle.Hidden;
            //перенаправление стандартного вывода
            psiOpt.RedirectStandardOutput = true;
            psiOpt.UseShellExecute = false;
            psiOpt.CreateNoWindow = true;
            // запускаем процесс
            Process procCommand = Process.Start(psiOpt);
            // получаем ответ запущенного процесса
            StreamReader srIncoming = procCommand.StandardOutput;
            // выводим результат
            string answer = srIncoming.ReadToEnd();
            // закрываем процесс
            procCommand.WaitForExit();
            return answer;
        }
    }
}
/*
Модуль по работе с командной строкой windows
Команды посылаются в формате string a = @"/C hostname";
При неверном вводе команды модуль вернет пустую строку
Кириллица отображается некорректно
Примеры:
    @"/C SCHTASKS /Create /SC MINUTE /MO 1 /TR calc.exe h/TN myhobbi" - это неверный формат и при выполнении кода венутся пустое значение, хотя консоль выдает ошибку
    @"/C SCHTASKS /Create /SC MINUTE /MO 1 /TR calc.exe /TN myhobbi" - это правильный формат и при выполнении кода вернется непустое значение, но прочитать
    его не удается, так как есть проблемы с кодировкой. Команда запускает калькулятор каждую минуту
    @"/C SCHTASKS /Create /SC MINUTE /MO 1 /TR D:\Games\World_of_Tanks\WoTLauncher.exe /TN myhobbi" - команда запускает игру каждую минуту
    @"/C hostname"- команда сообщает хост компьютера
    @"/C SCHTASKS  /Query" - отобразит все задачи
    @"/C SCHTASKS /Delete /TN myhobbi" - удалит задание с именем myhobbi из планировщика заданий
    @ нужна для исключения некоторых ошибок, так было в примере, не стал менять
 */
