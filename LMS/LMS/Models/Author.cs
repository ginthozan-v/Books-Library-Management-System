using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LMS.Models
{
    [Table("Author")]
    public class Author
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }

        [ManyToMany(typeof(BookAuthor))]
        public ObservableCollection<Book> BookList { get; set; }
    }
}
