using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.RangeSlider;
using QuickDate.Helpers.Utils;
using Exception = System.Exception;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class BasicFragment : Android.Support.V4.App.Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView IconLocation,LocationTextView,LocationPlace,LocationMoreIcon,IconFire,GenderTextView,IconAge,AgeTextView,AgeNumberTextView,OnlineTextView,IconOnline,ResetTextView;
        private RelativeLayout LocationLayout, MainLayout;
        private Button ButtonMan, ButtonGirls, ButtonBoth, ButtonApply;
        public RangeSliderControl AgeSeekBar;
        public Switch OnlineSwitch;

        public int AgeMin = 18, AgeMax = 75;
        public string Gender = "4525,4526", Location = "";
        public bool SwitchState;
        private AdView MAdView;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (SearchFilterTabbedActivity) Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.ButtomSheetSearchFilter, container, false);

                InitComponent(view); 

                AddOrRemoveEvent(true);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnResume()
        {
            try
            {
                MAdView?.Resume();
                base.OnResume();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override void OnPause()
        {
            try
            {
                MAdView?.Pause();
                base.OnPause();
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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public override void OnDestroy()
        {
            try
            { 
                MAdView?.Destroy();

                base.OnDestroy();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                MainLayout = view.FindViewById<RelativeLayout>(Resource.Id.mainLayout);
                IconLocation = view.FindViewById<TextView>(Resource.Id.IconLocation);
                LocationTextView = view.FindViewById<TextView>(Resource.Id.LocationTextView);
                LocationPlace = view.FindViewById<TextView>(Resource.Id.LocationPlace);
                LocationMoreIcon = view.FindViewById<TextView>(Resource.Id.LocationMoreIcon);
                GenderTextView = view.FindViewById<TextView>(Resource.Id.GenderTextView);
                IconFire = view.FindViewById<TextView>(Resource.Id.IconFire);
                IconAge = view.FindViewById<TextView>(Resource.Id.IconAge);
                AgeTextView = view.FindViewById<TextView>(Resource.Id.AgeTextView);
                AgeNumberTextView = view.FindViewById<TextView>(Resource.Id.Agenumber);
                OnlineTextView = view.FindViewById<TextView>(Resource.Id.OnlineTextView);
                IconOnline = view.FindViewById<TextView>(Resource.Id.Icononline);
                ResetTextView = view.FindViewById<TextView>(Resource.Id.Resetbutton);
                LocationLayout = view.FindViewById<RelativeLayout>(Resource.Id.LayoutLocation);
                ButtonMan = view.FindViewById<Button>(Resource.Id.ManButton);
                ButtonGirls = view.FindViewById<Button>(Resource.Id.GirlsButton);
                ButtonBoth = view.FindViewById<Button>(Resource.Id.BothButton);
                ButtonApply = view.FindViewById<Button>(Resource.Id.ApplyButton);
                AgeSeekBar = view.FindViewById<RangeSliderControl>(Resource.Id.seekbar);
                OnlineSwitch = view.FindViewById<Switch>(Resource.Id.togglebutton);

                ResetTextView.Visibility = ViewStates.Gone;
                ButtonApply.Visibility = ViewStates.Gone;
                MainLayout.Visibility = ViewStates.Gone;

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLocation, IonIconsFonts.IosLocationOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, LocationMoreIcon, IonIconsFonts.ChevronRight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconFire, IonIconsFonts.IosPersonOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconAge, IonIconsFonts.Calendar);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconOnline, IonIconsFonts.Ionic);

                FontUtils.SetFont(LocationTextView, Fonts.SfRegular);
                FontUtils.SetFont(LocationPlace, Fonts.SfRegular);
                FontUtils.SetFont(GenderTextView, Fonts.SfRegular);
                FontUtils.SetFont(AgeTextView, Fonts.SfRegular);
                FontUtils.SetFont(OnlineTextView, Fonts.SfRegular);

                AgeSeekBar.SetSelectedMinValue(18);
                AgeSeekBar.SetSelectedMaxValue(75);

                OnlineSwitch.Checked = false;

                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));

                MAdView = view.FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);

                SetLocalData();
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
                    LocationLayout.Click += LocationLayoutOnClick;
                    ButtonMan.Click += ButtonManOnClick;
                    ButtonGirls.Click += ButtonGirlsOnClick;
                    ButtonBoth.Click += ButtonBothOnClick;
                    AgeSeekBar.DragCompleted += AgeSeekBarOnDragCompleted;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Save and sent data and set new search 
        private void ButtonApplyOnClick(object sender, EventArgs e)
        {
            try
            {
                // check current state of a Switch (true or false).
                SwitchState = OnlineSwitch.Checked;

                UserDetails.AgeMin = AgeMin = (int)AgeSeekBar.GetSelectedMinValue();
                UserDetails.AgeMax = AgeMax = (int)AgeSeekBar.GetSelectedMaxValue();
                UserDetails.Gender = Gender;
                UserDetails.Location = Location;
                UserDetails.SwitchState = SwitchState; 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Location
        private void LocationLayoutOnClick(object sender, EventArgs e)
        {
            try
            {
                //string[] countriesArray = Context.Resources.GetStringArray(Resource.Array.countriesArray);
                var countriesArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (countriesArray != null) arrayAdapter.AddRange(countriesArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault()?.Name)));

                dialogList.Title(GetText(Resource.String.Lbl_Location));
                dialogList.Items(arrayAdapter);
                dialogList.PositiveText(GetText(Resource.String.Lbl_Close)).OnPositive(this);
                dialogList.AlwaysCallSingleChoiceCallback();
                dialogList.ItemsCallback(this).Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Select gender >> Both (0,1)
        private void ButtonBothOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));

                Gender = "4525,4526";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select gender >> Girls (1)
        private void ButtonGirlsOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                Gender = "4526";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select gender >> Man (0)
        private void ButtonManOnClick(object sender, EventArgs e)
        {
            try
            {
                //follow_button_profile_friends >> Un click
                //follow_button_profile_friends_pressed >> click
                ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                Gender = "4525";
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Select Age SeekBar >> Right #Max and >> Left #Min
        private void AgeSeekBarOnDragCompleted(object sender, EventArgs e)
        {
            try
            { 
                GC.Collect(GC.MaxGeneration);

                AgeMin = (int)AgeSeekBar.GetSelectedMinValue();
                AgeMax = (int)AgeSeekBar.GetSelectedMaxValue();

                AgeNumberTextView.Text = AgeMin + " - " + AgeMax;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region MaterialDialog

        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {

                }
                else if (p1 == DialogAction.Negative)
                {
                    p0.Dismiss();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                var id = itemId;
                var text = itemString.ToString();

                //string[] countriesArrayId = Context.Resources.GetStringArray(Resource.Array.countriesArray_id);
                var countriesArrayId = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries?.FirstOrDefault(a => a.Values.FirstOrDefault()?.Name == itemString.ToString())?.Keys.FirstOrDefault();
                var data = countriesArrayId;

                Location = data;
                LocationPlace.Text = text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion
         
        private void SetLocalData()
        {
            try
            {
                // check current state of a Switch (true or false).
                AgeSeekBar.SetSelectedMinValue(UserDetails.AgeMin);
                AgeSeekBar.SetSelectedMaxValue(UserDetails.AgeMax);

                OnlineSwitch.Checked = UserDetails.SwitchState;

                if (UserDetails.Gender == "4525,4526")
                {
                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonBoth.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonGirls.SetTextColor(Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == "4526")
                {
                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonGirls.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonMan.SetTextColor(Color.ParseColor("#444444"));
                }
                else if (UserDetails.Gender == "4525")
                {
                    ButtonMan.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends_pressed);
                    ButtonMan.SetTextColor(Color.ParseColor("#ffffff"));

                    ButtonBoth.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonBoth.SetTextColor(Color.ParseColor("#444444"));

                    ButtonGirls.SetBackgroundResource(Resource.Drawable.follow_button_profile_friends);
                    ButtonGirls.SetTextColor(Color.ParseColor("#444444"));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}