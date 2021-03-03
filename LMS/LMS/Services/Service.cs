using LMS.Helpers;
using LMS.Models;
using LMS.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Net;
using Xamarin.Essentials;

namespace LMS.Services
{
    public class Service
    {
        static HttpClient client = new HttpClient();
        static string BaseURL = Constants.BaseURL;

        ///////////////////////////////////////
        //** Login**/ //////////////////////// 
        public async Task<string> LoginAsync(string userName, string password)
        {
            string URL = BaseURL + "token";
            var accessToken = string.Empty;
            await Task.Run(() =>
            {
                try
                {
                    var keyValues = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("username", userName),
                        new KeyValuePair<string, string>("password", password),
                        new KeyValuePair<string, string>("grant_type", "password"),
                    };

                    var request = new HttpRequestMessage(
                        HttpMethod.Post, URL);

                    request.Content = new FormUrlEncodedContent(keyValues);

                    var response = client.SendAsync(request).Result;
                    using (HttpContent content = response.Content)
                    {
                        var json = content.ReadAsStringAsync();
                        JObject jwtDynamic = JsonConvert.DeserializeObject<dynamic>(json.Result);

                        var accessTokenExpiration = jwtDynamic.Value<DateTime>(".expires");
                        accessToken = jwtDynamic.Value<string>("access_token");
                        var username = jwtDynamic.Value<string>("userName");
                        var AccessTokenExpirationDate = accessTokenExpiration;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            });
            return accessToken;
        }

        ///////////////////////////////////////
        //** Get Current User **/ ////////////
        public async Task GetUserAsync()
        {
            User userData;
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
            var Response = await client.GetAsync(BaseURL + "api/AspNetUsers");
            Response.EnsureSuccessStatusCode();
            var content = Response.Content.ReadAsStringAsync();
            userData = JsonConvert.DeserializeObject<User>(content.Result);

            //Store data to local SQLite//////////
            var cn = DependencyService.Get<ISQLite>().GetConnection();
            cn.DropTable<User>();
            cn.CreateTable<User>();
            try
            {
                User user = new User();
                user.Id = userData.Id;
                user.UserName = userData.UserName;

                cn.Insert(user);
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///////////////////////////////////////
        //** Get Books List **/ //////////////
        public async Task GetBookAsync()
        {
            try
            {
                ObservableCollection<Book> BookData = null;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Settings.AccessToken);

                var Response = await client.GetAsync(BaseURL + "api/Books");
                Response.EnsureSuccessStatusCode();
                var content = Response.Content.ReadAsStringAsync();
                BookData = JsonConvert.DeserializeObject<ObservableCollection<Book>>(content.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<Book>();

                foreach (Book b in BookData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM Book WHERE Id=?)", b.Id);

                    if (exists == false)
                    {
                        Book book = new Book();
                        book.Id = b.Id;
                        book.Title = b.Title;
                        book.Subtitle = b.Subtitle;
                        book.Image = b.Image;
                        book.Description = b.Description;
                        book.Qty = b.Qty;
                        cn.Insert(book);
                    }
                    else
                    {
                        Book book = new Book();
                        book.Id = b.Id;
                        book.Title = b.Title;
                        book.Subtitle = b.Subtitle;
                        book.Image = b.Image;
                        book.Description = b.Description;
                        book.Qty = b.Qty;
                        cn.Update(book);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///////////////////////////////////////
        //** Get Categories List **/ /////////
        public async Task GetCategoryAsync()
        {
            try
            {
                ObservableCollection<Category> CategoryData = null;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Settings.AccessToken);

                var Response = await client.GetAsync(BaseURL + "api/Categories");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                CategoryData = JsonConvert.DeserializeObject<ObservableCollection<Category>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<Category>();

                foreach (Category c in CategoryData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM Category WHERE Id=?)", c.Id);

                    if (exists == false)
                    {
                        Category cat = new Category();
                        cat.Id = c.Id;
                        cat.Name = c.Name;

                        cn.Insert(cat);
                    }
                    else
                    {
                        Category cat = new Category();
                        cat.Id = c.Id;
                        cat.Name = c.Name;

                        cn.Update(cat);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                var msg = ex.GetBaseException().Message;
            }
        }

        ///////////////////////////////////////
        //** Get Book Category **/ ///////////
        public async Task GetBookCategory()
        {
            try
            {
                IList<BookCategory> BookCategoryData = null;
                client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.GetAsync(BaseURL + "api/Book_Category");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                BookCategoryData = JsonConvert.DeserializeObject<List<BookCategory>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<BookCategory>();

                foreach (BookCategory bc in BookCategoryData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM BookCategory WHERE Id=?)", bc.Id);

                    if (exists == false)
                    {
                        BookCategory bookcat = new BookCategory();
                        bookcat.Id = bc.Id;
                        bookcat.BookId = bc.BookId;
                        bookcat.CategoryId = bc.CategoryId;
                        cn.Insert(bookcat);
                    }
                    else
                    {
                        BookCategory bookcat = new BookCategory();
                        bookcat.Id = bc.Id;
                        bookcat.BookId = bc.BookId;
                        bookcat.CategoryId = bc.CategoryId;
                        cn.Update(bookcat);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///////////////////////////////////////
        //** Get Author **/ //////////////////
        public async Task GetAuthorsAsync()
        {
            try
            {
                IList<Author> AuthorData = null;
                client.DefaultRequestHeaders.Authorization =
                                new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.GetAsync(BaseURL + "api/Authors");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                AuthorData = JsonConvert.DeserializeObject<List<Author>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<Author>();

                foreach (Author a in AuthorData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM Author WHERE Id=?)", a.Id);

                    if (exists == false)
                    {
                        Author author = new Author();
                        author.Id = a.Id;
                        author.Name = a.Name;
                        author.Picture = a.Picture;
                        cn.Insert(author);
                    }
                    else
                    {
                        Author author = new Author();
                        author.Id = a.Id;
                        author.Name = a.Name;
                        author.Picture = a.Picture;
                        cn.Update(author);
                    }
                }
                cn.Close();
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///////////////////////////////////////
        //** Get Book Author **/ /////////////
        public async Task GetBookAuthorAsync()
        {
            try
            {
                IList<BookAuthor> BookAuthorData = null;
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.GetAsync(BaseURL + "api/Book_Author");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                BookAuthorData = JsonConvert.DeserializeObject<List<BookAuthor>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<BookAuthor>();

                foreach (BookAuthor ba in BookAuthorData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM BookAuthor WHERE Id=?)", ba.Id);

                    if (exists == false)
                    {
                        BookAuthor bookauth = new BookAuthor();
                        bookauth.Id = ba.Id;
                        bookauth.BookId = ba.BookId;
                        bookauth.AuthorId = ba.AuthorId;
                        cn.Insert(bookauth);
                    }
                    else
                    {
                        BookAuthor bookauth = new BookAuthor();
                        bookauth.Id = ba.Id;
                        bookauth.BookId = ba.BookId;
                        bookauth.AuthorId = ba.AuthorId;
                        cn.Update(bookauth);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ///////////////////////////////////////
        //** Post Lend Book **/ //////////////
        public async Task PostLendBookAsync(BookLend bookLend)
        {
            try
            {
                string URL = BaseURL + "api/Book_Lend";
                string bl = JsonConvert.SerializeObject(bookLend);
                StringContent content = new StringContent(bl, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.PostAsync(URL, content);
                if (Response.IsSuccessStatusCode == true)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Lending Success", "Ok");
                    await GetLendBookAsync();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        ///////////////////////////////////////
        //** Get Lend Book **/ ///////////////
        public async Task GetLendBookAsync()
        {
            try
            {
                IList<BookLend> BookLendData = null;
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.GetAsync(BaseURL + "api/Book_Lend");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                BookLendData = JsonConvert.DeserializeObject<List<BookLend>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.DropTable<BookLend>();
                cn.CreateTable<BookLend>();

                foreach (BookLend bl in BookLendData)
                {
                    bool exists = cn.ExecuteScalar<bool>
                        ("SELECT EXISTS(SELECT * FROM BookLend WHERE Id=?)", bl.Id);

                    if (exists == false)
                    {
                        BookLend bookLend = new BookLend();
                        bookLend.Id = bl.Id;
                        bookLend.BookId = bl.BookId;
                        bookLend.UserId = bl.UserId;
                        bookLend.StartDate = bl.StartDate;
                        bookLend.EndDate = bl.EndDate;
                        bookLend.ExtendedDate = bl.ExtendedDate;
                        bookLend.Returned = bl.Returned;
                        cn.Insert(bookLend);
                    }
                    else
                    {
                        BookLend bookLend = new BookLend();
                        bookLend.Id = bl.Id;
                        bookLend.BookId = bl.BookId;
                        bookLend.UserId = bl.UserId;
                        bookLend.StartDate = bl.StartDate;
                        bookLend.ExtendedDate = bl.ExtendedDate;
                        bookLend.Returned = bl.Returned;
                        cn.Update(bookLend);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        ////////////////////////////////////////
        //** Post Return Book **/ /////////////
        public async Task PostReturnBookAsync(BookLend bookLend)
        {
            try
            {
                string URL = BaseURL + "api/Book_Lend";
                string bl = JsonConvert.SerializeObject(bookLend);
                StringContent content = new StringContent(bl, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.PutAsync(URL, content);
                if (Response.IsSuccessStatusCode == true)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Success", "Ok");
                    await GetLendBookAsync();
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }
        }

        ///////////////////////////////////////
        //** Post Reserve Book **/ ///////////
        public async Task PostReserveBookAsync(BookReserve bookReserve)
        {
            try
            {
                string URL = BaseURL + "api/Book_Reserve";
                string bl = JsonConvert.SerializeObject(bookReserve);
                StringContent content = new StringContent(bl, Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.PostAsync(URL, content);
                if (Response.IsSuccessStatusCode == true)
                {
                    await App.Current.MainPage.DisplayAlert("Success", "Reserved Success", "Ok");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
            }

        }

        ///////////////////////////////////////
        //** Get Lend Book **/ ///////////////
        public async Task GetReserveBookAsync()
        {
            try
            {
                ObservableCollection<BookReserve> BookReserveData = null;
                client.DefaultRequestHeaders.Authorization =
                  new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
                var Response = await client.GetAsync(BaseURL + "api/Book_Reserve");
                Response.EnsureSuccessStatusCode();
                var jsonContent = Response.Content.ReadAsStringAsync();
                BookReserveData = JsonConvert.DeserializeObject<ObservableCollection<BookReserve>>(jsonContent.Result);

                //Store data to local SQLite//////////
                var cn = DependencyService.Get<ISQLite>().GetConnection();
                cn.CreateTable<BookReserve>();

                foreach (BookReserve br in BookReserveData)
                {
                    bool exists = cn.ExecuteScalar<bool>("SELECT EXISTS(SELECT * FROM BookReserve WHERE Id=?)", br.Id);

                    if (exists == false)
                    {
                        BookReserve bookReserve = new BookReserve();
                        bookReserve.Id = br.Id;
                        bookReserve.BookId = br.BookId;
                        bookReserve.UserId = br.UserId;
                        bookReserve.ReserveOn = br.ReserveOn;
                        cn.Insert(bookReserve);
                    }
                    else
                    {
                        BookReserve bookReserve = new BookReserve();
                        bookReserve.Id = br.Id;
                        bookReserve.BookId = br.BookId;
                        bookReserve.UserId = br.UserId;
                        bookReserve.ReserveOn = br.ReserveOn;
                        cn.Update(bookReserve);
                    }
                }
                cn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}

