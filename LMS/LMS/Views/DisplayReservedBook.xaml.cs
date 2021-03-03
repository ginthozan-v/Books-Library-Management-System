using LMS.Models;
using LMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LMS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DisplayReservedBook : ContentPage
    {
        public DisplayReservedBook(Book book)
        {
            InitializeComponent();
            BookReserveViewModel bookreserve = new BookReserveViewModel(book);
            this.BindingContext = bookreserve;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}