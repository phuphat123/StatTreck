using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            GPS.Clicked += Button_Clicked;
            Motion.Clicked += Button_Clicked;

        }

        private void SwipeGestureRecognizer_Swiped(object sender, SwipedEventArgs e)
        {
            Navigation.PushAsync(new Data());
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Debug.WriteLine("Button Clicked!");
            Data emptyPage = new Data();
            Button button = sender as Button;
            if (button == GPS)
            {
                Debug.WriteLine("GPS Clicked");
                StackLayout s = new StackLayout();
                s.Children.Add(new Label {Text = "You've clicked GPS!" });
                emptyPage.Content = s;
                Navigation.PushAsync(emptyPage);


            }
            else if (button == Motion) {
                Debug.WriteLine("Motion Clicked");
                StackLayout s = new StackLayout();
                s.Children.Add(new Label { Text = "You've clicked Motion!" });
                emptyPage.Content = s;
                Navigation.PushAsync(emptyPage);
            }

        }
    }
}
