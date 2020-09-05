using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;
using Com.Bumptech.Glide.Request;
using System;

namespace MovieBuddy
{
    public class Helper
    {
        public static void SetImage(Context castInfoActivity, int resourceId, ImageView imageView)
        {
            try
            {
                Glide.With(castInfoActivity).Load(resourceId).Into(imageView);
            }
            catch (Exception)
            {
            }

        }

        internal static void Clear(Context context, View view)
        {
            Glide.With(context).Clear(view);
        }

        internal static void SetImage(Context castInfoActivity, string backdrop, ImageView imageView, int placeholder)
        {
            if (string.IsNullOrWhiteSpace(backdrop))
            {
                SetImage(castInfoActivity, placeholder, imageView);
                return;
            }
            try
            {
                var requestOptions = new RequestOptions()
                    .Apply(RequestOptions.DiskCacheStrategyOf(DiskCacheStrategy.All))
                    .Apply(RequestOptions.PlaceholderOf(Resource.Drawable.noimage));
                //.Apply(RequestOptions.SignatureOf(new ObjectKey(DateTime.Now.Millisecond)));
                //var requestOptions = req .diskCacheStrategy(DiskCacheStrategy.All)
                Glide.With(castInfoActivity).Load($"https://image.tmdb.org/t/p/w500/{backdrop}").Apply(requestOptions).Into(imageView);
            }
            catch (Exception)
            {
            }

        }
    }
}