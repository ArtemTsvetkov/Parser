using ServerKeyLogsParser.CommonComponents.DataConverters.Basic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.ParserComponents.DataConverters
{
    class MappingIDWithNameConverter : DataConverter<DataSet, List<MappingIdWithName>>
    {
        public List<MappingIdWithName> convert(DataSet data)
        {
            List<MappingIdWithName> result = new List<MappingIdWithName>();
            for (int i = 0; i < data.Tables[0].Rows.Count; i++)
            {
                int id = int.Parse(data.Tables[0].Rows[i][0].ToString());
                string code = data.Tables[0].Rows[i][1].ToString();
                code = code.Replace(" ", "");

                result.Add(new MappingIdWithName(id,code));
            }
            return result;
        }
    }
}
