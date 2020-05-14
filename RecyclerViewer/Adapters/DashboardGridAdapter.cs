using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using MovieBuffLib;
using Square.Picasso;
using System;
using System.Collections.Generic;
//using TMDbLib.Objects.Search;

namespace RecyclerViewer
{
    public class DashboardGridAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public event EventHandler<int> MoreClick;
        public List<MovieList> mPhotoAlbum;
        private bool mainPage = false;
        private bool dashBoard = false;
        private static RecyclerView horizontalList;
        private static RecyclerView horizontalListBig;
        private List<String> mDataList;

        public DashboardGridAdapter(List<MovieList> movies, bool mainPage = false, bool dashBoard = false)
        {
            mPhotoAlbum = movies;
            this.mainPage = mainPage;
            this.dashBoard = dashBoard;
            mDataList = new List<string> { "Released", "Upcoming" };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.DashboardItems, parent, false);
            return new DashboardViewHolder(itemView, OnClick, OnMoreClick);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            DashboardViewHolder vh = holder as DashboardViewHolder;
            vh.Header.Text = position == 0 ? "Released" : "Upcoming";
            //Context context = vh.Image.Context;
            vh.horizontalAdapter.SetData(mPhotoAlbum[position].Others);
            vh.horizontalAdapterBig.SetData(mPhotoAlbum[position].Latest);
            //SetPoster(vh.Image, context, mPhotoAlbum[position].PosterPath);

        }

        private void SetPoster(ImageView img, Context context, string poster)
        {
            try
            {
                if (!string.IsNullOrEmpty(poster))
                {
                    if (poster.StartsWith("http"))
                        Helper.SetImage(context, poster, img);
                    //Glide.With(context).Load(poster).Into(vh.Image);
                    else
                        Helper.SetImage(context, $"https://image.tmdb.org/t/p/w500/{poster}", img);
                }
                else
                    Helper.SetImage(context, Resource.Drawable.noimage, img);
                //Glide.With(context).Load(Resource.Drawable.noimage).Into(vh.Image);
            }
            catch (Exception)
            {
                Helper.SetImage(context, Resource.Drawable.noimage, img);
                //Glide.With(context).Load(Resource.Drawable.noimage).Into(vh.Image);
                //throw;
            }
        }

        public override int ItemCount
        {
            get { return mDataList.Count; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }

        void OnMoreClick(int position)
        {
            if (ItemClick != null)
                MoreClick(this, position);
        }

    }
}