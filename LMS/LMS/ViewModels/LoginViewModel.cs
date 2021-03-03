using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using LMS.Services;
using LMS.Views;
using LMS.Helpers;
using Xamarin.Essentials;

namespace LMS.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        Service _service = new Service();

        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        //public ICommand SubmitCommand { get; set; }
        public ICommand LoginCommand
        {
            get
            {
                return new Command(async () =>
                {
                    var current = Connectivity.NetworkAccess;
                    if (current == NetworkAccess.Internet)
                    {
                        if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                        {
                            await App.Current.MainPage.DisplayAlert("Alert", "All fields are required!", "ok");
                        }
                        else
                        {
                            var accesstoken = await _service.LoginAsync(UserName, Password);
                            if (!string.IsNullOrEmpty(accesstoken))
                            {
                                Settings.AccessToken = accesstoken;
                                Settings.Username = UserName;
                                Settings.Password = Password;
                                await _service.GetUserAsync();
                                App.Current.MainPage = new AppShell();
                            }
                            else
                            {
                                await App.Current.MainPage.DisplayAlert("Alert", "Username or password is incorrect", "ok");

                            }
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert("Alert","Mobile network not available", "ok");
                    }
                });
            }
        }

        public LoginViewModel()
        {
            UserName = Settings.Username;
            Password = Settings.Password;
        }
    }
}
