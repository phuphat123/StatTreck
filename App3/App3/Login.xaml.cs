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
            GPS_Switch.Toggled += Toggle_Clicked;

            s.Children.Add(new Label { Text = "Settings!" });
            s.Children.Add(new Label { Text = "GPS Toggle" });
            s.Children.Add(GPS_Switch);
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
            if (isToggled)
            {
                //where i want to create my inteent


                System.Diagnostics.Debug.WriteLine("Toggle On");
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    Debug.WriteLine("No Permission yet");
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }
                if (status == PermissionStatus.Granted)
                {
                    DependencyService.Get<IStartService>().StartService();
                    

                }
                

            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Toggle Off");
                DependencyService.Get<IStopService>().StopService();
            }
        }

    }
}