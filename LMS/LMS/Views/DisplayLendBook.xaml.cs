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
    public partial class DisplayLendBook : ContentPage
    {
        public DisplayLendBook(Book book)
        {
            InitializeComponent();
            LendBookViewModel BookDet = new LendBookViewModel(book);
            this.BindingContext = BookDet;
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
    }
}