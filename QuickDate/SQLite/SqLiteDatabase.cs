using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using QuickDate.Activities.Chat;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDate.OneSignal;
using QuickDateClient;
using QuickDateClient.Classes.Chat;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using SQLite;

namespace QuickDate.SQLite
{
    public class SqLiteDatabase : IDisposable
    {
        //############# DON'T MODIFY HERE #############
        private static readonly string Folder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public static string PathCombine = Path.Combine(Folder, "QuickDate.db");
        private SQLiteConnection Connection;

        //Open Connection in Database
        //*********************************************************

        #region Connection

        public void CheckTablesStatus()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.CreateTable<DataTables.LoginTb>();
                    Connection?.CreateTable<DataTables.SettingsTb>();
                    Connection?.CreateTable<DataTables.InfoUsersTb>();
                    Connection?.CreateTable<DataTables.GiftsTb>();
                    Connection?.CreateTable<DataTables.StickersTb>();
                    Connection?.CreateTable<DataTables.LastChatTb>();
                    Connection?.CreateTable<DataTables.MessageTb>();
                    Connection?.CreateTable<DataTables.FavoriteUsersTb>();
                
                    Connection?.Dispose();
                    Connection?.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Close Connection in Database
        public void Dispose()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.Dispose();
                    Connection?.Close();
                    GC.SuppressFinalize(this);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearAll()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.DeleteAll<DataTables.LoginTb>();
                    Connection?.DeleteAll<DataTables.SettingsTb>();
                    Connection?.DeleteAll<DataTables.InfoUsersTb>();
                    Connection?.DeleteAll<DataTables.GiftsTb>();
                    Connection?.DeleteAll<DataTables.StickersTb>();
                    Connection?.DeleteAll<DataTables.LastChatTb>();
                    Connection?.DeleteAll<DataTables.MessageTb>();
                    Connection?.DeleteAll<DataTables.FavoriteUsersTb>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        //Delete table
        public void DropAll()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection?.DropTable<DataTables.LoginTb>();
                    Connection?.DropTable<DataTables.SettingsTb>();
                    Connection?.DropTable<DataTables.InfoUsersTb>();
                    Connection?.DropTable<DataTables.GiftsTb>();
                    Connection?.DropTable<DataTables.StickersTb>();
                    Connection?.DropTable<DataTables.LastChatTb>();
                    Connection?.DropTable<DataTables.MessageTb>();
                    Connection?.DropTable<DataTables.FavoriteUsersTb>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e); 
            }
        }

        #endregion Connection

        //########################## End SQLite_Entity ##########################

        //Start SQL_Commander >>  General
        //*********************************************************

        #region General

        public void InsertRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Insert(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void UpdateRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Update(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteRow(object row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.Delete(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void InsertListOfRows(List<object> row)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.InsertAll(row);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion General

        //Start SQL_Commander >>  Custom
        //*********************************************************

        #region Login

        //Get data Login
        public DataTables.LoginTb Get_data_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        UserDetails.Username = dataUser.Username;
                        UserDetails.FullName = dataUser.Username;
                        UserDetails.Password = dataUser.Password;
                        UserDetails.AccessToken = dataUser.AccessToken;
                        UserDetails.UserId = Convert.ToInt32(dataUser.UserId);
                        UserDetails.Status = dataUser.Status;
                        UserDetails.Cookie = dataUser.Cookie;
                        UserDetails.Email = dataUser.Email;
                        UserDetails.DeviceId = dataUser.DeviceId;
                        AppSettings.Lang = dataUser.Lang;
                        Current.AccessToken = dataUser.AccessToken;

                        ListUtils.DataUserLoginList.Clear();
                        ListUtils.DataUserLoginList.Add(dataUser);

                        return dataUser;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        //Insert Or Update data Login
        public void InsertOrUpdateLogin_Credentials(DataTables.LoginTb db)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var dataUser = Connection.Table<DataTables.LoginTb>().FirstOrDefault();
                    if (dataUser != null)
                    {
                        dataUser.UserId = UserDetails.UserId.ToString();
                        dataUser.AccessToken = UserDetails.AccessToken;
                        dataUser.Cookie = UserDetails.Cookie;
                        dataUser.Username = UserDetails.Username;
                        dataUser.Password = UserDetails.Password;
                        dataUser.Status = UserDetails.Status;
                        dataUser.Lang = AppSettings.Lang;
                        dataUser.DeviceId = UserDetails.DeviceId;
                        dataUser.Email = UserDetails.Email;

                        Connection.Update(dataUser);
                    }
                    else
                    {
                        Connection.Insert(db);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Delete data Login
        public void Delete_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var data = Connection.Table<DataTables.LoginTb>().FirstOrDefault(c => c.Status == "Active");
                    if (data != null)
                    {
                        Connection.Delete(data);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Clear All data Login
        public void Clear_Login_Credentials()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.LoginTb>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Settings

        public void InsertOrUpdateSettings(GetOptionsObject.DataOptions settingsData)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    if (settingsData != null)
                    {
                        var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                        if (select == null)
                        { 
                            var db = Mapper.Map<DataTables.SettingsTb>(settingsData);

                            db.Height = JsonConvert.SerializeObject(settingsData.Height);
                            db.Notification = JsonConvert.SerializeObject(settingsData.Notification);
                            db.Gender = JsonConvert.SerializeObject(settingsData.Gender);
                            db.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            db.Countries = JsonConvert.SerializeObject(settingsData.Countries);
                            db.HairColor = JsonConvert.SerializeObject(settingsData.HairColor);
                            db.Travel = JsonConvert.SerializeObject(settingsData.Travel);
                            db.Drink = JsonConvert.SerializeObject(settingsData.Drink);
                            db.Smoke = JsonConvert.SerializeObject(settingsData.Smoke);
                            db.Religion = JsonConvert.SerializeObject(settingsData.Religion);
                            db.Car = JsonConvert.SerializeObject(settingsData.Car); 
                            db.LiveWith = JsonConvert.SerializeObject(settingsData.LiveWith);
                            db.Pets = JsonConvert.SerializeObject(settingsData.Pets);
                            db.Friends = JsonConvert.SerializeObject(settingsData.Friends);
                            db.Children = JsonConvert.SerializeObject(settingsData.Children);
                            db.Character = JsonConvert.SerializeObject(settingsData.Character);
                            db.Body = JsonConvert.SerializeObject(settingsData.Body);
                            db.Ethnicity = JsonConvert.SerializeObject(settingsData.Ethnicity);
                            db.Education = JsonConvert.SerializeObject(settingsData.Education); 
                            db.WorkStatus = JsonConvert.SerializeObject(settingsData.WorkStatus);
                            db.Relationship = JsonConvert.SerializeObject(settingsData.Relationship);
                            db.Language = JsonConvert.SerializeObject(settingsData.Language);

                            Connection.Insert(db); 
                        }
                        else
                        {
                            var db = Mapper.Map<DataTables.SettingsTb>(settingsData);

                            db.Height = JsonConvert.SerializeObject(settingsData.Height);
                            db.Notification = JsonConvert.SerializeObject(settingsData.Notification);
                            db.Gender = JsonConvert.SerializeObject(settingsData.Gender);
                            db.BlogCategories = JsonConvert.SerializeObject(settingsData.BlogCategories);
                            db.Countries = JsonConvert.SerializeObject(settingsData.Countries);
                            db.HairColor = JsonConvert.SerializeObject(settingsData.HairColor);
                            db.Travel = JsonConvert.SerializeObject(settingsData.Travel);
                            db.Drink = JsonConvert.SerializeObject(settingsData.Drink);
                            db.Smoke = JsonConvert.SerializeObject(settingsData.Smoke);
                            db.Religion = JsonConvert.SerializeObject(settingsData.Religion);
                            db.Car = JsonConvert.SerializeObject(settingsData.Car);
                            db.LiveWith = JsonConvert.SerializeObject(settingsData.LiveWith);
                            db.Pets = JsonConvert.SerializeObject(settingsData.Pets);
                            db.Friends = JsonConvert.SerializeObject(settingsData.Friends);
                            db.Children = JsonConvert.SerializeObject(settingsData.Children);
                            db.Character = JsonConvert.SerializeObject(settingsData.Character);
                            db.Body = JsonConvert.SerializeObject(settingsData.Body);
                            db.Ethnicity = JsonConvert.SerializeObject(settingsData.Ethnicity);
                            db.Education = JsonConvert.SerializeObject(settingsData.Education);
                            db.WorkStatus = JsonConvert.SerializeObject(settingsData.WorkStatus);
                            db.Relationship = JsonConvert.SerializeObject(settingsData.Relationship);
                            db.Language = JsonConvert.SerializeObject(settingsData.Language);

                            Connection.Update(db);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Settings
        public GetOptionsObject.DataOptions GetSettings()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var select = Connection.Table<DataTables.SettingsTb>().FirstOrDefault();
                    if (select != null)
                    {
                        var db = Mapper.Map<GetOptionsObject.DataOptions>(select); 
                        if (db != null)
                        {
                            GetOptionsObject.DataOptions asd = db;
                            asd.Height = new List<Dictionary<string, string>> { };

                            asd.Notification  = new List<Dictionary<string, string>> { };
                            asd.Gender  = new List<Dictionary<string, string>> { };
                            asd.BlogCategories = new List<Dictionary<string, string>> { };
                            asd.Countries = new List<Dictionary<string, GetOptionsObject.Country>> { };
                            asd.HairColor  = new List<Dictionary<string, string>> { };
                            asd.Travel  = new List<Dictionary<string, string>> { };
                            asd.Drink  = new List<Dictionary<string, string>> { };
                            asd.Smoke  = new List<Dictionary<string, string>> { };
                            asd.Religion  = new List<Dictionary<string, string>> { };
                            asd.Car  = new List<Dictionary<string, string>> { };
                            asd.LiveWith  = new List<Dictionary<string, string>> { };
                            asd.Pets  = new List<Dictionary<string, string>> { };
                            asd.Friends  = new List<Dictionary<string, string>> { };
                            asd.Children  = new List<Dictionary<string, string>> { };
                            asd.Character  = new List<Dictionary<string, string>> { };
                            asd.Body  = new List<Dictionary<string, string>> { };
                            asd.Ethnicity  = new List<Dictionary<string, string>> { };
                            asd.Education  = new List<Dictionary<string, string>> { };
                            asd.WorkStatus  = new List<Dictionary<string, string>> { };
                            asd.Relationship  = new List<Dictionary<string, string>> { };
                            asd.Language  = new List<Dictionary<string, string>> { };


                            if (!string.IsNullOrEmpty(select.Height))
                                asd.Height = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Height);

                            if (!string.IsNullOrEmpty(select.Notification))
                                asd.Notification = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Notification);

                            if (!string.IsNullOrEmpty(select.BlogCategories))
                                asd.BlogCategories = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.BlogCategories);

                            if (!string.IsNullOrEmpty(select.Gender))
                                asd.Gender = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Gender);

                            if (!string.IsNullOrEmpty(select.Countries))
                                asd.Countries = JsonConvert.DeserializeObject<List<Dictionary<string, GetOptionsObject.Country>>>(select.Countries);

                            if (!string.IsNullOrEmpty(select.HairColor))
                                asd.HairColor = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.HairColor);

                            if (!string.IsNullOrEmpty(select.Travel))
                                asd.Travel = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Travel);

                            if (!string.IsNullOrEmpty(select.Drink))
                                asd.Drink = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Drink);

                            if (!string.IsNullOrEmpty(select.Smoke))
                                asd.Smoke = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Smoke);

                            if (!string.IsNullOrEmpty(select.Religion))
                                asd.Religion = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Religion);

                            if (!string.IsNullOrEmpty(select.Car))
                                asd.Car = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Car);

                            if (!string.IsNullOrEmpty(select.LiveWith))
                                asd.LiveWith = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.LiveWith);

                            if (!string.IsNullOrEmpty(select.Pets))
                                asd.Pets = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Pets);

                            if (!string.IsNullOrEmpty(select.Friends))
                                asd.Friends = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Friends);

                            if (!string.IsNullOrEmpty(select.Children))
                                asd.Children = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Children);

                            if (!string.IsNullOrEmpty(select.Character))
                                asd.Character = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Character);

                            if (!string.IsNullOrEmpty(select.Body))
                                asd.Body = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Body);

                            if (!string.IsNullOrEmpty(select.Ethnicity))
                                asd.Ethnicity = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Ethnicity);

                            if (!string.IsNullOrEmpty(select.Education))
                                asd.Education = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Education);

                            if (!string.IsNullOrEmpty(select.WorkStatus))
                                asd.WorkStatus = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.WorkStatus);

                            if (!string.IsNullOrEmpty(select.Relationship))
                                asd.Relationship = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Relationship);

                            if (!string.IsNullOrEmpty(select.Language))
                                asd.Language = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(select.Language);


                            AppSettings.OneSignalAppId = asd.PushId;
                            OneSignalNotification.RegisterNotificationDevice();

                            ListUtils.SettingsSiteList.Clear();
                            ListUtils.SettingsSiteList.Add(db);

                            return db;
                        } 
                    }
                    return null;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        #endregion
        
        #region My Info Data

        //Insert Or Update data MyInfo 
        public void InsertOrUpdate_DataMyInfo(UserInfoObject info)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var resultInfoTb = Connection.Table<DataTables.InfoUsersTb>().FirstOrDefault();
                    if (resultInfoTb != null)
                    {
                        var db = Mapper.Map<DataTables.InfoUsersTb>(info);
                        db.ProfileCompletionMissing = JsonConvert.SerializeObject(info.ProfileCompletionMissing);
                        db.Mediafiles = JsonConvert.SerializeObject(info.Mediafiles);
                        db.Likes = JsonConvert.SerializeObject(info.Likes);
                        db.Blocks = JsonConvert.SerializeObject(info.Blocks);
                        db.Payments = JsonConvert.SerializeObject(info.Payments);
                        db.Reports = JsonConvert.SerializeObject(info.Reports);
                        db.Visits = JsonConvert.SerializeObject(info.Visits);
                        Connection.Update(db);
                    }
                    else
                    {
                        var db = Mapper.Map<DataTables.InfoUsersTb>(info);
                        db.ProfileCompletionMissing = JsonConvert.SerializeObject(info.ProfileCompletionMissing);
                        db.Mediafiles = JsonConvert.SerializeObject(info.Mediafiles);
                        db.Likes = JsonConvert.SerializeObject(info.Likes);
                        db.Blocks = JsonConvert.SerializeObject(info.Blocks);
                        db.Payments = JsonConvert.SerializeObject(info.Payments);
                        db.Reports = JsonConvert.SerializeObject(info.Reports);
                        db.Visits = JsonConvert.SerializeObject(info.Visits); 
                        Connection.Insert(db);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Data My Info
        public UserInfoObject GetDataMyInfo()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    DataTables.InfoUsersTb myInfo = Connection.Table<DataTables.InfoUsersTb>().FirstOrDefault();
                    if (myInfo != null)
                    {
                        UserInfoObject infoObject = new UserInfoObject()
                        {
                            Id = myInfo.Id,
                            Username = myInfo.Username,
                            Email = myInfo.Email,
                            FirstName = myInfo.FirstName,
                            LastName = myInfo.LastName,
                            Avater = myInfo.Avater,
                            Address = myInfo.Address,
                            Gender = myInfo.Gender,
                            Facebook = myInfo.Facebook,
                            Google = myInfo.Google,
                            Twitter = myInfo.Twitter,
                            Linkedin = myInfo.Linkedin,
                            Website = myInfo.Website,
                            Instagrem = myInfo.Instagrem,
                            WebDeviceId = myInfo.WebDeviceId,
                            Language = myInfo.Language,
                            Src = myInfo.Src,
                            IpAddress = myInfo.IpAddress,
                            Type = myInfo.Type,
                            PhoneNumber = myInfo.PhoneNumber,
                            Timezone = myInfo.Timezone,
                            Lat = myInfo.Lat,
                            Lng = myInfo.Lng,
                            About = myInfo.About,
                            Birthday = myInfo.Birthday,
                            Country = myInfo.Country,
                            Registered = myInfo.Registered,
                            Lastseen = myInfo.Lastseen,
                            SmsCode = myInfo.SmsCode,
                            ProTime = myInfo.ProTime,
                            LastLocationUpdate = myInfo.LastLocationUpdate,
                            Balance = myInfo.Balance,
                            Verified = myInfo.Verified,
                            Status = myInfo.Status,
                            Active = myInfo.Active,
                            Admin = myInfo.Admin,
                            StartUp = myInfo.StartUp,
                            IsPro = myInfo.IsPro,
                            ProType = myInfo.ProType,
                            SocialLogin = myInfo.SocialLogin,
                            CreatedAt = myInfo.CreatedAt,
                            UpdatedAt = myInfo.UpdatedAt,
                            DeletedAt = myInfo.DeletedAt,
                            MobileDeviceId = myInfo.MobileDeviceId,
                            WebToken = myInfo.WebToken,
                            MobileToken = myInfo.MobileToken,
                            Height = myInfo.Height,
                            HairColor = myInfo.HairColor,
                            WebTokenCreatedAt = myInfo.WebTokenCreatedAt,
                            MobileTokenCreatedAt = myInfo.MobileTokenCreatedAt,
                            MobileDevice = myInfo.MobileDevice,
                            Interest = myInfo.Interest,
                            Location = myInfo.Location,
                            Relationship = myInfo.Relationship,
                            WorkStatus = myInfo.WorkStatus,
                            Education = myInfo.Education,
                            Ethnicity = myInfo.Ethnicity,
                            Body = myInfo.Body,
                            Character = myInfo.Character,
                            Children = myInfo.Children,
                            Friends = myInfo.Friends,
                            Pets = myInfo.Pets,
                            LiveWith = myInfo.LiveWith,
                            Car = myInfo.Car,
                            Religion = myInfo.Religion,
                            Smoke = myInfo.Smoke,
                            Drink = myInfo.Drink,
                            Travel = myInfo.Travel,
                            Music = myInfo.Music,
                            Dish = myInfo.Dish,
                            Song = myInfo.Song,
                            Hobby = myInfo.Hobby,
                            City = myInfo.City,
                            Sport = myInfo.Sport,
                            Book = myInfo.Book,
                            Movie = myInfo.Movie,
                            Colour = myInfo.Colour,
                            Tv = myInfo.Tv,
                            PrivacyShowProfileOnGoogle = myInfo.PrivacyShowProfileOnGoogle,
                            PrivacyShowProfileRandomUsers = myInfo.PrivacyShowProfileRandomUsers,
                            PrivacyShowProfileMatchProfiles = myInfo.PrivacyShowProfileMatchProfiles,
                            EmailOnProfileView = myInfo.EmailOnProfileView,
                            EmailOnNewMessage = myInfo.EmailOnNewMessage,
                            EmailOnProfileLike = myInfo.EmailOnProfileLike,
                            EmailOnPurchaseNotifications = myInfo.EmailOnPurchaseNotifications,
                            EmailOnSpecialOffers = myInfo.EmailOnSpecialOffers,
                            EmailOnAnnouncements = myInfo.EmailOnAnnouncements,
                            PhoneVerified = myInfo.PhoneVerified,
                            Online = myInfo.Online,
                            IsBoosted = myInfo.IsBoosted,
                            BoostedTime = myInfo.BoostedTime,
                            IsBuyStickers = myInfo.IsBuyStickers,
                            UserBuyXvisits = myInfo.UserBuyXvisits,
                            XvisitsCreatedAt = myInfo.XvisitsCreatedAt,
                            UserBuyXmatches = myInfo.UserBuyXmatches,
                            XmatchesCreatedAt = myInfo.XmatchesCreatedAt,
                            UserBuyXlikes = myInfo.UserBuyXlikes,
                            XlikesCreatedAt = myInfo.XlikesCreatedAt,
                            VerifiedFinal = myInfo.VerifiedFinal,
                            Fullname = myInfo.Fullname,
                            Age = myInfo.Age,
                            LastseenTxt = myInfo.LastseenTxt,
                            LastseenDate = myInfo.LastseenDate,
                            IsOwner = myInfo.IsOwner,
                            IsLiked = myInfo.IsLiked,
                            IsBlocked = myInfo.IsBlocked,
                            ProfileCompletion = myInfo.ProfileCompletion,
                            ProfileCompletionMissing = new List<string>(),
                            Mediafiles = new List<MediaFile>(),
                            Likes = new List<Like>(),
                            Blocks = new List<Block>(),
                            Payments = new List<Payment>(),
                            Reports = new List<Report>(),
                            Visits = new List<Visit>(),
                            CountryTxt = myInfo.CountryTxt,
                            GenderTxt = myInfo.GenderTxt,
                            Instagram = myInfo.Instagram,
                            LanguageTxt = myInfo.LanguageTxt,
                            EmailCode = myInfo.EmailCode,
                            HeightTxt = myInfo.HeightTxt,
                            HairColorTxt = myInfo.HairColorTxt,
                            RelationshipTxt = myInfo.RelationshipTxt,
                            WorkStatusTxt = myInfo.WorkStatusTxt,
                            EducationTxt = myInfo.EducationTxt,
                            EthnicityTxt = myInfo.EthnicityTxt,
                            BodyTxt = myInfo.BodyTxt,
                            CharacterTxt = myInfo.CharacterTxt,
                            ChildrenTxt = myInfo.ChildrenTxt,
                            FriendsTxt = myInfo.FriendsTxt,
                            PetsTxt = myInfo.PetsTxt,
                            LiveWithTxt = myInfo.LiveWithTxt,
                            CarTxt = myInfo.CarTxt,
                            ReligionTxt = myInfo.ReligionTxt,
                            SmokeTxt = myInfo.SmokeTxt,
                            DrinkTxt = myInfo.DrinkTxt,
                            TravelTxt = myInfo.TravelTxt,
                            ShowMeTo = myInfo.ShowMeTo,
                            EmailOnGetGift = myInfo.EmailOnGetGift,
                            EmailOnGotNewMatch = myInfo.EmailOnGotNewMatch,
                            EmailOnChatRequest = myInfo.EmailOnChatRequest,
                            LastEmailSent = myInfo.LastEmailSent, 
                        };
                         
                        infoObject.ProfileCompletionMissing = JsonConvert.DeserializeObject<List<string>>(myInfo.ProfileCompletionMissing);
                        infoObject.Mediafiles = JsonConvert.DeserializeObject<List<MediaFile>>(myInfo.Mediafiles);
                        infoObject.Likes = JsonConvert.DeserializeObject<List<Like>>(myInfo.Likes);
                        infoObject.Blocks = JsonConvert.DeserializeObject<List<Block>>(myInfo.Blocks);
                        infoObject.Payments = JsonConvert.DeserializeObject<List<Payment>>(myInfo.Payments);
                        infoObject.Reports = JsonConvert.DeserializeObject<List<Report>>(myInfo.Reports);
                        infoObject.Visits = JsonConvert.DeserializeObject<List<Visit>>(myInfo.Visits);
                     
                        ListUtils.MyUserInfo.Clear();
                        ListUtils.MyUserInfo.Add(infoObject);
                     
                        return infoObject;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
         
        #endregion

        #region Gifts

        //Insert data Gifts
        public void InsertAllGifts(ObservableCollection<DataFile> listData)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.GiftsTb>().ToList();

                    List<DataTables.GiftsTb> list = listData.Select(gift => new DataTables.GiftsTb
                    {
                        IdGifts = gift.Id,
                        File = gift.File,
                    }).ToList();

                    if (list.Count <= 0) return;
                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.IdGifts).Contains(c.IdGifts)).ToList();
                    if (newItemList.Count > 0)
                    {
                        Connection.InsertAll(newItemList);
                    }
                         
                    Connection.UpdateAll(list);
                    Connection.Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get List Gifts 
        public ObservableCollection<DataFile> GetGiftsList()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.GiftsTb>().ToList();
                    if (result?.Count > 0)
                    {
                        List<DataFile> list = result.Select(gift => new DataFile
                        {
                            Id = gift.IdGifts,
                            File = gift.File,
                        }).ToList();
                        
                        return new ObservableCollection<DataFile>(list);
                    }
                    else
                    {
                        return new ObservableCollection<DataFile>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<DataFile>();
            }
        }

        #endregion

        #region Stickers

        //Insert data Stickers
        public void InsertAllStickers(ObservableCollection<DataFile> listData)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.StickersTb>().ToList();
                    List<DataTables.StickersTb> list = listData.Select(stickers => new DataTables.StickersTb
                    {
                        IdStickers = stickers.Id,
                        File = stickers.File,
                    }).ToList();

                    if (list.Count <= 0) return;
                    Connection.BeginTransaction();
                    //Bring new  
                    var newItemList = list.Where(c => !result.Select(fc => fc.IdStickers).Contains(c.IdStickers)).ToList();
                    if (newItemList.Count > 0)
                    {
                        Connection.InsertAll(newItemList);
                    }

                    Connection.UpdateAll(list);
                    Connection.Commit();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get List Stickers 
        public ObservableCollection<DataFile> GetStickersList()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var result = Connection.Table<DataTables.StickersTb>().ToList();
                    if (result?.Count > 0)
                    {
                        List<DataFile> list = result.Select(stickers => new DataFile
                        {
                            Id = stickers.IdStickers,
                            File = stickers.File,
                        }).ToList();
                         
                        return new ObservableCollection<DataFile>(list);
                    }
                    else
                    {
                        return new ObservableCollection<DataFile>();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<DataFile>();
            }
        }

        #endregion
        
        #region Last Chat

        //Insert data To Last Chat Table
        public void InsertOrReplaceLastChatTable(ObservableCollection<GetConversationListObject.DataConversation> usersContactList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {

                    var result = Connection.Table<DataTables.LastChatTb>().ToList();
                    var list = usersContactList.Select(user => new DataTables.LastChatTb
                    {
                        Id = user.Id,
                        Owner = user.Owner,
                        Seen = user.Seen,
                        Text = user.Text,
                        Media = user.Media,
                        Sticker = user.Sticker,
                        Time = user.Time,
                        CreatedAt = user.CreatedAt,
                        UserId = user.User.Id.ToString(),
                        UserDataJson = JsonConvert.SerializeObject(user.User),
                        NewMessages = user.NewMessages,
                        MessageType = user.MessageType,
                        ToId = user.ToId.ToString(),
                        FromId = user.FromId.ToString(),
                    }).ToList();

                    if (list.Count > 0)
                    {
                        Connection.BeginTransaction();
                        //Bring new  
                        var newItemList = list.Where(c => !result.Select(fc => fc.Id).Contains(c.Id)).ToList();
                        if (newItemList.Count > 0)
                        {
                            Connection.InsertAll(newItemList);
                        }

                        var deleteItemList = result.Where(c => !list.Select(fc => fc.Id).Contains(c.Id)).ToList();
                        if (deleteItemList.Count > 0)
                        {
                            foreach (var delete in deleteItemList)
                            {
                                Connection.Delete(delete);
                            }
                        }

                        Connection.UpdateAll(list);
                        Connection.Commit();
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To LastChat Table
        public ObservableCollection<GetConversationListObject.DataConversation> GetAllLastChat()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var select = Connection.Table<DataTables.LastChatTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = select.Select(user => new GetConversationListObject.DataConversation
                        {
                            Id = user.Id,
                            Owner = user.Owner,
                            Seen = user.Seen,
                            Text = user.Text,
                            Media = user.Media,
                            Sticker = user.Sticker,
                            Time = user.Time,
                            CreatedAt = user.CreatedAt,
                            User = JsonConvert.DeserializeObject<UserInfoObject>(user.UserDataJson),
                            NewMessages = user.NewMessages,
                            MessageType = user.MessageType,
                            ToId = Convert.ToInt32(user.ToId),
                            FromId = Convert.ToInt32(user.FromId),  
                        }).ToList();

                        return new ObservableCollection<GetConversationListObject.DataConversation>(list);
                    }
                    else
                        return new ObservableCollection<GetConversationListObject.DataConversation>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<GetConversationListObject.DataConversation>();
            }
        }

        // Get data To LastChat Table By Id >> Load More
        public ObservableCollection<GetConversationListObject.DataConversation> GetLastChatById(int id, int nSize)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var query = Connection.Table<DataTables.LastChatTb>().Where(w => w.AutoIdLastChat >= id)
                        .OrderBy(q => q.AutoIdLastChat).Take(nSize).ToList();
                    if (query.Count > 0)
                    {
                        var list = query.Select(user => new GetConversationListObject.DataConversation
                        {
                            Id = user.Id,
                            Owner = user.Owner,
                            Seen = user.Seen,
                            Text = user.Text,
                            Media = user.Media,
                            Sticker = user.Sticker,
                            Time = user.Time,
                            CreatedAt = user.CreatedAt,
                            User = JsonConvert.DeserializeObject<UserInfoObject>(user.UserDataJson),
                            NewMessages = user.NewMessages,
                            MessageType = user.MessageType,
                        }).ToList();

                        if (list.Count > 0)
                            return new ObservableCollection<GetConversationListObject.DataConversation>(list);
                        else
                            return new ObservableCollection<GetConversationListObject.DataConversation>();
                    }
                    else
                        return new ObservableCollection<GetConversationListObject.DataConversation>();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new ObservableCollection<GetConversationListObject.DataConversation>();
            }
        }

        //Remove data To LastChat Table
        public void DeleteUserLastChat(string userId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.LastChatTb>().FirstOrDefault(c => c.UserId.ToString() == userId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Clear All data LastChat
        public void ClearLastChat()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    Connection.DeleteAll<DataTables.LastChatTb>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion
         
        #region Message

        //Insert data To Message Table
        public void InsertOrReplaceMessages(ObservableCollection<GetChatConversationsObject.Messages> messageList)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    List<DataTables.MessageTb> listOfDatabaseForInsert = new List<DataTables.MessageTb>();
                    List<DataTables.MessageTb> listOfDatabaseForUpdate = new List<DataTables.MessageTb>();

                    // get data from database
                    var resultMessage = Connection.Table<DataTables.MessageTb>().ToList();
                    var listAllMessage = resultMessage.Select(messages => new GetChatConversationsObject.Messages()
                    {
                        Id = messages.Id,
                        FromName = messages.FromName,
                        FromAvater = messages.FromAvater,
                        ToName = messages.ToName,
                        ToAvater = messages.ToAvater,
                        From = messages.FromId,
                        To = messages.ToId,
                        Text = messages.Text,
                        Media = messages.Media,
                        FromDelete = messages.FromDelete,
                        ToDelete = messages.ToDelete,
                        Sticker = messages.Sticker,
                        CreatedAt = messages.CreatedAt,
                        Seen = messages.Seen,
                        Type = messages.Type,
                        MessageType = messages.MessageType,
                    }).ToList();

                    foreach (var messages in messageList)
                    {
                        DataTables.MessageTb maTb = new DataTables.MessageTb()
                        {
                            Id = messages.Id,
                            FromName = messages.FromName,
                            FromAvater = messages.FromAvater,
                            ToName = messages.ToName,
                            ToAvater = messages.ToAvater,
                            FromId = messages.From,
                            ToId = messages.To,
                            Text = messages.Text,
                            Media = messages.Media,
                            FromDelete = messages.FromDelete,
                            ToDelete = messages.ToDelete,
                            Sticker = messages.Sticker,
                            CreatedAt = messages.CreatedAt,
                            Seen = messages.Seen,
                            Type = messages.Type,
                            MessageType = messages.MessageType,
                        };

                        var dataCheck = listAllMessage.FirstOrDefault(a => a.Id == messages.Id);
                        if (dataCheck != null)
                        {
                            var checkForUpdate = resultMessage.FirstOrDefault(a => a.Id == dataCheck.Id);
                            if (checkForUpdate != null)
                            {
                                checkForUpdate.Id = messages.Id;
                                checkForUpdate.FromName = messages.FromName;
                                checkForUpdate.FromAvater = messages.FromAvater;
                                checkForUpdate.ToName = messages.ToName;
                                checkForUpdate.ToAvater = messages.ToAvater;
                                checkForUpdate.FromId = messages.From;
                                checkForUpdate.ToId = messages.To;
                                checkForUpdate.Text = messages.Text;
                                checkForUpdate.Media = messages.Media;
                                checkForUpdate.FromDelete = messages.FromDelete;
                                checkForUpdate.ToDelete = messages.ToDelete;
                                checkForUpdate.Sticker = messages.Sticker;
                                checkForUpdate.CreatedAt = messages.CreatedAt;
                                checkForUpdate.Seen = messages.Seen;
                                checkForUpdate.Type = messages.Type;
                                checkForUpdate.MessageType = messages.MessageType;

                                listOfDatabaseForUpdate.Add(checkForUpdate);
                            }
                            else
                            {
                                listOfDatabaseForInsert.Add(maTb);
                            }
                        }
                        else
                        {
                            listOfDatabaseForInsert.Add(maTb);
                        }
                    }

                    Connection.BeginTransaction();

                    //Bring new  
                    if (listOfDatabaseForInsert.Count > 0)
                    {
                        Connection.InsertAll(listOfDatabaseForInsert);
                    }

                    if (listOfDatabaseForUpdate.Count > 0)
                    {
                        Connection.UpdateAll(listOfDatabaseForUpdate);
                    }

                    Connection.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Update one Messages Table
        public void InsertOrUpdateToOneMessages(GetChatConversationsObject.Messages message)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {

                    var data = Connection.Table<DataTables.MessageTb>().FirstOrDefault(a => a.Id == message.Id);
                    if (data != null)
                    {
                        data.Id = message.Id;
                        data.FromName = message.FromName;
                        data.FromAvater = message.FromAvater;
                        data.ToName = message.ToName;
                        data.ToAvater = message.ToAvater;
                        data.FromId = message.From;
                        data.ToId = message.To;
                        data.Text = message.Text;
                        data.Media = message.Media;
                        data.FromDelete = message.FromDelete;
                        data.ToDelete = message.ToDelete;
                        data.Sticker = message.Sticker;
                        data.CreatedAt = message.CreatedAt;
                        data.Seen = message.Seen;
                        data.Type = message.Type;
                        data.MessageType = message.MessageType;
                        Connection.Update(data);
                    }
                    else
                    {
                        DataTables.MessageTb mdb = new DataTables.MessageTb
                        {
                            Id = message.Id,
                            FromName = message.FromName,
                            FromAvater = message.FromAvater,
                            ToName = message.ToName,
                            ToAvater = message.ToAvater,
                            FromId = message.From,
                            ToId = message.To,
                            Text = message.Text,
                            Media = message.Media,
                            FromDelete = message.FromDelete,
                            ToDelete = message.ToDelete,
                            Sticker = message.Sticker,
                            CreatedAt = message.CreatedAt,
                            Seen = message.Seen,
                            Type = message.Type,
                            MessageType = message.MessageType,
                        };

                        //Insert  one Messages Table
                        Connection.Insert(mdb);
                    }


                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Get data To Messages
       public string GetMessagesList(int fromId, int toId, int beforeMessageId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var beforeQ = "";
                    if (beforeMessageId != 0)
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);
                    List<DataTables.MessageTb> queryList = query.Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId).OrderBy(q => q.CreatedAt).TakeLast(35).ToList();
                    if (queryList.Count > 0)
                    {
                        foreach (var m in queryList.Select(message => new GetChatConversationsObject.Messages
                        {
                            Id = message.Id,
                            FromName = message.FromName,
                            FromAvater = message.FromAvater,
                            ToName = message.ToName,
                            ToAvater = message.ToAvater,
                            From = message.FromId,
                            To = message.ToId,
                            Text = message.Text,
                            Media = message.Media,
                            FromDelete = message.FromDelete,
                            ToDelete = message.ToDelete,
                            Sticker = message.Sticker,
                            CreatedAt = message.CreatedAt,
                            Seen = message.Seen,
                            Type = message.Type,
                            MessageType = message.MessageType,
                        }))
                        {
                            if (beforeMessageId == 0)
                            {
                                if (MessagesBoxActivity.MAdapter != null)
                                {
                                    MessagesBoxActivity.MAdapter.MessageList.Add(m);

                                    int index = MessagesBoxActivity.MAdapter.MessageList.IndexOf(MessagesBoxActivity.MAdapter.MessageList.Last());
                                    if (index > -1)
                                    { 
                                        MessagesBoxActivity.MAdapter.NotifyItemInserted(index);

                                        //Scroll Down >> 
                                        MessagesBoxActivity.GetInstance()?.ChatBoxRecyclerView.ScrollToPosition(index); 
                                    }
                                }
                            }
                            else
                            {
                                MessagesBoxActivity.MAdapter?.MessageList.Insert(0, m);
                                MessagesBoxActivity.MAdapter?.NotifyItemInserted(MessagesBoxActivity.MAdapter.MessageList.IndexOf(MessagesBoxActivity.MAdapter.MessageList.FirstOrDefault()));

                                var index = MessagesBoxActivity.MAdapter?.MessageList.FirstOrDefault(a => a.Id == beforeMessageId);
                                if (index != null)
                                {
                                    MessagesBoxActivity.MAdapter?.NotifyItemChanged(MessagesBoxActivity.MAdapter.MessageList.IndexOf(index));
                                    //Scroll Down >> 
                                    MessagesBoxActivity.GetInstance()?.ChatBoxRecyclerView.ScrollToPosition(MessagesBoxActivity.MAdapter.MessageList.IndexOf(MessagesBoxActivity.MAdapter.MessageList.Last()));

                                } 
                            }
                        }

                        return "1";
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "0";
            }
        }

        //Get data To where first Messages >> load more
        public List<DataTables.MessageTb> GetMessageList(int fromId, int toId, int beforeMessageId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                { 
                    var beforeQ = "";
                    if (beforeMessageId != 0)
                    {
                        beforeQ = "AND Id < " + beforeMessageId + " AND Id <> " + beforeMessageId + " ";
                    }

                    var query = Connection.Query<DataTables.MessageTb>("SELECT * FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + ")) " + beforeQ);
                    List<DataTables.MessageTb> queryList = query
                        .Where(w => w.FromId == fromId && w.ToId == toId || w.ToId == fromId && w.FromId == toId)
                        .OrderBy(q => q.CreatedAt).TakeLast(35).ToList();
                    return queryList;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<DataTables.MessageTb>();
            }
        }

        //Remove data To Messages Table
        public void Delete_OneMessageUser(int messageId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var user = Connection.Table<DataTables.MessageTb>().FirstOrDefault(c => c.Id == messageId);
                    if (user != null)
                    {
                        Connection.Delete(user);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void DeleteAllMessagesUser(string fromId, string toId)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var query = Connection.Query<DataTables.MessageTb>("Delete FROM MessageTb WHERE ((FromId =" + fromId + " and ToId=" + toId + ") OR (FromId =" + toId + " and ToId=" + fromId + "))");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //Remove All data To Messages Table
        public void ClearAll_Messages()
        {
            try
            {
                Connection.DeleteAll<DataTables.MessageTb>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Favorite

        //Insert Or Update Favorite 
        public void InsertOrUpdate_Favorite(UserInfoObject info)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var resultInfoTb = Connection.Table<DataTables.FavoriteUsersTb>().FirstOrDefault(a => a.Id == info.Id);
                    if (resultInfoTb != null)
                    {
                        var db = Mapper.Map<DataTables.FavoriteUsersTb>(info);
                        db.Mediafiles = JsonConvert.SerializeObject(info.Mediafiles);
                        Connection.Update(db);
                    }
                    else
                    {
                        var db = Mapper.Map<DataTables.FavoriteUsersTb>(info);
                        db.Mediafiles = JsonConvert.SerializeObject(info.Mediafiles);
                        Connection.Insert(db);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void Remove_Favorite(UserInfoObject info)
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                {
                    var resultInfoTb = Connection.Table<DataTables.FavoriteUsersTb>().FirstOrDefault(a => a.Id == info.Id);
                    if (resultInfoTb != null)
                    {
                        Connection.Delete(resultInfoTb);
                    } 
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Get Data Favorite
        public ObservableCollection<UserInfoObject> GetDataFavorite()
        {
            try
            {
                using (Connection = new SQLiteConnection(PathCombine))
                { 
                    List<DataTables.FavoriteUsersTb> select = Connection.Table<DataTables.FavoriteUsersTb>().ToList();
                    if (select.Count > 0)
                    {
                        var list = new ObservableCollection<UserInfoObject>();
                        foreach (var item in select)
                        {
                            UserInfoObject infoObject = new UserInfoObject()
                            {
                                Id = item.Id,
                                Username = item.Username,
                                Email = item.Email,
                                FirstName = item.FirstName,
                                LastName = item.LastName,
                                Avater = item.Avater,
                                Address = item.Address,
                                Gender = item.Gender,
                                Facebook = item.Facebook,
                                Google = item.Google,
                                Twitter = item.Twitter,
                                Linkedin = item.Linkedin,
                                Website = item.Website,
                                Instagrem = item.Instagrem,
                                WebDeviceId = item.WebDeviceId,
                                Language = item.Language,
                                Src = item.Src,
                                IpAddress = item.IpAddress,
                                Type = item.Type,
                                PhoneNumber = item.PhoneNumber,
                                Timezone = item.Timezone,
                                Lat = item.Lat,
                                Lng = item.Lng,
                                About = item.About,
                                Birthday = item.Birthday,
                                Country = item.Country,
                                Registered = item.Registered,
                                Lastseen = item.Lastseen,
                                SmsCode = item.SmsCode,
                                ProTime = item.ProTime,
                                LastLocationUpdate = item.LastLocationUpdate,
                                Balance = item.Balance,
                                Verified = item.Verified,
                                Status = item.Status,
                                Active = item.Active,
                                Admin = item.Admin,
                                StartUp = item.StartUp,
                                IsPro = item.IsPro,
                                ProType = item.ProType,
                                SocialLogin = item.SocialLogin,
                                CreatedAt = item.CreatedAt,
                                UpdatedAt = item.UpdatedAt,
                                DeletedAt = item.DeletedAt,
                                MobileDeviceId = item.MobileDeviceId,
                                WebToken = item.WebToken,
                                MobileToken = item.MobileToken,
                                Height = item.Height,
                                HairColor = item.HairColor,
                                WebTokenCreatedAt = item.WebTokenCreatedAt,
                                MobileTokenCreatedAt = item.MobileTokenCreatedAt,
                                MobileDevice = item.MobileDevice,
                                Interest = item.Interest,
                                Location = item.Location,
                                Relationship = item.Relationship,
                                WorkStatus = item.WorkStatus,
                                Education = item.Education,
                                Ethnicity = item.Ethnicity,
                                Body = item.Body,
                                Character = item.Character,
                                Children = item.Children,
                                Friends = item.Friends,
                                Pets = item.Pets,
                                LiveWith = item.LiveWith,
                                Car = item.Car,
                                Religion = item.Religion,
                                Smoke = item.Smoke,
                                Drink = item.Drink,
                                Travel = item.Travel,
                                Music = item.Music,
                                Dish = item.Dish,
                                Song = item.Song,
                                Hobby = item.Hobby,
                                City = item.City,
                                Sport = item.Sport,
                                Book = item.Book,
                                Movie = item.Movie,
                                Colour = item.Colour,
                                Tv = item.Tv,
                                PrivacyShowProfileOnGoogle = item.PrivacyShowProfileOnGoogle,
                                PrivacyShowProfileRandomUsers = item.PrivacyShowProfileRandomUsers,
                                PrivacyShowProfileMatchProfiles = item.PrivacyShowProfileMatchProfiles,
                                EmailOnProfileView = item.EmailOnProfileView,
                                EmailOnNewMessage = item.EmailOnNewMessage,
                                EmailOnProfileLike = item.EmailOnProfileLike,
                                EmailOnPurchaseNotifications = item.EmailOnPurchaseNotifications,
                                EmailOnSpecialOffers = item.EmailOnSpecialOffers,
                                EmailOnAnnouncements = item.EmailOnAnnouncements,
                                PhoneVerified = item.PhoneVerified,
                                Online = item.Online,
                                IsBoosted = item.IsBoosted,
                                BoostedTime = item.BoostedTime,
                                IsBuyStickers = item.IsBuyStickers,
                                UserBuyXvisits = item.UserBuyXvisits,
                                XvisitsCreatedAt = item.XvisitsCreatedAt,
                                UserBuyXmatches = item.UserBuyXmatches,
                                XmatchesCreatedAt = item.XmatchesCreatedAt,
                                UserBuyXlikes = item.UserBuyXlikes,
                                XlikesCreatedAt = item.XlikesCreatedAt,
                                VerifiedFinal = item.VerifiedFinal,
                                Fullname = item.Fullname,
                                Age = item.Age,
                                LastseenTxt = item.LastseenTxt,
                                LastseenDate = item.LastseenDate,
                                IsOwner = item.IsOwner,
                                IsLiked = item.IsLiked,
                                IsBlocked = item.IsBlocked,
                                ProfileCompletion = item.ProfileCompletion,
                                ProfileCompletionMissing = new List<string>(),
                                Mediafiles = new List<MediaFile>(),
                                Likes = new List<Like>(),
                                Blocks = new List<Block>(),
                                Payments = new List<Payment>(),
                                Reports = new List<Report>(),
                                Visits = new List<Visit>(),
                                CountryTxt = item.CountryTxt,
                                GenderTxt = item.GenderTxt,
                                Instagram = item.Instagram,
                                LanguageTxt = item.LanguageTxt,
                                EmailCode = item.EmailCode,
                                HeightTxt = item.HeightTxt,
                                HairColorTxt = item.HairColorTxt,
                                RelationshipTxt = item.RelationshipTxt,
                                WorkStatusTxt = item.WorkStatusTxt,
                                EducationTxt = item.EducationTxt,
                                EthnicityTxt = item.EthnicityTxt,
                                BodyTxt = item.BodyTxt,
                                CharacterTxt = item.CharacterTxt,
                                ChildrenTxt = item.ChildrenTxt,
                                FriendsTxt = item.FriendsTxt,
                                PetsTxt = item.PetsTxt,
                                LiveWithTxt = item.LiveWithTxt,
                                CarTxt = item.CarTxt,
                                ReligionTxt = item.ReligionTxt,
                                SmokeTxt = item.SmokeTxt,
                                DrinkTxt = item.DrinkTxt,
                                TravelTxt = item.TravelTxt,
                                ShowMeTo = item.ShowMeTo,
                                EmailOnGetGift = item.EmailOnGetGift,
                                EmailOnGotNewMatch = item.EmailOnGotNewMatch,
                                EmailOnChatRequest = item.EmailOnChatRequest,
                                LastEmailSent = item.LastEmailSent,
                            };

                            infoObject.Mediafiles = JsonConvert.DeserializeObject<List<MediaFile>>(item.Mediafiles);

                            list.Add(infoObject);
                        }

                        return list;
                    } 
                    return new ObservableCollection<UserInfoObject>();
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return new ObservableCollection<UserInfoObject>();
            }
        }

        #endregion
    }
}