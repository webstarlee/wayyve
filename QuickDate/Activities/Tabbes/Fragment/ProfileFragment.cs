using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using ME.Itangqi.Waveloadingview;
using QuickDate.Activities.Blogs;
using QuickDate.Activities.Favorite;
using QuickDate.Activities.InviteFriends;
using QuickDate.Activities.MyProfile;
using QuickDate.Activities.Premium;
using QuickDate.Activities.SettingsUser;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Classes.Authorization;
using QuickDateClient.Requests;
using Console = System.Console;

namespace QuickDate.Activities.Tabbes.Fragment
{
    public class ProfileFragment : Android.Support.V4.App.Fragment
    {
        #region Variables Basic

        private HomeActivity GlobalContext;
        private TextView Username, XtBoostMe, TxtUpgrade;
        public TextView WalletNumber;
        public ImageView ProfileImage;
        private CircleButton EditButton, SettingsButton, BoostButton;
        private RelativeLayout WalletButton, PopularityButton, UpgradeButton, FavoriteButton, HelpButton, InviteButton, BlogsButton;
        private LinearLayout HeaderSection;
        public FavoriteUserFragment FavoriteFragment;
        public TimerTime Time;
        private WaveLoadingView MWaveLoadingView;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (HomeActivity)Activity;
        }
          
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.TProfileLayout, container, false);

                //Get Value 
                InitComponent(view); 
              
                WalletButton.Click += WalletButtonOnClick;
                PopularityButton.Click += PopularityButtonOnClick;
                UpgradeButton.Click += UpgradeButtonOnClick;
                EditButton.Click += EditButtonOnClick;
                ProfileImage.Click += ProfileImageOnClick;
                SettingsButton.Click += SettingsButtonOnClick;
                FavoriteButton.Click += FavoriteButtonOnClick;
                HelpButton.Click += HelpButtonOnClick;
                InviteButton.Click += InviteButtonOnClick;
                BlogsButton.Click += BlogsButtonOnClick;
                BoostButton.Click += BoostButtonOnClick;
                                 
                return view;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
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

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                ProfileImage = view.FindViewById<ImageView>(Resource.Id.Iconimage2);
                Username = view.FindViewById<TextView>(Resource.Id.username);
                WalletNumber = view.FindViewById<TextView>(Resource.Id.walletnumber);
                TxtUpgrade = view.FindViewById<TextView>(Resource.Id.upgradeText);
                XtBoostMe = view.FindViewById<TextView>(Resource.Id.tv_Boost);
                EditButton = view.FindViewById<CircleButton>(Resource.Id.EditButton);
                SettingsButton = view.FindViewById<CircleButton>(Resource.Id.SettingsButton);
                BoostButton = view.FindViewById<CircleButton>(Resource.Id.BoostButton);
                WalletButton = view.FindViewById<RelativeLayout>(Resource.Id.walletSection);
                PopularityButton = view.FindViewById<RelativeLayout>(Resource.Id.popularitySection);
                UpgradeButton = view.FindViewById<RelativeLayout>(Resource.Id.upgradeSection);
                FavoriteButton = view.FindViewById<RelativeLayout>(Resource.Id.StFirstLayout);
                InviteButton = view.FindViewById<RelativeLayout>(Resource.Id.StsecoundLayout);
                BlogsButton = view.FindViewById<RelativeLayout>(Resource.Id.StfourthLayout);
                HelpButton = view.FindViewById<RelativeLayout>(Resource.Id.StthirdLayout);
                HeaderSection = view.FindViewById<LinearLayout>(Resource.Id.headerSection);

                MWaveLoadingView = (WaveLoadingView)view.FindViewById(Resource.Id.waveLoadingView);
                MWaveLoadingView.Visibility = ViewStates.Gone;

                BoostButton.Tag = "Off";

                GlideImageLoader.LoadImage(Activity, UserDetails.Avatar, ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);
                 
                if (AppSettings.EnableAppFree)
                {
                    WalletButton.Visibility = ViewStates.Invisible;
                    UpgradeButton.Visibility = ViewStates.Invisible; 
                }
                 
                if (!AppSettings.PremiumSystemEnabled)
                    Activity.RunOnUiThread(()=>
                    {
                        UpgradeButton.Visibility = ViewStates.Invisible;
                        UpgradeButton.Enabled = false;
                    });

                if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)
                    return;

                Activity.Window.ClearFlags(WindowManagerFlags.TranslucentStatus);
                Activity.Window.AddFlags(WindowManagerFlags.DrawsSystemBarBackgrounds);
                Activity.Window.SetStatusBarColor(Color.ParseColor(AppSettings.MainColor));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Open Blogs
        private void BlogsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(BlogsActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open edit my info
        private void EditButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Activity, typeof(EditProfileActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Upgrade
        private void UpgradeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var window = new PopupController(Activity);
                window.DisplayPremiumWindow();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Popularity >> Very Low
        private void PopularityButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(PopularityActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Wallet
        private void WalletButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                var window = new PopupController(Activity);
                window.DisplayCreditWindow("credits");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Boost me
        private async void BoostButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (dataUser != null)
                {
                    if (BoostButton.Tag.ToString() == "Off")
                    {
                        BoostButton.Tag = "Run";

                        if (!AppSettings.EnableAppFree)
                        {
                            string myBalance = dataUser.Balance ?? "0.00";
                            if (!string.IsNullOrEmpty(myBalance) && myBalance != "0.00")
                            {
                                //sent new api
                                (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("boost").ConfigureAwait(false);
                                if (apiStatus == 200)
                                {
                                    if (respond is AmountObject result)
                                    {
                                        Activity.RunOnUiThread(() =>
                                        {
                                            try
                                            {
                                                myBalance = result.CreditAmount.ToString();
                                                WalletNumber.Text = result.CreditAmount.ToString();

                                                var timeBoost = ListUtils.SettingsSiteList.FirstOrDefault()?.BoostExpireTime ?? "4";
                                                var timeBoostMilliseconds = Methods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                                                dataUser.BoostedTime = Convert.ToInt32(timeBoostMilliseconds);
                                                dataUser.IsBoosted = 1;

                                                GetMyInfoData();
                                            }
                                            catch (Exception exception)
                                            {
                                                Console.WriteLine(exception);
                                            }
                                        });
                                    }
                                }
                                else Methods.DisplayReportResult(Activity, respond);
                            }
                            else
                            {
                                var window = new PopupController(Activity);
                                window.DisplayCreditWindow("credits");
                            }
                        }
                        else
                        {
                            //sent new api
                            (int apiStatus, var respond) = await RequestsAsync.Users.ManagePopularityAsync("boost").ConfigureAwait(false);
                            if (apiStatus == 200)
                            {
                                if (respond is AmountObject result)
                                {
                                    Activity.RunOnUiThread(() =>
                                    {
                                        try
                                        {
                                            //myBalance = result.CreditAmount.ToString();
                                            WalletNumber.Text = result.CreditAmount.ToString();

                                            var timeBoost = ListUtils.SettingsSiteList.FirstOrDefault()?.BoostExpireTime ?? "4";
                                            var timeBoostMilliseconds = Methods.Time.ConvertMinutesToMilliseconds(Convert.ToDouble(timeBoost));
                                            dataUser.BoostedTime = Convert.ToInt32(timeBoostMilliseconds);
                                            dataUser.IsBoosted = 1;

                                            GetMyInfoData();
                                        }
                                        catch (Exception exception)
                                        {
                                            Console.WriteLine(exception);
                                        }
                                    });
                                }
                            }
                            else Methods.DisplayReportResult(Activity, respond);
                        } 
                    }
                    else
                    {
                        Toast.MakeText(Context,GetText(Resource.String.Lbl_YourBoostExpireInMinutes),ToastLength.Long).Show();
                    } 
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Settings
        private void SettingsButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(SettingsActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //Invite Friends
        private void InviteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                StartActivity(new Intent(Context, typeof(InviteFriendsActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Help
        private void HelpButtonOnClick(object sender, EventArgs e)
        {
            try
            { 
                var intent = new Intent(Context, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                Activity.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Favorite
        private void FavoriteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                FavoriteFragment = new FavoriteUserFragment();
                GlobalContext.FragmentBottomNavigator.DisplayFragment(FavoriteFragment);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        public void GetMyInfoData()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();
                    sqlEntity.Dispose();
                }
                PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(Activity, UserDetails.UserId.ToString()) });
                 
                var data = ListUtils.MyUserInfo.FirstOrDefault();
                if (data != null)
                {
                    GlideImageLoader.LoadImage(Activity,data.Avater, ProfileImage, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                    Username.Text = QuickDateTools.GetNameFinal(data);
                     
                    WalletNumber.Text = data.Balance.Replace(".00", "");

                    if (data.IsPro == "1")
                    {
                        #region UpgradeButton >> ViewStates.Gone

                        //UpgradeButton.Visibility = ViewStates.Gone;

                        //LinearLayout.LayoutParams layoutParam1 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, 100, 1f);
                        //LinearLayout.LayoutParams layoutParam2 = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, 100, 1f);

                        //((ViewGroup)WalletButton.Parent)?.RemoveView(WalletButton);
                        //((ViewGroup)PopularityButton.Parent)?.RemoveView(PopularityButton);
                        //((ViewGroup)UpgradeButton.Parent)?.RemoveView(UpgradeButton);

                        //HeaderSection.WeightSum = 2;

                        //layoutParam1.TopMargin = 20;
                        //layoutParam2.TopMargin = 20;
                        //layoutParam2.MarginStart = 20;

                        //WalletButton.LayoutParameters = layoutParam1;
                        //PopularityButton.LayoutParameters = layoutParam2;

                        //HeaderSection.AddView(WalletButton, layoutParam1);
                        //HeaderSection.AddView(PopularityButton, layoutParam2); 

                        #endregion

                        switch (data.ProType)
                        {
                            case "1":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Weekly);
                                break;
                            case "2":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Monthly);
                                break;
                            case "3":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Yearly);
                                break;
                            case "4":
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Lifetime);
                                break;
                            default:
                                TxtUpgrade.Text = GetText(Resource.String.Lbl_Upgrade);
                                break;
                        }
                    }
                    else
                    {
                        if (AppSettings.PremiumSystemEnabled)
                        {
                            TxtUpgrade.Text = GetText(Resource.String.Lbl_Upgrade);
                            UpgradeButton.Visibility = ViewStates.Visible;
                        }

                    }
                     
                    if (data.BoostedTime != 0 && data.BoostedTime > 0)
                    {
                        var timeBoost = ListUtils.SettingsSiteList.FirstOrDefault()?.BoostExpireTime ?? "4";
                        var timeBoostSeconds = Methods.Time.ConvertMinutesToSeconds(Convert.ToDouble(timeBoost)); //240

                        
                        double progressStart;
                        double progress = 100 / timeBoostSeconds; //0.4

                        if (Time == null)
                        { 
                           double progressPlus = 100 / timeBoostSeconds;
                             
                            Time = new TimerTime();
                            TimerTime.TimerCount = Time.GetTimer();
                            var plus1 = progressPlus;
                            TimerTime.TimerCount.Elapsed += (sender, args) =>   
                            {
                                var plus = plus1;
                                Activity.RunOnUiThread(() =>
                                {
                                    try
                                    {
                                        var (minutes, seconds) = Time.TimerCountOnElapsed();
                                        if ((minutes == "" || minutes == "0") && (seconds == "" || seconds == "0"))
                                        {
                                            Time.SetStopTimer();
                                            Time = null;
                                            TimerTime.TimerCount = null;

                                            data.BoostedTime = 0;
                                            XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);
                                            SetStopAnimationPopularity();
                                            progress = 0;
                                            progressStart = 0;
                                            MWaveLoadingView.CancelAnimation();

                                            BoostButton.Tag = "Off";
                                        }
                                        else
                                        {
                                            XtBoostMe.Text = minutes + ":" + seconds;
                                            progress += plus;

                                            progressStart = Math.Round(progress,MidpointRounding.AwayFromZero); 
                                            MWaveLoadingView.ProgressValue = Convert.ToInt32(progressStart);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e);
                                    }
                                });
                            }; 
                        }

                        string countTime = Time.CheckCountTime(Convert.ToInt32(data.BoostedTime));
                        if (countTime != "0:0" && !countTime.Contains("-") && !string.IsNullOrEmpty(countTime))
                        { 
                            int min = Convert.ToInt32(countTime.Split(":")[0]); 
                            int sec = Convert.ToInt32(countTime.Split(":")[1]); 
                            Time.SetMinutes(min);
                            Time.SetSeconds(sec);
                            Time.SetStartTimer();
                            XtBoostMe.Text = countTime;

                            var minSeconds = Methods.Time.ConvertMinutesToSeconds(Convert.ToDouble(min));

                            //start in here  
                            progress = (timeBoostSeconds - minSeconds) * 100 / timeBoostSeconds; 

                            SetStartAnimationPopularity(); 
                        }
                        else
                        {
                            Time.SetStopTimer();
                            Time = null;
                            TimerTime.TimerCount = null;

                            XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe); 
                            SetStopAnimationPopularity();

                            BoostButton.Tag = "Off";
                        }
                    }
                    else
                    {
                        if (Time != null)
                        {
                            Time.SetStopTimer();
                            Time = null;
                            TimerTime.TimerCount = null;

                            XtBoostMe.Text = Context.GetText(Resource.String.Lbl_BoostMe);
                            SetStopAnimationPopularity();

                            BoostButton.Tag = "Off";
                        }
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update Avatar Async
        private void ProfileImageOnClick(object sender, EventArgs e)
        {
            try
            {
                GlobalContext.OpenDialogGallery();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void SetStartAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Invisible;
                MWaveLoadingView.Visibility = ViewStates.Visible;
                MWaveLoadingView.StartAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SetStopAnimationPopularity()
        {
            try
            {
                BoostButton.Visibility = ViewStates.Visible;
                MWaveLoadingView.Visibility = ViewStates.Gone;

                MWaveLoadingView?.CancelAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        } 
    }
}