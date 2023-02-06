using System;
using System.Data;
using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Content;
using Npgsql;
using Android.Widget;
using App3.Helpers;

namespace App3.Droid
{
    [Activity(Label = "App3", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IConnectionProvider
    {

        string connString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
        NpgsqlConnection conn;
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            conn = new NpgsqlConnection(connString);
            NpgsqlConnectionStringBuilder connBuilder = new NpgsqlConnectionStringBuilder(connString);
            try
            { 
                    conn.Open();

                if (conn.State == ConnectionState.Open) {
                    Toast.MakeText(this, "Connected to :" + connBuilder.Host, ToastLength.Long).Show();
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(this, "Unable to connect to the database." + ex.Message, ToastLength.Long).Show();
                Finish();
            }


            
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App(connString,conn));


            var packageManager = Android.App.Application.Context.PackageManager;
            var componentName = new ComponentName(Android.App.Application.Context, Java.Lang.Class.FromType(typeof(LocationService)));
            var serviceInfo = packageManager.GetServiceInfo(componentName, PackageInfoFlags.Services);
            if (serviceInfo != null)
            {//Location Service is registered
                System.Diagnostics.Debug.WriteLine("Location Service is registered");
            }
            componentName = new ComponentName(Android.App.Application.Context, Java.Lang.Class.FromType(typeof(ScreenTimeService)));
            serviceInfo = packageManager.GetServiceInfo(componentName, PackageInfoFlags.Services);
            if (serviceInfo != null)
            {//Screen Service is registered
                System.Diagnostics.Debug.WriteLine("Screen Service is registered");
            }

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (conn != null)
            {
                conn.Close();
                conn.Dispose();
            }
        }
        public NpgsqlConnection GetConnection()
        {
            return conn;
        }
    }
}