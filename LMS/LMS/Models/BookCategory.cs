using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Models
{
    [Table("BookCategory")]
    public class BookCategory
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Book))]
        public int BookId { get; set; }

        [ForeignKey(typeof(Category))]
        public int CategoryId { get; set; }
    }
}
