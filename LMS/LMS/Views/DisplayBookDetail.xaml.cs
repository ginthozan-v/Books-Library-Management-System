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
    public partial class DisplayBookDetail : ContentPage
    {
        public BookDetailViewModel BookDet;

        public DisplayBookDetail(Book book)
        {
            InitializeComponent();
            BookDet = new BookDetailViewModel(book);
            this.BindingContext = BookDet;
            //NavigationPage.SetHasNavigationBar(this, false);

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}