using OMDbApiNet;
using OMDbApiNet.Model;
using System;

namespace MovieBuddy.Data
{
    public class OClient
    {
        private readonly OmdbClient omdb;

        public OClient()
        {
            omdb = new OmdbClient("975f33e", true);
        }

        public Item GetItem(string id)
        {
            try
            {
                return omdb.GetItemById(id);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}