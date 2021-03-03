using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LMS.Services;
using LMS.ViewModels;

namespace LMS.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginViewModel login;

        public LoginPage()
        {
            InitializeComponent();
            login = new LoginViewModel();
            this.BindingContext = login;
        }
    }
}