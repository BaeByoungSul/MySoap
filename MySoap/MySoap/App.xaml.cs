using MySoap.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MySoap
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            //MainPage = new MainPage();
            MainPage = new Test_Item();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
