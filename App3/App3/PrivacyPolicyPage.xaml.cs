using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrivacyPolicyPage : ContentPage
    {
        public PrivacyPolicyPage()
        {
            InitializeComponent();

            // Set the text of the label to the terms and conditions text
            PrivacyPolicyLabel.Text = "At StatTreck, we respect your privacy and are committed to protecting your personal information. This Privacy Policy describes how we collect, use, and protect your data when you use our services.\n\n" +
                "1. Information We Collect: We collect information about your phone usage, including but not limited to the apps you use, the duration of your usage, and your location data. We may also collect personal information you provide to us, such as your name and email address.\n\n" +
                "2. How We Use Your Information: We use the information we collect to provide and improve our services, including analyzing your phone usage to provide insights into your daily habits and behaviors. We may also use your information to communicate with you, to send you marketing and promotional materials, or to respond to your requests.\n\n" +
                "3. How We Protect Your Information: We take reasonable measures to protect your information from unauthorized access, use, or disclosure. However, no method of transmission over the internet or electronic storage is 100% secure, and we cannot guarantee the absolute security of your information.\n\n" +
                "4. Sharing Your Information: We may share your information with our affiliates, service providers, or other third parties for the purpose of providing and improving our services. We may also disclose your information to comply with legal obligations or to protect our rights and property.\n\n" +
                "5. Changes to this Privacy Policy: We may update this Privacy Policy from time to time. We will notify you of any material changes by posting the new Privacy Policy on our website or through other means.";
        }
    }
}