using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MovieBuddy
{
    [Activity(Label = "Explore Movies")]
    public class ExploreActivity : AppCompatActivity
    {
        ExploreMovieInfo exploreInfo;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Explore);
            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

            Title = "Explore Movies";
            var endYearIndex = Globals.Years.FindIndex(x => x == DateTime.Now.Year);
            var startYearIndex = endYearIndex - 9;
            var genres = new List<string> { "Any" };
            genres.AddRange(MovieManager.GenreMap.Values.ToList());

            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinner);
            spinner.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner_ItemSelected);
            ArrayAdapter<int> adapter = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleSpinnerItem, Globals.Years);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.SetSelection(startYearIndex);

            Spinner spinner2 = FindViewById<Spinner>(Resource.Id.spinner2);            
            spinner2.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner2_ItemSelected);
            ArrayAdapter<int> adapter2 = new ArrayAdapter<int>(this, Android.Resource.Layout.SimpleSpinnerItem, Globals.Years);
            adapter2.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner2.Adapter = adapter2;
            spinner2.SetSelection(endYearIndex);

            Spinner spinner3 = FindViewById<Spinner>(Resource.Id.spinner3);
            spinner3.ItemSelected += new EventHandler<AdapterView.ItemSelectedEventArgs>(spinner3_ItemSelected);
            ArrayAdapter<string> adapter3 = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, genres);
            adapter3.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner3.Adapter = adapter3;
            spinner3.SetSelection(0);

            exploreInfo = new ExploreMovieInfo { StartYear = Globals.Years[startYearIndex], EndYear = Globals.Years[endYearIndex], Genre = genres[0] };

            var toolbar = (Android.Support.V7.Widget.Toolbar)FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }
        bool loaded1 = false, loaded2 = false;
        private void LoadData()
        {
            var details = ExploreMoviesFragment.NewInstance(exploreInfo);
            SupportFragmentManager.Fragments.Clear();
            var fragmentTransaction = SupportFragmentManager.BeginTransaction();
            fragmentTransaction.Replace(Resource.Id.frameLayout1, details);
            fragmentTransaction.Commit();
        }

        private void spinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if(!loaded1)
            {
                loaded1 = true;
                return;
            }
            exploreInfo.StartYear = int.Parse(((Spinner)sender).GetItemAtPosition(e.Position).ToString());
            LoadData();
        }

        private void spinner2_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            if (!loaded2)
            {
                loaded2 = true;
                return;
            }
            exploreInfo.EndYear = int.Parse(((Spinner)sender).GetItemAtPosition(e.Position).ToString());
            LoadData();
        }

        private void spinner3_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            exploreInfo.Genre = ((Spinner)sender).GetItemAtPosition(e.Position).ToString();
            LoadData();
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

            var mAdView = FindViewById<AdView>(Resource.Id.adView);
            var adRequest = new AdRequest.Builder().Build();
            mAdView.LoadAd(adRequest);

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