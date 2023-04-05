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
[assembly: Xamarin.Forms.Dependency(typeof(StopServiceImplementation))]
namespace App3.Droid
{
    public class StopServiceImplementation : IStopService
    {
        public void StopService(String s)
        {
            if (s == "LocationService")
            {
                System.Diagnostics.Debug.WriteLine("ISTOPSERVICE called");
                System.Diagnostics.Debug.WriteLine("LocationService Stop ");
                var serviceToStop = new Intent(Android.App.Application.Context, typeof(LocationService));
                Android.App.Application.Context.StopService(serviceToStop);
            }
            if (s == "ScreenTime") {
                System.Diagnostics.Debug.WriteLine("ScreenService Stop");
                var serviceToStop = new Intent(Android.App.Application.Context, typeof(ScreenTimeService));
                Android.App.Application.Context.StopService(serviceToStop);
            }
            if (s == "Battery")
            {
                System.Diagnostics.Debug.WriteLine("Battery Stop");
                var serviceToStop = new Intent(Android.App.Application.Context, typeof(BatteryLevelService));
                Android.App.Application.Context.StopService(serviceToStop);
            }
            if (s == "Motion") {
                System.Diagnostics.Debug.WriteLine("Motion Stop");
                var serviceToStop = new Intent(Android.App.Application.Context, typeof(MotionTrackingBackgroundService));
                Android.App.Application.Context.StopService(serviceToStop);
            }
        }


    }
}