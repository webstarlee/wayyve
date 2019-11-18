using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Ads;
using Android.OS;
using Android.Support.V7.App;
using Android.Widget;
using AndroidHUD;
using QuickDate.Helpers.Ads;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using QuickDate.SQLite;
using QuickDateClient.Classes.Global;
using QuickDateClient.Classes.Users;
using QuickDateClient.Requests;

namespace QuickDate.Activities.MyProfile
{
    [Activity(Icon = "@drawable/icon", Theme = "@style/MyTheme", ConfigurationChanges = ConfigChanges.Locale | ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class FavoriteEditActivity : AppCompatActivity
    {
        #region Variables Basic

        private TextView BackIcon, MusicIcon, DishIcon, SongIcon, HobbyIcon, CityIcon, SportIcon, BookIcon, MovieIcon, ColorIcon, TvShowIcon;
        private EditText EdtMusic, EdtDish, EdtSong, EdtHobby, EdtCity, EdtSport, EdtBook, EdtMovie, EdtColor, EdtTvShow;
        private Button BtnSave;
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
                SetContentView(Resource.Layout.ButtomSheetFavoriteEdit);

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

                MusicIcon = FindViewById<TextView>(Resource.Id.IconMusic);
                EdtMusic = FindViewById<EditText>(Resource.Id.MusicEditText);

                DishIcon = FindViewById<TextView>(Resource.Id.IconDish);
                EdtDish = FindViewById<EditText>(Resource.Id.DishEditText);

                SongIcon = FindViewById<TextView>(Resource.Id.IconSong);
                EdtSong = FindViewById<EditText>(Resource.Id.SongEditText);

                HobbyIcon = FindViewById<TextView>(Resource.Id.IconHobby);
                EdtHobby = FindViewById<EditText>(Resource.Id.HobbyEditText);

                CityIcon = FindViewById<TextView>(Resource.Id.IconCity);
                EdtCity = FindViewById<EditText>(Resource.Id.CityEditText);

                SportIcon = FindViewById<TextView>(Resource.Id.IconSport);
                EdtSport = FindViewById<EditText>(Resource.Id.SportEditText);

                BookIcon = FindViewById<TextView>(Resource.Id.IconBook);
                EdtBook = FindViewById<EditText>(Resource.Id.BookEditText);

                MovieIcon = FindViewById<TextView>(Resource.Id.IconMovie);
                EdtMovie = FindViewById<EditText>(Resource.Id.MovieEditText);

                ColorIcon = FindViewById<TextView>(Resource.Id.IconColor);
                EdtColor = FindViewById<EditText>(Resource.Id.ColorEditText);

                TvShowIcon = FindViewById<TextView>(Resource.Id.IconTvShow);
                EdtTvShow = FindViewById<EditText>(Resource.Id.TvShowEditText);

                BtnSave = FindViewById<Button>(Resource.Id.ApplyButton);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, BackIcon, IonIconsFonts.ChevronLeft);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, MusicIcon, FontAwesomeIcon.Music);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, DishIcon, FontAwesomeIcon.Fish);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeBrands, SongIcon, FontAwesomeIcon.Mandalorian);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, HobbyIcon, FontAwesomeIcon.Smile);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, CityIcon, FontAwesomeIcon.City);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, SportIcon, FontAwesomeIcon.FootballBall);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, BookIcon, FontAwesomeIcon.Book);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, MovieIcon, FontAwesomeIcon.Film);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, ColorIcon, FontAwesomeIcon.Palette);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, TvShowIcon, FontAwesomeIcon.Tv); 
           
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
                }
                else
                {
                    BackIcon.Click -= BackIconOnClick;
                    BtnSave.Click -= BtnSaveOnClick;
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
                if (Methods.CheckConnectivity())
                {
                    //Show a progress
                    AndHUD.Shared.Show(this, GetText(Resource.String.Lbl_Loading));

                    var dictionary = new Dictionary<string, string>
                    {
                        {"music", EdtMusic.Text},
                        {"dish", EdtDish.Text},
                        {"song", EdtSong.Text},
                        {"hobby",EdtHobby.Text},
                        {"city",EdtCity.Text},
                        {"sport",EdtSport.Text},
                        {"book",EdtBook.Text},
                        {"movie",EdtMovie.Text},
                        {"colour",EdtColor.Text},
                        {"tv",EdtTvShow.Text},
                    };

                    (int apiStatus, var respond) = await RequestsAsync.Users.UpdateProfileAsync(dictionary);
                    if (apiStatus == 200)
                    {
                        if (respond is UpdateProfileObject result)
                        {
                            var local = ListUtils.MyUserInfo.FirstOrDefault();
                            if (local != null)
                            {
                                local.Music = EdtMusic.Text;
                                local.Dish = EdtDish.Text;
                                local.Song = EdtSong.Text;
                                local.Hobby = EdtHobby.Text;
                                local.City = EdtCity.Text;
                                local.Sport = EdtSport.Text;
                                local.Book = EdtBook.Text;
                                local.Movie = EdtMovie.Text;
                                local.Colour = EdtColor.Text;
                                local.Tv = EdtTvShow.Text;

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
                    EdtMusic.Text = dataUser.Music;
                    EdtDish.Text = dataUser.Dish;
                    EdtSong.Text = dataUser.Song;
                    EdtHobby.Text = dataUser.Hobby;
                    EdtCity.Text = dataUser.City;
                    EdtSport.Text = dataUser.Sport;
                    EdtBook.Text = dataUser.Book;
                    EdtMovie.Text = dataUser.Movie;
                    EdtColor.Text = dataUser.Colour;
                    EdtTvShow.Text = dataUser.Tv;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}