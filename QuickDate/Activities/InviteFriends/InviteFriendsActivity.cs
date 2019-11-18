using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Plugin.Share;
using Plugin.Share.Abstractions;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.InviteFriends
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class InviteFriendsActivity : AppCompatActivity
    {
        #region Variables Basic

        private TextView TxtSubTitle;
        private TextView CircleCopy , CircleTextInvite, CircleSocialInvite;
        private TextView IconCopy, IconTextInvite, IconSocialInvite;
        private TextView IconGo1, IconGo2, IconGo3;
        private RelativeLayout CopyLayouts,  TextLayouts, SocialLayouts;
        private AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                // Create your application here
                SetContentView(Resource.Layout.InviteFriendsLayout);

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
                MAdView?.Resume();
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
                MAdView?.Pause();
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

        protected override void OnDestroy()
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
                TxtSubTitle = FindViewById<TextView>(Resource.Id.subTitle);
                CircleCopy = FindViewById<TextView>(Resource.Id.circleCopy);
                CircleTextInvite = FindViewById<TextView>(Resource.Id.circleTextInvite);
                CircleSocialInvite = FindViewById<TextView>(Resource.Id.circleSocialInvite);
                IconCopy = FindViewById<TextView>(Resource.Id.IconCopy);
                IconTextInvite = FindViewById<TextView>(Resource.Id.IconTextInvite);
                IconSocialInvite = FindViewById<TextView>(Resource.Id.IconSocialInvite);
                IconGo1 = FindViewById<TextView>(Resource.Id.iconGo1);
                IconGo2 = FindViewById<TextView>(Resource.Id.iconGo2);
                IconGo3 = FindViewById<TextView>(Resource.Id.iconGo3);

                CopyLayouts = FindViewById<RelativeLayout>(Resource.Id.copyLayouts);
                TextLayouts = FindViewById<RelativeLayout>(Resource.Id.textLayouts);
                SocialLayouts = FindViewById<RelativeLayout>(Resource.Id.socialLayouts);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, CircleCopy, IonIconsFonts.Record);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, CircleTextInvite, IonIconsFonts.Record);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, CircleSocialInvite, IonIconsFonts.Record);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconGo1, FontAwesomeIcon.AngleRight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconGo2, FontAwesomeIcon.AngleRight);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, IconGo3, FontAwesomeIcon.AngleRight);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconCopy, IonIconsFonts.Link);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconTextInvite, IonIconsFonts.AndroidTextsms);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconSocialInvite, IonIconsFonts.Share);

                CircleCopy.SetTextColor(Color.ParseColor("#8e24aa"));
                CircleTextInvite.SetTextColor(Color.ParseColor("#3949ab"));
                CircleSocialInvite.SetTextColor(Color.ParseColor("#1E88E5"));
                 
                TxtSubTitle.Text = GetText(Resource.String.Lbl_ShareThe) + " " + AppSettings.ApplicationName + " " + GetText(Resource.String.Lbl_byInvitingFriends);


                TextLayouts.Visibility = ViewStates.Gone;

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null);
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
                    toolbar.Title = GetText(Resource.String.Lbl_InviteFriends);
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
                    CopyLayouts.Click += CopyLayoutsOnClick;
                    TextLayouts.Click += TextLayoutsOnClick;
                    SocialLayouts.Click += SocialLayoutsOnClick;
                }
                else
                {
                    CopyLayouts.Click -= CopyLayoutsOnClick;
                    TextLayouts.Click -= TextLayoutsOnClick;
                    SocialLayouts.Click -= SocialLayoutsOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Share
        private async void SocialLayoutsOnClick(object sender, EventArgs e)
        {
            try
            {
                if (!CrossShare.IsSupported)
                    return;
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                await CrossShare.Current.Share(new ShareMessage
                {
                    Title = dataUser?.Username,
                    Text = "",
                    Url = UserDetails.Url
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Read Contacts Numbers
        private void TextLayoutsOnClick(object sender, EventArgs e)
        {
            try
            {
                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(new Intent(this, typeof(InviteContactActivity)));
                }
                else
                {
                    if (CheckSelfPermission(Manifest.Permission.ReadContacts) == Permission.Granted && CheckSelfPermission(Manifest.Permission.ReadPhoneNumbers) == Permission.Granted)
                        StartActivity(new Intent(this, typeof(InviteContactActivity)));
                    else
                    new PermissionsController(this).RequestPermission(101); 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        // Copy link
        private void CopyLayoutsOnClick(object sender, EventArgs e)
        {
            try
            {
                ClipboardManager clipboard = (ClipboardManager)GetSystemService(ClipboardService);
                ClipData clip = ClipData.NewPlainText("clipboard", UserDetails.Url);
                clipboard.PrimaryClip = clip;

                Toast.MakeText(this, GetText(Resource.String.Lbl_Text_copied), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Permissions   

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 101)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        StartActivity(new Intent(this, typeof(InviteContactActivity)));
                    }
                    else
                    {
                        Toast.MakeText(this, GetText(Resource.String.Lbl_Permission_is_denied), ToastLength.Long).Show();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion
    }
}