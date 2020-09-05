using Android.App;
using Android.Content;
using Android.OS;
using Xamarin.Essentials;

namespace MovieBuddy
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        protected AdapterBase adapter;
        protected string movieName;

        protected int MovieId { get { return Arguments.GetInt("movieId"); } }
        protected int CastId { get { return Arguments.GetInt("castId"); } }
        protected string Query { get { return Arguments.GetString("query"); } }

        public BaseFragment()
        {

        }

        protected bool IsConnected()
        {
            switch (Connectivity.NetworkAccess)
            {
                case NetworkAccess.None:
                case NetworkAccess.Unknown:
                    new AlertDialog.Builder(Context)
                    .SetTitle("No Internet")
                    .SetMessage("Please check your internet connection")
                    .SetPositiveButton("Retry", (sender, args) =>
                    {
                        StartActivity(new Intent(Context, typeof(MainActivity)));
                    })
                    .Show();
                    return false;
            }
            return true;
        }
        public override void OnCreate(Bundle savedInstanceState)
        {
            MovieManager.EnsureLoaded();
            base.OnCreate(savedInstanceState);
        }

        protected virtual void OnItemClick(object sender, int position)
        {
        }
    }
}