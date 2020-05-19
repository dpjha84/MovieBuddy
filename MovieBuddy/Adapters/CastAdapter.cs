using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using System;
using System.Collections.Generic;
using TMDbLib.Objects.Search;
using Java.Lang;

namespace MovieBuddy
{
    public class CastAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<TMDbLib.Objects.Movies.Cast> Cast;

        public CastAdapter(List<TMDbLib.Objects.Movies.Cast> cast)
        {
            this.Cast = cast;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.CastViewItem, parent, false);
            var vh = new CastViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CastViewHolder;
            vh.CastName.Text = Cast[position].Name;
            vh.Character.Text = Cast[position].Character;
            Context context = vh.CastImage.Context;
            Helper.SetImage(context, Cast[position].ProfilePath, vh.CastImage, Resource.Drawable.NoCast);
        }

        public override int ItemCount
        {
            get { return Cast.Count; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}