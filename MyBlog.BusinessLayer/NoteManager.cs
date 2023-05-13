using MyBlog.DataAccessLayer.Concrete;
using MyBlog.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBlog.BusinessLayer
{
    public class NoteManager : BaseManager<Note>
    {
        public int InsertNote(Note note, string userName)
        {
            note.CreatedDate = DateTime.Now;
            note.ModifiedDate = DateTime.Now;
            note.ModifiedUserName = userName;

            return Insert(note);
        }
    }
}
