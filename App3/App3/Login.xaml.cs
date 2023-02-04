using App3.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;



namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class Page1 : ContentPage
    {
        MainPage main;
        public Page1()
        {

            InitializeComponent();

            Data settings = new Data();
            StackLayout s = new StackLayout();
            Xamarin.Forms.Switch GPS_Switch = new Xamarin.Forms.Switch();
            GPS_Switch.AutomationId = "GPS_Switch";
            Xamarin.Forms.Switch ScreenT_Switch = new Xamarin.Forms.Switch();
            ScreenT_Switch.AutomationId = "Screen_Switch";
            GPS_Switch.Toggled += Toggle_Clicked;
            ScreenT_Switch.Toggled += Toggle_Clicked;

            s.Children.Add(new Label { Text = "Settings!" });
            s.Children.Add(new Label { Text = "GPS Toggle" });
            s.Children.Add(GPS_Switch);
            s.Children.Add(new Label { Text = "Screen Time Toggle" });
            s.Children.Add(ScreenT_Switch);
            settings.Content = s;

            main = new MainPage();
            main.ToolbarItems.Add(new ToolbarItem
            {
                Text = "Settings",
                Command = new Command(() =>
                {
                    Navigation.PushAsync(settings);
                })
            });
            NavigationPage.SetHasBackButton(main, false);



        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(main, true);




        }

        private async void Toggle_Clicked(object sender, ToggledEventArgs e)
        {
            bool isToggled = e.Value;
            Xamarin.Forms.Switch s = sender as Xamarin.Forms.Switch;
            
            if (isToggled && s.AutomationId == "GPS_Switch")
            {

                //GPS Permission Requesting
                System.Diagnostics.Debug.WriteLine("GPS_Switch Toggle On");
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("No Permission yet");
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }


                if (status == PermissionStatus.Granted)
                {
                    DependencyService.Get<IStartService>().StartService("LocationService");
                }
            }
            if (!isToggled && s.AutomationId == "GPS_Switch") {
                System.Diagnostics.Debug.WriteLine("GPS_Switch Toggle Off");
                DependencyService.Get<IStopService>().StopService("LocationService");
            }


            //ScreenTime Service

            if (isToggled && s.AutomationId == "Screen_Switch")
            {
                

                Debug.WriteLine("Screen_Time Toggled On");
                DependencyService.Get<IStartService>().StartService("ScreenTime");
            }
            if (!isToggled && s.AutomationId == "Screen_Switch") {
                Debug.WriteLine("Screen_Time Toggled Off");
                DependencyService.Get<IStopService>().StopService("ScreenTime");
            }
        }

    }
}