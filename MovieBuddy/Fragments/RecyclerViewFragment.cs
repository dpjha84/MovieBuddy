using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Fab;
using System;
using System.Linq;
using static Android.Widget.AbsListView;

namespace MovieBuddy
{
    public abstract class RecyclerViewFragment : BaseFragment, IScrollDirectorListener, IOnScrollListener
    {

        protected RecyclerView rv;
        protected NestedScrollView nsv;
        protected GridLayoutManager llm;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                if (!IsConnected()) return null;

                View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

                rv = rootView.FindViewById<RecyclerView>(Resource.Id.rv_recycler_view);
                var emptyView = rootView.FindViewById<TextView>(Resource.Id.empty_view);
                nsv = rootView.FindViewById<NestedScrollView>(Resource.Id.nsv);
                rv.NestedScrollingEnabled = false;
                rv.HasFixedSize = true;

                llm = new GridLayoutManager(this.Context, SpanCount, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                SetupOnScroll();

                var adapter1 = SetAdapter();

                adapter1.HasStableIds = true;
                bool dataAvailable = GetData();
                if (adapter1 is VideosAdapter va)
                {
                    if (!va.videos.Any())
                    {
                        rv.Visibility = ViewStates.Gone;
                        emptyView.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        rv.Visibility = ViewStates.Visible;
                        emptyView.Visibility = ViewStates.Gone;
                    }
                }
                rv.SetAdapter(adapter1);
                HideLoading();
                return rootView;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is Java.Net.UnknownHostException)
                {
                    new AlertDialog.Builder(Context)
                    .SetTitle("No Internet")
                    .SetMessage("Please check your internet connection")
                    .SetPositiveButton("Retry", (sender, args) =>
                    {
                        StartActivity(new Intent(Context, typeof(MainActivity)));
                    })
                    .Show();
                }
            }

            return null;
        }

        protected virtual void SetFab() { }

        protected virtual void SetupOnScroll() { }

        protected virtual bool GetData() { return true; }

        protected virtual int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 3 : 5;
            }
        }

        protected abstract RecyclerView.Adapter SetAdapter();

        public void OnScrollDown()
        {
        }

        public void OnScrollUp()
        {
        }

        public void OnScroll(AbsListView view, int firstVisibleItem, int visibleItemCount, int totalItemCount)
        {
        }

        public void OnScrollStateChanged(AbsListView view, [GeneratedEnum] ScrollState scrollState)
        {
        }
    }
}