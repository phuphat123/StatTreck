//using Android.App;
//using Android.Content;
//using Android.OS;
//using Android.Provider;
//using Android.Runtime;
//using Android.Views;
//using Android.Widget;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Npgsql;


//namespace App3.Droid
//{
//    /*
//     * This class is responsible of tracking sms data for the text functionality.
//     */
//    public class TextData
//    {
//        public long Id { get; set; }
//        public string Address { get; set; }
//        public string Body { get; set; }
//        public long Timestamp { get; set; }
//        public long Duration { get; set; }
//    }

//    [BroadcastReceiver(Enabled = true, Exported = true, Permission = "android.permission.BROADCAST_SMS")]
//    [IntentFilter(new[] { "android.provider.Telephony.SMS_RECEIVED" })]
//    public class SmsBroadcastReceiver : BroadcastReceiver
//    {
//        private List<TextData> _smsMessages = new List<TextData>();

//        public override void OnReceive(Context context, Intent intent)
//        {
//            if (intent.Action != "android.provider.Telephony.SMS_RECEIVED")
//                return;

//            var smsMessages = Telephony.Sms.Intents.GetMessagesFromIntent(intent);
//            foreach (var message in smsMessages)
//            {
//                // Responsible of tracking sms data
//                var address = message.DisplayOriginatingAddress;
//                var body = message.DisplayMessageBody;
//                var timestamp = message.TimestampMillis;

//                var smsMessageData = new TextData
//                {
//                    Address = address,
//                    Body = body,
//                    Timestamp = timestamp,
//                };

//                _smsMessages.Add(smsMessageData);

//                // This saves the sms messages to the database
//                SaveSmsMessageToDatabase(smsMessageData, context);
//            }
//        }

//        //This function saves the sms messages to the database
//        public void SaveSmsMessageToDatabase(TextData smsMessageData, Context context)
//        {
            
//            var connectionString = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
//            using (var conn = new NpgsqlConnection(connectionString))
//            {
//                conn.Open();
//                // This creates a command object with parameters
//                using (var cmd = new NpgsqlCommand("INSERT INTO sms_messages (address, body, timestamp) VALUES (@Address, @Body, @Timestamp)", conn))
//                {
//                    cmd.Parameters.AddWithValue("Address", smsMessageData.Address);
//                    cmd.Parameters.AddWithValue("Body", smsMessageData.Body);
//                    cmd.Parameters.AddWithValue("Timestamp", smsMessageData.Timestamp);

                  
//                    cmd.ExecuteNonQuery();
//                }
//            }
//        }


//        public static void ScheduleDailyTask(Context context)
//        {
//            var alarmManager = (AlarmManager)context.GetSystemService(Context.AlarmService);

//            // Create a pending intent to start the background task
//            var intent = new Intent(context, typeof(TextProcessingService));
//            var pendingIntent = PendingIntent.GetService(context, 0, intent, PendingIntentFlags.UpdateCurrent);

//            // This schedules backgrounds task to run daily
//            var interval = AlarmManager.IntervalDay;
//            var firstRunTime = GetFirstRunTime();
//            alarmManager.SetInexactRepeating(AlarmType.RtcWakeup, firstRunTime, interval, pendingIntent);
//        }

//        private static long GetFirstRunTime()
//        {
//            var now = DateTime.Now;
//            var nextRunTime = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0).AddDays(1);
//            var timeSpan = nextRunTime - now;
//            return (long)timeSpan.TotalMilliseconds;
//        }
//    }
//}
