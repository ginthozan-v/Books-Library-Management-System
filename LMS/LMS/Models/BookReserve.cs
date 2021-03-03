using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Models
{
    public class BookReserve
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Book))]
        public int BookId { get; set; }

        [ForeignKey(typeof(User))]
        public string UserId { get; set; }

        public DateTime ReserveOn { get; set; }


        public string Title { get; set; }
    }
}
