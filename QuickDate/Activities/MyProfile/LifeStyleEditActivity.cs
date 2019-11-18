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
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;
using Exception = System.Exception;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class LifeStyleEditActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView BackIcon, LiveWithIcon, CarIcon, ReligionIcon, SmokeIcon, DrinkIcon, TravelIcon;
        private EditText EdtLiveWith, EdtCar, EdtReligion, EdtSmoke, EdtDrink, EdtTravel;
        private Button BtnSave;
        private string TypeDialog;
        private int IdLiveWith, IdCar, IdReligion, IdSmoke, IdDrink, IdTravel;
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
                SetContentView(Resource.Layout.ButtomSheetLifeStyleEdit);

                //Get Value And Set Toolbar
                InitComponent(); 

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

        #region Functions

        private void InitComponent()
        {
            try
            {
                BackIcon = FindViewById<TextView>(Resource.Id.IconBack);

                LiveWithIcon = FindViewById<TextView>(Resource.Id.IconLiveWith);
                EdtLiveWith = FindViewById<EditText>(Resource.Id.LiveWithEditText);

                CarIcon = FindViewById<TextView>(Resource.Id.IconCar);
                EdtCar = FindViewById<EditText>(Resource.Id.CarEditText);

                ReligionIcon = FindViewById<TextView>(Resource.Id.IconReligion);
                EdtReligion = FindViewById<EditText>(Resource.Id.ReligionEditText);

                SmokeIcon = FindViewById<TextView>(Resource.Id.IconSmoke);
                EdtSmoke = FindViewById<EditText>(Resource.Id.SmokeEditText);

                DrinkIcon = FindViewById<TextView>(Resource.Id.IconDrink);
                EdtDrink = FindViewById<EditText>(Resource.Id.DrinkEditText);

                TravelIcon = FindViewById<TextView>(Resource.Id.IconTravel);
                EdtTravel = FindViewById<EditText>(Resource.Id.TravelEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LiveWithIcon, FontAwesomeIcon.GlobeAmericas);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CarIcon, FontAwesomeIcon.Car);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, ReligionIcon, FontAwesomeIcon.Church);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, SmokeIcon, FontAwesomeIcon.Smoking);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, DrinkIcon, FontAwesomeIcon.Beer);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, TravelIcon, FontAwesomeIcon.PlaneDeparture);
                 
                Methods.SetFocusable(EdtLiveWith); 
                Methods.SetFocusable(EdtCar); 
                Methods.SetFocusable(EdtReligion); 
                Methods.SetFocusable(EdtSmoke); 
                Methods.SetFocusable(EdtDrink); 
                Methods.SetFocusable(EdtTravel); 

                MAdView = FindViewById<AdView>(Resource.Id.adView);
                AdsGoogle.InitAdView(MAdView, null); 
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
                    BackIcon.Click += BackIconOnClick;
                    BtnSave.Click += BtnSaveOnClick;
                    EdtLiveWith.Touch += EdtLiveWithOnClick;
                    EdtCar.Touch += EdtCarOnClick;
                    EdtReligion.Touch += EdtReligionOnClick;
                    EdtSmoke.Touch += EdtSmokeOnClick;
                    EdtDrink.Touch += EdtDrinkOnClick;
                    EdtTravel.Touch += EdtTravelOnClick;  
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtLiveWith.Touch -= EdtLiveWithOnClick;
                    EdtCar.Touch -= EdtCarOnClick;
                    EdtReligion.Touch -= EdtReligionOnClick;
                    EdtSmoke.Touch -= EdtSmokeOnClick;
                    EdtDrink.Touch -= EdtDrinkOnClick;
                    EdtTravel.Touch -= EdtTravelOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
         
        #endregion

        #region Events
         
        //Travel
        private void EdtTravelOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Travel";
                //string[] travelArray = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);
               var travelArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Travel;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (travelArray != null) arrayAdapter.AddRange(travelArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Travel));
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

        //Drink
        private void EdtDrinkOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Drink";
                //string[] drinkArray = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
                var drinkArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Drink;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (drinkArray != null) arrayAdapter.AddRange(drinkArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Drink));
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

        //Smoke
        private void EdtSmokeOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Smoke";
                //string[] smokeArray = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
                var smokeArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Smoke;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (smokeArray != null) arrayAdapter.AddRange(smokeArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Smoke));
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

        //Religion
        private void EdtReligionOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Religion";
                //string[] religionArray = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
                var religionArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Religion;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (religionArray != null) arrayAdapter.AddRange(religionArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Religion));
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

        //Car
        private void EdtCarOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Car";
                //string[] carArray = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);
                var carArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Car;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (carArray != null) arrayAdapter.AddRange(carArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetString(Resource.String.Lbl_Car));
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

        //LiveWith
        private void EdtLiveWithOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "LiveWith";
                //string[] liveWithArray = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);
                var liveWithArray = ListUtils.SettingsSiteList.FirstOrDefault()?.LiveWith;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (liveWithArray != null) arrayAdapter.AddRange(liveWithArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_LiveWith));
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

        //Click save data and sent api
        private async void BtnSaveOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"live_with", IdLiveWith.ToString()},
                        {"car", IdCar.ToString()},
                        {"religion", IdReligion.ToString()},
                        {"smoke",IdSmoke.ToString()},
                        {"drink",IdDrink.ToString()},
                        {"travel",IdTravel.ToString()},
                    };

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault();
                            if (local != null)
                            {
                                local.LiveWith = IdLiveWith;
                                local.Car = IdCar;
                                local.Religion = IdReligion;
                                local.Smoke = IdSmoke;
                                local.Drink = IdDrink;
                                local.Travel = IdTravel;

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
                        //Show a Error image with a message
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
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                AndHUD.Shared.Dismiss(this);
            } 
        }

        private void BackIconOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent resultIntent = new Intent();
                SetResult(Result.Canceled, resultIntent);
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                    string liveWith = QuickDateTools.GetLiveWith(Convert.ToInt32(dataUser.LiveWith));
                    if (Methods.FunString.StringNullRemover(liveWith) != "-----")
                    {
                        IdLiveWith = Convert.ToInt32(dataUser.LiveWith);
                        EdtLiveWith.Text = liveWith;
                    }

                    string car = QuickDateTools.GetCar(Convert.ToInt32(dataUser.Car));
                    if (Methods.FunString.StringNullRemover(car) != "-----")
                    {
                        IdCar = Convert.ToInt32(dataUser.Car);
                        EdtCar.Text = car;
                    }

                    string religion = QuickDateTools.GetReligion(Convert.ToInt32(dataUser.Religion));
                    if (Methods.FunString.StringNullRemover(religion) != "-----")
                    {
                        IdReligion = Convert.ToInt32(dataUser.Religion);
                        EdtReligion.Text = religion;
                    }

                    string smoke = QuickDateTools.GetSmoke(Convert.ToInt32(dataUser.Smoke));
                    if (Methods.FunString.StringNullRemover(smoke) != "-----")
                    {
                        IdSmoke = Convert.ToInt32(dataUser.Smoke);
                        EdtSmoke.Text = smoke;
                    }

                    string drink = QuickDateTools.GetDrink(Convert.ToInt32(dataUser.Drink));
                    if (Methods.FunString.StringNullRemover(drink) != "-----")
                    {
                        IdDrink = Convert.ToInt32(dataUser.Drink);
                        EdtDrink.Text = drink;
                    }

                    string travel = QuickDateTools.GetTravel(Convert.ToInt32(dataUser.Travel));
                    if (Methods.FunString.StringNullRemover(travel) != "-----")
                    {
                        IdTravel = Convert.ToInt32(dataUser.Travel);
                        EdtTravel.Text = travel;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
        }

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "LiveWith")
                {
                    var liveWithArray = ListUtils.SettingsSiteList.FirstOrDefault()?.LiveWith?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdLiveWith = int.Parse(liveWithArray ?? "1");
                    EdtLiveWith.Text = itemString.ToString();
                }
                else if (TypeDialog == "Car")
                {
                    var carArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Car?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdCar = int.Parse(carArray ?? "1");
                    EdtCar.Text = itemString.ToString();
                }
                else if (TypeDialog == "Religion")
                {
                    var religionArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Religion?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdReligion = int.Parse(religionArray ?? "1");
                    EdtReligion.Text = itemString.ToString();
                }
                else if (TypeDialog == "Smoke")
                {
                    var smokeArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Smoke?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdSmoke = int.Parse(smokeArray ?? "1");
                    EdtSmoke.Text = itemString.ToString();
                }
                else if (TypeDialog == "Drink")
                {
                    var drinkArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Drink?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdDrink = int.Parse(drinkArray ?? "1");
                    EdtDrink.Text = itemString.ToString();
                }
                else if (TypeDialog == "Travel")
                {
                    var travelArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Travel?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdTravel = int.Parse(travelArray ?? "1");
                    EdtTravel.Text = itemString.ToString(); 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

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

        #endregion

    }
}