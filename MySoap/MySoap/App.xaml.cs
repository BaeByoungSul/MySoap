using MySoapDB.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MySoapDB
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
