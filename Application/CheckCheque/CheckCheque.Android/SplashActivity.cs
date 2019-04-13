using Android.App;
using Android.Content;


namespace CheckCheque.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            this.StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }

        // Prevents the back button from doing anything..
        public override void OnBackPressed() { }
    }
}