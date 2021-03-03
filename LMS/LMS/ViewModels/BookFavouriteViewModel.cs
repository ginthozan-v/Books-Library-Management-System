using LMS.Models;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;


namespace LMS.ViewModels
{
    public class BookFavouriteViewModel : BaseViewModel
    {
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

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await Task.Run(() => FavouriteBook());
            IsRefreshing = false;
        }

        public User user;
        public User AppUser;
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
        public BookFavouriteViewModel()
        {
            Title = "Favourite Books";
            FavouriteBook();
        }

        private void FavouriteBook()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            AppUser = cn.Table<User>().First();
            user = cn.GetWithChildren<User>(AppUser.Id);
            BookList.Clear();
            foreach (var book in user.FavouriteBookList)
            {
                BookList.Add(book);
            }
        }
    }
}
