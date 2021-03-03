using LMS.Models;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace LMS.ViewModels
{
    public class BookByAuthorViewModels : BaseViewModel
    {
        public BookByAuthorViewModels(int Id)
        {
            Title = "Authors";
            ViewBookByAuthorList(Id);
        }

        public Author author { get; set; }

        ObservableCollection<Book> _bookList;
        public ObservableCollection<Book> BookList
        {
            get
            {
                if (_bookList == null)
                {
                    _bookList = new ObservableCollection<Book>();
                }
                return _bookList;
            }
        }

        private void ViewBookByAuthorList(int id)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();

            author = cn.GetWithChildren<Author>(id);
            var Books = author.BookList;
            User user = cn.Table<User>().First();

            BookList.Clear();
            foreach (var book in Books)
            {
                bool fBooks = cn.Table<BookFavourite>()
                    .Where(bf => bf.BookId == book.Id && bf.UserId == user.Id).Any();

                if (fBooks == true)
                {
                    book.FavouriteBtn = "heart.png";
                }
                else
                {
                    book.FavouriteBtn = "like.png";
                }
                if (book.Subtitle != null)
                {
                    book.BookTitle = book.Subtitle;
                }
                else if (book.Subtitle == null)
                {
                    book.BookTitle = book.Title;
                }
                BookList.Add(book);
            }

        }
    }
}   
