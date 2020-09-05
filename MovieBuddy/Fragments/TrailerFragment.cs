using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;

namespace MovieBuddy
{
    public class TrailerFragment : BaseFragment
    {
        string backdrop;
        string trailerId;
        ImageView playImage;
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
            backdrop = Arguments.GetString("backdrop");

            try
            {
                if (!IsConnected()) return null;

                trailerId = MovieManager.Instance.GetTrailer(MovieId);
                if (trailerId == null) return base.OnCreateView(inflater, container, savedInstanceState);

                var rootView = inflater.Inflate(Resource.Layout.trailer, container, false);
                var backdropImage = (ImageView)rootView.FindViewById(Resource.Id.mediaPreview);
                playImage = (ImageView)rootView.FindViewById(Resource.Id.playButton);
                Helper.SetImage(Context, backdrop, backdropImage, Resource.Drawable.noimage);
                playImage.Click += Iv_Click;
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

        public override void OnDestroy()
        {
            if (playImage != null)
                playImage.Click -= Iv_Click;
            base.OnDestroy();
        }
    }
}