using ServerKeyLogsParser.CommonComponents.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.InitialyzerComponent.ReadConfig
{
    class ConfigReader
    {
        private static ConfigReader reader;
        private string connectionString;
        private IniFiles INI = new IniFiles("config.ini");

        private ConfigReader()
        {

        }

        public static ConfigReader getInstance()
        {
            if (reader == null)
            {
                reader = new ConfigReader();
            }

            return reader;
        }

        public void read()
        {
            if (INI.KeyExists("connectionString", "Settings"))
            {
                connectionString = INI.ReadINI("Settings", "connectionString");
            }
            else
            {
                throw new NoConfigurationSpecified("No configuration specified, check ini-files");
            }
        }

        public string getConnectionString()
        {
            return reader.connectionString;
        }
    }
}

