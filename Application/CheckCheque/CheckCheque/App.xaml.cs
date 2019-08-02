using CheckCheque.Views;
using Xamarin.Forms;

namespace CheckCheque
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

#if DEBUG
            HotReloader.Current.Run(this);
#endif

            // this.MainPage = new NavigationPage(new LoginPage());
            var tabPage = new TabbedPage();
            tabPage.Children.Add(new NavigationPage(new InvoicesPage()) { Title = "Invoices", Icon = "invoice_icon_30" });
            tabPage.Children.Add(new NavigationPage(new AddInvoicesPage()) { Title = "Add Invoices", Icon = "add_invoice_icon_30" });
            tabPage.Children.Add(new NavigationPage(new Page()) { Title = "Settings", Icon = "settings_icon_30" });

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
