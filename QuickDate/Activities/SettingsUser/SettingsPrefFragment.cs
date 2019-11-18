using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Preferences;
using Android.Views;
using Android.Widget;
using QuickDate.Activities.SettingsUser.General;
using QuickDate.Activities.SettingsUser.Support;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient;
using QuickDateClient.Requests;
using Preference = Android.Support.V7.Preferences.Preference;

namespace QuickDate.Activities.SettingsUser
{
    public class SettingsPrefFragment : PreferenceFragmentCompat, ISharedPreferencesOnSharedPreferenceChangeListener, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private Preference MyAccountPref, PasswordPref, SocialLinksPref, BlockedUsersPref, StoragePref, HelpPref, AboutPref, DeleteAccountPref, LogoutPref;
        private SwitchPreferenceCompat ChatOnlinePref, PSearchEnginesPref, PRandomUsersPref, PFindMatchPagePref;
        private readonly Activity ActivityContext;
        private int ChatOnline, PSearchEngines, PRandomUsers, PFindMatchPage;
       
        #endregion

        #region General

        public SettingsPrefFragment(Activity context)
        {
            try
            {
                ActivityContext = context;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                // create ContextThemeWrapper from the original Activity Context with the custom theme
                Context contextThemeWrapper =  new ContextThemeWrapper(Activity, Resource.Style.SettingsTheme);

                // clone the inflater using the ContextThemeWrapper
                LayoutInflater localInflater = inflater.CloneInContext(contextThemeWrapper);

                View view = base.OnCreateView(localInflater, container, savedInstanceState);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
        {
            try
            {
                AddPreferencesFromResource(Resource.Xml.SettingsPrefs);

                InitComponent();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override PreferenceScreen PreferenceScreen
        {
            get => base.PreferenceScreen;
            set
            {
                base.PreferenceScreen = value;
                if (PreferenceScreen != null)
                {
                    var count = PreferenceScreen.PreferenceCount;
                    for (var i = 0; i < count; i++)
                    {
                        PreferenceScreen.GetPreference(i).IconSpaceReserved = false;
                    }
                }
            }
        }

        public override void OnResume()
        {
            try
            {
                base.OnResume();
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                //Add OnChange event to Preferences
                AddOrRemoveEvent(true);
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
                base.OnPause();
                PreferenceScreen.SharedPreferences.UnregisterOnSharedPreferenceChangeListener(this);

                //Close OnChange event to Preferences
                AddOrRemoveEvent(false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                MainSettings.SharedData = PreferenceManager.SharedPreferences;
                PreferenceManager.SharedPreferences.RegisterOnSharedPreferenceChangeListener(this);

                MyAccountPref = FindPreference("myAccount_key");
                PasswordPref = FindPreference("editPassword_key");
                SocialLinksPref = FindPreference("socialLinks_key");
                BlockedUsersPref = FindPreference("blocked_key");
                StoragePref = FindPreference("Storage_key");
                HelpPref = FindPreference("help_key");
                AboutPref = FindPreference("about_key");
                DeleteAccountPref = FindPreference("deleteAccount_key");
                LogoutPref = FindPreference("logout_key");

                ChatOnlinePref = (SwitchPreferenceCompat)FindPreference("chatOnline_key");
                PSearchEnginesPref = (SwitchPreferenceCompat)FindPreference("searchEngines_key");
                PRandomUsersPref = (SwitchPreferenceCompat)FindPreference("randomUsers_key");
                PFindMatchPagePref = (SwitchPreferenceCompat)FindPreference("findMatchPage_key");

                //Update Preferences data on Load
                OnSharedPreferenceChanged(MainSettings.SharedData, "chatOnline_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "searchEngines_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "randomUsers_key");
                OnSharedPreferenceChanged(MainSettings.SharedData, "findMatchPage_key");
                 
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (dataUser != null)
                {
                    ChatOnlinePref.Checked = dataUser.Online== 1; 
                    PFindMatchPagePref.Checked = dataUser.PrivacyShowProfileMatchProfiles == 1; 
                    PRandomUsersPref.Checked = dataUser.PrivacyShowProfileRandomUsers == 1; 
                    PSearchEnginesPref.Checked = dataUser.PrivacyShowProfileOnGoogle == 1; 
                }

                ChatOnlinePref.IconSpaceReserved = false;
                PSearchEnginesPref.IconSpaceReserved = false;
                PRandomUsersPref.IconSpaceReserved = false;
                PFindMatchPagePref.IconSpaceReserved = false;
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
                    MyAccountPref.PreferenceClick += MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick += PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick += SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick += BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick += StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick += HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick += AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick += DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick += LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange += ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange += PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange += PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange += PFindMatchPagePrefOnPreferenceChange;
                }
                else
                {
                    MyAccountPref.PreferenceClick -= MyAccountPrefOnPreferenceClick;
                    PasswordPref.PreferenceClick -= PasswordPrefOnPreferenceClick;
                    SocialLinksPref.PreferenceClick -= SocialLinksPrefOnPreferenceClick;
                    BlockedUsersPref.PreferenceClick -= BlockedUsersPrefOnPreferenceClick;
                    StoragePref.PreferenceClick -= StoragePrefOnPreferenceClick;
                    HelpPref.PreferenceClick -= HelpPrefOnPreferenceClick;
                    AboutPref.PreferenceClick -= AboutPrefOnPreferenceClick;
                    DeleteAccountPref.PreferenceClick -= DeleteAccountPrefOnPreferenceClick;
                    LogoutPref.PreferenceClick -= LogoutPrefOnPreferenceClick;
                    ChatOnlinePref.PreferenceChange -= ChatOnlinePrefOnPreferenceChange;
                    PSearchEnginesPref.PreferenceChange -= PSearchEnginesPrefOnPreferenceChange;
                    PRandomUsersPref.PreferenceChange -= PRandomUsersPrefOnPreferenceChange;
                    PFindMatchPagePref.PreferenceChange -= PFindMatchPagePrefOnPreferenceChange;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Event Privacy

        //Privacy >> Show my profile in find match page
        private void PFindMatchPagePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = 1;
                        PFindMatchPage = 1;
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileMatchProfiles = 0;
                        PFindMatchPage = 0;
                    }

                    dataUser.PrivacyShowProfileMatchProfiles = PFindMatchPage;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_match_profiles", PFindMatchPage.ToString()}
                    };
                    RequestsAsync.Users.UpdateProfileAsync(dataPrivacy).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            } 
        }

        //Privacy >> Show my profile in random users
        private void PRandomUsersPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileRandomUsers = 1;
                        PRandomUsers = 1;
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileRandomUsers = 0;
                        PRandomUsers = 0;
                    }

                    dataUser.PrivacyShowProfileRandomUsers = PRandomUsers;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_random_users", PRandomUsers.ToString()}
                    };
                    RequestsAsync.Users.UpdateProfileAsync(dataPrivacy).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Privacy >> Show my profile on search engines (google)
        private void PSearchEnginesPrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.PrivacyShowProfileOnGoogle = 1;
                        PSearchEngines = 1;
                    }
                    else
                    {
                        dataUser.PrivacyShowProfileOnGoogle = 0;
                        PSearchEngines = 0;
                    }

                    dataUser.PrivacyShowProfileOnGoogle = PSearchEngines;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();

                    var dataPrivacy = new Dictionary<string, string>
                    {
                        {"privacy_show_profile_on_google", PSearchEngines.ToString()}
                    };
                    RequestsAsync.Users.UpdateProfileAsync(dataPrivacy).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Event Support

        //Logout
        private void LogoutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var dialog = new MaterialDialog.Builder(ActivityContext);
                dialog.Title(Resource.String.Lbl_Warning);
                dialog.Content(GetText(Resource.String.Lbl_Are_you_logout));
                dialog.PositiveText(GetText(Resource.String.Lbl_Ok)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //DeleteAccount
        private void DeleteAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(DeleteAccountActivity))); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //About
        private void AboutPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(AboutAppActivity));
                ActivityContext.StartActivity(intent); 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Help
        private void HelpPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                var intent = new Intent(ActivityContext, typeof(LocalWebViewActivity));
                intent.PutExtra("URL", Client.WebsiteUrl + "/contact");
                intent.PutExtra("Type", GetText(Resource.String.Lbl_Help));
                ActivityContext.StartActivity(intent);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        #endregion

        #region Event General

        //BlockedUsers
        private void BlockedUsersPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(BlockedUsersActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //SocialLinks
        private void SocialLinksPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(SocialLinksActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Change Password
        private void PasswordPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(PasswordActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //MyAccount
        private void MyAccountPrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                ActivityContext.StartActivity(new Intent(ActivityContext, typeof(MyAccountActivity)));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        //Clear Cache >> Media
        private void StoragePrefOnPreferenceClick(object sender, Preference.PreferenceClickEventArgs e)
        {
            try
            {
                Methods.Path.DeleteAll_MyFolderDisk();
                Methods.Path.Chack_MyFolder();

                Toast.MakeText(Context, GetText(Resource.String.Lbl_Done), ToastLength.Long).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
         
        //ChatOnline
        private void ChatOnlinePrefOnPreferenceChange(object sender, Preference.PreferenceChangeEventArgs eventArgs)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    if (!eventArgs.Handled) return;
                    var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                    var etp = (SwitchPreferenceCompat)sender;
                    var value = eventArgs.NewValue.ToString();
                    etp.Checked = bool.Parse(value);
                    if (dataUser == null) return;
                    if (etp.Checked)
                    {
                        dataUser.Online = 1;
                        ChatOnline = 1;
                    }
                    else
                    {
                        dataUser.Online = 0;
                        ChatOnline = 0;
                    }

                    dataUser.Online = ChatOnline;

                    SqLiteDatabase database = new SqLiteDatabase();
                    database.InsertOrUpdate_DataMyInfo(dataUser);
                    database.Dispose();
                             
                    RequestsAsync.Chat.SwitchOnlineAsync(ChatOnline.ToString()).ConfigureAwait(false);
                }
                else
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Long).Show();
                }

             
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void OnSharedPreferenceChanged(ISharedPreferences sharedPreferences, string key)
        {
            try
            {
                if (key.Equals("chatOnline_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("chatOnline_key", true);
                    ChatOnlinePref.Checked = getValue;
                }
                else if (key.Equals("searchEngines_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("searchEngines_key", true);
                    PSearchEnginesPref.Checked = getValue;
                }
                else if (key.Equals("randomUsers_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("randomUsers_key", true);
                    PRandomUsersPref.Checked = getValue;
                }
                else if (key.Equals("findMatchPage_key"))
                {
                    var getValue = MainSettings.SharedData.GetBoolean("findMatchPage_key", true);
                    PFindMatchPagePref.Checked = getValue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region MaterialDialog
         
        public void OnClick(MaterialDialog p0, DialogAction p1)
        {
            try
            {
                if (p1 == DialogAction.Positive)
                {
                    Toast.MakeText(ActivityContext, ActivityContext.GetText(Resource.String.Lbl_You_will_be_logged), ToastLength.Long).Show();
                    ApiRequest.RunLogout = false;
                    ApiRequest.Logout(ActivityContext);
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
     
        #endregion

       
    }
}