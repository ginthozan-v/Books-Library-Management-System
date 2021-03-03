using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LMS.Views;
using System.Windows.Input;
using LMS.Helpers;

namespace LMS
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Shell.SetNavBarHasShadow(this, false);
            //this.CurrentItem.CurrentItem = ReserveTab;
            //Routing.RegisterRoute("loginPage", typeof(LoginPage));
        }

        private void Logout_Clicked(object sender, EventArgs e)
        {
            Settings.AccessToken = "";
            Settings.Username = "";
            Settings.Password = "";
            App.Current.MainPage = new LoginPage();
        }
    }
}