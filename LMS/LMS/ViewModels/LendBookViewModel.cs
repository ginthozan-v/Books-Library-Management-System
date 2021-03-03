using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using LMS.Models;
using Xamarin.Forms;
using LMS.Services;
using LMS.Views;
using Xamarin.Essentials;

namespace LMS.ViewModels
{
    public class LendBookViewModel : BaseViewModel
    {
        public ICommand SubmitCommand => new Command(Extend_or_Return_Book);

        private Book selectedBook;
        public Book SelectedBook
        {
            get { return selectedBook; }
            set
            {
                selectedBook = value;
                OnPropertyChanged();
            }
        }

        public BookLend bookLend { get; set; }
        public User user { get; set; }

        public LendBookViewModel(Book book)
        {
            Title = "Lend Book";
            SelectedBook = book;

            //Show dates
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            user = cn.Table<User>().First();
            bookLend = (from bl in cn.Table<BookLend>()
                        where bl.BookId == SelectedBook.Id &&
                        bl.UserId == user.Id
                        select new BookLend
                        {
                            DueDate = bl.EndDate.ToString("dddd, dd MMMM yyyy"),
                            ExtendedDateString = (bl.ExtendedDate.HasValue ? bl.ExtendedDate?.ToString("dddd, dd MMMM yyyy") : "n/a")
                        }).FirstOrDefault();
        }

        private async void Extend_or_Return_Book(object obj)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            user = cn.Table<User>().First();
            BookLend lendedbook = cn.Table<BookLend>()
                .Where(bl => bl.BookId == selectedBook.Id && bl.UserId == user.Id).FirstOrDefault();

            var BookLendModel = new BookLend();
            BookLendModel.Id = lendedbook.Id;
            BookLendModel.BookId = selectedBook.Id;
            BookLendModel.ActionType = (string)obj;

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                if ((string)obj == "Extend" && lendedbook.ExtendedDate != null)
                {
                    await App.Current.MainPage.DisplayAlert("Not Allowed", "You can extend only one time", "OK");
                }
                else
                {
                    Service _service = new Service();
                    await _service.PostReturnBookAsync(BookLendModel);
                }

                await App.Current.MainPage.Navigation.PopModalAsync();
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Mobile network not available", "ok");
            }
        }
    }
}
