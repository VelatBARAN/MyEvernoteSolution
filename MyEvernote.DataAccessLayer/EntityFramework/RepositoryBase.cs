using MyEvernote.DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.DataAccessLayer.EntityFramework
{
    // SingletonPattern oluşumu 
    public class RepositoryBase
    {
        protected static DatabaseContext context;   // static metodlar static değişkenlere erişebilir.
        private static object _lockSync = new object();

        protected RepositoryBase() // new lenemez. Miras alınacak
        {
            CreateContext();
        }

        private static void CreateContext() // static olması new lenmeden kullanılmasını sağlar. Doğrudan DatabaseContext nesnesini döndürür.
        {
            if (context == null)
            {
                lock (_lockSync) // lock ile sadece tek bir iş parçacığın çalışmasını sağlar.
                {
                    if (context == null)
                    {
                        context = new DatabaseContext();
                    }
                }
            }
        }
    }
}
