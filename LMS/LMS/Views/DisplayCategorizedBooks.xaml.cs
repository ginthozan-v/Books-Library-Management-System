﻿using LMS.Models;
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
    public partial class DisplayCategorizedBooks : ContentPage
    {
        public DisplayCategorizedBooks(int Id)
        {
            InitializeComponent();
            this.BindingContext = new CategorizedBooksViewModels(Id);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
        }
        private void LikeBtn_Tapped(object sender, EventArgs e)
        {
            Navigation.PopModalAsync();
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
    }
}