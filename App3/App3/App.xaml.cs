using Npgsql;
using System;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


[assembly: ExportFont("Clear.ttf")]

namespace App3
{
    public partial class App : Application
    {
        String primaryKey;
        int pk;
        public App(string connection, NpgsqlConnection c)
        {
            InitializeComponent();
            
            
            if (Preferences.ContainsKey("IsLoggedIn") && Preferences.Get("IsLoggedIn", false))
            {
                // Navigate to main page
                Debug.WriteLine("Already Logged In detected");
                Page1 loginpage = new Page1(null,null);
                
                primaryKey = Preferences.Get("PK",null);
                Debug.WriteLine("Primary Key Collected: " + primaryKey);
                pk = int.Parse(primaryKey);
                loginpage.pk = GetPrimaryKey();

                //MainPage = new NavigationPage(new MainPage(loginpage));
                MainPage = new FlyoutPage(loginpage);

                

            }
            else
            {
                // Navigate to login page
                Debug.WriteLine("Not Logged In Detected");
                

                MainPage = new NavigationPage(new Page1(connection, c));
                ((NavigationPage)MainPage).BarBackgroundColor = Color.FromHex("#F4D03F");
            }

            

        }

        public int GetPrimaryKey() {
            return pk;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
