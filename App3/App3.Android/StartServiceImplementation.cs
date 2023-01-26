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
[assembly: Xamarin.Forms.Dependency(typeof(StartServiceImplementation))]

namespace App3.Droid
{
    public class StartServiceImplementation : IStartService
    {
        public void StartService()
        {
            System.Diagnostics.Debug.WriteLine("ISTARTSERVICE called" +
                "");

            var serviceToStart = new Intent(Android.App.Application.Context, typeof(LocationService));;
            Android.App.Application.Context.StartService(serviceToStart);


        }

        
    }
}