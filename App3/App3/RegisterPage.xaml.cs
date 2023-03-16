using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Npgsql;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private async void TermOfService_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermsOfServicePage());
        }
        private async void PrivacyPolicy_Tapped(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new PrivacyPolicyPage());

        }
        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }



        private async void Button_Clicked(object sender, EventArgs e)
        {
            

            string username = this.usernameEntry.Text;
            string password = this.passwordEntry.Text;
            string email = this.emailEntry.Text;

            // Check if any fields are empty
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            // All fields are valid, proceed with database insert
            if (!this.agreeCheckBox.IsChecked)
            {
                await DisplayAlert("Error", "You must agree to the terms of service and privacy policy", "OK");
                return;
            }
            using (NpgsqlConnection connection = new NpgsqlConnection("Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434;"))
            {
                connection.Open();

                NpgsqlCommand command = new NpgsqlCommand($"INSERT INTO users (username, password) VALUES ('{username}', '{password}')", connection);

                int rowCount = command.ExecuteNonQuery();

                if (rowCount > 0)
                {
                    await DisplayAlert("Success", "Your account has been created successfully.", "OK");
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", "An error occurred while creating your account. Please try again later.", "OK");
                }
            }
        }
    }
}