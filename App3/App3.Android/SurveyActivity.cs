using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util.Prefs;
using Npgsql;
using System;
using static Xamarin.Essentials.Platform;

namespace App3.Droid
{
    [Activity(Label = "SurveyActivity")]
    public class SurveyActivity : Activity
    {
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            string pageToLaunch = Intent.GetStringExtra("PageToLaunch");
            if (pageToLaunch == "Survey")
            {
                // Set the activity layout
                SetContentView(Resource.Layout.survey_activity);

                // Find the button views
                var happyButton = FindViewById<Button>(Resource.Id.happy_button);
                var mediumButton = FindViewById<Button>(Resource.Id.medium_button);
                var sadButton = FindViewById<Button>(Resource.Id.sad_button);

                // Attach event handlers to the button clicks
                happyButton.Click += OnHappyButtonClicked;
                mediumButton.Click += OnMediumButtonClicked;
                sadButton.Click += OnSadButtonClicked;
            }
        }
        string mood = "";
        string connString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";

        private void OnHappyButtonClicked(object sender, EventArgs e)
        {
            // Handle the happy button click
            // ...
            mood = "happy";
            DatabaseSend(mood);
            Finish();

        }

        private void OnMediumButtonClicked(object sender, EventArgs e)
        {
            // Handle the medium button click
            // ...
            mood = "medium";
            DatabaseSend(mood);
            Finish();

        }

        private void OnSadButtonClicked(object sender, EventArgs e)
        {
            // Handle the sad button click
            // ...
            mood = "sad";
            DatabaseSend(mood);
            Finish();


        }
        

        private void DatabaseSend(string s) {
            try
            {

                using (var conn = new NpgsqlConnection(connString))

                {
                    conn.Open();
                    var currentDate = DateTime.Now;
                    string sql = "INSERT INTO emotions (id,mood, date) VALUES (@id,@mood, @date)";
                    var id = int.Parse(Xamarin.Essentials.Preferences.Get("PK", null));
                    ;
                    using (var cmd = new NpgsqlCommand(sql, conn))
                    {
                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("id",id);
                        cmd.Parameters.AddWithValue("mood", s);
                        cmd.Parameters.AddWithValue("date", currentDate);

                        // Execute the command
                        cmd.ExecuteNonQuery();
                        Toast.MakeText(this, "Successfully Submitted", ToastLength.Long).Show();
                        conn.Close();
                        BatteryLevelService.Stop();
                    }
                }
            }
            catch(Exception ex) {
                Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
            }

        }
    }
}
