using Xamarin.Forms;

namespace CheckCheque
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(this.PasswordEntry.Text))
            {
                this.PasswordEntry.PlaceholderColor = Color.Red;
                return;
            }

            if (!"1244".Equals(this.PasswordEntry.Text))
            {
                this.PasswordEntry.Placeholder = "Wrong Password";
                this.PasswordEntry.PlaceholderColor = Color.Red;
                this.PasswordEntry.Text = "";
                return;
            }

            this.Navigation.PushAsync(new MainPage());
        }
    }
}
