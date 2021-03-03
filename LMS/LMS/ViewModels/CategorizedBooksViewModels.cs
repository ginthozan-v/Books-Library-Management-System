using LMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using LMS.Services;
using Xamarin.Forms;
using SQLiteNetExtensions.Extensions;
using System.Collections.ObjectModel;
using System.Linq;

namespace LMS.ViewModels
{
    public class CategorizedBooksViewModels : BaseViewModel
    {
        public CategorizedBooksViewModels(int Id)
        {
            Title = "Category";
            ViewCategorizedBookList(Id);
        }

        public Category category { get; set; }

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

        private void ViewCategorizedBookList(int Id)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();

            category = cn.GetWithChildren<Category>(Id);
            var Books = category.BooksList;
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
