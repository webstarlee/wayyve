using System;
using Android.App;
using Android.Content;
using Android.Preferences;
using QuickDate.SQLite;

namespace QuickDate.Activities.SettingsUser
{
    public static class MainSettings
    {
        public static ISharedPreferences SharedData;
        private static ISharedPreferences SharedTimer, SharedTime;
        private static readonly string PrefsTimer = "MyPrefsTimer";
        private static readonly string PrefsTime = "MyPrefsTime";

        public static void Init()
        {
            try
            {
                SqLiteDatabase dbDatabase = new SqLiteDatabase();
                dbDatabase.CheckTablesStatus();
                SharedData = PreferenceManager.GetDefaultSharedPreferences(Application.Context);

                SharedTimer = Application.Context.GetSharedPreferences(PrefsTimer, FileCreationMode.Private); 
                SharedTime = Application.Context.GetSharedPreferences(PrefsTime, FileCreationMode.Private); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}