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
using Android.Content.PM;

[assembly: Xamarin.Forms.Dependency(typeof(AppUsageTracker))]

namespace App3.Droid
{


    public class AppUsageTracker : IAppUsageTracker
    {
        public Dictionary<string, double> GetAppUsageTime()
        {
            // Get the usage stats manager for the current application context
            var usageStatsManager = (UsageStatsManager)Application.Context.GetSystemService(Context.UsageStatsService);

            // Get the current time in milliseconds
            var time = Java.Lang.JavaSystem.CurrentTimeMillis();

            // Query the usage events for the past day
            var usageEvents = usageStatsManager.QueryEvents(time - TimeSpan.FromDays(1).Ticks, time);

            // Create a dictionary to store the app usage time in milliseconds
            Dictionary<string, long> appUsageTime = new Dictionary<string, long>();

            // Create a dictionary to store the app usage time in minutes
            Dictionary<string, double> appUsageTimeInMinutes = new Dictionary<string, double>();

            // Get the package manager for the current application context
            PackageManager pm = Application.Context.PackageManager;

            // Initialize variables to keep track of the current app and foreground time
            string currentApp = "";
            long foregroundTime = 0;

            // Loop through the usage events
            while (usageEvents.HasNextEvent)
            {
                // Get the next usage event
                var usageEvent = new UsageEvents.Event();
                usageEvents.GetNextEvent(usageEvent);

                // If the usage event is a move to foreground event, update the current app and foreground time
                if (usageEvent.EventType == UsageEventType.MoveToForeground)
                {
                    currentApp = usageEvent.PackageName;
                    foregroundTime = usageEvent.TimeStamp;
                }
                // If the usage event is a move to background event and the current app matches the event package name,
                // add the usage time to the app usage time dictionary
                else if (usageEvent.EventType == UsageEventType.MoveToBackground && currentApp == usageEvent.PackageName)
                {
                    if (pm.GetLaunchIntentForPackage(currentApp) == null)
                    {
                        // Skip this usage event if the app is no longer installed
                        continue;
                    }
                    if (!appUsageTime.ContainsKey(currentApp))
                    {
                        appUsageTime[currentApp] = 0;
                    }
                    appUsageTime[currentApp] += usageEvent.TimeStamp - foregroundTime;
                    foregroundTime = 0; // reset foregroundTime to avoid counting time in the background
                }
            }

            // Loop through the app usage time dictionary and convert the usage time to minutes,
            // then add the app name and usage time in minutes to the app usage time in minutes dictionary
            foreach (var appUsage in appUsageTime)
            {
                var appName = pm.GetApplicationLabel(pm.GetApplicationInfo(appUsage.Key, 0));
                var usageTimeInMinutes = TimeSpan.FromMilliseconds(appUsage.Value).TotalMinutes;
                appUsageTimeInMinutes[appName] = usageTimeInMinutes;
            }

            // Return the app usage time in minutes dictionary
            return appUsageTimeInMinutes;
        }



        public bool HasUsageAccessGranted()
        {
            
            System.Diagnostics.Debug.WriteLine("HasUsageAccessGranted Function");

            // Get the usage stats manager for the current application context.
            var usageStatsManager = (UsageStatsManager)Application.Context.GetSystemService(Context.UsageStatsService);

            // Query the usage stats for the last 24 hours to check if any apps have been used during this period.
            var appList = usageStatsManager.QueryUsageStats(UsageStatsInterval.Daily, 0, System.DateTime.Now.Ticks);

            // Return true if the app list is not null and contains at least one element, indicating that usage access has been granted.
            return appList != null && appList.Any();
        }

        public void RequestUsageAccess()
        {
            
            System.Diagnostics.Debug.WriteLine("RequestUsageAccess Function");

            // Create an intent to open the usage access settings screen.
            Intent intent = new Intent(Android.Provider.Settings.ActionUsageAccessSettings);

            // Add the NewTask flag to the intent to start a new task for the settings activity.
            intent.AddFlags(ActivityFlags.NewTask);

            // Start the settings activity using the current application context.
            Android.App.Application.Context.StartActivity(intent);
        }
    }

}