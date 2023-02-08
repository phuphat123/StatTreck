using Android.App;
using Android.Content;
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
        private Dictionary<string, int> _appUsageTime;

        public string GetAppUsageTime()
        {
            ActivityManager am = (ActivityManager)Application.Context.GetSystemService(Context.ActivityService);
            var runningAppProcesses = am.RunningAppProcesses;
            _appUsageTime = new Dictionary<string, int>();
            foreach (var process in runningAppProcesses)
            {
                if (process.Importance == Importance.Foreground)
                {
                    string appName = process.ProcessName;
                    if (!_appUsageTime.ContainsKey(appName))
                    {
                        _appUsageTime[appName] = 0;
                    }
                    _appUsageTime[appName]++;
                }
            }

            string result = "App Usage Time per day:\n";
            foreach (var item in _appUsageTime)
            {
                result += $"{item.Key}: {item.Value} seconds\n";
            }
            return result;
        }
    }
}