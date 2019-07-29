using CheckCheque.Views;
using Xamarin.Forms;

namespace CheckCheque
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // this.MainPage = new NavigationPage(new LoginPage());
            var tabPage = new TabbedPage();
            tabPage.Children.Add(new InvoicesPage());
            tabPage.Children.Add(new AddInvoicesPage());

            this.MainPage = tabPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
