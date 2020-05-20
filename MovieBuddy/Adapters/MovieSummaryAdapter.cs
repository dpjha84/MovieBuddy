using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using TMDbLib.Objects.Search;

namespace MovieBuddy
{
    public class MovieSummaryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<int> ItemClick;
        public List<string> content;

        public MovieSummaryAdapter(List<string> content)
        {
            this.content = content;
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
            var data = string.Join('\n', content);
            SpannableString str = new SpannableString(data);
            int prev = 0;
            for (int i = 0; i < content.Count; i++)
            {
                if (i % 2 == 0)
                {
                    str.SetSpan(new StyleSpan(TypefaceStyle.Bold), prev, prev + content[i].Length,
                        SpanTypes.ExclusiveExclusive);
                }
                prev += content[i].Length;
                prev++;
            }
            vh.MovieSummary.TextFormatted = str;
        }

        public override int ItemCount
        {
            get { return 1; }
        }

        void OnClick(int position)
        {
            ItemClick?.Invoke(this, position);
        }

        class MovieSummaryViewHolder : RecyclerView.ViewHolder
        {
            public TextView MovieSummary { get; private set; }

            public MovieSummaryViewHolder(View itemView, Action<int> listener) : base(itemView)
            {
                MovieSummary = itemView.FindViewById<TextView>(Resource.Id.movieSummary);
                itemView.Click += (sender, e) => listener(base.LayoutPosition);
            }
        }
    }
}