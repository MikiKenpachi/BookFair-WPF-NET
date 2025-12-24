using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Storage.Serialization
{
    public interface ISeriazable
    {
        string[] ToCSV();

        void FromCSV(string[] values);
    }
}
