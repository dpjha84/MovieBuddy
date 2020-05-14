using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;

namespace MovieBuddy
{
    public class BaseFragment : Android.Support.V4.App.Fragment
    {
        
        protected string movieName;
        protected int movieId;

        public BaseFragment(string movieName, int movieId)
        {
            this.movieName = movieName;
            this.movieId = movieId;
            Arguments = new Bundle();
        }

        public BaseFragment()
        {

        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        protected virtual void OnItemClick(object sender, int position)
        {
        }
    }
}