using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Widget;
using QuickDate.Activities.Default;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;

namespace QuickDate.Activities
{
    [Activity(MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/SplashScreenTheme", NoHistory = true, ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class SplashScreenActivity : Activity
    {
        #region Variables Basic

        private SqLiteDatabase DbDatabase;

        #endregion

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);
                 
                DbDatabase = new SqLiteDatabase();
                DbDatabase.CheckTablesStatus();

                ClassMapper.SetMappers();
                 
                if (AppSettings.Lang != "")
                { 
                    LangController.SetApplicationLang(this, AppSettings.Lang);
                }
                else
                {
                    UserDetails.LangName = Resources.Configuration.Locale.DisplayLanguage.ToLower();
                    LangController.SetAppLanguage(this);
                }

                if (AppSettings.ShowAdmobBanner || AppSettings.ShowAdmobInterstitial || AppSettings.ShowAdmobRewardVideo)
                    MobileAds.Initialize(this, GetString(Resource.String.admob_app_id)); 
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
                Task startupWork = new Task(FirstRunExcite);
                startupWork.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void FirstRunExcite()
        {
            try
            {
                DbDatabase.GetSettings();
                 
                var result = DbDatabase.Get_data_Login_Credentials();
                if (result != null)
                {
                    Current.AccessToken = result.AccessToken; 
                    switch (result.Status)
                    {
                        case "Active":
                        case "Pending":
                            StartActivity(new Intent(this, typeof(HomeActivity)));
                            break;
                        default:
                            StartActivity(new Intent(this, typeof(FirstActivity)));
                            break;
                    }
                }
                else
                {
                    StartActivity(new Intent(this, typeof(FirstActivity)));
                }
                DbDatabase.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Toast.MakeText(this, e.Message, ToastLength.Short).Show();
            }
        }
    }
}