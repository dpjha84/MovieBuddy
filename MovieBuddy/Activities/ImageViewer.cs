using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace MovieBuddy
{
    [Activity(Label = "FullScreenViewer")]
    public class ImageViewer : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ImageViewer);
            var image = FindViewById<ImageView>(Resource.Id.imgViewer);
            Helper.SetImage(this, Intent.GetStringExtra("url"), image, Resource.Drawable.noimage);
        }
    }
}