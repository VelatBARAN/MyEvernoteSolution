using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common
{
    class DefaultCommon : ICommon  // varsayılan username in olduğu class
    {
        public string GetCurrentUsername()
        {
            return "system";
        }
    }
}
