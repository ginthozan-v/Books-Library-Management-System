using LMS.Models;
using LMS.ViewModels;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LMS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FavouritePage : ContentPage
    {
        public FavouritePage()
        {
            InitializeComponent();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            BookFavouriteViewModel favPage = new BookFavouriteViewModel();
            this.BindingContext = favPage;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Image bookSender = (Image)sender;
            if (bookSender.GestureRecognizers.Count > 0)
            {
                var gesture = (TapGestureRecognizer)bookSender.GestureRecognizers[0];
                int SelectedBookId = (int)gesture.CommandParameter;

                var cn = DependencyService.Get<ISQLite>().GetConnection();
                BookFavourite books = cn.Table<BookFavourite>()
                    .Where(b => b.BookId == SelectedBookId).First();
                var result = cn.Delete<BookFavourite>(books.Id);
                this.BindingContext = new BookFavouriteViewModel();

            }
        }
    }
}