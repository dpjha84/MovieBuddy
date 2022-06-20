using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class PopularPersonAdapter<T> : SearchPersonAdapter<T>
    {
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CastViewHolder;
            ImageViewsToClean.Add(vh.CastImage);
            var searchPerson = Cast[position] as PersonResult;
            vh.CastName.Text = searchPerson.Name;
            Context context = vh.CastImage.Context;
            Helper.SetImage(context, searchPerson.ProfilePath, vh.CastImage, Resource.Drawable.NoCast);
        }
    }
    public class SearchPersonAdapter<T> : ClickableWithPagingAdapter<T>
    {
        public List<T> Cast;

        public SearchPersonAdapter()
        {
            this.Cast = new List<T>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CastViewHolder;
            ImageViewsToClean.Add(vh.CastImage);
            var searchPerson = Cast[position] as SearchPerson;
            vh.CastName.Text = searchPerson.Name;
            Context context = vh.CastImage.Context;
            Helper.SetImage(context, searchPerson.ProfilePath, vh.CastImage, Resource.Drawable.NoCast);
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view, View contentView, View parentView)
        {
            return new CastViewHolder(view, OnClick);
        }

        protected override void AddToCollection(List<T> data)
        {
            Cast.AddRange(data);
        }

        public override int ItemCount
        {
            get { return Cast.Count; }
        }
    }

    public class CastAdapter : ClickableWithPagingAdapter<TMDbLib.Objects.Movies.Cast>
    {
        public List<TMDbLib.Objects.Movies.Cast> Cast;

        public CastAdapter()
        {
            this.Cast = new List<TMDbLib.Objects.Movies.Cast>();
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var vh = holder as CastViewHolder;
            ImageViewsToClean.Add(vh.CastImage);
            vh.CastName.Text = Cast[position].Name;
            vh.Character.Text = Cast[position].Character;
            Context context = vh.CastImage.Context;
            Helper.SetImage(context, Cast[position].ProfilePath, vh.CastImage, Resource.Drawable.NoCast);
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view, View contentView, View parentView)
        {
            return new CastViewHolder(view, OnClick);
        }

        protected override void AddToCollection(List<TMDbLib.Objects.Movies.Cast> data)
        {
            Cast.AddRange(data);
        }

        public override int ItemCount
        {
            get { return Cast.Count; }
        }
    }

    internal class CastViewHolder : RecyclerView.ViewHolder
    {
        public TextView CastName { get; private set; }

        public TextView Character { get; private set; }
        public ImageView CastImage { get; private set; }

        public CastViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Character = itemView.FindViewById<TextView>(Resource.Id.Description);
            CastName = itemView.FindViewById<TextView>(Resource.Id.Name);
            CastImage = itemView.FindViewById<ImageView>(Resource.Id.Image);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}