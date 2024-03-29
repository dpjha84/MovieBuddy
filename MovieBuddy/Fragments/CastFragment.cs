using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using System;
using System.Linq;

namespace MovieBuddy
{
    public class CastFragment : RecyclerViewFragment
    {
        protected CastAdapter castAdapter;
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
            base.OnItemClick(sender, position);
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("castId", (sender as CastAdapter).Cast[position].Id);
            b.PutString("castName", (sender as CastAdapter).Cast[position].Name);
            var cast = (sender as CastAdapter).Cast[position];
            var backdrop = cast.ProfilePath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
            Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);
        }

        protected override void HideLoading()
        {
            loading?.Dismiss();
            loading = null;
        }

        protected override void SetupOnScroll()
        {
            var onScrollListener = new RecyclerViewOnScrollListener();
            onScrollListener.LoadMoreEvent += (object sender, EventArgs e) =>
            {
                GetData();
            };
            nsv.SetOnScrollChangeListener(onScrollListener);
        }

        private int page = 1;
        protected override bool GetData()
        {
            var data = MovieManager.Instance.GetCastAndCrew(MovieId, page++);
            if (data == null || !data.Any()) return false;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            castAdapter.LoadData(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
            return true;
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            castAdapter = new CastAdapter();
            castAdapter.ItemClick += OnItemClick;
            return castAdapter;
        }
    }
}