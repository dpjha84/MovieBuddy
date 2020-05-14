using Android.Content;
using Android.Widget;
using Com.Bumptech.Glide;
using Square.Picasso;

namespace RecyclerViewer
{
    public class Helper
    {
        internal static void SetImage(Context castInfoActivity, int resourceId, ImageView imageView)
        {
            Picasso.With(castInfoActivity).Load(resourceId).Into(imageView);
        }

        //internal static void SetImage(Context castInfoActivity, Java.Lang.Object resourceId, ImageView imageView)
        //{
        //    Picasso.With(castInfoActivity).Load(resourceId).Into(imageView);
        //}

        internal static void SetImage(Context castInfoActivity, string backdrop, ImageView imageView)
        {
            Picasso.With(castInfoActivity).Load(backdrop).Into(imageView);
        }
    }
}