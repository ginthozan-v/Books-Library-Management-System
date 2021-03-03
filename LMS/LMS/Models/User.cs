using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace LMS.Models
{
    [Table("User")]
    public class User
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string UserName { get; set; }

        [ManyToMany(typeof(BookLend))]
        public List<Book> LendBookList { get; set; }

        [ManyToMany(typeof(BookReserve))]
        public List<Book> ReserveBookList { get; set; }

        [ManyToMany(typeof(BookFavourite))]
        public List<Book> FavouriteBookList { get; set; }
    }
}
