using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace MovieBuddy
{
    public class PhotoViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Caption { get; private set; }
        public TextView Date { get; private set; }
        public ImageView ImdbImage { get; private set; }
        public ImageButton PlayImage { get; private set; }
        public TextView ImdbRating { get; private set; }
        public TextView Genre { get; private set; }

        public PhotoViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            //Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
            //Date = itemView.FindViewById<TextView>(Resource.Id.movieDetail);
            //ImdbRating = itemView.FindViewById<TextView>(Resource.Id.movieReview);
            //Genre = itemView.FindViewById<TextView>(Resource.Id.movieGenre);
            //ImdbImage = itemView.FindViewById<ImageView>(Resource.Id.imdbView);

            //PlayImage = itemView.FindViewById<ImageButton>(Resource.Id.play_button);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
            if (PlayImage != null)
                PlayImage.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}