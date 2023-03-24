using Android.App;
using Android.Content;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace App3.Droid
{
    public class SmsMessageData
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string Body { get; set; }
        public long Timestamp { get; set; }
        public long Duration { get; set; }
    }

    [BroadcastReceiver(Enabled = true, Exported = true, Permission = "android.permission.BROADCAST_SMS")]
    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
    public class SmsBroadcastReceiver : BroadcastReceiver
    {
        private List<SmsMessageData> _smsMessages = new List<SmsMessageData>();

        public override void OnReceive(Context context, Intent intent)
        {
            if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
                return;

            var smsMessages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
            foreach (var message in smsMessages)
            {
                // Track the SMS data
                var address = message.DisplayOriginatingAddress;
                var body = message.DisplayMessageBody;
                var timestamp = message.TimestampMillis;

                var smsMessageData = new SmsMessageData
                {
                    Address = address,
                    Body = body,
                    Timestamp = timestamp,
                };

                _smsMessages.Add(smsMessageData);

                // Save the SMS message to a database
                SaveSmsMessageToDatabase(smsMessageData, context);
            }
        }

        private void SaveSmsMessageToDatabase(SmsMessageData smsMessageData, Context context)
        {
            // Get a connection to the database
            var dbPath = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "postgres.db");
            using (var conn = new SQLite.SQLiteConnection(dbPath))
            {
                // Create a table to store SMS messages if it doesn't already exist
                conn.CreateTable<SmsMessageData>();

                // Insert the SMS message into the table
                conn.Insert(smsMessageData);
            }
        }

        public static void ScheduleDailyTask(Context context)
        {
            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

            // Create a pending intent to start the background task
            var intent = new Intent(context, typeof(SmsProcessingService));
            var pendingIntent = PendingIntent.GetService(context, 0, intent, PendingIntentFlags.UpdateCurrent);

            // Schedule the background task to run daily
            var interval = AlarmManager.IntervalDay;
            var firstRunTime = GetFirstRunTime();
            alarmManager.SetInexactRepeating(AlarmType.RtcWakeup, firstRunTime, interval, pendingIntent);
        }

        private static long GetFirstRunTime()
        {
            var now = DateTime.Now;
            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
            var timeSpan = nextRunTime - now;
            return (long)timeSpan.TotalMilliseconds;
        }
    }


}