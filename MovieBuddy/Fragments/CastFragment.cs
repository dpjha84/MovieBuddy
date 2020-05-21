using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Linq;

namespace MovieBuddy
{
    public class CastFragment : RecyclerViewFragment
    {
        public static CastFragment NewInstance(int movieId)
        {
            var frag1 = new CastFragment();
            Bundle bundle = new Bundle();
            bundle.PutInt("movieId", movieId);
            frag1.Arguments = bundle;            
            return frag1;
        }

        protected override void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("castId", (sender as CastAdapter).Cast[position].Id);
            b.PutString("castName", (sender as CastAdapter).Cast[position].Name);
            var cast = (sender as CastAdapter).Cast[position];
            var backdrop = cast.ProfilePath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
        }

        protected override void SetAdapter()
        {
            adapter = new CastAdapter(MovieManager.Instance.GetCastAndCrew(MovieId).Cast.Take(10).ToList());
            adapter.ItemClick += OnItemClick;
        }
    }
}