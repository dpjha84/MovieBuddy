using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
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
                Glide.With(castInfoActivity).Load($"https://image.tmdb.org/t/p/w500/{backdrop}").Thumbnail(0.2f).Into(imageView);
            }
            catch (Exception)
            {
            }
            
        }
    }
}