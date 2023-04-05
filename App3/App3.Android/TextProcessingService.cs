//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Provider;
//using Android.Runtime;
//using Android.Util;
//using Android.Views;
//using Android.Widget;
//using AndroidX.Core.App;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Android.Telephony;

//namespace App3.Droid
//{
//    /*
//     * This class is supposed to provide a service to track data in the background.
//     */
//    [Service]
//    public class TextProcessingService : Service
//    {
//        private const int NotificationId = 1;
//        private const int NumDaysToKeepSmsMessages = 7;


//        private List<TextData> _smsMessages = new List<TextData>();

//        public override IBinder OnBind(Intent intent)
//        {
//            return null;
//        }
//        private void LogMessage(string message)
//        {
//            var tag = Resources.GetString(Resource.String.common_signin_button_text_long);
//            Log.Debug(tag, message);
//        }

//        private void ShowNotification()
//        {
//            // Create a notification channel for Android 8.0 and higher
//            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
//            {
//                var channelName = Resources.GetString(Resource.String.status_bar_notification_info_overflow);
//                var importance = NotificationImportance.Default;
//                var channel = new NotificationChannel(NotificationId.ToString(), channelName, importance);
//                var notificationManager = GetSystemService(NotificationService) as NotificationManager;
//                notificationManager.CreateNotificationChannel(channel);
//            }

//            // Creates a notification
//            var title = Resources.GetString(Resource.String.summary_collapsed_preference_list);
//            var text = Resources.GetString(Resource.String.v7_preference_off);
            
//            var intent = new Intent(this, typeof(MainActivity));
//            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);
//            var notificationBuilder = new NotificationCompat.Builder(this, NotificationId.ToString())
//                .SetContentTitle(title)
//                .SetContentText(text)
//                .SetContentIntent(pendingIntent)
//                .SetOngoing(true);

//            // This shows the notification
//            var notificationManagerCompat = NotificationManagerCompat.From(this);
//            notificationManagerCompat.Notify(NotificationId, notificationBuilder.Build());
//        }


//        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
//        {
//            LogMessage("TextProcessingService started");

//            // This schedules the next run of the service
//            ScheduleNextRun();

//            // This processes the SMS messages received during the past 24 hours
//            ProcessSmsMessages();

//            // This shows a notification to indicate that the service is running
//            ShowNotification();

//            return StartCommandResult.Sticky;
//        }

//        public override void OnDestroy()
//        {
//            LogMessage("TextProcessingService destroyed");

//            // Removes the notification
//            NotificationManagerCompat.From(this).Cancel(NotificationId);

//            base.OnDestroy();
//        }

//        // This function schedules the next run
//        private void ScheduleNextRun()
//        {
//            var nextRunTime = DateTime.Today.AddDays(1);
//            var interval = nextRunTime - DateTime.Now;
//            var pendingIntent = PendingIntent.GetService(this, 0, new Intent(this, typeof(TextProcessingService)), PendingIntentFlags.UpdateCurrent);
//            var alarmManager = (AlarmManager)GetSystemService(AlarmService);
//            alarmManager.SetExactAndAllowWhileIdle(AlarmType.ElapsedRealtime, SystemClock.ElapsedRealtime() + (long)interval.TotalMilliseconds, pendingIntent);
//        }

//        //This function processes sms messages 
//        private void ProcessSmsMessages()
//        {
//            var queryUri = Telephony.Sms.Inbox.ContentUri;
//            var projection = new[] { Telephony.Sms.InterfaceConsts.Id, Telephony.Sms.InterfaceConsts.Address, Telephony.Sms.InterfaceConsts.Body, Telephony.Sms.InterfaceConsts.Date };
//            var selection = $"{Telephony.Sms.InterfaceConsts.Date} >= {DateTime.Today.AddDays(-NumDaysToKeepSmsMessages).Ticks}";
//            var sortOrder = $"{Telephony.Sms.InterfaceConsts.Date} ASC";
//            var cursor = ContentResolver.Query(queryUri, projection, selection, null, sortOrder);

//            if (cursor != null && cursor.MoveToFirst())
//            {
//                do
//                {
//                    // Reads the SMS message data
//                    var id = cursor.GetLong(cursor.GetColumnIndex(projection[0]));
//                    var address = cursor.GetString(cursor.GetColumnIndex(projection[1]));
//                    var body = cursor.GetString(cursor.GetColumnIndex(projection[2]));
//                    var timestamp = cursor.GetLong(cursor.GetColumnIndex(projection[3]));

//                    var smsMessageData = new TextData
//                    {
//                        Id = id,
//                        Address = address,
//                        Body = body,
//                        Timestamp = timestamp,
//                    };

//                    _smsMessages.Add(smsMessageData);
//                } while (cursor.MoveToNext());
//            }

//            // Calculate the duration of each SMS message received during the past NumDaysToKeepSmsMessages days
//            foreach (var smsMessageData in _smsMessages)
//            {
//                var duration = DateTime.Now.Ticks - smsMessageData.Timestamp;
//                smsMessageData.Duration = duration;
//            }
//        }
//    }
//}