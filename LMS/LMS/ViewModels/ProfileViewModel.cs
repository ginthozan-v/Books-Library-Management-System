using System;
using System.Collections.Generic;
using System.Text;
using LMS.Services;
using LMS.Helpers;
using Xamarin.Forms;
using LMS.Models;
using SQLiteNetExtensions.Extensions;
using System.Linq;
using LMS.Views;
using Xamarin.Essentials;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Threading.Tasks;

namespace LMS.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        Service _service = new Service();

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());
        const int RefreshDuration = 2;
        bool isRefreshing;
        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ProfileViewModel()
        {
            Title = "Profile";
            ShowLendedReservedBooks();
            //DependencyService.Get<IStatusBarStyleManager>()?.SetColor(Android.Graphics.Color.Red);
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await Task.Run(() => ShowLendedReservedBooks());
            IsRefreshing = false;
        }

        public User AppUser { get; set; }
        public User user { get; set; }

        ObservableCollection<Book> _reservedBook;
        public ObservableCollection<Book> ReservedBooks
        {
            get
            {
                if (_reservedBook == null)
                {
                    _reservedBook = new ObservableCollection<Book>();
                }
                return _reservedBook;
            }
        }

        ObservableCollection<Book> _lendBooks;
        public ObservableCollection<Book> LendBooks
        {
            get
            {
                if (_lendBooks == null)
                {
                    _lendBooks = new ObservableCollection<Book>();
                }
                return _lendBooks;
            }
        }

        private void ShowLendedReservedBooks()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                Task.Run(() => _service.GetLendBookAsync());
                Task.Run(() => _service.GetReserveBookAsync());
            }

            ///////////////////////////////////////
            //** Fetch data from SQLite **/ //////
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            AppUser = cn.Table<User>().First();
            user = cn.GetWithChildren<User>(AppUser.Id);

            //** Lend Book **/ //////
            var lendbooks = user.LendBookList;
            LendBooks.Clear();
            foreach (var book in lendbooks)
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
                LendBooks.Add(book);
            }

            //** Reserved Book **/ //////
            var reservedbooks = user.ReserveBookList;
            ReservedBooks.Clear();
            foreach (var book in reservedbooks)
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
                ReservedBooks.Add(book);
            }
        }


        async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = e.CurrentSelection.FirstOrDefault() as Book;
            if (selectedBook != null)
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new DisplayLendBook(selectedBook));
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}
