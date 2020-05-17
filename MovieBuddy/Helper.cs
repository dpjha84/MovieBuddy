using Android.Content;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
//using Square.Picasso;
using System;

namespace MovieBuddy
{
    public class Helper
    {
        internal static void SetImage(Context castInfoActivity, int resourceId, ImageView imageView)
        {
            try
            {
                Glide.With(castInfoActivity).Load(resourceId).Thumbnail(0.2f).Into(imageView);
            }
            catch (Exception)
            {
            }
            
        }

        internal static void Clear(Context context, View view)
        {
            Glide.With(context).Clear(view);
        }

        internal static void SetImage(Context castInfoActivity, string backdrop, ImageView imageView)
        {
            try
            {
                Glide.With(castInfoActivity).Load(backdrop).Thumbnail(0.2f).Into(imageView);
            }
            catch (Exception)
            {
            }
            
        }
    }
}