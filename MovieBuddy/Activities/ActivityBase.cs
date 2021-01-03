using Android.App;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Jaeger;
using Java.Lang;
using System;
using System.Collections.Generic;

namespace MovieBuddy
{
    public interface IAdRenderer
    {
        void RenderAd(AdView adView);
    }

    public interface ITransparentStatusBarSetter
    {
        void SetTransparent(Activity activity);
    }

    public class TransparentStatusBarSetter : ITransparentStatusBarSetter
    {
        public void SetTransparent(Activity activity)
        {
            StatusBarUtil.SetTransparent(activity);
        }
    }

    public class AdRenderer : IAdRenderer
    {
        public void RenderAd(AdView adView)
        {
            adView.LoadAd(new AdRequest.Builder().Build());
        }
    }

    public abstract class ActivityBase : AppCompatActivity
    {
        protected ITransparentStatusBarSetter statusBar;
        protected IAdRenderer adRenderer;
        protected Android.Support.V7.Widget.Toolbar toolbar;
        protected virtual float ImageToScreenRatio { get; set; } = 0.3F;
        protected TypedValue ColorNormal;
        protected TypedValue ColorDark;
        protected TypedValue ColorAccent;
        readonly Random random = new Random();

        protected void InitView(int resourceId, Bundle bundle)
        {
            PreCreate();

            base.OnCreate(bundle);
            SetContentView(resourceId);
            Xamarin.Essentials.Platform.Init(this, bundle);

            FindViewById<View>(Resource.Id.viewTransparent)?.SetBackgroundColor(Color.ParseColor("#" + Integer.ToHexString(ColorNormal.Data).Substring(2)));
            if(ColorAccent != null)
                FindViewById<TabLayout>(Resource.Id.tabs)?.SetSelectedTabIndicatorColor(ColorAccent.Data);

            //Toast.MakeText(this, $"Calls: {TClientBase.calls}", ToastLength.Long).Show();
            var appbar = FindViewById<AppBarLayout>(Resource.Id.appbar);
            if (appbar != null)
            {
                var height = ImageToScreenRatio * Resources.DisplayMetrics.HeightPixels;
                CoordinatorLayout.LayoutParams lp = (CoordinatorLayout.LayoutParams)appbar.LayoutParameters;
                lp.Height = (int)height;
            }

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }
        }

        protected virtual void PreCreate()
        {
            var styles = new List<int> { Resource.Style.LightGreen, Resource.Style.Red, Resource.Style.Pink, Resource.Style.Brown,
            Resource.Style.Purple, Resource.Style.Lime, Resource.Style.Amber, Resource.Style.DarkOrange, Resource.Style.BlueGrey};
            var style = styles[random.Next(0, styles.Count)];
            SetTheme(style);

            ColorNormal = new TypedValue();
            ColorDark = new TypedValue();
            ColorAccent = new TypedValue();
            Theme.ResolveAttribute(Resource.Attribute.colorPrimary, ColorNormal, true);
            Theme.ResolveAttribute(Resource.Attribute.colorPrimaryDark, ColorDark, true);
            Theme.ResolveAttribute(Resource.Attribute.colorAccent, ColorAccent, true);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;

                default:
                    return base.OnOptionsItemSelected(item);
            }
        }
    }
}