﻿using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Support;
using Npgsql;
using App3.Helpers;

namespace App3.Droid
{
    [Service]
    public class LocationService : Service, ILocationListener
    {

        LocationManager _locationManager;
        Context c;
        string _locationProvider;

        const int NOTIFICATION_ID = 1;
        
        
        private int primaryKey;

        string connString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
        NpgsqlConnection _conn;

        public override IBinder OnBind(Intent intent)
        {
            return null;
            
        }

        public override void OnCreate()
        {
            base.OnCreate();
            c = this;
            _conn = new NpgsqlConnection(connString);
            
        }


        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {

            Android.Util.Log.Debug("LocationService", "OnStartCommand Called");

            primaryKey = intent.GetIntExtra("PrimaryKey", -1);


            var channel = new NotificationChannel("location_channel", "Location Channel", NotificationImportance.High);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            var notification = new Notification.Builder(this, "location_channel")
                .SetContentTitle("Location Service")
                .SetContentText("Location Service is running")
                .SetSmallIcon(Resource.Drawable.ic_mtrl_chip_checked_black)
                .Build();

            StartForeground(NOTIFICATION_ID, notification);

            _locationManager = (LocationManager)this.GetSystemService(LocationService);
            _locationProvider = LocationManager.GpsProvider;
            System.Diagnostics.Debug.WriteLine(_locationManager);
            System.Diagnostics.Debug.WriteLine(_locationProvider);
            System.Diagnostics.Debug.WriteLine(_locationManager.IsProviderEnabled(_locationProvider) + "," + _locationManager.AllProviders.Contains(_locationProvider));


            if (_locationManager.IsProviderEnabled(_locationProvider) && _locationManager.AllProviders.Contains(_locationProvider))
            {

                _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);

            }
            else
            {
                Toast.MakeText(this, "Please enable GPS.", ToastLength.Long).Show();

            }


            return StartCommandResult.Sticky;

        }

        public override void OnDestroy()
        {
            Log.Debug("LocationService", "StopService called");
            base.OnDestroy();
            _locationManager.RemoveUpdates(this);
            _conn.Close();
            
        }

        public void OnLocationChanged(Location location)
        {
            double latitude = location.Latitude;
            double longitude = location.Longitude;
            DateTime currentDateTime = DateTime.Now;
            string formattedDateTime = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
            string coordinates = latitude + "," + longitude;

            System.Diagnostics.Debug.WriteLine(latitude + "," + longitude);
            String msg = "New Latitude: " + latitude + "New Longitude: " + longitude;
            try
            {
                var cmd = new NpgsqlCommand();
                _conn.Open();
                try
                {
                    cmd.Connection = _conn;
                    cmd.CommandText = "INSERT INTO coordinates(id ,latitude, longitude, date) VALUES (@id ,@latitude, @longitude, to_timestamp(@date, 'YYYY-MM-DD HH24:MI:SS'))";
                    cmd.Parameters.AddWithValue("@id", primaryKey);
                    cmd.Parameters.AddWithValue("@latitude", latitude);
                    cmd.Parameters.AddWithValue("@longitude", longitude);
                    cmd.Parameters.AddWithValue("@date", formattedDateTime);
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, "Error1" + ex.Message, ToastLength.Long).Show();
                }
                finally
                {
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, "Error 2" + ex.Message, ToastLength.Long).Show();
            }


        }

        public void OnProviderDisabled(string provider) { }

    public void OnProviderEnabled(string provider) { }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
}

}
