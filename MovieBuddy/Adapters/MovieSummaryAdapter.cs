using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Square.Picasso;
using System;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieSummaryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public string movie;

        public MovieSummaryAdapter(string movie)
        {
            this.movie = movie;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Summary, parent, false);
            MovieSummaryViewHolder vh = new MovieSummaryViewHolder(itemView, OnClick);
            return vh;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieSummaryViewHolder vh = holder as MovieSummaryViewHolder;
            //String normalBefore = "First Part Not Bold ";
            //String normalBOLD = "BOLD ";
            //String normalAfter = "rest not bold";
            //String finalString = normalBefore + normalBOLD + normalAfter;

            //String boldText = "id";
            //String normalText = "name";
            SpannableString str = new SpannableString(movie);
            //str.SetSpan(new TypefaceSpan(Typeface.Create("", TypefaceStyle.Bold)), 0, boldText.Length, 
            //    SpanTypes.ExclusiveExclusive);
            
            vh.MovieSummary.TextFormatted = str;// $"{mPhotoAlbum[position].Title}\n{mPhotoAlbum[position].Date.ToString("m")}";
        }

        public override int ItemCount
        {
            get { return 1; }
        }

        void OnClick(int position)
        {
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }
}