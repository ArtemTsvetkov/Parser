using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    interface LogHandler
    {
        void setNextHandler();
        void parse();//Тестирует возможность парсинга текущего лог-файла и парсит, 
                     //если возможно, иначе отдаст управление другому хэндлеру
    }
}
/*
Представляет собой часть паттерна "Цепочка обязанностей", релизует парсинг конкретного лог-файла, если не
может его распарсить, то передает выполнение другому парсеру 
*/