using System;
using System.Linq;
using Android.Content;
using Android.Gms.Ads;
using Android.Gms.Ads.Reward;
using Android.Support.V7.Widget;
using Android.Views;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;

namespace QuickDate.Helpers.Ads
{
    public static class AdsGoogle
    {
        private static int CountInterstitial;
        private static int CountRewarded;

        #region Interstitial

        private class AdmobInterstitial
        {
            private InterstitialAd Ad;

            public void ShowAd(Context context)
            {
                try
                {
                    Ad = new InterstitialAd(context);
                    Ad.AdUnitId = AppSettings.AdInterstitialKey;

                    var intlistener = new InterstitialAdListener(Ad);
                    intlistener.OnAdLoaded();
                    Ad.AdListener = intlistener;

                    var requestbuilder = new AdRequest.Builder();
                    requestbuilder.AddTestDevice(UserDetails.AndroidId);
                    Ad.LoadAd(requestbuilder.Build());
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private class InterstitialAdListener : AdListener
        {
            private readonly InterstitialAd Ad;

            public InterstitialAdListener(InterstitialAd ad)
            {
                Ad = ad;
            }

            public override void OnAdLoaded()
            {
                base.OnAdLoaded();

                if (Ad.IsLoaded)
                    Ad.Show();
            }
        }


        public static void Ad_Interstitial(Context context)
        {
            try
            {
                if (AppSettings.ShowAdmobInterstitial)
                {
                    var isPro = ListUtils.MyUserInfo.FirstOrDefault()?.IsPro ?? "0";
                    if (isPro == "0")
                    {
                        if (CountInterstitial == AppSettings.ShowAdmobInterstitialCount)
                        {
                            CountInterstitial = 0;
                            AdmobInterstitial ads = new AdmobInterstitial();
                            ads.ShowAd(context);
                        }

                        CountInterstitial++;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        //Rewarded Video >>
        //===================================================

        #region Rewarded

        private class AdmobRewardedVideo : AdListener, IRewardedVideoAdListener
        {
            private IRewardedVideoAd Rad;

            public void ShowAd(Context context)
            {
                try
                {
                    // Use an activity context to get the rewarded video instance.
                    Rad = MobileAds.GetRewardedVideoAdInstance(context);
                    Rad.RewardedVideoAdListener = this;

                    OnRewardedVideoAdLoaded();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    base.OnAdLoaded();

                    OnRewardedVideoAdLoaded();

                    if (Rad.IsLoaded)
                        Rad.Show();
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }

            public void OnRewarded(IRewardItem reward)
            {
                //Toast.MakeText(Application.Context, "onRewarded! currency: " + reward.Type + "  amount: " + reward.Amount , ToastLength.Short).Show();

                if (Rad.IsLoaded)
                    Rad.Show();
            }


            public void OnRewardedVideoAdClosed()
            {

            }

            public void OnRewardedVideoAdFailedToLoad(int errorCode)
            {
                //Toast.MakeText(Application.Context, "No ads currently available", ToastLength.Short).Show();
            }

            public void OnRewardedVideoAdLeftApplication()
            {

            }

            public void OnRewardedVideoAdLoaded()
            {
                try
                {
                    if (!Rad.IsLoaded)
                    {
                        Rad.LoadAd(AppSettings.AdRewardVideoKey, new AdRequest.Builder().Build());
                        Rad.Show();
                    }


                    //Bundle extras = new Bundle();
                    //extras.PutBoolean("_noRefresh", true);

                    //var requestBuilder = new AdRequest.Builder();
                    //requestBuilder.AddTestDevice(UserDetails.AndroidId);
                    //requestBuilder.AddNetworkExtrasBundle(new AdMobAdapter().Class, extras);
                    //Rad.UserId = Application.Context.GetString(Resource.String.admob_app_id);
                    //Rad.LoadAd(AppSettings.AdRewardVideoKey, requestBuilder.Build());
                    //Rad.Show();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public void OnRewardedVideoAdOpened()
            {

            }

            public void OnRewardedVideoCompleted()
            {

            }

            public void OnRewardedVideoStarted()
            {

            }
        }

        public static void Ad_RewardedVideo(Context context)
        {
            try
            {
                if (AppSettings.ShowAdmobRewardVideo)
                {
                    var isPro = ListUtils.MyUserInfo.FirstOrDefault()?.IsPro ?? "0";
                    if (isPro == "0")
                    {
                        if (CountRewarded == AppSettings.ShowAdmobRewardedVideoCount)
                        {
                            CountRewarded = 0;
                            AdmobRewardedVideo ads = new AdmobRewardedVideo();
                            ads.ShowAd(context);
                        }

                        CountRewarded++;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion


        //Banner >>
        //=================================================== 
        public static void InitAdView(AdView mAdView, RecyclerView mRecycler)
        {
            try
            {
                if (AppSettings.ShowAdmobBanner)
                {
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    if (dataUser?.IsPro == "0")
                    {
                        mAdView.Visibility = ViewStates.Visible;
                        var adRequest = new AdRequest.Builder();
                        adRequest.AddTestDevice(UserDetails.AndroidId);
                        mAdView.LoadAd(adRequest.Build());
                        mAdView.AdListener = new MyAdListener(mAdView, mRecycler);
                    }
                    else
                    {
                        mAdView.Pause();
                        mAdView.Visibility = ViewStates.Gone;
                        Methods.SetMargin(mAdView, 0, 0, 0, 0);

                        if (mRecycler != null) Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                    } 
                }
                else
                {
                    mAdView.Pause();
                    mAdView.Visibility = ViewStates.Gone;
                    Methods.SetMargin(mAdView, 0, 0, 0, 0);
                    if (mRecycler != null) Methods.SetMargin(mRecycler, 0, 0, 0, 0);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private class MyAdListener : AdListener
        {
            private readonly AdView MAdView;
            private readonly RecyclerView MRecycler;
            public MyAdListener(AdView mAdView, RecyclerView mRecycler)
            {
                MAdView = mAdView;
                MRecycler = mRecycler;
            }

            public override void OnAdFailedToLoad(int p0)
            {
                try
                {
                    MAdView.Visibility = ViewStates.Gone;
                    if (MRecycler != null) Methods.SetMargin(MRecycler, 0, 0, 0, 0);
                    base.OnAdFailedToLoad(p0);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            public override void OnAdLoaded()
            {
                try
                {
                    MAdView.Visibility = ViewStates.Visible;
                    base.OnAdLoaded();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

    }
}