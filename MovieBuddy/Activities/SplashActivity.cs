using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.App;
using System.Threading;
using System.Threading.Tasks;

namespace MovieBuddy.Activities
{
    [Activity(Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : AppCompatActivity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
            MovieManager.Init();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }
    }
}