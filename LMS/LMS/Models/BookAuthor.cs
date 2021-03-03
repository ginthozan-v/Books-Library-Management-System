using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Models
{
    [Table("BookAuthor")]
    public class BookAuthor
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Book))]
        public int BookId { get; set; }

        [ForeignKey(typeof(Author))]
        public int AuthorId { get; set; }
    }
}
