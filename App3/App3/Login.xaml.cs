using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
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

            main = new MainPage();
            NavigationPage.SetHasBackButton(main, false);

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(main, true);
            
            

        }
    }

    //private async void SignInButton_Clicked(object sender, EventArgs e)
    //{
        // Navigate to the next page.
        //await Navigation.PushAsync(new NextPage());
    //}
}