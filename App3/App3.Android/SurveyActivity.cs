using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

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
                // Launch the MyPage fragment or activity here
               
                if (Xamarin.Forms.Application.Current != null)
                {
                    // Push the Survey page onto the navigation stack
                    var surveyPage = new NavigationPage(new Survey());
                    Xamarin.Forms.Application.Current.MainPage.Navigation.PushAsync(surveyPage);
                }
                else
                {
                    // The Xamarin.Forms application is not running, so start it and push the Survey page
                    Xamarin.Forms.Forms.Init(this, savedInstanceState);
                    var surveyPage = new Survey();
                    var navigationPage = new NavigationPage(surveyPage);
                    Xamarin.Forms.Application.Current.MainPage = navigationPage;
                }

                //var notificationManager = (NotificationManager)GetSystemService(NotificationService);
                //notificationManager.Cancel(5);
                //var surveyPageIntent = new Intent(this, typeof(Survey));
                //StartActivity(surveyPageIntent);

            }
        }
    }
}