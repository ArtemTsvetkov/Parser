using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface Command
    {
        void setModelState();
        void execute();
    }
}
/*
 * Представляет собой объект команды, выполняет некоторые действия и хранит стейт модели на случай отката
 */