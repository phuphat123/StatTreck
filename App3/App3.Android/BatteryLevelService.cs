using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using Xamarin.Essentials;
using FontAwesome;
using Android.Graphics.Drawables;
using Java.Util;
using Android.Icu.Util;

namespace App3.Droid
{
    [Service]
    public class BatteryLevelService : Service
    {
        private readonly object _lock = new object();
        private bool _isRunning = false;
         int pk;
        private NotificationManager _notificationManager;
        private NotificationChannel _channel;

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }
        public static BatteryLevelService Instance { get; set; }
        public bool _isEventHandlerEnabled;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            _isEventHandlerEnabled = true;
            

            
                pk = int.Parse(Preferences.Get("PK",null));
            
            
            System.Diagnostics.Debug.WriteLine("Primary Key Retrieved BLS : " + pk);
            lock (_lock)
            {
                if (_isRunning)
                    return StartCommandResult.Sticky;

                _isRunning = true;
            }
            BatteryLevelService.Instance = this;

            // Register for battery level change events
            _lastBatteryLevel = Battery.ChargeLevel * 100;
            _lastChangeTime = DateTime.Now;
            Battery.BatteryInfoChanged += OnBatteryChanged;

            _notificationManager = (NotificationManager)GetSystemService(NotificationService);
            _channel = new NotificationChannel("app", "app", NotificationImportance.High);
            _notificationManager.CreateNotificationChannel(_channel);

            var notification = new Notification.Builder(this, "app")
                .SetContentTitle("Battery Tracking")
                .SetContentText("Battery Tracking currently on")
                .SetSmallIcon(Resource.Drawable.ic_stat_battery_unknown)
                .Build();
            StartForeground(4, notification);

            Java.Util.Calendar calendar = Java.Util.Calendar.GetInstance(Java.Util.TimeZone.GetTimeZone("UTC"));
            calendar.TimeInMillis = Java.Lang.JavaSystem.CurrentTimeMillis();
            calendar.Set(Java.Util.CalendarField.HourOfDay, 0);
            calendar.Set(Java.Util.CalendarField.Minute, 0);
            calendar.Set(Java.Util.CalendarField.Second, 0);
            calendar.Set(Java.Util.CalendarField.Millisecond, 0);
            calendar.Add(Java.Util.CalendarField.DayOfMonth, 1);
            if (calendar.TimeInMillis <= Java.Lang.JavaSystem.CurrentTimeMillis())
            {
                calendar.Add(Java.Util.CalendarField.DayOfMonth, 1);
            }
            var midnightIntent = new Intent(this, typeof(MidnightReceiver));
            var pendingIntent = PendingIntent.GetBroadcast(this, 0, midnightIntent, PendingIntentFlags.UpdateCurrent);
            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
            alarmManager.Set(AlarmType.RtcWakeup, calendar.TimeInMillis, pendingIntent);


            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();

            // Unregister from battery level change events
            lock (_lock)
            {
                _isRunning = false;
            }

            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
            var intent = new Intent(this, typeof(BatteryLevelService));
            var pendingIntent = PendingIntent.GetService(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            alarmManager.Cancel(pendingIntent);
        }

        private DateTime _lastChangeTime;
        private double _lastBatteryLevel;
        public void OnBatteryChanged(object sender, BatteryInfoChangedEventArgs e)
        {
            if (!_isEventHandlerEnabled)
            {
                return;
            }
            TimeSpan timeSinceLastChange = DateTime.Now - _lastChangeTime;
            double batteryLevel = e.ChargeLevel * 100;

            // Check if the battery level has decreased by 1%
            if ((int)_lastBatteryLevel - (int)batteryLevel == 1)
            {
                // Calculate the time since the last battery level change
                if (timeSinceLastChange.TotalMinutes <= 20)
                {
                    var activityIntent = new Intent(this, typeof(SurveyActivity));
                    activityIntent.PutExtra("PageToLaunch", "Survey");
                    activityIntent.PutExtra("PrimaryKey", pk);
                    var pendingIntent = PendingIntent.GetActivity(this, 0, activityIntent, PendingIntentFlags.OneShot);

                    var notification = new Notification.Builder(this, "app")
                        .SetContentTitle("Mood Survey")
                        .SetContentText("Tell us how you're feeling")
                        .SetContentIntent(pendingIntent)
                        .SetSmallIcon(Resource.Drawable.ic_stat_tag_faces)
                        .SetAutoCancel(true)
                        .Build();

                    _notificationManager.Notify(5, notification);

                    _isEventHandlerEnabled = false;
                    Battery.BatteryInfoChanged -= OnBatteryChanged;


                }

                // Update the last battery level and change time
                _lastBatteryLevel = batteryLevel;
                _lastChangeTime = DateTime.Now;
            }
        }
        public static void Stop()
        {
            if (Instance != null)
            {
                Instance.StopSelf();
            }
        }
    }
    [BroadcastReceiver(Enabled = true)]
    public class MidnightReceiver : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            if (BatteryLevelService.Instance != null)
            {
                BatteryLevelService.Instance._isEventHandlerEnabled = true;
                Battery.BatteryInfoChanged += BatteryLevelService.Instance.OnBatteryChanged;
            }
        }
    }
}