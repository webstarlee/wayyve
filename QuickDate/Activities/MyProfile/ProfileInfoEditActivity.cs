using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android;
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
using QuickDate.Helpers.Controller;
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
    public class ProfileInfoEditActivity : AppCompatActivity , MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView BackIcon, NameIcon, GenderIcon, BirthdayIcon, LocationIcon, LanguageIcon, RelationshipIcon, WorkStatusIcon, EducationIcon;
        private EditText EdtFirstName, EdtLastName, EdtGender, EdtBirthday, EdtLocation, EdtLanguage, EdtRelationship, EdtWorkStatus, EdtEducation; 
        private Button BtnSave;
        private string TypeDialog = "";
        private int IdGender, IdRelationShip, IdWorkStatus, IdEducation;
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
                SetContentView(Resource.Layout.ButtomSheetProfileInfoEdit);

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

                NameIcon = FindViewById<TextView>(Resource.Id.IconName);
                EdtFirstName = FindViewById<EditText>(Resource.Id.FirstNameEditText);
                EdtLastName = FindViewById<EditText>(Resource.Id.LastNameEditText);

                GenderIcon = FindViewById<TextView>(Resource.Id.IconGender);
                EdtGender = FindViewById<EditText>(Resource.Id.GenderEditText);
               
                BirthdayIcon = FindViewById<TextView>(Resource.Id.IconBirthday);
                EdtBirthday = FindViewById<EditText>(Resource.Id.BirthdayEditText);

                LocationIcon = FindViewById<TextView>(Resource.Id.IconLocation);
                EdtLocation = FindViewById<EditText>(Resource.Id.LocationEditText);

                LanguageIcon = FindViewById<TextView>(Resource.Id.IconLanguage);
                EdtLanguage = FindViewById<EditText>(Resource.Id.LanguageEditText);

                RelationshipIcon = FindViewById<TextView>(Resource.Id.IconRelationship);
                EdtRelationship = FindViewById<EditText>(Resource.Id.RelationshipEditText);

                WorkStatusIcon = FindViewById<TextView>(Resource.Id.IconWorkStatus);
                EdtWorkStatus = FindViewById<EditText>(Resource.Id.WorkStatusEditText);

                EducationIcon = FindViewById<TextView>(Resource.Id.IconEducation);
                EdtEducation = FindViewById<EditText>(Resource.Id.EducationEditText);

                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft); 
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, NameIcon, FontAwesomeIcon.User);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, GenderIcon, FontAwesomeIcon.Transgender);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BirthdayIcon, FontAwesomeIcon.BirthdayCake);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LocationIcon, FontAwesomeIcon.MapMarkerAlt);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LanguageIcon, FontAwesomeIcon.Language);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, RelationshipIcon, FontAwesomeIcon.Heart);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, WorkStatusIcon, FontAwesomeIcon.Briefcase);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EducationIcon, FontAwesomeIcon.GraduationCap);

                Methods.SetFocusable(EdtGender);
                Methods.SetFocusable(EdtBirthday);
                Methods.SetFocusable(EdtLanguage);
                Methods.SetFocusable(EdtRelationship);
                Methods.SetFocusable(EdtWorkStatus);
                Methods.SetFocusable(EdtEducation); 

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
                    EdtGender.Touch += EdtGenderOnTouch;
                    EdtBirthday.Touch += EdtBirthdayOnClick;
                    EdtLocation.FocusChange += EdtLocationOnFocusChange;
                    EdtLanguage.Touch += EdtLanguageOnClick;
                    EdtRelationship.Touch += EdtRelationshipOnClick;
                    EdtWorkStatus.Touch += EdtWorkStatusOnClick;
                    EdtEducation.Touch += EdtEducationOnClick;
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtGender.Touch -= EdtGenderOnTouch; 
                    EdtBirthday.Touch -= EdtBirthdayOnClick;
                    EdtLocation.FocusChange -= EdtLocationOnFocusChange;
                    EdtLanguage.Touch -= EdtLanguageOnClick;
                    EdtRelationship.Touch -= EdtRelationshipOnClick;
                    EdtWorkStatus.Touch -= EdtWorkStatusOnClick;
                    EdtEducation.Touch -= EdtEducationOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region Events

        private void EdtGenderOnTouch(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Gender";
                var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (genderArray != null) arrayAdapter.AddRange(genderArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Gender));
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
         
        //Education
        private void EdtEducationOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Education";
                //string[] educationArray = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
                var educationArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Education;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (educationArray != null) arrayAdapter.AddRange(educationArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetString(Resource.String.Lbl_EducationLevel));
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

        //WorkStatus
        private void EdtWorkStatusOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "WorkStatus";
                //string[] workStatusArray = Application.Context.Resources.GetStringArray(Resource.Array.WorkStatusArray);
                var workStatusArray = ListUtils.SettingsSiteList.FirstOrDefault()?.WorkStatus;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (workStatusArray != null) arrayAdapter.AddRange(workStatusArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_ChooseWorkStatus));
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
         
        //Open DatePicker And Get Short Date 
        private void EdtBirthdayOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                var frag = DatePickerFragment.NewInstance(delegate (DateTime time)
                {
                    EdtBirthday.Text = time.Date.ToString("dd-MM-yyyy");
                });
                frag.Show(FragmentManager, DatePickerFragment.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception); 
            }
        }

        //RelationShip
        private void EdtRelationshipOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Relationship";
                //string[] relationshipArray = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
                var relationshipArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Relationship;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (relationshipArray != null) arrayAdapter.AddRange(relationshipArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_ChooseRelationshipStatus));
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

        //Language
        private void EdtLanguageOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Language";
                //string[] languageArray = Application.Context.Resources.GetStringArray(Resource.Array.LanguageArray); 
                var languageArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Language;
                 
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (languageArray != null) arrayAdapter.AddRange(languageArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_ChooseLanguage));
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
         
        //Get Location
        private void EdtLocationOnFocusChange(object sender, View.FocusChangeEventArgs e)
        {
            try
            {
                if (e.HasFocus)
                {
                    // Check if we're running on Android 5.0 or higher
                    if ((int) Build.VERSION.SdkInt < 23)
                    {
                        // result Code 502
                        new IntentController(this).OpenIntentLocation();
                    }
                    else
                    {
                        if (CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted && CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted)
                            new IntentController(this).OpenIntentLocation();
                        else
                            new PermissionsController(this).RequestPermission(105);
                    }
                }
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
                        {"first_name", EdtFirstName.Text}, 
                        {"last_name", EdtLastName.Text},
                        {"gender", IdGender.ToString()},
                        {"birthday", EdtBirthday.Text},
                        {"location", EdtLocation.Text},
                        {"language", EdtLanguage.Text.ToLower()},
                        {"relationship", IdRelationShip.ToString()},
                        {"work_status", IdWorkStatus.ToString()},
                        {"education", IdEducation.ToString()},
                    };
                     
                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            Console.WriteLine(result.Message);
                            var local = ListUtils.MyUserInfo.FirstOrDefault();
                            if (local != null)
                            {
                                local.FirstName = EdtFirstName.Text;
                                local.LastName = EdtLastName.Text;
                                local.Gender = IdGender.ToString();
                                local.Birthday = EdtBirthday.Text;
                                local.Address = EdtLocation.Text;
                                local.Language = EdtLanguage.Text;
                                local.Relationship = IdRelationShip;
                                local.WorkStatus = IdWorkStatus;
                                local.Education = IdEducation;

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
                            AndHUD.Shared.ShowError(this, error.ErrorData.ErrorText, MaskType.Clear, TimeSpan.FromSeconds(2)); 
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
        
        //Back
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
                    EdtFirstName.Text = dataUser.FirstName;
                    EdtLastName.Text = dataUser.LastName;

                    EdtGender.Text = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender?.FirstOrDefault(a => a.ContainsKey(dataUser.Gender))?.Values.FirstOrDefault() ?? dataUser.GenderTxt;
                    IdGender = Convert.ToInt32(dataUser.Gender);

                    EdtBirthday.Text = dataUser.Birthday;

                    if (Methods.FunString.StringNullRemover(dataUser.Location) != "-----")
                    {
                        EdtLocation.Text = dataUser.Location;
                    }

                    if (Methods.FunString.StringNullRemover(dataUser.Language) != "-----")
                    {
                        EdtLanguage.Text = dataUser.Language;
                    }

                    string relationship = QuickDateTools.GetRelationship(Convert.ToInt32(dataUser.Relationship));
                    if (Methods.FunString.StringNullRemover(relationship) != "-----")
                    {
                        EdtRelationship.Text = relationship;
                        IdRelationShip = Convert.ToInt32(dataUser.Relationship);
                    }

                    string work = QuickDateTools.GetWorkStatus(Convert.ToInt32(dataUser.WorkStatus));
                    if (Methods.FunString.StringNullRemover(work) != "-----")
                    {
                        EdtWorkStatus.Text = work;
                        IdWorkStatus = Convert.ToInt32(dataUser.WorkStatus);
                    }

                    string education = QuickDateTools.GetEducation(Convert.ToInt32(dataUser.Education));
                    if (Methods.FunString.StringNullRemover(education) != "-----")
                    {
                        EdtEducation.Text = education;
                        IdEducation = Convert.ToInt32(dataUser.Education);
                    } 
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Permissions && Result

        //Result
        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            try
            {
                base.OnActivityResult(requestCode, resultCode, data);

                if (requestCode == 502 && resultCode == Result.Ok) // Location
                {
                    var placeAddress = data.GetStringExtra("Address") ?? "";
                    var placeLatLng = data.GetStringExtra("latLng") ?? "";
                    if (!string.IsNullOrEmpty(placeAddress))
                        EdtLocation.Text = placeAddress; 
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 105)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        new IntentController(this).OpenIntentLocation();
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

        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Language")
                {
                    EdtLanguage.Text = itemString.ToString();
                }
                else if (TypeDialog == "Relationship")
                {
                    var relationshipArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Relationship?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdRelationShip = int.Parse(relationshipArray ?? "1");
                    EdtRelationship.Text = itemString.ToString();
                }
                else if (TypeDialog == "WorkStatus")
                {
                    var workStatusArray = ListUtils.SettingsSiteList.FirstOrDefault()?.WorkStatus?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdWorkStatus = int.Parse(workStatusArray ?? "1");
                    EdtWorkStatus.Text = itemString.ToString();
                }
                else if (TypeDialog == "Education")
                {
                    var educationArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Education?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdEducation = int.Parse(educationArray ?? "1");
                    EdtEducation.Text = itemString.ToString();
                }
                else if (TypeDialog == "Gender")
                {
                    var genderArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Gender?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdGender = int.Parse(genderArray ?? "4525");
                    EdtGender.Text = itemString.ToString();
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