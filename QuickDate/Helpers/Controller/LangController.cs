using System;
using System.Globalization;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Java.Util;

namespace QuickDate.Helpers.Controller
{
    public class LangController : ContextWrapper
    {
        private Context Context;
        private static string Language = "";

        protected LangController(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public LangController(Context context) : base(context)
        {
            Context = context;
        }

        public static Context SetAppLanguage(Context activityContext)
        {
            try
            {
                var res = activityContext.Resources; // Get the string 

                Configuration config = activityContext.Resources.Configuration;
                Locale locale = null;

                locale = activityContext.Resources.Configuration.Locale;

                AppSettings.Lang = locale.Language;

                //locale = config.Locales.Get(0);

                Configuration conf = res.Configuration;
                conf.SetLocale(locale);

                Locale.Default = locale;

                if ((int)Build.VERSION.SdkInt > 17)
                    conf.SetLayoutDirection(locale);

                DisplayMetrics dm = res.DisplayMetrics;

                //   res.UpdateConfiguration(conf, dm);

                if (config.Locale.Language.Contains("ar"))
                {
                    AppSettings.Lang = "ar";
                    AppSettings.FlowDirectionRightToLeft = true;
                }
                else
                {
                    AppSettings.FlowDirectionRightToLeft = false;
                }

                if ((int)Build.VERSION.SdkInt >= 24)
                {
                    LocaleList localeList = new LocaleList(locale);
                    LocaleList.Default = localeList;
                    conf.Locales = localeList;

                    //Locale.SetDefault(Locale.Category.Display, locale);
                    activityContext = activityContext.CreateConfigurationContext(conf);

                    return activityContext;
                }

                conf.Locale = locale;
                res.UpdateConfiguration(conf, dm);
                return activityContext;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return activityContext;
            }
        }

        public static ContextWrapper Wrap(Context context, string language)
        {
            try
            {
                Language = language;

                Configuration config = context.Resources.Configuration;
                Locale sysLocale = null;

                sysLocale = config.Locales.Get(0);

                if (!language.Equals("") && !sysLocale.Language.Equals(language))
                {
                    sysLocale = new Locale(language);
                    Locale.Default = sysLocale;
                }
                CultureInfo myCulture = new CultureInfo(language);
                CultureInfo.DefaultThreadCurrentCulture = myCulture;
                config.SetLocale(sysLocale);

                var ss = context.Resources.Configuration.Locale;

                //SharedPref.SharedData.Edit().PutString("Lang_key", language).Commit();

                //context = context.CreateConfigurationContext(config);
                context.Resources.UpdateConfiguration(config, null);

                return new LangController(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new LangController(context);
            }
        }

        public static void SetDefaultAppSettings()
        {
            try
            {
                //Shared_Data.Edit().PutString("Lang_key", "Auto").Commit();
                if (AppSettings.Lang != "")
                {
                    if (AppSettings.Lang == "ar")
                    {
                        //SharedPref.SharedData.Edit().PutString("Lang_key", "ar").Commit();
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirectionRightToLeft = true;
                    }
                    else
                    {
                        // SharedPref.SharedData.Edit().PutString("Lang_key", AppSettings.Lang).Commit();
                        AppSettings.FlowDirectionRightToLeft = false;
                    }
                }
                else
                {
                    AppSettings.FlowDirectionRightToLeft = false;

                    //var Lang = SharedPref.SharedData.GetString("Lang_key", AppSettings.Lang);
                    //if (Lang == "ar")
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", "ar").Commit();
                    //    AppSettings.Lang = "ar";
                    //    AppSettings.FlowDirectionRightToLeft = true;
                    //}
                    //else if (Lang == "Auto")
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", "Auto").Commit();
                    //}
                    //else
                    //{
                    //    SharedPref.SharedData.Edit().PutString("Lang_key", Lang).Commit();
                    //}
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public static void SetApplicationLang(Context context, string lang)
        {
            try
            {
                var config = new Configuration();
                AppSettings.Lang = lang;

                if (string.IsNullOrEmpty(lang))
                {
                    if (lang == "Auto" || lang == "")
                    {
                        config.Locale = Locale.Default;
                        Language = config.Locale.Language;
                    }
                    else
                    {
                        config.Locale = Locale.Default = new Locale(lang);
                    }

                    if (config.Locale.Language.Contains("ar"))
                    {
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirectionRightToLeft = true;
                    }
                    else
                    {
                        AppSettings.FlowDirectionRightToLeft = false;
                    }
                }
                else
                {
                    config.Locale = Locale.Default = new Locale(lang);
                    context.Resources.Configuration.Locale = Locale.Default = new Locale(lang);
                    //SharedPref.SharedData.Edit().PutString("Lang_key", lang).Commit();

                    if (lang.Contains("ar"))
                    {
                        AppSettings.Lang = "ar";
                        AppSettings.FlowDirectionRightToLeft = true;
                    }
                    else
                    {
                        AppSettings.Lang = lang;
                    }
                }

                //Shared_Data.Edit().PutString("Lang_key", lang).Commit();
                //context.Resources.UpdateConfiguration(config, context.Resources.DisplayMetrics);

                SetDefaultAppSettings();

                Wrap(context, AppSettings.Lang);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


    }
}