using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using FontAwesome;
using FontAwesome2;
using App3.Helpers;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FlyoutPage : Xamarin.Forms.FlyoutPage
    {
        Page1 p;
        MainPage main;
        public FlyoutPage(Page1 p)
        {




            this.p = p;
            NavigationPage.SetHasNavigationBar(this, false);
            NavigationPage.SetHasBackButton(this, false);
            // Set up the flyout menu

            //EventHandler for LogOut Button
            var tapGestureRecognizer = new TapGestureRecognizer();
            tapGestureRecognizer.Tapped += (sss, e) =>
            {
                IsBusy = true;
                Debug.WriteLine("Logged out Clicked");
                Preferences.Clear();
                String ss = "Host=penguin.kent.ac.uk;Username=pp434;Password=rolibb8;Database=pp434";
                NpgsqlConnection conn = new NpgsqlConnection(ss);
                conn.Open();
                Application.Current.MainPage = new NavigationPage(new Page1(ss, conn));
                ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex("#F4D03F");
                try
                {
                    DependencyService.Get<IStopService>().StopService("Battery");

                }
                catch { }
                IsBusy = false;

            };

            //FlyOutMenu Content
            Grid grid = new Grid();
            grid.BackgroundColor = Color.FromHex("#EFE5B0");
            grid.Padding = (new Thickness(20, 10, 20, 10));
            grid.HeightRequest = 30;
            grid.WidthRequest = 40;

            var pressedState = new VisualState { Name = "Pressed" };
            var unpressedState = new VisualState { Name = "Unpressed" };
            var backgroundColorSetter = new Setter { Property = VisualElement.BackgroundColorProperty, Value = Color.Gray };

            pressedState.Setters.Add(backgroundColorSetter);
            unpressedState.Setters.Add(backgroundColorSetter);

            // apply press appearance changes to grid
            VisualStateManager.SetVisualStateGroups(grid, new VisualStateGroupList
            {
                new VisualStateGroup
                {
                        Name = "CommonStates",
                        States =
                        {
                                pressedState,
                                unpressedState
                        }
                }
            });


            grid.GestureRecognizers.Add(tapGestureRecognizer);
            Label logout = new Label
            {
                Text = FontAwesome2.FontAwesomeIcons2.ArrowRight,
                BackgroundColor = Color.Transparent,
                TextColor = Color.FromHex("#D95055"),
                FontFamily = "FA2",
                FontSize = 18,
                VerticalOptions = LayoutOptions.Center,
                HorizontalTextAlignment = TextAlignment.End

            };
            Label logoutLabel = new Label
            {
                Text = "Log Out",
                HorizontalTextAlignment = TextAlignment.Start,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 18,
                TextColor = Color.FromHex("#D95055"),
                FontFamily = "BUB2",

            };
            Grid.SetColumn(logout, 1);
            Grid.SetColumn(logoutLabel, 0);
            grid.Children.Add(logoutLabel);
            grid.Children.Add(logout);

            StackLayout s = new StackLayout();

            s.Children.Add(new Label { Text = "Menu", FontFamily = "BUB2", HorizontalTextAlignment = TextAlignment.Center, FontSize = 30, Margin = new Thickness(10, 10, 10, 650) });
            s.Children.Add(grid);

            var menuPage = new ContentPage
            {
                Title = "Menu",
                BackgroundColor = Color.FromHex("#FFF2B3"),
                Content = new StackLayout
                {
                    Children =
                    {
                       s

                    }
                }
            };
            this.Flyout = menuPage;

            //MainPage set up as Main Content
            main = new MainPage(p);
            this.Detail = new NavigationPage(main);
            ((NavigationPage)Detail).BarBackgroundColor = Color.FromHex("#F4D03F");
            Application.Current.MainPage = this;





        }
    }
}
