using LMS.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LMS.Helpers;
using LMS.Services;
using System.Collections.Generic;
using LMS.Models;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace LMS
{
    public partial class App : Application
    {
        Service _service = new Service();
        public App()
        {
            InitializeComponent();
            SetMainPage();
        }

        private void SetMainPage()
        {
            if (!string.IsNullOrEmpty(Settings.AccessToken))
            {
                MainPage = new AppShell();
            }
            else
            {
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.DropTable<User>();
                cn.CreateTable<User>();
                MainPage = new LoginPage();
            }
        }

        protected override void OnStart()
        {
            //Create Tables////////////////////////
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            cn.DropTable<Book>();
            cn.DropTable<Category>();
            cn.DropTable<Author>();
            cn.DropTable<BookAuthor>();
            cn.DropTable<BookCategory>();
            cn.DropTable<BookLend>();
            cn.DropTable<BookReserve>();

            cn.CreateTable<Category>();
            cn.CreateTable<Book>();
            cn.CreateTable<Author>();
            cn.CreateTable<BookAuthor>();
            cn.CreateTable<BookCategory>();
            cn.CreateTable<BookLend>();
            cn.CreateTable<BookFavourite>();
            cn.CreateTable<BookReserve>();


            Task.Run(() => RetrieveData()); 
           Task.Run(() => Notification());
        }

        private async Task RetrieveData()
        {
            //Retrieve Data From Server and store to local///////////////////
            var current = Connectivity.NetworkAccess;
            if (current == NetworkAccess.Internet)
            {
                await _service.GetBookAsync();
                await _service.GetCategoryAsync();
                await _service.GetBookCategory();
                await _service.GetAuthorsAsync();
                await _service.GetBookAuthorAsync();
                await _service.GetLendBookAsync();
                await _service.GetReserveBookAsync();
            }
        }

        private async Task Notification()
        {
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            await Task.Delay(5);
            //Notification ///////////////////
            List<BookLend> dueDateList;
            List<BookLend> lateBooks;
            List<BookReserve> reserveBook;
            User user;
            Book book;

            var UserTable = cn.Table<User>().ToList();
            if (UserTable.Count > 0)
            {
                user = cn.Table<User>().First();

                dueDateList = cn.Table<BookLend>()
                    .Where(bl => bl.UserId == user.Id && (bl.EndDate == System.DateTime.Today ||
                    bl.ExtendedDate == System.DateTime.Today)).ToList();

                if (dueDateList.Count > 0)
                {
                    foreach (BookLend bl in dueDateList)
                    {
                        book = cn.Table<Book>().Where(b => b.Id == bl.BookId).First();
                        DependencyService.Get<INotification>()
                            .CreateNotification("LMS", "The due date of the book:" + book.Title + "is Today");
                    }
                }

                lateBooks = cn.Table<BookLend>()
                    .Where(lb =>
                    (lb.UserId == user.Id) &&
                    ((lb.EndDate < System.DateTime.Now.Date && lb.ExtendedDate == null) ||
                    (lb.ExtendedDate < System.DateTime.Now.Date)) &&
                    (lb.Returned == 0)).ToList();

                if (lateBooks.Count > 0)
                {
                    foreach (BookLend lb in lateBooks)
                    {
                        book = cn.Table<Book>().Where(b => b.Id == lb.BookId).First();
                        DependencyService.Get<INotification>()
                            .CreateNotification("LMS", "Please return the book: " + book.Title + " or we will call the police!");
                    }
                }

                reserveBook = cn.Table<BookReserve>()
                    .Where(br =>
                    (br.UserId == user.Id) &&
                    (br.ReserveOn == DateTime.Now.Date)).ToList();

                if (reserveBook.Count > 0)
                {
                    foreach (BookReserve br in reserveBook)
                    {
                        book = cn.Table<Book>().Where(b => b.Id == br.BookId).First();
                        DependencyService.Get<INotification>()
                            .CreateNotification("LMS", "The book: " + book.Title + " you reserved is available, Please collect it soon!");
                    }
                }
            }
        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {
        }
    }


}
