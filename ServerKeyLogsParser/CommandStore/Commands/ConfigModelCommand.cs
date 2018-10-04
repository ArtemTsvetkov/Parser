using ServerKeyLogsParser.ParserComponents;
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
        private ParseConfig config;

        public ConfigModelCommand(Model model, ParseConfig config)
        {
            this.model = model;
            state = this.model.copySelf();
            this.config = config;
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
            model.setConfig(config);
        }   
    }
}
