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
            //vh.CastImage. = movie.Overview;// $"{mPhotoAlbum[position].Title}\n{mPhotoAlbum[position].Date.ToString("m")}";
            vh.CastName.Text = Cast[position].Name;
            vh.Character.Text = Cast[position].Character;
            //vh.Character.Text = Cast[position].Character;
            Context context = vh.CastImage.Context;
            if (string.IsNullOrWhiteSpace(Cast[position].ProfilePath))
                //Glide.With(context).Load(Resource.Drawable.NoCast).Into(vh.CastImage);
                Helper.SetImage(context, Resource.Drawable.NoCast, vh.CastImage);
            else
                Helper.SetImage(context, $"https://image.tmdb.org/t/p/w500/{Cast[position].ProfilePath}", vh.CastImage);
                //Glide.With(context).Load($"https://image.tmdb.org/t/p/w500/{Cast[position].ProfilePath}").Into(vh.CastImage);
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