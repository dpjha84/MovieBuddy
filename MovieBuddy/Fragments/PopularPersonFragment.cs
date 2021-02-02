using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using TMDbLib.Objects.General;

namespace MovieBuddy
{
    public class PopularPersonFragment : SearchPersonFragment
    {
        protected PopularPersonAdapter<PersonResult> popularPersonAdapter;

        public static PopularPersonFragment NewInstance()
        {
            var frag1 = new PopularPersonFragment();
            Bundle bundle = new Bundle();
            frag1.Arguments = bundle;
            return frag1;
        }

        protected override void GetData()
        {
            var data = MovieManager.Instance.GetPopularPersons(page++);
            if (data == null) return;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            popularPersonAdapter.LoadData(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            popularPersonAdapter = new PopularPersonAdapter<PersonResult>();
            popularPersonAdapter.ItemClick += OnItemClick;
            return popularPersonAdapter;
        }

        protected override void OnItemClick(object sender, int position)
        {
            ShowLoading();
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            var cast = (sender as PopularPersonAdapter<PersonResult>).Cast[position];
            b.PutInt("castId", cast.Id);
            b.PutString("castName", cast.Name);
            b.PutString("imageUrl", cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
            Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);
        }
    }
}