using App3.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using FontAwesome;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Page1 : ContentPage
    {
        //MainPage main;
        FlyoutPage f;
        string connection;
        NpgsqlConnection c;
        public int pk;
        public Page1(string connection, NpgsqlConnection c)
        {
            InitializeComponent();
            this.c = c;
            this.connection = connection;
            Page1 reference = this;
            //main = new MainPage(reference);
            f = new FlyoutPage(reference);
            
        }


        private void ShowPassword_Clicked(object sender, EventArgs e) {
            //show the password
            
            passwordEntry.IsPassword = !passwordEntry.IsPassword;
            if (passwordEntry.IsPassword == false)
            {
                ShowPassword.Text = FontAwesome.FontAwesomeIcons.EyeSlash;
            }
            else
            {
                ShowPassword.Text = FontAwesome.FontAwesomeIcons.Eye;
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            String username = usernameEntry.Text;
            String password = passwordEntry.Text;
            Debug.WriteLine(username + "," + password);

           

            if (!agreeCheckBox.IsChecked)
            {
                await DisplayAlert("Error", "Please accept the terms of service and privacy policy to continue", "OK");
                return;
            }


            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter your username and password", "OK");
                return;
            }




            try
            {
                using (var command = new NpgsqlCommand("SELECT * FROM users WHERE username = @username AND password = @password", c))
                {
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Login successful
                            Debug.WriteLine("Login Success!");
                            reader.Dispose();
                            String selectPrimaryKey = "SELECT id FROM users WHERE username = @username";
                             //retrieving primary key
                            using (NpgsqlCommand commandd = new NpgsqlCommand(selectPrimaryKey, c))
                            {
                                commandd.Parameters.AddWithValue("@username", username);

                                var result = commandd.ExecuteScalar();
                                if (result != null)
                                {
                                    int primaryKey = Convert.ToInt32(result);
                                    pk = primaryKey;
                                    Debug.WriteLine("Retrieved primary key." + pk);

                                    
                                    c.Close();
                                    Preferences.Set("IsLoggedIn", true);
                                    Preferences.Set("Username", username);
                                    Preferences.Set("PK", primaryKey.ToString());
                                    DependencyService.Get<IStartService>().StartService("Battery", pk);
                                    await Navigation.PushAsync(f, true);



                                    Navigation.RemovePage(this);
                                }
                                else
                                {
                                    Debug.WriteLine("Error: Could not retrieve primary key.");
                                }
                            }
                        }
                        else
                        {
                            // Login failed
                            Debug.WriteLine("Login Failed");
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                Debug.WriteLine("Error!" + ex.Message);

            }

        }
        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new RegisterPage());
        }
        private async void TermOfService_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermsOfServicePage());
        }
        private async void PrivacyPolicy_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrivacyPolicyPage());

        }



        public int getPrimaryKey()
        {
            return pk;
        }
        

        
    }
}
