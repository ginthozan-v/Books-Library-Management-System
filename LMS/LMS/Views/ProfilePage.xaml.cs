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
    public partial class ProfilePage : ContentPage
    {
        ProfileViewModel profilePage;

        public ProfilePage()
        {
            InitializeComponent();
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            profilePage = new ProfileViewModel();
            this.BindingContext = profilePage;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
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

        async void ShowReservedBooks(object sender, SelectionChangedEventArgs e)
        {
            var selectedBook = e.CurrentSelection.FirstOrDefault() as Book;
            if (selectedBook != null)
            {
                await App.Current.MainPage.Navigation.PushModalAsync(new DisplayReservedBook(selectedBook));
                ((CollectionView)sender).SelectedItem = null;
            }
        }
    }
}