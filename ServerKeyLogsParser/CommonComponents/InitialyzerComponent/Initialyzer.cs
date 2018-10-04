using ServerKeyLogsParser.CommonComponents.ExceptionHandler.Concrete;
using ServerKeyLogsParser.CommonComponents.InitialyzerComponent.ReadConfig;
using ServerKeyLogsParser.ParserComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.InitialyzerComponent
{
    class Initialyzer
    {
        private InitComponents components;

        public Initialyzer(InitComponents components)
        {
            this.components = components;
        }

        public void init()
        {
            //
            //Init ExceptionHandler
            //
            ConcreteExceptionHandlerInitializer.initThisExceptionHandler(
                ExceptionHandler.Concrete.ExceptionHandler.getInstance());
            //
            //Read ini-files
            //
            ConfigReader configReader = new ConfigReader();
            configReader.read();
            components.config = configReader.getConfig();
            //
            //Read computers name
            //
            string command = @"/C hostname";
            WorkWithWindowsCommandLine wwwcl = new WorkWithWindowsCommandLine();
            components.config.serversHost = wwwcl.Run_command(command);
            components.config.serversHost = components.config.serversHost.Remove(components.config.serversHost.Length-2,2);
            //
            //CommandStore
            //
            components.commandsStore = new ConcreteCommandStore();
            //
            //Model
            //
            components.model = new ParseModel();
        }
    }
}