using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser
{
    class ConfigModelCommand : Command
    {
        private Model model;
        private ModelsState state;

        public ConfigModelCommand(Model model)
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
            model.setConfig(Directory.GetCurrentDirectory() + "\\settings.txt");
        }   
    }
}
