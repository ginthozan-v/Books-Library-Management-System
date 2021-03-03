using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LMS.Models
{
    [Table("Category")]
    public class Category
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }


        [ManyToMany(typeof(BookCategory))] 
        public ObservableCollection<Book> BooksList { get; set; }
    }
}
