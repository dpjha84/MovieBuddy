using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Java.Net;

namespace MovieBuddy
{
    public class TrailerFragment : BaseFragment
    {
        string backdrop;
        string trailerId;
        public static TrailerFragment NewInstance(string movieName, int movieId, string backdrop)
        {
            var frag1 = new TrailerFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            bundle.PutString("movieName", movieName);
            bundle.PutString("backdrop", backdrop);
            frag1.Arguments = bundle;
            return frag1;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            movieName = Arguments.GetString("movieName");
            movieId = Arguments.GetInt("movieId");
            backdrop = Arguments.GetString("backdrop");
            
            try
            {
                trailerId = MovieManager.Instance.GetTrailer(movieId, movieName);
                if (trailerId == null) return base.OnCreateView(inflater, container, savedInstanceState);

                var rootView = inflater.Inflate(Resource.Layout.trailer, container, false);
                var backdropImage = (ImageView)rootView.FindViewById(Resource.Id.mediaPreview);
                var playImage = (ImageView)rootView.FindViewById(Resource.Id.playButton);
                Helper.SetImage(Context, backdrop, backdropImage, Resource.Drawable.noimage);
                playImage.Click += Iv_Click;
                //simpleVideoView.SetVideoURI(Android.Net.Uri.Parse("https://www.youtube.com/embed/hlWiI4xVXKY"));// $"https://www.youtube.com/embed/{trailerId}"));
                //simpleVideoView.SetMediaController(new MediaController(Context));
                //simpleVideoView.RequestFocus();
                //simpleVideoView.Start();
                //rv.Settings.JavaScriptEnabled = true;
                //rv.SetWebChromeClient(new WebChromeClient());
                //rv.LoadUrl($"https://www.youtube.com/embed/{trailerId}");
                return rootView;
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
                return null;
            }
        }

        private void Iv_Click(object sender, EventArgs e)
        {
            try
            {
                Intent intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse($"https://www.youtube.com/embed/{trailerId}"));
                StartActivity(intent);
            }
            catch (Exception ex)
            {
                Toast.MakeText(Application.Context, ex.ToString(), ToastLength.Long).Show();
            }
        }
    }
}