using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;
using static MovieBuddy.MoviesFragment;

namespace MovieBuddy
{
    public class XamarinRecyclerViewOnScrollListener : Java.Lang.Object, NestedScrollView.IOnScrollChangeListener
    {
        public delegate void LoadMoreEventHandler(object sender, EventArgs e);
        public event LoadMoreEventHandler LoadMoreEvent;
        private int currentPage = 0;

        public void OnScrollChange(NestedScrollView scrollView, int scrollX, int scrollY, int oldScrollX, int oldScrollY)
        {
            View view = (View)scrollView.GetChildAt(scrollView.ChildCount - 1);
            int diff = (view.Bottom - (scrollView.Height + scrollView.ScrollY));
            if (diff == 0)
            {
                LoadMoreEvent(currentPage, null);
            }
        }
    }
    public abstract class RecyclerViewFragment : BaseFragment
    {

        protected RecyclerView rv;
        protected NestedScrollView nsv;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                
                View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

                rv = rootView.FindViewById<RecyclerView>(Resource.Id.rv_recycler_view);
                nsv = rootView.FindViewById<NestedScrollView>(Resource.Id.nsv);
                rv.NestedScrollingEnabled = false;
                rv.HasFixedSize = true;

                var llm = new GridLayoutManager(this.Context, SpanCount, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                SetupOnScroll();

                var adapter1 = SetAdapter();
                adapter1.HasStableIds = true;
                GetData();
                rv.SetAdapter(adapter1);
                return rootView;
            }
            catch (Exception)
            {
            }
            return null;
        }

        List<SearchMovie> movies = new List<SearchMovie>();
        //private void GetData(RecyclerView rv)
        //{
        //    var recyclerViewState = rv.GetLayoutManager().OnSaveInstanceState();
        //    adapter.AddMovies();
        //    rv.GetLayoutManager().OnRestoreInstanceState(recyclerViewState);
        //}

        protected virtual void SetupOnScroll() { }

        protected virtual void GetData() { }

        public override void OnStop()
        {
            //foreach (var imageView in adapter.ImageViewsToClean)
            //{
            //    Helper.Clear(Context, imageView);
            //}
            //adapter = null;
            base.OnStop();
        }

        public override void OnResume()
        {
            base.OnResume();
            //if (adapter == null)
            //{
            //    SetAdapter();
            //    rv.SetAdapter(adapter);
            //}
        }

        protected virtual int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 3 : 5;
            } 
        }

        protected abstract RecyclerView.Adapter SetAdapter();
    }
}