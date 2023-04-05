using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading.Tasks;

namespace App3.Droid
{
    [Service]
    public class ScreenTimeService : Service
    {
        
        
        
        private Timer timer;
        private int primaryKey;

        string connString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
        NpgsqlConnection _conn;
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            //getting primarykey
            primaryKey = intent.GetIntExtra("PrimaryKey", -1);
            System.Diagnostics.Debug.WriteLine("Primary Key Retrieved S.T.S : " + primaryKey);
            // Create and show the notification that says the app usage time is being stored
            var channel = new NotificationChannel("app", "app", NotificationImportance.High);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
            
            var notification = new Notification.Builder(this, "app")
                .SetContentTitle("App Usage Tracking")
                .SetContentText("App Usage Tracking currently on")
                .SetSmallIcon(Resource.Drawable.ic_stat_add_to_home_screen)
                .Build();

            StartForeground(2, notification);

            var now = DateTime.Now;
            var nextMidnight = now.AddDays(1).Date;
            var delay = nextMidnight - now;
            this.timer = new Timer();
            this.timer.AutoReset = true;
            this.timer.Interval = delay.TotalMilliseconds;
            this.timer.Elapsed += SaveAppUsageTime;
            this.timer.Start();
            Toast.MakeText(this, "next Save is :" + delay, ToastLength.Long).Show();
            return StartCommandResult.Sticky;
        }



        private void SaveAppUsageTime(Object sender, ElapsedEventArgs e)
        {
            // Create and show the notification that says the app usage time is being saved
            var channel = new NotificationChannel("app", "app", NotificationImportance.High);
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);

            var notification = new Notification.Builder(this, "app")
                .SetContentTitle("App Usage Tracking")
                .SetContentText("Saving app usage time...")
                .SetSmallIcon(Resource.Drawable.ic_stat_file_upload)
                .Build();

            var finishedNoti = new Notification.Builder(this, "app")
                .SetContentTitle("App Usage Tracking")
                .SetContentText("Successfully Saved!")
                .SetSmallIcon(Resource.Drawable.ic_stat_file_upload)
                .Build();

            var failedNoti = new Notification.Builder(this, "app")
                .SetContentTitle("App Usage Tracking")
                .SetContentText("Saving Failed")
                .SetSmallIcon(Resource.Drawable.ic_stat_file_upload)
                .Build();
            notificationManager.Notify(3, notification);

            // Get the app usage time and store it in a database or file
            var appUsageTracker = new AppUsageTracker();
            var appUsageTime = appUsageTracker.GetAppUsageTime();

            if (appUsageTracker.HasUsageAccessGranted())
            {
                // Use a background thread to fetch the app usage data
                Task.Run(() =>
                {
                    Dictionary<string, double> appUsageTime = appUsageTracker.GetAppUsageTime();
                    var appUsageData = appUsageTime.ToDictionary(pair => pair.Key, pair => (float)TimeSpan.FromMilliseconds(pair.Value).TotalSeconds);

                    try
                    {
                        _conn = new NpgsqlConnection(connString);
                        _conn.Open();
                        foreach (var appUsage in appUsageData)
                        {
                            var cmd = new NpgsqlCommand();
                            cmd.Connection = _conn;
                            cmd.CommandText = "INSERT INTO app_usage(id,package_name, usage_duration, usage_date) VALUES (@id,@package_name, @usage_duration, to_timestamp(@usage_date, 'YYYY-MM-DD HH24:MI:SS'))";
                            cmd.Parameters.AddWithValue("@id", primaryKey);
                            cmd.Parameters.AddWithValue("@package_name", appUsage.Key);
                            cmd.Parameters.AddWithValue("@usage_duration", appUsage.Value);
                            cmd.Parameters.AddWithValue("@usage_date", DateTime.Today.ToString("yyyy-MM-dd") + " 00:00:00");
                            cmd.ExecuteNonQuery();
                        }
                        notificationManager.Notify(3, finishedNoti);
                    }
                    catch (Exception ex)
                    {
                        notificationManager.Notify(10, failedNoti);
                        notificationManager.Cancel(3);
                        return;
                    }
                    finally
                    {
                        _conn.Close();
                    }

                });
            }

            // Reset the app usage time for the new day
            this.timer?.Dispose();
            this.timer = new Timer();
            this.timer.AutoReset = true;
            this.timer.Interval = TimeSpan.FromDays(1).TotalMilliseconds;
            this.timer.Elapsed += SaveAppUsageTime;
            this.timer.Start();
            


        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            Log.Debug("ScreenTimeService", "StopService called");
            base.OnDestroy();
            

        }
    }
}