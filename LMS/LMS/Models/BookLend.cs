using LMS.ViewModels;
using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace LMS.Models
{
    [Table("BookLend")]
    public class BookLend
    {
        [PrimaryKey]
        public int Id { get; set; }

        [ForeignKey(typeof(Book))]
        public int BookId { get; set; }

        [ForeignKey(typeof(User))]
        public string UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? ExtendedDate { get; set; }
        public string ActionType { get; set; }
        public int Returned { get; set; }

        public string DueDate { get; set; }
        public string ExtendedDateString { get; set; }
        public string AvailableDate { get; set; }
    }
}
