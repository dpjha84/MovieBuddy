using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using System;

namespace MovieBuddy
{
    public abstract class RecyclerViewFragment : BaseFragment
    {
        protected RecyclerView rv;
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View rootView = inflater.Inflate(Resource.Layout.fragment_blank, container, false);

                rv = (RecyclerView)rootView.FindViewById(Resource.Id.rv_recycler_view);
                rv.NestedScrollingEnabled = false;
                rv.HasFixedSize = true;

                var llm = new GridLayoutManager(this.Context, SpanCount, GridLayoutManager.Vertical, false);
                rv.SetLayoutManager(llm);

                SetAdapter();
                rv.SetAdapter(adapter);
                return rootView;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public override void OnStop()
        {
            foreach (var imageView in adapter.ImageViewsToClean)
            {
                Helper.Clear(Context, imageView);
            }
            adapter = null;
            base.OnStop();
        }

        public override void OnResume()
        {
            base.OnResume();
            if (adapter == null)
            {
                SetAdapter();
                rv.SetAdapter(adapter);
            }
        }

        protected virtual int SpanCount
        {
            get
            {
                return Resources.Configuration.Orientation == Android.Content.Res.Orientation.Portrait ? 3 : 5;
            } 
        }

        protected abstract void SetAdapter();
    }
}