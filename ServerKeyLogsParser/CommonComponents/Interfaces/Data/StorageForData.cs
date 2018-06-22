using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerKeyLogsParser.CommonComponents.Interfaces.Data
{
    interface StorageForData<T>
    {
        T getData();
        void setData(T newData);
    }
}
