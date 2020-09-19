using Android.App;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using static Android.Widget.AdapterView;

namespace MovieBuddy
{
    [Activity(Label = "Explore Movies")]
    public class ExploreActivity : ActivityBase
    {
        ExploreMovieInfo exploreInfo;
        bool loaded1 = false, loaded2 = false;

        public ExploreActivity()
        {
            adRenderer = new AdRenderer();
        }
        protected override void OnCreate(Bundle bundle)
        {
            InitView(Resource.Layout.Explore, bundle);
            adRenderer.RenderAd(FindViewById<AdView>(Resource.Id.adView));

            Title = "Explore Movies";
            var endYearIndex = Globals.Years.FindIndex(x => x == DateTime.Now.Year);
            var startYearIndex = endYearIndex - 9;
            var genres = new List<string> { "Any" };
            genres.AddRange(MovieManager.GenreMap.Values.ToList());

            SetupSpinner(Resource.Id.spinner, spinner_ItemSelected, Globals.Years, startYearIndex);
            SetupSpinner(Resource.Id.spinner2, spinner2_ItemSelected, Globals.Years, endYearIndex);
            SetupSpinner(Resource.Id.spinner3, spinner3_ItemSelected, genres, 0);

            exploreInfo = new ExploreMovieInfo { StartYear = Globals.Years[startYearIndex], EndYear = Globals.Years[endYearIndex], Genre = genres[0] };

            var toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetHomeButtonEnabled(true);
        }

        private void SetupSpinner<T>(int resourceId, Action<object, ItemSelectedEventArgs> action, IList<T> data, int initialPosition)
        {
            var spinner = FindViewById<Spinner>(resourceId);
            spinner.ItemSelected += new EventHandler<ItemSelectedEventArgs>(action);
            var adapter = new ArrayAdapter<T>(this, Android.Resource.Layout.SimpleSpinnerItem, data);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
            spinner.SetSelection(initialPosition);
        }

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
    }
}