using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.Common
{
    public static class App      // dışarıdan erişilecek bir class tır. Static yapılarak new lenmeden kullanılacak.
    {
        //Common alanı artık DefaultCommon() metoduyla çalışacak. Bu metodda ICommon interface inden türediği için ICommon türünden olabiliyor
        public static ICommon Common = new DefaultCommon(); // golabal.asax.cs de metodlar ile Common alanı artık golabal.asax.cs class ındaki WebCommon() class ile çalışacak
    }
}
