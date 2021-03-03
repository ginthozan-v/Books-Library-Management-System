using LMS.Models;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LMS.ViewModels;
using LMS.Helpers;
using LMS.Services;
using System.Collections.ObjectModel;

namespace LMS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Home : ContentPage
    {
        HomeViewModel home;
        public Home()
        {
            InitializeComponent();
            var ct = new CreateLmsTable();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            home = new HomeViewModel();
            this.BindingContext = home;
        }

        async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = e.CurrentSelection.FirstOrDefault() as Book;
            if (selectedBook != null)
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new DisplayBookDetail(selectedBook));

                ((CollectionView)sender).SelectedItem = null;
            }
        }

        public User AppUser { get; set; }
        public ObservableCollection<Book> Books { get; set; }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Image bookSender = (Image)sender;
            if (bookSender.GestureRecognizers.Count > 0)
            {
                var gesture = (TapGestureRecognizer)bookSender.GestureRecognizers[0];
                int SelectedBookId = (int)gesture.CommandParameter;


                var cn = DependencyService.Get<ISQLite>().GetConnection();
                AppUser = cn.Table<User>().First();
                
                cn.CreateTable<BookFavourite>();
                var favBooks = cn.Table<BookFavourite>().ToList();

                bool bookAvailability = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookFavourite WHERE BookId=? AND UserId=?)", SelectedBookId, AppUser.Id);
                if(bookAvailability == true)
                {
                    BookFavourite books = cn.Table<BookFavourite>().Where(b => b.BookId == SelectedBookId).First();
                    var result = cn.Delete<BookFavourite>(books.Id);

                    this.BindingContext = new HomeViewModel();
                }
                else
                {
                    BookFavourite books = new BookFavourite();
                    books.BookId = SelectedBookId;
                    books.UserId = AppUser.Id;
                    var result = cn.Insert(books);
                    if (result == 1)
                    {
                        cn.Close();
                        this.BindingContext =new HomeViewModel();
                    }
                }
            }
        }
    }
}