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
    public class PersonalityInfoEditActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView BackIcon, CharacterIcon, ChildrenIcon, FriendsIcon, PetsIcon;
        private EditText EdtCharacter, EdtChildren, EdtFriends, EdtPets;
        private Button BtnSave;
        private string TypeDialog;
        private int IdCharacter, IdChildren, IdFriends, IdPets;
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
                SetContentView(Resource.Layout.ButtomSheetPersonalityInfoEdit);

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

                CharacterIcon = FindViewById<TextView>(Resource.Id.IconCharacter);
                EdtCharacter = FindViewById<EditText>(Resource.Id.CharacterEditText);

                ChildrenIcon = FindViewById<TextView>(Resource.Id.IconChildren);
                EdtChildren = FindViewById<EditText>(Resource.Id.ChildrenEditText);

                FriendsIcon = FindViewById<TextView>(Resource.Id.IconFriends);
                EdtFriends = FindViewById<EditText>(Resource.Id.FriendsEditText);

                PetsIcon = FindViewById<TextView>(Resource.Id.IconPets);
                EdtPets = FindViewById<EditText>(Resource.Id.PetsEditText);
                 
                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, CharacterIcon, FontAwesomeIcon.YinYang);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, ChildrenIcon, FontAwesomeIcon.Baby);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, FriendsIcon, FontAwesomeIcon.UserFriends);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, PetsIcon, FontAwesomeIcon.Cat);

                Methods.SetFocusable(EdtCharacter);
                Methods.SetFocusable(EdtChildren);
                Methods.SetFocusable(EdtFriends);
                Methods.SetFocusable(EdtPets);

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
                    EdtCharacter.Touch += EdtCharacterOnClick;
                    EdtChildren.Touch += EdtChildrenOnClick;
                    EdtFriends.Touch += EdtFriendsOnClick;
                    EdtPets.Touch += EdtPetsOnClick;
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
                    EdtCharacter.Touch -= EdtCharacterOnClick;
                    EdtChildren.Touch -= EdtChildrenOnClick;
                    EdtFriends.Touch -= EdtFriendsOnClick;
                    EdtPets.Touch -= EdtPetsOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Pets
        private void EdtPetsOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Pets";
                //string[] petsArray = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
                var petsArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Pets;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (petsArray != null) arrayAdapter.AddRange(petsArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Pets));
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

        //Friends
        private void EdtFriendsOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Friends";
                //string[] friendsArray = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);
                var friendsArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Friends;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (friendsArray != null) arrayAdapter.AddRange(friendsArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Friends));
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

        //Children
        private void EdtChildrenOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Children";
                //string[] childrenArray = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);
                var childrenArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Children;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (childrenArray != null) arrayAdapter.AddRange(childrenArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Children));
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

        //Character
        private void EdtCharacterOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Character";
                //string[] characterArray = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);
                var characterArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Character;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                if (characterArray != null) arrayAdapter.AddRange(characterArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Character));
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
                        {"character", IdCharacter.ToString()},
                        {"children", IdChildren.ToString()},
                        {"friends", IdFriends.ToString()},
                        {"pets",IdPets.ToString()},
                    };

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault();
                            if (local != null)
                            {
                                local.Character = IdCharacter;
                                local.Children = IdChildren;
                                local.Friends = IdFriends;
                                local.Pets = IdPets;

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
                        if (respond is ErrorObject error)
                        {
                            Methods.DisplayReportResult(this, respond);
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
                    string character = QuickDateTools.GetCharacter(Convert.ToInt32(dataUser.Character));
                    if (Methods.FunString.StringNullRemover(character) != "-----")
                    {
                        IdCharacter = Convert.ToInt32(dataUser.Character);
                        EdtCharacter.Text = character;
                    }

                    string children = QuickDateTools.GetChildren(Convert.ToInt32(dataUser.Children));
                    if (Methods.FunString.StringNullRemover(children) != "-----")
                    {
                        IdChildren = Convert.ToInt32(dataUser.Children);
                        EdtChildren.Text = children;
                    }

                    string friends = QuickDateTools.GetFriends(Convert.ToInt32(dataUser.Friends));
                    if (Methods.FunString.StringNullRemover(friends) != "-----")
                    {
                        IdFriends = Convert.ToInt32(dataUser.Friends);
                        EdtFriends.Text = friends;
                    }

                    string pets = QuickDateTools.GetPets(Convert.ToInt32(dataUser.Pets));
                    if (Methods.FunString.StringNullRemover(pets) != "-----")
                    {
                        IdPets = Convert.ToInt32(dataUser.Pets);
                        EdtPets.Text = pets;
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
                if (TypeDialog == "Character")
                {
                    var characterArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Character?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdCharacter = int.Parse(characterArray ?? "1");
                    EdtCharacter.Text = itemString.ToString();
                }
                else if (TypeDialog == "Children")
                {
                    var childrenArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Children?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdChildren = int.Parse(childrenArray ?? "1");
                    EdtChildren.Text = itemString.ToString();
                }
                else if (TypeDialog == "Friends")
                {
                    var friendsArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Friends?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdFriends = int.Parse(friendsArray ?? "1");
                    EdtFriends.Text = itemString.ToString();
                }
                else if (TypeDialog == "Pets")
                {
                    var petsArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Pets?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdPets = int.Parse(petsArray ?? "1");
                    EdtPets.Text = itemString.ToString();
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