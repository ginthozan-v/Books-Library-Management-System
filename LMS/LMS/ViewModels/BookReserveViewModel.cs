using LMS.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using System.Linq;
using System.Windows.Input;
using LMS.Services;
using SQLiteNetExtensions.Extensions;
using Xamarin.Essentials;

namespace LMS.ViewModels
{
    public class BookReserveViewModel : BaseViewModel
    {
        public ICommand SubmitReserveCommand => new Command(Reserve_Book);

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
        public BookReserve bookReserve { get; set; }
        public User user { get; set; }
        public BookReserveViewModel(Book book)
        {
            Title = "Lend Book";
            SelectedBook = book;

            ///Show available date
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            bookLend = (from bl in cn.Table<BookLend>()
                        where bl.BookId == SelectedBook.Id
                        select new BookLend
                        {
                            AvailableDate = (
                            bl.ExtendedDate.HasValue ? bl.ExtendedDate?.AddDays(1).ToString("dddd, dd MMMM yyyy") :
                            bl.EndDate == null ? bl.EndDate.AddDays(1).ToString("dddd, dd MMMM yyyy") : null
                            )
                        }).FirstOrDefault();

            if (bookLend == null)
            {
                bookLend = new BookLend();
                bookLend.BookId = SelectedBook.Id;
                bookLend.AvailableDate = DateTime.Now.ToString("dddd, dd MMMM yyyy");
            }
        }


        DateTime _reservationDate;
        public DateTime ReservationDate
        {
            get
            {
                return _reservationDate;
            }
            set
            {
                _reservationDate = value;
                OnPropertyChanged("ReservationDate");

            }
        }

        //Post reserve book
        private async void Reserve_Book(object obj)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            User user = cn.Table<User>().First();

            var BookReserveModel = new BookReserve();
            BookReserveModel.BookId = selectedBook.Id;
            BookReserveModel.ReserveOn = _reservationDate.Date;
            BookReserveModel.UserId = user.Id;

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                bool alreadyLend = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookLend WHERE BookId=? AND UserId=?)", selectedBook.Id, user.Id);
                if (alreadyLend == false)
                {
                    bool alreadyReserved = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookReserve WHERE BookId=?)", selectedBook.Id);
                    if (alreadyReserved == false)
                    {
                        bool availablity = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookReserve WHERE ReserveOn=?)", _reservationDate.Date);
                        if (availablity == false)
                        {
                            Service _service = new Service();
                            _service.PostReserveBookAsync(BookReserveModel);
                        }
                        else
                        {
                            await App.Current.MainPage.DisplayAlert("Some one already reserved on that date", _reservationDate.Date.ToString(), "OK");
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("You have already reserved", selectedBook.Title, "OK");
                    }
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("You have already lend", selectedBook.Title, "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Mobile network not available", "ok");
            }
        }
    }
}
