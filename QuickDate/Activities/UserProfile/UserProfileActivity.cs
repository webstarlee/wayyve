using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using AFollestad.MaterialDialogs;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using AT.Markushi.UI;
using Java.Lang;
using Me.Relex;
using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.Activities.Gift;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.UserProfile.Adapters;
using QuickDate.Helpers.Controller;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Requests;
using Exception = System.Exception;
using String = System.String;

namespace QuickDate.Activities.UserProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/ProfileTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class UserProfileActivity : AppCompatActivity, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region Variables Basic

        private TextView Username, IconOnline, IconMore, IconFavorite, IconLocation, LocationTextView, Description, DescriptionInterest, DescriptionLanguages, DescriptionWork;
        private ViewPager ViewPagerView;
        private CircleIndicator CircleIndicatorView;
        private TextView IconBack;
        private Button GiftButton;
        private CircleButton LikeButton, DesLikeButton, ChatButton;
        private Button WebsiteButton, SocialGoogle, SocialFacebook, SocialTwitter, SocialLinkedIn, SocialInstagram;
        private RelativeLayout SocialLayout, WorkAndEducationLayout, IntersetLayout, LanguagesLayout, FooterButtonSection;
        private UserInfoObject DataUser;
        private List<string> ListImageUser = new List<string>();
        private string EventType, DataType, LinkedIn, Twitter, Facebook, Google, Instagrem, Website;
        private HomeActivity GlobalContext;

        #endregion

        #region General

        protected override void OnCreate(Bundle savedInstanceState)
        {
            try
            {
                base.OnCreate(savedInstanceState);

                Methods.App.FullScreenApp(this);

                GlobalContext = HomeActivity.GetInstance();

                // Create your application here
                SetContentView(Resource.Layout.UserProfileLayout);

                EventType = Intent.GetStringExtra("EventPage");
                DataType = Intent.GetStringExtra("DataType") ?? "";

                DataUser = JsonConvert.DeserializeObject<UserInfoObject>(Intent.GetStringExtra("ItemUser"));

                //Get Value  
                InitComponent();
                GetStickersGiftsLists();

                if (DataType == "OneSignal")
                {
                    //Run Api 
                    GetUserData(DataUser, "RunApi");
                }
                else
                {
                    GetUserData(DataUser);
                }
                
               
                GlobalContext?.TracksCounter?.CheckTracksCounter();
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

        #endregion

        #region Functions

        private void InitComponent()
        {
            try
            {
                IconBack = FindViewById<TextView>(Resource.Id.back);

                Username = FindViewById<TextView>(Resource.Id.Username);
                IconOnline = FindViewById<TextView>(Resource.Id.iconOnline);
                IconLocation = FindViewById<TextView>(Resource.Id.IconLocation);
                IconMore = FindViewById<TextView>(Resource.Id.iconMore);
                IconFavorite = FindViewById<TextView>(Resource.Id.iconFavorite);
                LocationTextView = FindViewById<TextView>(Resource.Id.LocationTextView);
                Description = FindViewById<TextView>(Resource.Id.Description);
                DescriptionInterest = FindViewById<TextView>(Resource.Id.DescriptionIntersetsSection);
                DescriptionLanguages = FindViewById<TextView>(Resource.Id.DescriptionLanguagesSection);
                DescriptionWork = FindViewById<TextView>(Resource.Id.DescriptionworkSection);
                GiftButton = FindViewById<Button>(Resource.Id.GiftButton);
                LikeButton = FindViewById<CircleButton>(Resource.Id.likebutton2);
                DesLikeButton = FindViewById<CircleButton>(Resource.Id.closebutton1);
                ChatButton = FindViewById<CircleButton>(Resource.Id.Chatbutton1);

                WorkAndEducationLayout = FindViewById<RelativeLayout>(Resource.Id.workAndEducationLayout);
                IntersetLayout = FindViewById<RelativeLayout>(Resource.Id.intersetLayout);
                LanguagesLayout = FindViewById<RelativeLayout>(Resource.Id.languagesLayout);
                FooterButtonSection = FindViewById<RelativeLayout>(Resource.Id.footerButtonSection);

                SocialLayout = FindViewById<RelativeLayout>(Resource.Id.socialinfolayout);
                SocialGoogle = FindViewById<Button>(Resource.Id.social1);
                SocialFacebook = FindViewById<Button>(Resource.Id.social2);
                SocialTwitter = FindViewById<Button>(Resource.Id.social3);
                SocialLinkedIn = FindViewById<Button>(Resource.Id.social4);
                SocialInstagram = FindViewById<Button>(Resource.Id.social5);
                WebsiteButton = FindViewById<Button>(Resource.Id.website);

                ViewPagerView = FindViewById<ViewPager>(Resource.Id.pager);
                CircleIndicatorView = FindViewById<CircleIndicator>(Resource.Id.indicator);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconLocation, IonIconsFonts.IosLocationOutline);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconBack, IonIconsFonts.IosArrowBack);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconOnline, IonIconsFonts.Record);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, IconMore, IonIconsFonts.AndroidMoreVertical);

                if (EventType == "HideButton")
                {
                    FooterButtonSection.Visibility = ViewStates.Gone;
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
                    IconBack.Click += IconBackOnClick;
                    GiftButton.Click += GiftButtonOnClick;
                    LikeButton.Click += LikeButtonOnClick;
                    DesLikeButton.Click += DesLikeButtonOnClick;
                    ChatButton.Click += ChatButtonOnClick;
                    SocialGoogle.Click += SocialGoogleOnClick;
                    SocialFacebook.Click += SocialFacebookOnClick;
                    SocialTwitter.Click += SocialTwitterOnClick;
                    SocialLinkedIn.Click += SocialLinkedInOnClick;
                    SocialInstagram.Click += SocialInstagramOnClick;
                    WebsiteButton.Click += WebsiteButtonOnClick;
                    IconMore.Click += IconMoreOnClick;
                    IconFavorite.Click += IconFavoriteOnClick;
                }
                else
                {
                    IconBack.Click -= IconBackOnClick;
                    GiftButton.Click -= GiftButtonOnClick;
                    LikeButton.Click -= LikeButtonOnClick;
                    DesLikeButton.Click -= DesLikeButtonOnClick;
                    ChatButton.Click -= ChatButtonOnClick;
                    SocialGoogle.Click -= SocialGoogleOnClick;
                    SocialFacebook.Click -= SocialFacebookOnClick;
                    SocialTwitter.Click -= SocialTwitterOnClick;
                    SocialLinkedIn.Click -= SocialLinkedInOnClick;
                    SocialInstagram.Click -= SocialInstagramOnClick;
                    WebsiteButton.Click -= WebsiteButtonOnClick;
                    IconMore.Click -= IconMoreOnClick;
                    IconFavorite.Click -= IconFavoriteOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Back
        private void IconBackOnClick(object sender, EventArgs e)
        {
            try
            {
                Finish();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open chat
        private void ChatButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Intent Int = new Intent(this, typeof(MessagesBoxActivity));
                Int.PutExtra("UserId", DataUser.Id.ToString());
                Int.PutExtra("TypeChat", "LastChat");
                Int.PutExtra("UserItem", JsonConvert.SerializeObject(DataUser));

                // Check if we're running on Android 5.0 or higher
                if ((int)Build.VERSION.SdkInt < 23)
                {
                    StartActivity(Int);
                }
                else
                {
                    //Check to see if any permission in our group is available, if one, then all are
                    if (CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                        CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
                    {
                        StartActivity(Int);

                    }
                    else
                        new PermissionsController(this).RequestPermission(100);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //DesLike
        private void DesLikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var cardFragment = GlobalContext?.CardFragment;
                cardFragment?.SetDesLikeDirection();

                //sent api 
                RequestsAsync.Users.AddLikesAsync("", DataUser.Id.ToString()).ConfigureAwait(false);

                ObservableCollection<UserInfoObject> list;
                switch (EventType)
                {
                    case "Close":
                    case "likeAndClose":
                        Finish();
                        break;
                    case "LikeAndMoveTrending":
                    {
                        list = AppSettings.ShowUsersAsCards ? GlobalContext?.TrendingFragment?.CardDateAdapter2?.UsersDateList : GlobalContext?.TrendingFragment?.NearByAdapter?.NearByList; 
                        var user = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (list != null)
                        {
                            int index = list.IndexOf(user);
                            if (index > -1)
                                index += 1;

                            var nextDataUser = list[index];
                            GetUserData(nextDataUser);
                        }

                        break;
                    } 
                    case "LikeAndMoveCardMach":
                    {
                        list = GlobalContext?.CardFragment?.CardDateAdapter?.UsersDateList;
                        var data = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (data != null)
                        {
                            int index = list.IndexOf(data);
                            if (index > -1)
                                index += 1;

                            var nextDataUser = list[index];
                            GetUserData(nextDataUser);
                        }

                        break;
                    }
                    default:
                    {
                        Finish();
                        break;
                    }
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Like
        private void LikeButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                var cardFragment = GlobalContext?.CardFragment;
                cardFragment?.SetLikeDirection();

                //sent api 
                RequestsAsync.Users.AddLikesAsync(DataUser.Id.ToString(), "").ConfigureAwait(false);

                ObservableCollection<UserInfoObject> list;

                switch (EventType)
                {
                    case "Close":
                    case "likeAndClose":
                        Finish();
                        break;
                    case "LikeAndMoveTrending":
                    {
                        list = AppSettings.ShowUsersAsCards ? GlobalContext?.TrendingFragment?.CardDateAdapter2?.UsersDateList : GlobalContext?.TrendingFragment?.NearByAdapter?.NearByList;
                        var user = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (list != null)
                        {
                            int index = list.IndexOf(user);
                            if (index > -1)
                                index += 1;

                            var nextDataUser = list[index];
                            GetUserData(nextDataUser);
                        }

                        break;
                    }
                    case "LikeAndMoveCardMach":
                    {
                        list = GlobalContext?.CardFragment?.CardDateAdapter?.UsersDateList;
                        var data = list?.FirstOrDefault(a => a.Id == DataUser.Id);
                        if (data != null)
                        {
                            int index = list.IndexOf(data);
                            if (index > -1)
                                index += 1;

                            var nextDataUser = list[index];
                            GetUserData(nextDataUser);
                        }

                        break;
                    }
                    default:
                    {
                        Finish();
                        break;
                    }
                } 
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Sent Gift
        private void GiftButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                Bundle bundle = new Bundle();
                bundle.PutString("UserId", DataUser.Id.ToString());

                GiftDialogFragment mGiftFragment = new GiftDialogFragment
                {
                    Arguments = bundle
                };

                mGiftFragment.Show(SupportFragmentManager, mGiftFragment.Tag);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Favorite
        private void IconFavoriteOnClick(object sender, EventArgs e)
        {
            try
            {
                var dataUser = ListUtils.FavoriteUserList.FirstOrDefault(a => a.Id == DataUser.Id);
                var sqlEntity = new SqLiteDatabase();
                var favorite = SetFav(IconFavorite);
                if (favorite)
                {
                    if (dataUser == null)
                        ListUtils.FavoriteUserList.Add(DataUser);

                    //Insert in DB 
                    sqlEntity.InsertOrUpdate_Favorite(DataUser);

                    //User added to favorites
                    Toast.MakeText(this, GetString(Resource.String.Lbl_AddedFavorite), ToastLength.Short).Show();
                }
                else
                {
                    if (dataUser != null)
                        ListUtils.FavoriteUserList.Remove(DataUser);

                    // Remove in DB 
                    sqlEntity.Remove_Favorite(DataUser);

                    //User removed from favorites
                    Toast.MakeText(this, GetString(Resource.String.Lbl_RemovedFavorite), ToastLength.Short).Show();
                }
                sqlEntity.Dispose();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Click More (Block , Report )
        private void IconMoreOnClick(object sender, EventArgs e)
        {
            try
            {
                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(this);

                arrayAdapter.Add(GetText(Resource.String.Lbl_Block));
                arrayAdapter.Add(GetText(Resource.String.Lbl_Report));

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

        //Open WebView Website
        private void WebsiteButtonOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    Methods.App.OpenbrowserUrl(this, Website);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Instagram
        private void SocialInstagramOnClick(object sender, EventArgs e)
        {
            try
            {
                new IntentController(this).OpenInstagramIntent(Instagrem);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open LinkedIn
        private void SocialLinkedInOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://www.linkedin.com/in/" + LinkedIn);
                    new IntentController(this).OpenLinkedInIntent(LinkedIn);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Twitter
        private void SocialTwitterOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://twitter.com/"+ Twitter);

                    new IntentController(this).OpenTwitterIntent(Twitter);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Facebook
        private void SocialFacebookOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    //IMethods.IApp.OpenbrowserUrl(this, "https://www.facebook.com/"+ Facebook);

                    new IntentController(this).OpenFacebookIntent(this, Facebook);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Open Google
        private void SocialGoogleOnClick(object sender, EventArgs e)
        {
            try
            {
                if (Methods.CheckConnectivity())
                {
                    Methods.App.OpenbrowserUrl(this, "https://plus.google.com/u/0/" + Google);
                }
                else
                {
                    Toast.MakeText(this, GetString(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        private async void GetUserData(UserInfoObject dataUser,string type = "")
        {
            try
            {
                if (dataUser != null)
                {
                    if (type == "RunApi")
                    {
                        //Sent Api Visit
                       var data = await ApiRequest.GetInfoData(this,dataUser.Id.ToString());
                       if (data != null)
                       {
                           dataUser = data.Data;
                           DataUser = data.Data;
                       } 
                    }
                    else
                    {
                        //Sent Api Visit
                        PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetInfoData(this, dataUser.Id.ToString()) }); 
                        DataUser = dataUser;
                    }
                     
                    ListImageUser = new List<string>();
                    if (dataUser.Mediafiles?.Count > 0)
                        foreach (var t in dataUser.Mediafiles)
                            ListImageUser.Add(t.Avater);
                    else
                        ListImageUser.Add(dataUser.Avater);

                    ViewPagerView.Adapter = new MultiImagePagerAdapter(this, ListImageUser);
                    ViewPagerView.CurrentItem = 0;
                    CircleIndicatorView.SetViewPager(ViewPagerView);
                    ViewPagerView.Adapter.NotifyDataSetChanged();

                    Username.Text = QuickDateTools.GetNameFinal(dataUser);

                    IconOnline.Visibility = QuickDateTools.GetStatusOnline(dataUser.Lastseen, dataUser.Online) ? ViewStates.Visible : ViewStates.Gone;

                    LocationTextView.Text = !string.IsNullOrEmpty(dataUser.Location) ? dataUser.Location : GetText(Resource.String.Lbl_Unknown);

                    if (string.IsNullOrEmpty(dataUser.Interest))
                    {
                        IntersetLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        DescriptionInterest.Text = dataUser.Interest.Remove(dataUser.Interest.Length - 1, 1);
                    }

                    if (string.IsNullOrEmpty(dataUser.Language))
                    {
                        LanguagesLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        DescriptionLanguages.Text = dataUser.Language;
                    }

                    var favoriteUser = ListUtils.FavoriteUserList?.FirstOrDefault(a => a.Id == DataUser.Id);
                    IconFavorite.Tag = favoriteUser != null ? "Add" : "Added";
                    SetFav(IconFavorite);

                    try
                    {
                        string totalTextDescription = String.Empty;
                         
                        string relationship = QuickDateTools.GetRelationship(Convert.ToInt32(dataUser.Relationship)) ?? "";
                        string workStatus = QuickDateTools.GetWorkStatus(Convert.ToInt32(dataUser.WorkStatus)) ?? "";
                        string education = QuickDateTools.GetEducation(Convert.ToInt32(dataUser.Education)) ?? "";
                        string ethnicity = QuickDateTools.GetEthnicity(Convert.ToInt32(dataUser.Ethnicity)) ?? "";
                        string body = QuickDateTools.GetBody(Convert.ToInt32(dataUser.Body)) ?? "";
                        string height = dataUser.Height + " cm";
                        string hairColor = QuickDateTools.GetHairColor(Convert.ToInt32(dataUser.HairColor)) ?? "";
                        string character = QuickDateTools.GetCharacter(Convert.ToInt32(dataUser.Character)) ?? "";
                        string children = QuickDateTools.GetChildren(Convert.ToInt32(dataUser.Children)) ?? "";
                        string friends = QuickDateTools.GetFriends(Convert.ToInt32(dataUser.Friends)) ?? "";
                        string pets = QuickDateTools.GetPets(Convert.ToInt32(dataUser.Pets)) ?? "";
                        string liveWith = QuickDateTools.GetLiveWith(Convert.ToInt32(dataUser.LiveWith)) ?? "";
                        string car = QuickDateTools.GetCar(Convert.ToInt32(dataUser.Car)) ?? "";
                        string religion = QuickDateTools.GetReligion(Convert.ToInt32(dataUser.Religion)) ?? "";
                        string smoke = QuickDateTools.GetSmoke(Convert.ToInt32(dataUser.Smoke)) ?? "";
                        string drink = QuickDateTools.GetDrink(Convert.ToInt32(dataUser.Drink)) ?? "";
                        string travel = QuickDateTools.GetTravel(Convert.ToInt32(dataUser.Travel)) ?? "";
                        string music = dataUser.Music ?? "";
                        string dish = dataUser.Dish ?? "";
                        string song = dataUser.Song ?? "";
                        string hobby = dataUser.Hobby ?? "";
                        string city = dataUser.City ?? "";
                        string sport = dataUser.Sport ?? "";
                        string book = dataUser.Book ?? "";
                        string movie = dataUser.Movie ?? "";
                        string color = dataUser.Colour ?? "";
                        string tvShow = dataUser.Tv ?? "";

                        if (string.IsNullOrEmpty(workStatus))
                        {
                            WorkAndEducationLayout.Visibility = ViewStates.Gone;
                        }
                        else
                        {
                            DescriptionWork.Text = workStatus;
                        }

                        if (!string.IsNullOrEmpty(relationship))
                        {
                            relationship = GetText(Resource.String.Lbl_IAm) + " " + relationship + ", ";
                            totalTextDescription = totalTextDescription + relationship;
                        }

                        if (!string.IsNullOrEmpty(ethnicity))
                        {
                            ethnicity = ethnicity + " " + GetText(Resource.String.Lbl_person) + ", ";
                            totalTextDescription = totalTextDescription + ethnicity;
                        }

                        if (!string.IsNullOrEmpty(body))
                        {
                            body = GetText(Resource.String.Lbl_IAm) + " " + body + " ";
                            totalTextDescription = totalTextDescription + body;
                        }

                        if (!string.IsNullOrEmpty(height))
                        {
                            height = !string.IsNullOrEmpty(totalTextDescription)
                                ? GetText(Resource.String.Lbl_And) + " " + height + " " +
                                  GetText(Resource.String.Lbl_tall) + ", "
                                : height + " " + GetText(Resource.String.Lbl_tall) + ", ";

                            totalTextDescription = totalTextDescription + height;
                        }

                        if (!string.IsNullOrEmpty(hairColor))
                        {
                            hairColor = GetText(Resource.String.Lbl_IHave) + " " + hairColor + " " + GetText(Resource.String.Lbl_Hair) + ", ";
                            totalTextDescription = totalTextDescription + hairColor;
                        }

                        if (!string.IsNullOrEmpty(character))
                        {
                            character = GetText(Resource.String.Lbl_IAm) + " " + character + ", ";
                            totalTextDescription = totalTextDescription + character;
                        }

                        if (!string.IsNullOrEmpty(friends))
                        {
                            friends = GetText(Resource.String.Lbl_personAnd_IHave) + " " + friends + ", ";
                            totalTextDescription = totalTextDescription + friends;
                        }

                        if (!string.IsNullOrEmpty(children))
                        {
                            children = GetString(Resource.String.Lbl_My_plans_for_children) + " : " + children + ", ";
                            totalTextDescription = totalTextDescription + children;
                        }

                        if (!string.IsNullOrEmpty(religion))
                        {
                            religion = GetText(Resource.String.Lbl_IAm_A) + " " + religion + ", ";
                            totalTextDescription = totalTextDescription + religion;
                        }

                        if (!string.IsNullOrEmpty(liveWith))
                        {
                            if (liveWith == "Alone")
                            {
                                liveWith = GetString(Resource.String.Lbl_ILiveAlone) + ", ";
                            }
                            else
                            {
                                liveWith = GetString(Resource.String.Lbl_ILiveAlone) + " " + liveWith + ", ";
                            }

                            totalTextDescription = totalTextDescription + liveWith;
                        }

                        if (!string.IsNullOrEmpty(smoke))
                        {
                            if (smoke == "Never")
                            {
                                smoke = GetText(Resource.String.Lbl_IDontSmoke) + ", ";
                            }
                            else
                            {
                                smoke = smoke + ", ";
                            }

                            totalTextDescription = totalTextDescription + smoke;
                        }

                        if (!string.IsNullOrEmpty(drink))
                        {
                            if (drink == "Never")
                            {
                                drink = GetText(Resource.String.Lbl_IDontDrink) + ", ";
                            }
                            else
                            {
                                drink = drink + ", ";
                            }

                            totalTextDescription = totalTextDescription + drink;
                        }

                        if (!string.IsNullOrEmpty(pets))
                        {
                            pets = pets + ", ";
                            totalTextDescription = totalTextDescription + pets;
                        }

                        if (!string.IsNullOrEmpty(car))
                        {
                            car = car + ", ";
                            totalTextDescription = totalTextDescription + car;
                        }

                        if (!string.IsNullOrEmpty(music))
                        {
                            music = GetText(Resource.String.Lbl_Music) + " : " + music + ", ";
                            totalTextDescription = totalTextDescription + music;
                        }

                        if (!string.IsNullOrEmpty(dish))
                        {
                            dish = GetText(Resource.String.Lbl_Dish) + " : " + dish + ", ";
                            totalTextDescription = totalTextDescription + dish;
                        }

                        if (!string.IsNullOrEmpty(song))
                        {
                            song = GetText(Resource.String.Lbl_Song) + " : " + song + ", ";
                            totalTextDescription = totalTextDescription + song;
                        }

                        if (!string.IsNullOrEmpty(hobby))
                        {
                            hobby = GetText(Resource.String.Lbl_Hobby) + " : " + hobby + ", ";
                            totalTextDescription = totalTextDescription + hobby;
                        }

                        if (!string.IsNullOrEmpty(city))
                        {
                            city = GetText(Resource.String.Lbl_City) + " : " + city + ", ";
                            totalTextDescription = totalTextDescription + city;
                        }

                        if (!string.IsNullOrEmpty(sport))
                        {
                            sport = GetText(Resource.String.Lbl_Sport) + " : " + sport + ", ";
                            totalTextDescription = totalTextDescription + sport;
                        }

                        if (!string.IsNullOrEmpty(book))
                        {
                            book = GetText(Resource.String.Lbl_Book) + " : " + book + ", ";
                            totalTextDescription = totalTextDescription + book;
                        }

                        if (!string.IsNullOrEmpty(movie))
                        {
                            movie = GetText(Resource.String.Lbl_Movie) + " : " + movie + ", ";
                            totalTextDescription = totalTextDescription + movie;
                        }

                        if (!string.IsNullOrEmpty(color))
                        {
                            color = GetText(Resource.String.Lbl_Color) + " : " + color + ", ";
                            totalTextDescription = totalTextDescription + color;
                        }

                        if (!string.IsNullOrEmpty(tvShow))
                        {
                            tvShow = GetText(Resource.String.Lbl_TvShow) + " : " + tvShow + ", ";
                            totalTextDescription = totalTextDescription + tvShow;
                        }

                        Description.Text = totalTextDescription;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Description.Text = GetText(Resource.String.Lbl_NoDescription);
                        WorkAndEducationLayout.Visibility = ViewStates.Gone;
                    }

                    if (string.IsNullOrEmpty(dataUser.Google) && string.IsNullOrEmpty(dataUser.Facebook) && string.IsNullOrEmpty(dataUser.Linkedin) && string.IsNullOrEmpty(dataUser.Instagrem)
                        && string.IsNullOrEmpty(dataUser.Website) && string.IsNullOrEmpty(dataUser.Twitter))
                    {
                        SocialLayout.Visibility = ViewStates.Gone;
                    }
                    else
                    {
                        Typeface font = Typeface.CreateFromAsset(Application.Context.Resources.Assets, "ionicons.ttf");
                        if (!string.IsNullOrEmpty(dataUser.Google))
                        {
                            Google = dataUser.Google;
                            SocialGoogle.SetTypeface(font, TypefaceStyle.Normal);
                            SocialGoogle.Text = IonIconsFonts.SocialGoogle;
                            SocialGoogle.Visibility = ViewStates.Visible;
                        }

                        if (!string.IsNullOrEmpty(dataUser.Facebook))
                        {
                            Facebook = dataUser.Facebook;
                            SocialFacebook.SetTypeface(font, TypefaceStyle.Normal);
                            SocialFacebook.Text = IonIconsFonts.SocialFacebook;
                            SocialFacebook.Visibility = ViewStates.Visible;
                        }

                        if (!string.IsNullOrEmpty(dataUser.Linkedin))
                        {
                            LinkedIn = dataUser.Linkedin;
                            SocialLinkedIn.SetTypeface(font, TypefaceStyle.Normal);
                            SocialLinkedIn.Text = IonIconsFonts.SocialLinkedin;
                            SocialLinkedIn.Visibility = ViewStates.Visible;
                        }

                        if (!string.IsNullOrEmpty(dataUser.Instagrem))
                        {
                            Instagrem = dataUser.Instagrem;
                            SocialInstagram.SetTypeface(font, TypefaceStyle.Normal);
                            SocialInstagram.Text = IonIconsFonts.SocialInstagram;
                            SocialInstagram.Visibility = ViewStates.Visible;
                        }

                        if (!string.IsNullOrEmpty(dataUser.Website))
                        {
                            Website = dataUser.Website;
                            WebsiteButton.SetTypeface(font, TypefaceStyle.Normal);
                            WebsiteButton.Text = IonIconsFonts.AndroidGlobe;
                            WebsiteButton.Visibility = ViewStates.Visible;
                        }

                        if (!string.IsNullOrEmpty(dataUser.Twitter))
                        {
                            Twitter = dataUser.Twitter;
                            SocialTwitter.SetTypeface(font, TypefaceStyle.Normal);
                            SocialTwitter.Text = IonIconsFonts.SocialTwitter;
                            SocialTwitter.Visibility = ViewStates.Visible;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private bool SetFav(TextView favButton)
        {
            try
            {
                if (favButton.Tag.ToString() == "Added")
                {
                    favButton.SetTextColor(Color.ParseColor("#444444"));
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, favButton, IonIconsFonts.IosStarOutline);
                    favButton.Tag = "Add";
                    return false;
                }
                else
                {
                    favButton.SetTextColor(Color.ParseColor("#FFCE00"));
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, favButton, IonIconsFonts.AndroidStar);
                    favButton.Tag = "Added";
                    return true;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return false;
            }
        }

        private void GetStickersGiftsLists()
        {
            try
            {
                var sqlEntity = new SqLiteDatabase();

                var listGifts = sqlEntity.GetGiftsList();
                var listStickers = sqlEntity.GetStickersList();

                if (ListUtils.StickersList.Count == 0 && listStickers != null)
                    ListUtils.StickersList = listStickers;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetStickers(this) });

                if (ListUtils.GiftsList.Count == 0 && listGifts != null)
                    ListUtils.GiftsList = listGifts;
                else
                    PollyController.RunRetryPolicyFunction(new List<Func<Task>> { () => ApiRequest.GetGifts(this) });  

                sqlEntity.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #region Permissions

        //Permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            try
            {
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

                if (requestCode == 100)
                {
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        Intent Int = new Intent(this, typeof(MessagesBoxActivity));
                        Int.PutExtra("UserId", DataUser.Id.ToString());
                        Int.PutExtra("UserItem", JsonConvert.SerializeObject(DataUser));
                        StartActivity(Int);
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

        public async void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (itemString.ToString() == "Block")
                {
                    try
                    {
                        if (Methods.CheckConnectivity())
                        {
                            var list = GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter?.FavoritesList;
                            if (list?.Count > 0)
                            {
                                var dataFav = list.FirstOrDefault(a => a.Id == DataUser?.Id);
                                if (dataFav != null)
                                {
                                    list.Remove(dataFav);
                                    GlobalContext?.ProfileFragment?.FavoriteFragment?.MAdapter.NotifyDataSetChanged();
                                }
                            }

                            // Remove in DB 
                            var sqlEntity = new SqLiteDatabase();
                            sqlEntity.Remove_Favorite(DataUser);
                            sqlEntity.Dispose();

                            Toast.MakeText(this, GetText(Resource.String.Lbl_Blocked_successfully), ToastLength.Long).Show();

                            var (apiStatus, respond) = await RequestsAsync.Users.BlockAsync(DataUser?.Id.ToString()).ConfigureAwait(false);

                            Finish();
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                else if (itemString.ToString() == "Report")
                {
                    try
                    {
                        if (Methods.CheckConnectivity())
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_Text_report), ToastLength.Short).Show();
                            var (apiStatus, respond) = await RequestsAsync.Users.ReportAsync(DataUser?.Id.ToString()).ConfigureAwait(false);
                        }
                        else
                        {
                            Toast.MakeText(this, GetText(Resource.String.Lbl_CheckYourInternetConnection), ToastLength.Short).Show();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
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