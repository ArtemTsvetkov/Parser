using ServerKeyLogsParser.CommonComponents.DataConverters.Basic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.DataConverters
{
    class ConverterSingleColumnFromDataSet : DataConverter<DataSet, string[]>
    {
        public string[] convert(DataSet data)
        {
            DataSet ds = data;
            string[] newData = new string[ds.Tables[0].Rows.Count];
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                newData[i] = ds.Tables[0].Rows[i][0].ToString();
            }
            return newData;
        }
    }
}
