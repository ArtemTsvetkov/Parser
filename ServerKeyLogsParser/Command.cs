using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface Command
    {
        ModelsState getModelState();//Получить сохраненное состояние модели
        void execute();
        void recoveryModel();
    }
}
/*
 * Представляет собой объект команды, выполняет некоторые действия и хранит стейт модели на случай отката
 */