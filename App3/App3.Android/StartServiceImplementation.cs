using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.Core.App;
using App3.Droid;
using App3.Helpers;
using Android.Content.PM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using AndroidX.Core.Content;
using System.Runtime.Remoting.Contexts;
using Xamarin.Essentials;

[assembly: Xamarin.Forms.Dependency(typeof(StartServiceImplementation))]

namespace App3.Droid
{
    public class StartServiceImplementation : IStartService
    {
        public void StartService(String s, int pk)
        {
            System.Diagnostics.Debug.WriteLine("ISTARTSERVICE called" + " ");

            //Starting Location Service
            if (s == "LocationService")
            {
                { 

                    System.Diagnostics.Debug.WriteLine("LocationService On");
                    var serviceToStart = new Intent(Android.App.Application.Context, typeof(LocationService));
                    serviceToStart.PutExtra("PrimaryKey", pk);
                    Android.App.Application.Context.StartService(serviceToStart);
                }
                
            }

            //Starting ScreenTime Service
            if (s == "ScreenTime") {

                var serviceToStart = new Intent(Android.App.Application.Context, typeof(ScreenTimeService)); ;
                AppUsageTracker a = new AppUsageTracker();

                var status = a.HasUsageAccessGranted();
                if (status != true)
                {
                    a.RequestUsageAccess();
                }
                else
                {
                    serviceToStart.PutExtra("PrimaryKey", pk);
                    Android.App.Application.Context.StartService(serviceToStart);
                    System.Diagnostics.Debug.WriteLine("ScreenTime Service On");
                }

            }

            if (s == "Battery") {
                var serviceToStart = new Intent(Android.App.Application.Context, typeof(BatteryLevelService));
                serviceToStart.PutExtra("PrimaryKey", pk);
                Android.App.Application.Context.StartService(serviceToStart);
            }
            if (s == "Motion")
            {
                //try
                //{
                //    var status = await Permissions.RequestAsync<Permissions.Sensors>();
                //    if (status != PermissionStatus.Granted)
                //    {
                //        return;
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Console.WriteLine(ex);
                //    return;
                //}
                var serviceToStart = new Intent(Android.App.Application.Context, typeof(MotionTrackingBackgroundService));
                serviceToStart.PutExtra("PrimaryKey", pk);
                Android.App.Application.Context.StartService(serviceToStart);
            }
        }

        
    }
}