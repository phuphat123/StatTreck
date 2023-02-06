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

                //var serviceToStart = new Intent(Android.App.Application.Context, typeof(ScreenTimeService)); ;

                //var status = ActivityCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.GetTasks);
                //System.Diagnostics.Debug.WriteLine(status);
                //if (status != Permission.Granted)
                //{

                //    ActivityCompat.RequestPermissions((Activity)Forms.Context, new string[] { Android.Manifest.Permission.GetTasks }, 0);

                //}
                //if (status == Permission.Granted) {
                //    Android.App.Application.Context.StartService(serviceToStart);
                //    System.Diagnostics.Debug.WriteLine("ScreenTime Service On");

                var serviceToStart = new Intent(Android.App.Application.Context, typeof(ScreenTimeService)); ;
                                            //add PK
                var status = ActivityCompat.CheckSelfPermission(Android.App.Application.Context, Android.Manifest.Permission.GetTasks);
                System.Diagnostics.Debug.WriteLine(status);
                if (status != Permission.Granted)
                {
                    ActivityCompat.RequestPermissions((Activity)Forms.Context, new string[] { Android.Manifest.Permission.GetTasks }, 0);
                }
                else
                {
                    Android.App.Application.Context.StartService(serviceToStart);
                    System.Diagnostics.Debug.WriteLine("ScreenTime Service On");
                }

            }


        }

        
    }
}