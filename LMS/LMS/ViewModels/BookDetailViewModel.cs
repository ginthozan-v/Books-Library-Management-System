using LMS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using LMS.Services;
using System.Windows.Input;
using Xamarin.Forms;
using System.Linq;
using LMS.Helpers;
using SQLiteNetExtensions.Extensions;
using LMS.Views;
using Xamarin.Essentials;

namespace LMS.ViewModels
{
    public class BookDetailViewModel : BaseViewModel
    {
        public ICommand SubmitLendCommand => new Command(Lend_Book); 
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

        public Book book { get; set; }
        public List<Author> Authors { get; set; }
        public List<Category> Categories { get; set; }

        public BookDetailViewModel(Book book)
        {
            Title = "Book Detail";
            SelectedBook = book;
           
            var cn = DependencyService.Get<ISQLite>().GetConnection();           
            book = cn.GetWithChildren<Book>(SelectedBook.Id);
            Authors = book.AuthorList;
            Categories = book.CategoriesList;
        }

        ///////////////////////////////////////
        //** Lend Book **/ ///////////////
        private async void Lend_Book(object obj)
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            User user = cn.Table<User>().First();

            var BookLendModel = new BookLend();
            BookLendModel.BookId = selectedBook.Id;
            BookLendModel.UserId = user.Id;

            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                bool bookAvailability = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM Book WHERE Qty > 0 AND Id=?)", selectedBook.Id);
                if (bookAvailability == true)
                {
                    bool alreadyLend = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookLend WHERE BookId=? AND UserId=?)", selectedBook.Id, user.Id);
                    if (alreadyLend == true)
                    {
                        await App.Current.MainPage.DisplayAlert("You have already lend", selectedBook.Title, "OK");
                    }
                    else
                    {
                        Service _service = new Service();
                        await _service.PostLendBookAsync(BookLendModel);
                    }
                }
                else
                {
                    //await App.Current.MainPage.DisplayAlert("Currently Not Available!","You can reserver the book: " + selectedBook.Title + " and get it once available", "OK");
                    bool answer = await App.Current.MainPage.DisplayAlert("Currently Not Available!", "Would you like to reserve the book", "Yes", "No");
                    if (answer == true)
                    {
                        await App.Current.MainPage.Navigation.PushModalAsync(new BookReservation(selectedBook));
                    }
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Alert", "Mobile network not available", "ok");
            }
        }

        private void Reserve_Book(object obj)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new BookReservation(selectedBook));
        }
    }
}
