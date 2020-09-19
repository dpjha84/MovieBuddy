using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using Jaeger;

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
        protected void InitView(int resourceId, Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(resourceId);
            Xamarin.Essentials.Platform.Init(this, bundle);

            toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            if (toolbar != null)
            {
                SetSupportActionBar(toolbar);
                SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                SupportActionBar.SetHomeButtonEnabled(true);
            }
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