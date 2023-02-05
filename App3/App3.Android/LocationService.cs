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
using Android.Support;




namespace App3.Droid
{
[Service]
public class LocationService : Service, ILocationListener
{

    LocationManager _locationManager;
    Context c;
    string _locationProvider;
    //const int REQUEST_FOREGROUND_SERVICE = 0;
        

    const int NOTIFICATION_ID = 1;
    NotificationManager manager;

    //private string _notificationTitle = "App is running in background";
    //private string _notificationText = "App is tracking your location";
    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    public override void OnCreate()
    {
        base.OnCreate();
        c = this;
    }
        

    public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId) {

        Android.Util.Log.Debug("LocationService", "OnStartCommand Called");


        _locationManager = (LocationManager)this.GetSystemService(LocationService);
        _locationProvider = LocationManager.GpsProvider;
        System.Diagnostics.Debug.WriteLine(_locationManager);
        System.Diagnostics.Debug.WriteLine(_locationProvider);
        System.Diagnostics.Debug.WriteLine(_locationManager.IsProviderEnabled(_locationProvider) + "," + _locationManager.AllProviders.Contains(_locationProvider));


        if (_locationManager.IsProviderEnabled(_locationProvider) && _locationManager.AllProviders.Contains(_locationProvider))
        {
                
            _locationManager.RequestLocationUpdates(_locationProvider, 100, 0, this);
            
        }
        else
        {
            Toast.MakeText(this, "Please enable GPS.", ToastLength.Long).Show();

        }

           
        return StartCommandResult.Sticky;

    }
      
    public override void OnDestroy()
    {
        Log.Debug("LocationService", "StopService called");
        base.OnDestroy();
        _locationManager.RemoveUpdates(this);

    }

    public void OnLocationChanged(Location location)
    {
        double latitude = location.Latitude;
        double longitude = location.Longitude;
        string coordinates = latitude + "," + longitude;

        System.Diagnostics.Debug.WriteLine(latitude + "," + longitude);
        String msg = "New Latitude: " + latitude + "New Longitude: " + longitude;

        Toast.MakeText(this, msg, ToastLength.Long).Show();



    }

    public void OnProviderDisabled(string provider) { }

    public void OnProviderEnabled(string provider) { }

    public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras) { }
}

}
