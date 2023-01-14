using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using System.Data;
using Xamarin.Forms.Maps;

namespace App3
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();

            GPS.Clicked += Button_Clicked;
            Motion.Clicked += Button_Clicked;

            pin = new List<Pin>();




        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            Navigation.PushAsync(new Data());
        }
        double locationLat, locationLng;
        List<Pin> pin;


        private async void Button_Clicked(object sender, EventArgs e)
        {
            

            Debug.WriteLine("Button Clicked!");
            Data emptyPage = new Data();
            Button button = sender as Button;
            if (button == GPS)
            {
                
                Debug.WriteLine("GPS Clicked");
                StackLayout s = new StackLayout();
                s.Children.Add(new Label { Text = "You've clicked GPS!" });
                string locationStr = "";
                
                //check status
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("No Permission yet");
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>(); 
                }
                if (status == PermissionStatus.Granted)
                {
                    try //track location
                    {
                        var location = await Geolocation.GetLastKnownLocationAsync();
                        if (location != null)
                        {
                            Debug.WriteLine(location.Latitude + "" + location.Altitude);
                            locationStr += "Latitude: " + location.Latitude + " Longitude: " + location.Longitude;
                            locationLat = location.Latitude;
                            locationLng = location.Longitude;
                        }
                    }
                    catch (FeatureNotSupportedException fnsEx) { Debug.WriteLine("Feature not supported." + fnsEx.Message); }
                    catch (PermissionException pEx) { Debug.WriteLine("Feature not supported." + pEx.Message); }
                    catch (Exception ex) { Debug.WriteLine("Feature not supported." + ex.Message); }


                }
                
                
                var map = new Xamarin.Forms.Maps.Map();
                map.HeightRequest = 100;
                map.WidthRequest = 200;
                map.MapType = MapType.Street;
                map.IsShowingUser = false;
                var position = new Position(locationLat,locationLng);
                map.MoveToRegion(MapSpan.FromCenterAndRadius(position, Distance.FromKilometers(0.5)));
                
                map.Pins.Add(new Pin { Type = PinType.Generic, Position = position, Label = "Current Location" });
                



                s.Children.Add(new Label { Text = locationStr});
                s.Children.Add(map);
                emptyPage.Content = s;
                await Navigation.PushAsync(emptyPage);
            }
            else if (button == Motion) 
            {
                Debug.WriteLine("Motion Clicked");
                StackLayout s = new StackLayout();
                s.Children.Add(new Label { Text = "You've clicked Motion!" });
                emptyPage.Content = s;
                await Navigation.PushAsync(emptyPage);
            }
            

        }
    }
}
