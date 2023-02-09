using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.Provider;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using App3.Droid;
using App3.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(AppUsageTracker))]

namespace App3.Droid
{


    public class AppUsageTracker : IAppUsageTracker
    {
        

        public string GetAppUsageTime()
        {
            var usageStatsManager = (UsageStatsManager)Application.Context.GetSystemService(Context.UsageStatsService);
            var time = Java.Lang.JavaSystem.CurrentTimeMillis();
            var usageEvents = usageStatsManager.QueryEvents(time - TimeSpan.FromDays(1).Ticks, time);
            Dictionary<string, long> appUsageTime = new Dictionary<string, long>();
            string currentApp = "";
            long foregroundTime = 0;

            while (usageEvents.HasNextEvent)
            {
                var usageEvent = new UsageEvents.Event();
                usageEvents.GetNextEvent(usageEvent);

                if (usageEvent.EventType == UsageEventType.MoveToForeground)
                {
                    currentApp = usageEvent.PackageName;
                    foregroundTime = usageEvent.TimeStamp;
                }
                else if (usageEvent.EventType == UsageEventType.MoveToBackground && currentApp == usageEvent.PackageName)
                {
                    if (!appUsageTime.ContainsKey(currentApp))
                    {
                        appUsageTime[currentApp] = 0;
                    }
                    appUsageTime[currentApp] += usageEvent.TimeStamp - foregroundTime;
                }
            }

            StringBuilder result = new StringBuilder();
            foreach (var appUsage in appUsageTime)
            {
                result.AppendLine(appUsage.Key + ": " + TimeSpan.FromMilliseconds(appUsage.Value).TotalSeconds + " seconds");
            }

            return result.ToString();
        }

        public bool HasUsageAccessGranted()
        {
            System.Diagnostics.Debug.WriteLine("HasUsageAccessGranted Function");
            var usageStatsManager = (UsageStatsManager)Application.Context.GetSystemService(Context.UsageStatsService);
            var appList = usageStatsManager.QueryUsageStats(UsageStatsInterval.Daily, 0, System.DateTime.Now.Ticks);
            return appList != null && appList.Any();
        }

        public void RequestUsageAccess()
        {
            System.Diagnostics.Debug.WriteLine("RequestUsageAccess Function");
            Intent intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            Android.App.Application.Context.StartActivity(intent);
        }

    }
    
}