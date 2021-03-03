using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LMS.Models
{
    [Table("Book")]
    public class Book
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Subtitle { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }

        [ManyToMany(typeof(BookCategory))]
        public List<Category> CategoriesList { get; set; }

        [ManyToMany(typeof(BookAuthor))]
        public List<Author> AuthorList { get; set; }

        public string FavouriteBtn { get; set; }
        public string BookTitle { get; set; }

    }
}
