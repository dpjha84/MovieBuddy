using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace MovieBuddy
{
    public abstract class AdapterBase : RecyclerView.Adapter
    {
        //public event EventHandler<int> ItemClick;
        public List<ImageView> ImageViewsToClean = new List<ImageView>();

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CardViewItem, parent, false);
            //View mainView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.fragment_blank, parent, false);
            return GetViewHolder(itemView);
        }

        protected abstract RecyclerView.ViewHolder GetViewHolder(View view);
    }
}