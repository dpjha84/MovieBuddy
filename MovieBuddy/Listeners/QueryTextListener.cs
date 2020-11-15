using Android.Content;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using System;
using System.Threading;
using Xamarin.Essentials;
using static Android.Support.V7.Widget.SearchView;

namespace MovieBuddy
{
    public class QueryTextListener : Java.Lang.Object, IOnQueryTextListener
    {
        ViewPager mViewPager;
        TabLayout mTabLayout;
        Android.Support.V7.Widget.Toolbar toolbar;
        Context context;
        Android.Support.V4.App.FragmentManager mgr;
        TypeAssistant assistant;
        public QueryTextListener(Context c, ViewPager pager, TabLayout layout, Android.Support.V7.Widget.Toolbar t, Android.Support.V4.App.FragmentManager m)
        {
            mViewPager = pager;
            mTabLayout = layout;
            toolbar = t;
            context = c;
            mgr = m;
            assistant = new TypeAssistant();
            assistant.Idled += assistant_Idled;
        }

        void assistant_Idled(object sender, EventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var tabPagerAdapter = new SearchPagerAdapter(context, mgr, newText);

                mViewPager.Adapter = tabPagerAdapter;
                mTabLayout.SetupWithViewPager(mViewPager);

                for (int i = 0; i < mTabLayout.TabCount; i++)
                {
                    TabLayout.Tab tab = mTabLayout.GetTabAt(i);
                    tab.SetCustomView(tabPagerAdapter.GetTabView(toolbar, i));
                }
            });
        }

        string newText;
        public bool OnQueryTextChange(string text)
        {
            newText = text;
            assistant.TextChanged();
            return true;
        }

        public bool OnQueryTextSubmit(string newText)
        {
            return false;
        }
    }
    public class TypeAssistant
    {
        public event EventHandler Idled = delegate { };
        public int WaitingMilliSeconds { get; set; }
        System.Threading.Timer waitingTimer;

        public TypeAssistant(int waitingMilliSeconds = 600)
        {
            WaitingMilliSeconds = waitingMilliSeconds;
            waitingTimer = new Timer(p =>
            {
                Idled(this, EventArgs.Empty);
            });
        }
        public void TextChanged()
        {
            waitingTimer.Change(WaitingMilliSeconds, System.Threading.Timeout.Infinite);
        }
    }
}