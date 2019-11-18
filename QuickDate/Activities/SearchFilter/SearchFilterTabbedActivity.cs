using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.SearchFilter.Fragment;
using QuickDate.Activities.Tabbes;
using QuickDate.Adapters;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Requests;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SearchFilter
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SearchFilterTabbedActivity : AppCompatActivity
    {
        #region Variables Basic

        private MainTabAdapter Adapter;
        private ViewPager ViewPager;
        private TabLayout TabLayout;

        private FilterBackgroundFragment BackgroundTab;
        private BasicFragment BasicTab;
        private LooksFragment LooksTab;
        private MoreFragment MoreTab;
        private LifestyleFragment LifestyleTab;
        private TextView ActionButton;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.SearchFilterTabbedLayout);

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar();
                 
                AdsGoogle.Ad_RewardedVideo(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnResume()
        {
            try
            {
                base.OnResume();
                AddOrRemoveEvent(true);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnPause()
        {
            try
            {
                base.OnPause();
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnTrimMemory(TrimMemory level)
        {
            try
            {
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
                base.OnTrimMemory(level);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Menu

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    Finish();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }

        #endregion
         
        #region Functions

        private void InitComponent()
        {
            try
            {
                ViewPager = FindViewById<ViewPager>(Resource.Id.viewpager);
                TabLayout = FindViewById<TabLayout>(Resource.Id.tabs);

                ViewPager.OffscreenPageLimit = 5;
                SetUpViewPager(ViewPager);
                TabLayout.SetupWithViewPager(ViewPager);

                TabLayout.SetTabTextColors(AppSettings.TitleTextColor, Color.ParseColor(AppSettings.MainColor));
                 
                ActionButton = FindViewById<TextView>(Resource.Id.toolbar_title);
                ActionButton.Text = GetText(Resource.String.Lbl_ApplyFilter);
                ActionButton.SetTextColor(AppSettings.TitleTextColor);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void InitToolbar()
        {
            try
            {
                var toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (toolbar != null)
                {
                    toolbar.Title = GetString(Resource.String.Lbl_Filter);
                    toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(toolbar);
                    SupportActionBar.SetDisplayShowCustomEnabled(true);
                    SupportActionBar.SetDisplayHomeAsUpEnabled(true);
                    SupportActionBar.SetHomeButtonEnabled(true);
                    SupportActionBar.SetDisplayShowHomeEnabled(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AddOrRemoveEvent(bool addEvent)
        {
            try
            {
                // true +=  // false -=
                if (addEvent)
                {
                    ActionButton.Click += ActionButtonOnClick;
                }
                else
                {
                    ActionButton.Click -= ActionButtonOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Set Tab

        private void SetUpViewPager(ViewPager viewPager)
        {
            try
            {
                Adapter = new MainTabAdapter(SupportFragmentManager);

                if (AppSettings.ShowFilterBasic)
                {
                    BasicTab = new BasicFragment();
                    Adapter.AddFragment(BasicTab, GetText(Resource.String.Lbl_Basics));
                }

                if (AppSettings.ShowFilterLooks)
                {
                    LooksTab = new LooksFragment();
                    Adapter.AddFragment(LooksTab, GetText(Resource.String.Lbl_Looks));
                }

                if (AppSettings.ShowFilterBackground)
                {
                    BackgroundTab = new FilterBackgroundFragment();

                    Adapter.AddFragment(BackgroundTab, GetText(Resource.String.Lbl_Background));
                }
                if (AppSettings.ShowFilterLifestyle)
                {
                    LifestyleTab = new LifestyleFragment();
                    Adapter.AddFragment(LifestyleTab, GetText(Resource.String.Lbl_Lifestyle));
                }

                if (AppSettings.ShowFilterMore)
                {
                    MoreTab = new MoreFragment();
                    Adapter.AddFragment(MoreTab, GetText(Resource.String.Lbl_More));
                }
                 
                viewPager.CurrentItem = Adapter.Count;
                viewPager.Adapter = Adapter;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion Set Tab

        #region Event

        private void ActionButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                // check current state of a Switch (true or false).
                if (BasicTab != null)
                {
                    UserDetails.AgeMin = BasicTab.AgeMin = (int)BasicTab.AgeSeekBar.GetSelectedMinValue();
                    UserDetails.AgeMax = BasicTab.AgeMax = (int)BasicTab.AgeSeekBar.GetSelectedMaxValue();
                    UserDetails.Gender = BasicTab.Gender;
                    UserDetails.Location = BasicTab.Location;
                    UserDetails.SwitchState = BasicTab.SwitchState;
                }

                if (BackgroundTab != null)
                {
                    UserDetails.Language = BackgroundTab.Language;
                    UserDetails.Ethnicity = BackgroundTab.IdEthnicity.ToString();
                    UserDetails.Religion = BackgroundTab.IdReligion.ToString();
                }

                if (LifestyleTab != null)
                {
                    UserDetails.RelationShip = LifestyleTab.IdRelationShip.ToString();
                    UserDetails.Smoke = LifestyleTab.IdSmoke.ToString();
                    UserDetails.Drink = LifestyleTab.IdDrink.ToString();
                }

                if (LooksTab != null)
                {
                    UserDetails.Body = LooksTab.IdBody.ToString();
                    UserDetails.FromHeight = LooksTab.FromHeight;
                    UserDetails.ToHeight = LooksTab.ToHeight;
                }

                if (LooksTab != null)
                {
                    UserDetails.Interest = MoreTab.Interest;
                    UserDetails.Education = MoreTab.IdEducation.ToString();
                    UserDetails.Pets = MoreTab.IdPets.ToString();
                }

                var globalContext = HomeActivity.GetInstance();
                if (AppSettings.ShowUsersAsCards)
                {
                    if (globalContext?.TrendingFragment?.CardDateAdapter2?.UsersDateList?.Count > 0)
                    {
                        globalContext.TrendingFragment.CardDateAdapter2?.UsersDateList?.Clear();
                        globalContext.TrendingFragment.CardDateAdapter2.NotifyDataSetChanged();
                    }
                }
                else
                {
                    if (globalContext?.TrendingFragment?.NearByAdapter?.NearByList?.Count > 0)
                    {
                        globalContext.TrendingFragment.NearByAdapter.NearByList.Clear();
                        globalContext.TrendingFragment.NearByAdapter.NotifyDataSetChanged();
                    }
                }

                SetLocationUser();

                globalContext?.TrendingFragment?.LoadUser();

                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        private void SetLocationUser()
        {
            try
            {
                var dictionary = new Dictionary<string, string>
                {
                    {"show_me_to", UserDetails.Location},
                };
                RequestsAsync.Users.UpdateProfileAsync(dictionary).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}