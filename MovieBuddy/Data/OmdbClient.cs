using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using OMDbApiNet;
using OMDbApiNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MovieBuddy.Data
{
    public class OClient
    {
        OmdbClient omdb;

        public OClient()
        {
            omdb = new OmdbClient("975f33e", true);
        }

        public Item GetItem(string id)
        {
            return omdb.GetItemById(id);
        }
    }
}