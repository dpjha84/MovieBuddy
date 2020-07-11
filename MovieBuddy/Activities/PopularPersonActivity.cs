using Android.App;
using Android.OS;
using Android.Views;
using Android.Support.V7.App;
using Android.Widget;
using System;

namespace MovieBuddy
{
    [Activity(Label = "Explore Movies")]
    public class ExploreActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Explore);
            Title = "Explore Movies";

            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter = ArrayAdapter<string>.CreateFromResource(
                    this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;

            Spinner spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);
            spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            var adapter2 = ArrayAdapter<string>.CreateFromResource(
                    this, Resource.Array.planets_array, Android.Resource.Layout.SimpleSpinnerItem);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner2.Adapter = adapter2;

            //var details = PopularPersonFragment.NewInstance();
            //var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            //fragmentTransaction.Add(Resource.Id.frameLayout1, details);
            //fragmentTransaction.Commit();

            var toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            //Spinner spinner = (Spinner)sender;
            //string toast = string.Format("The planet is {0}", spinner.GetItemAtPosition(e.Position));
            //Toast.MakeText(this, toast, ToastLength.Long).Show();

            var details = PopularPersonFragment.NewInstance();
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, details);
            fragmentTransaction.Commit();
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

    [Activity(Label = "Popular People")]
    public class PopularPersonActivity : AppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.PopularPeople);
            Title = "Popular People";
            var details = PopularPersonFragment.NewInstance();
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Add(Resource.Id.frameLayout1, details);
            fragmentTransaction.Commit();

            var toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
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