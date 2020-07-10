using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

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
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            var cast = (sender as PopularPersonAdapter<PersonResult>).Cast[position];
            b.PutInt("castId", cast.Id);
            b.PutString("castName", cast.Name);
            b.PutString("imageUrl", cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
        }
    }
    public class SearchPersonFragment : CastFragment
    {
        protected SearchPersonAdapter<SearchPerson> searchPersonAdapter;

        public static SearchPersonFragment NewInstance(string query)
        {
            var frag1 = new SearchPersonFragment();
            Bundle bundle = new Bundle();
            bundle.PutString("query", query);
            frag1.Arguments = bundle;
            return frag1;
        }

        protected int page = 1;
        protected override void GetData()
        {
            var data = MovieManager.Instance.SearchPerson(Query, page++);
            if (data == null) return;
            var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
            searchPersonAdapter.LoadData(data);
            rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        }

        protected override RecyclerView.Adapter SetAdapter()
        {
            searchPersonAdapter = new SearchPersonAdapter<SearchPerson>();
            searchPersonAdapter.ItemClick += OnItemClick;
            return searchPersonAdapter;
        }

        protected override void OnItemClick(object sender, int position)
        {
            Intent intent = new Intent(this.Context, typeof(CastInfoActivity));
            Bundle b = new Bundle();
            var cast = (sender as SearchPersonAdapter<SearchPerson>).Cast[position];
            b.PutInt("castId", cast.Id);
            b.PutString("castName", cast.Name);
            b.PutString("imageUrl", cast.ProfilePath);
            intent.PutExtras(b);
            StartActivity(intent);
        }
    }
}