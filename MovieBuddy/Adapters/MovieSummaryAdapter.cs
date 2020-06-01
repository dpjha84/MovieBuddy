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
    public class MovieSummaryAdapter : ClickableAdapter
    {
        public Dictionary<string, string> content;

        public MovieSummaryAdapter(Dictionary<string, string> content)
        {
            this.content = content;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            MovieSummaryViewHolder vh = holder as MovieSummaryViewHolder;
            //var data = string.Join('\n', content);
            SpannableStringBuilder str = new SpannableStringBuilder();
            //int prev = 0;
            foreach (var item in content)
            {
                var ss = new SpannableString(item.Key);
                ss.SetSpan(new StyleSpan(TypefaceStyle.Bold), 0, item.Key.Length, SpanTypes.ExclusiveExclusive);
                str.Append(ss);
                str.Append(new SpannableString("\n"));
                str.Append(item.Value);
                str.Append(new SpannableString("\n\n"));
            }
            //for (int i = 0; i < content.Count; i++)
            //{
            //    if (i % 2 == 0)
            //    {
            //        str.SetSpan(new StyleSpan(TypefaceStyle.Bold), prev, prev + content[i].Length,
            //            SpanTypes.ExclusiveExclusive);
            //    }
            //    prev += content[i].Length;
            //    prev++;
            //}
            vh.MovieSummary.TextFormatted = str;
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Summary, parent, false);
            return GetViewHolder(itemView);
        }

        protected override RecyclerView.ViewHolder GetViewHolder(View view)
        {
            return new MovieSummaryViewHolder(view, OnClick);
        }

        public override int ItemCount
        {
            get { return 1; }
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