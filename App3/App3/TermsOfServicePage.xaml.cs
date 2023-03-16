using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App3
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TermsOfServicePage : ContentPage
    {
        public TermsOfServicePage()
        {
            InitializeComponent();

            // Set the text of the label to the terms and conditions text
            termsOfServiceLabel.Text = "Welcome to StatTreck, a mobile application that helps track your phone usage and provides insights into your daily habits and behaviors. By using our services, you agree to the following terms and conditions:\n\n" +
                "1. Use of Our Services: Our services are intended for personal, non-commercial use only. You may not use our services for any illegal or unauthorized purpose. You are solely responsible for any content or activity that occurs under your account.\n\n" +
                "2. User Content: You retain all ownership rights to the content you provide through our services. However, by submitting content, you grant us a non-exclusive, transferable, sub-licensable, royalty-free, worldwide license to use, modify, reproduce, distribute, and display the content for the purpose of providing and promoting our services.\n\n" +
                "3. Disclaimer of Warranties: We provide our services on an 'as is' and 'as available' basis, without any warranties, express or implied. We do not warrant that our services will be uninterrupted or error-free, or that any defects will be corrected. We also do not make any warranties regarding the accuracy, completeness, or reliability of any information provided through our services.\n\n" +
                "4. Limitation of Liability: In no event shall StatTreck or its affiliates be liable for any indirect, incidental, special, or consequential damages arising out of or in connection with your use of our services, whether or not such damages are foreseeable or whether we have been advised of the possibility of such damages. Our total liability for any claim arising out of or relating to these terms or our services shall not exceed the amount paid by you for the use of our services.\n\n" +
                "5. Governing Law: These terms and your use of our services shall be governed by and construed in accordance with the laws of the United Kingdom, without giving effect to any principles of conflicts of law.";
        }
    }
}
