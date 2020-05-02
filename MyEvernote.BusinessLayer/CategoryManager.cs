using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class CategoryManager : ManagerBase<Category>
    {
        // ManagerBase class ını oluşturduğumuz için aşağıdki kodlara ihtiyacımız kalmadı. MAnagerBase<T> den miras alarak bu yapıyı kullandık

        //private Repository<Category> repo_category = new Repository<Category>();

        //public List<Category> GetCategories()
        //{
        //    return repo_category.List();
        //}

        //public Category GetCategoryById(int id)
        //{
        //    return repo_category.Find(x => x.Id == id);
        //}


        

    }
}
