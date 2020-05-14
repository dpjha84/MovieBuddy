using System;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Support.V7.Widget;
using Square.Picasso;
using Android.Support.V4.App;
using MovieBuffLib;
using Android.Support.V4.View;

namespace RecyclerViewer
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
            Caption = itemView.FindViewById<TextView>(Resource.Id.textView);
            Date = itemView.FindViewById<TextView>(Resource.Id.movieDetail);
            //ImdbRating = itemView.FindViewById<TextView>(Resource.Id.movieReview);
            Genre = itemView.FindViewById<TextView>(Resource.Id.movieGenre);
            ImdbImage = itemView.FindViewById<ImageView>(Resource.Id.imdbView);

            PlayImage = itemView.FindViewById<ImageButton>(Resource.Id.play_button);

            itemView.Click += (sender, e) => listener(base.LayoutPosition);
            if (PlayImage != null)
                PlayImage.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }

    public class DashboardViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public TextView Header;
        public Button BtnMore;
        public OtherMoviesAdapter horizontalAdapter;
        public TrendingMoviesAdapter horizontalAdapterBig;
        public RecyclerView horizontalList;
        public RecyclerView horizontalListBig;

        // Get references to the views defined in the CardView layout.
        public DashboardViewHolder(View itemView, Action<int> listener, Action<int> moreListener) : base(itemView)
        {
            // Locate and cache view references:btn_more
            Header = itemView.FindViewById<TextView>(Resource.Id.course_item_name_tv);
            BtnMore = itemView.FindViewById<Button>(Resource.Id.btn_more);
            BtnMore.Click += (sender, e) => moreListener(base.LayoutPosition);;

            horizontalList = itemView.FindViewById<RecyclerView>(Resource.Id.horizontal_list);            
            horizontalList.SetLayoutManager(new LinearLayoutManager(itemView.Context, LinearLayoutManager.Horizontal, false));
            horizontalAdapter = new OtherMoviesAdapter();
            horizontalAdapter.ItemClick += HorizontalAdapter_ItemClick;
            horizontalList.SetAdapter(horizontalAdapter);

            horizontalListBig = itemView.FindViewById<RecyclerView>(Resource.Id.horizontal_list_big);
            horizontalListBig.SetLayoutManager(new LinearLayoutManager(itemView.Context, LinearLayoutManager.Horizontal, false));
            horizontalAdapterBig = new TrendingMoviesAdapter();
            horizontalAdapterBig.ItemClick += HorizontalAdapterBig_ItemClick;
            horizontalListBig.SetAdapter(horizontalAdapterBig);

            // Detect user clicks on the item view and report which item
            // was clicked (by layout position) to the listener:
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }

        private void BtnMore_Click(object sender, EventArgs e)
        {
            ViewPager pager = ((Activity)this.ItemView.Context).FindViewById<ViewPager>(Resource.Id.viewpager);
            pager.SetCurrentItem(1, true);
        }

        private void HorizontalAdapterBig_ItemClick(object sender, int position)
        {
            var movie = (sender as TrendingMoviesAdapter).TrendingMovies[position];
            Intent intent = new Intent(this.ItemView.Context, typeof(MovieInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("movieId", (int)movie.Id);
            b.PutString("movieName", movie.Title);
            var backdrop = movie.BackdropPath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
            intent.PutExtras(b);
            this.ItemView.Context.StartActivity(intent);
        }

        private void HorizontalAdapter_ItemClick(object sender, int position)
        {
            var movie = (sender as OtherMoviesAdapter).OtherMovies[position];
            Intent intent = new Intent(this.ItemView.Context, typeof(MovieInfoActivity));
            Bundle b = new Bundle();
            b.PutInt("movieId", (int)movie.Id);
            b.PutString("movieName", movie.Title);
            var backdrop = movie.BackdropPath;
            b.PutString("imageUrl", !string.IsNullOrWhiteSpace(backdrop) ? backdrop : movie.PosterPath);
            intent.PutExtras(b);
            this.ItemView.Context.StartActivity(intent);
        }
    }

    public class ItemViewHolder : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }

        public ItemViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageView);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
    public class ItemViewHolderBig : RecyclerView.ViewHolder
    {
        public ImageView Image { get; private set; }
        public ImageView ImdbImage { get; private set; }
        public TextView MovieName { get; private set; }
        public TextView Genre { get; private set; }
        public TextView Rating { get; private set; }

        public ItemViewHolderBig(View itemView, Action<int> listener) : base(itemView)
        {
            Image = itemView.FindViewById<ImageView>(Resource.Id.imageViewBig);
            ImdbImage = itemView.FindViewById<ImageView>(Resource.Id.imdbView); 
            MovieName = itemView.FindViewById<TextView>(Resource.Id.movie_name_big);
            Genre = itemView.FindViewById<TextView>(Resource.Id.movie_info_big);
            Rating = itemView.FindViewById<TextView>(Resource.Id.movie_rating_big);
            itemView.Click += (sender, e) => listener(base.LayoutPosition);
        }
    }
}