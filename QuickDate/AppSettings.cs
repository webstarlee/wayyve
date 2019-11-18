//###############################################################
// Author >> Elin Doughouz 
// Copyright (c) PixelPhoto 15/07/2018 All Right Reserved
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// Follow me on facebook >> https://www.facebook.com/Elindoughous
//=========================================================

using Android.Graphics;

namespace QuickDate
{
    public static class AppSettings
    {
        //Main Settings >>>>>
        //*********************************************************
        public static string TripleDesAppServiceProvider = "NFYnA2qriwLLUe74dlNM98fXF7R68wTcf47yFL3KTLK2m5TeQPL1e5Qh3lZo8h6ecsYHMMmQNXWUjmicqWGRg0XzY1UkEI0GBqmEJNToaBeHDenmFJBVpizwoT2XOwOvIg5S8IL88Src/J5etX/OHL6G07l0ki25K5yf2nSNztZk8NiExlYWpKafsEo/9ypcRtY7FV574jUSyw7dZjYgKJPNCTecbHna70okrNAeAyaH3/gIMgizk1mIX5Mwafal";

        public static string Version = "1.4";
        public static string ApplicationName = "Wayyve";

        //Main Colors >>
        //*********************************************************
        public static string MainColor = "#a33596";
        public static string StartColor = MainColor;
        public static string EndColor = "#63245c";
        public static Color TitleTextColor = Color.Black;//#New

        //Language Settings >> http://www.lingoes.net/en/translator/langcode.htm
        //*********************************************************
        public static bool FlowDirectionRightToLeft = false;
        public static string Lang = ""; //Default language ar_AE

        //Notification Settings >>
        //*********************************************************
        public static bool ShowNotification = true;
        public static string OneSignalAppId = "0eeb44be-0ee2-422c-99b7-d338c59c5906"; //#New

        //********************************************************* 

        //Add Animation Image User
        //*********************************************************
        public static bool EnableAddAnimationImageUser = false;
         
        //Set Theme Full Screen App
        //*********************************************************
        public static bool EnableFullScreenApp = false;

        //Social Logins >>
        //If you want login with facebook or google you should change id key in the analytic.xml file or AndroidManifest.xml
        //Facebook >> ../values/analytic.xml  
        //Google >> ../Properties/AndroidManifest.xml .. line 42
        //*********************************************************
        public static bool ShowFacebookLogin = true;


        //ADMOB >> Please add the code ads in the Here and analytic.xml 
        //*********************************************************
        public static bool ShowAdmobBanner = true;
        public static bool ShowAdmobInterstitial = true;
        public static bool ShowAdmobRewardVideo = true;
         
        public static string AdInterstitialKey = "ca-app-pub-5135691635931982/6657648824";
        public static string AdRewardVideoKey = "ca-app-pub-5135691635931982/7559666953";

        //Three times after entering the ad is displayed
        public static int ShowAdmobInterstitialCount = 3;
        public static int ShowAdmobRewardedVideoCount = 3;
         
        //########################### 

        //Last_Messages Page >>
        ///********************************************************* 
        public static bool RunSoundControl = true;
        public static int RefreshChatActivitiesSeconds = 6000; // 6 Seconds
        public static int MessageRequestSpeed = 3000; // 3 Seconds
                  
        //Set Theme Tab
        //*********************************************************
        public static bool SetTabColoredTheme = false;
        public static bool SetTabDarkTheme = false;

        public static string TabColoredColor = MainColor;
        public static bool SetTabIsTitledWithText = false;

        //Bypass Web Errors  
        //*********************************************************
        public static bool TurnTrustFailureOnWebException = true;
        public static bool TurnSecurityProtocolType3072On = true;

        //Show custom error reporting page
        public static bool RenderPriorityFastPostLoad = true;

        //New Version 
        //*********************************************************

        //Trending 
        //*********************************************************
        public static bool ShowTrending = true;//#New 
         
        public static bool ShowFilterBasic = true;//#New
        public static bool ShowFilterLooks = true;//#New
        public static bool ShowFilterBackground = true;//#New
        public static bool ShowFilterLifestyle = true;//#New
        public static bool ShowFilterMore = true;//#New
         
        //true = Show Users 2 Column and use CardView
        //false = Show Users 3 Column  and image Circle 
        public static bool ShowUsersAsCards = true;
         
        //*********************************************************

        //Premium system
        public static bool PremiumSystemEnabled = true;

        //Phone Validation system
        public static bool ValidationEnabled = true;
        public static bool CompressImage = false;//#New
        public static int AvatarSize = 60; //#New
        public static int ImageSize = 200; //#New  

        //Error Report Mode
        //*********************************************************
        public static bool SetApisReportMode = true;//#New 
         
        public static bool ShowWalkTroutPage = true;//#New

        //Payment System (ShowPaymentCardPage >> Paypal & Stripe ) (ShowLocalBankPage >> Local Bank ) 
        //*********************************************************
        public static bool ShowPaypal = true;//#New
        public static bool ShowCreditCard = true;//#New
        public static bool ShowBankTransfer = true;//#New

         
        //*********************************************************
        public static bool EnableAppFree = false;//#New 
    }
} 