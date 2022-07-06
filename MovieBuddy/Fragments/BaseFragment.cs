using Android.App;
using Android.Content;
using Android.OS;
using System;
using Xamarin.Essentials;

namespace MovieBuddy
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        protected AdapterBase adapter;
        protected string movieName;
        protected static AlertDialog loading;

        protected string MovieName { get { return Arguments.GetString("movieName"); } }
        protected int MovieId { get { return Arguments.GetInt("movieId"); } }
        protected int CastId { get { return Arguments.GetInt("castId"); } }
        protected string Query { get { return Arguments.GetString("query"); } }

        public BaseFragment()
        {

        }

        public void ShowLoading()
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetCancelable(false);
            builder.SetView(Resource.Layout.loading);
            loading = builder.Create();
            loading.Show();
        }

        protected virtual void HideLoading() { }

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
            try
            {
                MovieManager.EnsureLoaded();
                //Toast.MakeText(this.Context, $"Calls: {TClientBase.calls}", ToastLength.Long).Show();
                base.OnCreate(savedInstanceState);
            }
            catch (Exception)
            {
            }

        }

        protected virtual void OnItemClick(object sender, int position)
        {
            ShowLoading();
        }

        protected virtual void OnPosterClick(object sender, int position)
        {
        }
    }
}