using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Exceptions;
using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.ExceptionHandler.Concrete
{
    class ExceptionHandler : ExceptionHandlerInterface
    {
        private static ExceptionHandler currentInstanse;
        private List<ConcreteException> exceptions;

        private ExceptionHandler()
        {
            exceptions = new List<ConcreteException>();
        }

        public static ExceptionHandlerInterface getInstance()
        {
            if (currentInstanse == null)
            {
                currentInstanse = new ExceptionHandler();
                currentInstanse.addException(new NonFoundException());
                currentInstanse.addException(new AlreadyExistException());
            }
            return currentInstanse;
        }

        public void addException(ConcreteException exception)
        {
            //Проверка, возможно, такое исключение было создано ранее
            for (int i = 0; i < currentInstanse.exceptions.Count; i++)
            {
                if (currentInstanse.exceptions.ElementAt(i).GetType() == exception.GetType())
                {
                    throw new AlreadyExistException("Exception " +
                    exception.GetType() + " was added earlier, it will be skipped!");
                }
            }
            currentInstanse.exceptions.Add(exception);
        }

        public void processing(Exception exception)
        {
            try
            {
                for (int i = 0; i < currentInstanse.exceptions.Count; i++)
                {
                    if (currentInstanse.exceptions.ElementAt(i).GetType() == exception.GetType())
                    {
                        currentInstanse.exceptions.ElementAt(i).processing(exception);
                        return;
                    }
                }
                //Если до сюда дошло, то исключение не найдено
                throw new NonFoundException("Unknown exception" + exception.GetType());
            }
            catch (Exception ex)
            {
                processing(ex);
            }
        }
    }
}
