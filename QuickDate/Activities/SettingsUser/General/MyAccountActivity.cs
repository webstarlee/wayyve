using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using Java.Lang;
using QuickDate.Activities.Tabbes;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;
using Toolbar = Android.Support.V7.Widget.Toolbar;

namespace QuickDate.Activities.SettingsUser.General
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MyAccountActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView BackIcon, UsernameIcon, EmailIcon, PhoneNumberIcon, CountryIcon;
        private EditText EdtUsername, EdtEmail, EdtPhoneNumber, EdtCountry;
        private Button BtnSave;
        private Toolbar Toolbar;
        private string Country;
        private HomeActivity GlobalContext;
        private AdView MAdView;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                // Create your application here
                SetContentView(Resource.Layout.MyAccountLayout);

                GlobalContext = HomeActivity.GetInstance();

                //Get Value And Set Toolbar
                InitComponent();
                InitToolbar(); 

                GetMyInfoData(); 
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
                BackIcon = FindViewById<TextView>(Resource.Id.IconBack);

                UsernameIcon = FindViewById<TextView>(Resource.Id.IconUsername);
                EdtUsername = FindViewById<EditText>(Resource.Id.UsernameEditText);

                EmailIcon = FindViewById<TextView>(Resource.Id.IconEmail);
                EdtEmail = FindViewById<EditText>(Resource.Id.EmailEditText);

                PhoneNumberIcon = FindViewById<TextView>(Resource.Id.IconPhoneNumber);
                EdtPhoneNumber = FindViewById<EditText>(Resource.Id.PhoneNumberEditText);

                CountryIcon = FindViewById<TextView>(Resource.Id.IconCountry);
                EdtCountry = FindViewById<EditText>(Resource.Id.CountryEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, UsernameIcon, FontAwesomeIcon.Users);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EmailIcon, FontAwesomeIcon.At);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, PhoneNumberIcon, FontAwesomeIcon.Phone);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CountryIcon, FontAwesomeIcon.Flag);

                EdtCountry.SetFocusable(ViewFocusability.NotFocusable);

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
                Toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
                if (Toolbar != null)
                {
                    Toolbar.Title = GetString(Resource.String.Lbl_MyAccount);
                    Toolbar.SetTitleTextColor(AppSettings.TitleTextColor);
                    SetSupportActionBar(Toolbar);
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
                    BtnSave.Click += BtnSaveOnClick;
                    EdtCountry.Click += EdtCountryOnClick;
                }
                else
                {
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCountry.Click += EdtCountryOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

       
        #endregion

        #region Events

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                var correctly = Methods.FunString.IsPhoneNumber(EdtPhoneNumber.Text);
                 
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"username", EdtUsername.Text},
                        {"email", EdtEmail.Text},
                        {"country",Country},
                    };

                    if (correctly)
                        dictionary.Add("phone_number", EdtPhoneNumber.Text);

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault();
                            if (local != null)
                            {
                                local.Username = EdtUsername.Text;
                                local.Email = EdtEmail.Text;
                                local.PhoneNumber = EdtPhoneNumber.Text;
                                local.Country = Country;

                                SqLiteDatabase database = new SqLiteDatabase();
                                database.InsertOrUpdate_DataMyInfo(local);
                                database.Dispose(); 
                            }

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Done), ToastLength.Short).Show();
                            AndHUD.Shared.Dismiss(this);
                             
                            Intent resultIntent = new Intent();
                            SetResult(Result.Ok, resultIntent);
                            Finish();
                        }
                    }
                    else  
                    {
                        Methods.DisplayReportResult(this, respond);
                        if (respond is ErrorObject error)
                        {
                            var errorText = error.ErrorData.ErrorText;
                            AndHUD.Shared.ShowError(this, errorText, MaskType.Clear, TimeSpan.FromSeconds(2)); 
                        }
                    } 
                     
                    AndHUD.Shared.Dismiss(this);
                }
                else
                {
                    Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }

                if (!correctly)
                    Toast.MakeText(this, GetText(Resource.String.Lbl_PleaseAddPhoneNumberCorrectly), ToastLength.Short).Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            }
        }

        //Country
        private void EdtCountryOnClick(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    //string[] countriesArray = Context.Resources.GetStringArray(Resource.Array.countriesArray);
                    var countriesArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries;

                    var arrayAdapter = new List<string>();
                    var dialogList = new MaterialDialog.Builder(this);

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
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }


        #endregion
         
        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data); 
                if (requestCode == 125 && resultCode == Result.Ok)
                {
                    var status = data.GetStringExtra("status") ?? "";
                    if (status == "1")
                    {
                        EdtPhoneNumber.Text = ListUtils.MyUserInfo.FirstOrDefault()?.PhoneNumber ?? ""; 
                    }
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion
         
        private void GetMyInfoData()
        {
            try
            {
                if (ListUtils.MyUserInfo.Count == 0)
                {
                    var sqlEntity = new SqLiteDatabase();
                    sqlEntity.GetDataMyInfo();
                    sqlEntity.Dispose();
                }
                
                var dataUser = ListUtils.MyUserInfo.FirstOrDefault();
                if (dataUser != null)
                {
                    EdtUsername.Text = dataUser.Username;
                    EdtEmail.Text = dataUser.Email;
                    EdtPhoneNumber.Text = dataUser.PhoneNumber;
                    EdtCountry.Text = QuickDateTools.GetCountry(dataUser.Country);
                    
                    if (dataUser.PhoneVerified == 0 /*&& dataUser.Verified == "0" */&& AppSettings.ValidationEnabled)
                    {
                        var window = new PopupController(this);
                        window.DisplayAddPhoneNumber();

                        if (GlobalContext.TracksCounter != null)
                            GlobalContext.TracksCounter.LastCounterEnum = TracksCounter.TracksCounterEnum.AddPhoneNumber;
                    } 
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
                var countriesArrayId = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries?.FirstOrDefault(a => a.Values?.FirstOrDefault()?.Name == itemString.ToString())?.Keys.FirstOrDefault();
                 
                var data = countriesArrayId;
                Country = data;
                EdtCountry.Text = text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

    }
}