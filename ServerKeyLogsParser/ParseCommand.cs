using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ParseCommand : Command
    {
        private Model model;
        private ModelsState state;

        public ParseCommand(Model model)
        {
            this.model = model;
            state = this.model.copySelf();
        }

        public ModelsState getModelState()
        {
            return state;
        }

        public void recoveryModel()
        {
            model.recoverySelf(state);
        }

        public void execute()
        {
            model.parseFiles();
        }
    }
}
