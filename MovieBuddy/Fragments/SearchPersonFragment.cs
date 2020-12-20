﻿using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
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
            Activity.OverridePendingTransition(Resource.Animation.@Side_in_right, Resource.Animation.@Side_out_left);
        }
    }
}