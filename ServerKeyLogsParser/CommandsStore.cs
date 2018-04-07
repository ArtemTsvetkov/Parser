using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface CommandsStore
    {
        void push(Command command);//Добавление команды в стек
        Command pop();//Извлечение команды из стека
        void executeCommand(Command command);//Выполнение команды
        void recoveryModel();//Откат изменений модели
    }
}
