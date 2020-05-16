using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Square.Picasso;
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
            if (ItemClick != null)
                ItemClick(this, position);
        }
    }

    public class CustomTypefaceSpan : MetricAffectingSpan
{
    readonly Typeface typeFace;

    public CustomTypefaceSpan(Typeface typeFace)
    {
        this.typeFace = typeFace;
    }

    public override void UpdateDrawState(TextPaint tp)
    {
        tp.SetTypeface(typeFace);
    }

    public override void UpdateMeasureState(TextPaint p)
    {
        p.SetTypeface(typeFace);
    }
}
}