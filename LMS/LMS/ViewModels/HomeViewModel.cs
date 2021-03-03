using System;
using System.Collections.Generic;
using System.Text;
using LMS.Models;
using LMS.Services;
using Xamarin.Forms;
using System.Windows.Input;
using LMS.Views;
using System.Collections.ObjectModel;
using System.Linq;
using LMS.Helpers;
using Xamarin.Essentials;
using SQLiteNetExtensions.Extensions;
using System.Threading.Tasks;

namespace LMS.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        Service _service = new Service();

        public ICommand SelectionCommand => new Command(DisplayBookByCategory);
        public ICommand SelectionCommandAuthor => new Command(DisplayBookByAuthor);
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

        public HomeViewModel()
        {
            Title = "Home";
            ViewCategoryList();
            ViewAuthorList();
            ViewBookList();
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await Task.Delay(TimeSpan.FromSeconds(RefreshDuration));
            await Task.Run(() => ViewBookList());
            await Task.Run(() => ViewCategoryList());
            await Task.Run(() => ViewAuthorList());
            IsRefreshing = false;
        }


        ////<summary>
        ////View Book List
        ////</summary>
        ObservableCollection<Book> _books;
        public ObservableCollection<Book> Books
        {
            get
            {
                if (_books == null)
                {
                    _books = new ObservableCollection<Book>();
                }
                return _books;
            }
        }
        public async void ViewBookList()
        {
            //Retrieve Data From Server and store to local
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await _service.GetBookAsync();
            }

            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var books = cn.Table<Book>().ToList();
            User user = cn.Table<User>().First();

            Books.Clear();
            foreach (var book in books)
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
                Books.Add(book);
            }

        }

        ////<summary>
        ////View Category List
        ////</summary>
        ObservableCollection<Category> _categories;
        public ObservableCollection<Category> Categories
        {
            get
            {
                if (_categories == null)
                {
                    _categories = new ObservableCollection<Category>();
                }
                return _categories;
            }
        }
        public async void ViewCategoryList()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await _service.GetCategoryAsync();
            }

            //Load data from Local
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var categories = cn.Table<Category>().ToList();
            Categories.Clear();
            foreach (var category in categories)
            {
                Categories.Add(category);
            }
        }

        ////<summary>
        ////Display Book By Category
        ////</summary>
        private Category selectedCategory;
        public Category SelectedCategory
        {
            get { return selectedCategory; }
            set
            {
                selectedCategory = value;
                OnPropertyChanged();
            }

        }
        private void DisplayBookByCategory(object obj)
        {
            if (SelectedCategory != null)
            {
                int catId = selectedCategory.Id;
                App.Current.MainPage.Navigation.PushAsync(new DisplayCategorizedBooks(catId));
            }
        }

        ////<summary>
        ////View Author List
        ////</summary>
        ObservableCollection<Author> _authors;
        public ObservableCollection<Author> Authors
        {
            get
            {
                if (_authors == null)
                {
                    _authors = new ObservableCollection<Author>();
                }
                return _authors;
            }
        }
        private async void ViewAuthorList()
        {
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await _service.GetAuthorsAsync();
            }

            //Load data from Local
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            var authors = new ObservableCollection<Author>(cn.Table<Author>().ToList());
            Authors.Clear();
            foreach (var author in authors)
            {
                Authors.Add(author);
            }
        }

        ////<summary>
        ////Display Book By Author
        ////</summary>
        private Author selectedAuthor;
        public Author SelectedAuthor
        {
            get { return selectedAuthor; }
            set
            {
                selectedAuthor = value;
                OnPropertyChanged();
            }

        }
        private void DisplayBookByAuthor(object obj)
        {
            if (selectedAuthor != null)
            {
                int authId = selectedAuthor.Id;
                App.Current.MainPage.Navigation.PushAsync(new DisplayBooksByAuthor(authId));
            }
        }
    }
}
