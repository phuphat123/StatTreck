using Android.App;
using Android.App.Usage;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App3.Droid
{
    public class MotionTrackPerms
    {
        public bool HasUsageAccessGranted()
        {

            return false;
        }

        public void RequestUsageAccess()
        {

            System.Diagnostics.Debug.WriteLine("MotionRequest Function");

            // Create an intent to open the usage access settings screen.
            Intent intent = new Intent(Android.Provider.Settings.ActionAppNotificationSettings);

            // Add the NewTask flag to the intent to start a new task for the settings activity.
            intent.AddFlags(ActivityFlags.NewTask);

            // Start the settings activity using the current application context.
            Android.App.Application.Context.StartActivity(intent);
        }
    }
}