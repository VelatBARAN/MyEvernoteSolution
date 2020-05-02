using MyEvernote.Common;
using MyEvernote.Core.DataAccess;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MyEvernote.DataAccessLayer.EntityFramework
{                        //protected tanımlanarak Miraz alındı
    public class Repository<T> : RepositoryBase, IDataAccess<T> where T : class // class tipi yani new lenen bir yapı oldu. Interface den implement ettik. Interface deeki metodların hepsi burdada var.
    {                                                                          // Interface deki metodlar soyutlaşmış metodlardır. Burda kullanıldığında somutlaşır.Eksik bir metod olduğunda hata verir.
        private DbSet<T> _objectSet;

        public Repository()  // Repository classı bir kere new lendiğinde 
        {
          //  db = new RepositoryBase(); //  RepositoryBase class ında protected ve static olarak tanımlayarak new lemeyi engelledik.
            _objectSet = context.Set<T>();
        }

        public List<T> List()
        {
            return _objectSet.ToList();
        }

        public IQueryable<T> ListQueryable() // IQueryable olması bunun Orderby gibi metodların kullanmasını sağlar
        {
            return _objectSet.AsQueryable<T>();
        }

        public List<T> List(Expression<Func<T, bool>> where) 
        {
            return _objectSet.Where(where).ToList();
        }

        public int Insert(T obj)
        {
            _objectSet.Add(obj);

            if(obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                DateTime now = DateTime.Now;
                o.CreateOn = now;
                o.ModifiedOn = now;
                o.ModifiedUsurname = App.Common.GetCurrentUsername();  // TODO : işlem yapan kullanıcı yazılacak
            }

            return Save();
        }

        public int Update(T obj)
        {
            if (obj is MyEntityBase)
            {
                MyEntityBase o = obj as MyEntityBase;
                o.ModifiedOn = DateTime.Now;
                o.ModifiedUsurname = App.Common.GetCurrentUsername();
            }

            return Save();
        }
       
        public int Delete(T obj)
        {
            // aşağıdaki kodları eğer database den silme değilde silinmiş mi gibi bir kolon eklenirse yazılabilir.
            //if (obj is MyEntityBase)
            //{
            //    MyEntityBase o = obj as MyEntityBase;
            //    o.ModifiedOn = DateTime.Now;
            //    o.ModifiedUsurname = "system";  // TODO : işlem yapan kullanıcı yazılacak
            //}

            _objectSet.Remove(obj);
            return Save();
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        public T Find(Expression<Func<T, bool>> where) // tek bit T tipi döner.(Note,EvernoteUser gibi...)
        {
            return _objectSet.FirstOrDefault(where);
        }
        
    }
}
