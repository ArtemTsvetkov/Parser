using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ConcreteCommandStore : CommandsStore
    {
        private List<Command> history = new List<Command>();

        public void executeCommand(Command command)
        {
            command.execute();
            push(command);
        }

        public void recoveryModel()
        {
            Command command = pop();
            command.recoveryModel();
        }

        public Command pop()
        {
            return history.Last();
        }

        public void push(Command command)
        {
            history.Add(command);
        }
    }
}
