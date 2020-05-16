using Android.Content;
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
                Glide.With(castInfoActivity).Load(resourceId).Into(imageView);
            }
            catch (Exception)
            {
            }
            
        }

        internal static void SetImage(Context castInfoActivity, string backdrop, ImageView imageView)
        {
            try
            {
                Glide.With(castInfoActivity).Load(backdrop).Into(imageView);
            }
            catch (Exception)
            {
            }
            
        }
    }
}