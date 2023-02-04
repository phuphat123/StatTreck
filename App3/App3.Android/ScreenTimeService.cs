using Android.App;
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

namespace App3.Droid
{
    [Service]
    public class ScreenTimeService : Service
    {
        private ActivityManager activityManager;
        private string currentApp;
        private Dictionary<string, long> appTimers;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            //activityManager = (ActivityManager)GetSystemService(ActivityService);
            //appTimers = new Dictionary<string, long>();
            //currentApp = activityManager.GetRunningTasks(1).FirstOrDefault()?.TopActivity.PackageName;

            //Timer timer = new Timer(CheckForegroundApp, null, 0, 500);
            System.Diagnostics.Debug.WriteLine("OnStartCommand Called");


            return StartCommandResult.Sticky;
        }

        private void CheckForegroundApp(object state)
        {
            //string newApp = activityManager.GetRunningTasks(1).FirstOrDefault()?.TopActivity.PackageName;

            //if (currentApp != newApp)
            //{
            //    // Save the time spent in the previous app
            //    if (appTimers.ContainsKey(currentApp))
            //        appTimers[currentApp] += DateTime.UtcNow.Ticks - startTime;
            //    else
            //        appTimers[currentApp] = DateTime.UtcNow.Ticks - startTime;

            //    currentApp = newApp;
            //    startTime = DateTime.UtcNow.Ticks;
            //}
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