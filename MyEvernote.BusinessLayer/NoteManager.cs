using MyEvernote.BusinessLayer.Abstract;
using MyEvernote.DataAccessLayer.EntityFramework;
using MyEvernote.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyEvernote.BusinessLayer
{
    public class NoteManager : ManagerBase<Note>
    {
        //private Repository<Note> repo_note = new Repository<Note>();

        //public List<Note> GetAllNotes()
        //{
        //    return repo_note.List();
        //}

        //// yukarıdaki sorgu ile aynı çalışır
        //public IQueryable<Note> GetAllNotesQueryable()
        //{
        //    return repo_note.ListQueryable();
        //}

    }
}
