using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Support.V4.App;
using Android.Widget;
using Newtonsoft.Json;
using QuickDate.Activities.Tabbes;
using QuickDate.Activities.UserProfile;
using QuickDate.Helpers.Model;
using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;

namespace QuickDate.Helpers.Utils
{
    public static class QuickDateTools
    {
        //private static readonly string[] RelationshipLocal = Application.Context.Resources.GetStringArray(Resource.Array.RelationShipArray);
        //private static readonly string[] WorkStatusLocal = Application.Context.Resources.GetStringArray(Resource.Array.WorkStatusArray);
        //private static readonly string[] EducationLocal = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
        //private static readonly string[] HairColorLocal = Application.Context.Resources.GetStringArray(Resource.Array.HairColorArray);
        //private static readonly string[] BodyLocal = Application.Context.Resources.GetStringArray(Resource.Array.BodyArray);
        //private static readonly string[] EthnicityLocal = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
        //private static readonly string[] PetsLocal = Application.Context.Resources.GetStringArray(Resource.Array.PetsArray);
        //private static readonly string[] FriendsLocal = Application.Context.Resources.GetStringArray(Resource.Array.FriendsArray);
        //private static readonly string[] ChildrenLocal = Application.Context.Resources.GetStringArray(Resource.Array.ChildrenArray);
        //private static readonly string[] CharacterLocal = Application.Context.Resources.GetStringArray(Resource.Array.CharacterArray);
        //private static readonly string[] TravelLocal = Application.Context.Resources.GetStringArray(Resource.Array.TravelArray);
        //private static readonly string[] DrinkLocal = Application.Context.Resources.GetStringArray(Resource.Array.DrinkArray);
        //private static readonly string[] SmokeLocal = Application.Context.Resources.GetStringArray(Resource.Array.SmokeArray);
        //private static readonly string[] ReligionLocal = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
        //private static readonly string[] CarLocal = Application.Context.Resources.GetStringArray(Resource.Array.CarArray);
        //private static readonly string[] LiveWithLocal = Application.Context.Resources.GetStringArray(Resource.Array.LiveWithArray);
        //private static string[] HeightLocal = Application.Context.Resources.GetStringArray(Resource.Array.HeightArray);
        //private static readonly string[] CountriesArray = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray);
        //private static readonly string[] CountriesArrayId = Application.Context.Resources.GetStringArray(Resource.Array.countriesArray_id);

        private static readonly List<Dictionary<string, string>> RelationshipLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Relationship;
        private static readonly List<Dictionary<string, string>> WorkStatusLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.WorkStatus;
        private static readonly List<Dictionary<string, string>> EducationLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Education;
        private static readonly List<Dictionary<string, string>> HairColorLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.HairColor;
        private static readonly List<Dictionary<string, string>> BodyLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Body;
        private static readonly List<Dictionary<string, string>> EthnicityLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Ethnicity;
        private static readonly List<Dictionary<string, string>> PetsLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Pets;
        private static readonly List<Dictionary<string, string>> FriendsLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Friends;
        private static readonly List<Dictionary<string, string>> ChildrenLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Children;
        private static readonly List<Dictionary<string, string>> CharacterLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Character;
        private static readonly List<Dictionary<string, string>> TravelLocal =ListUtils.SettingsSiteList.FirstOrDefault()?.Travel;
        private static readonly List<Dictionary<string, string>> DrinkLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Drink;
        private static readonly List<Dictionary<string, string>> SmokeLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Smoke;
        private static readonly List<Dictionary<string, string>> ReligionLocal =ListUtils.SettingsSiteList.FirstOrDefault()?.Religion;
        private static readonly List<Dictionary<string, string>> CarLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Car;
        private static readonly List<Dictionary<string, string>> LiveWithLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.LiveWith;
        private static readonly List<Dictionary<string, string>> HeightLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.Height;
        private static readonly List<Dictionary<string, string>> BlogCategoriesLocal = ListUtils.SettingsSiteList.FirstOrDefault()?.BlogCategories;
        private static readonly List<Dictionary<string, GetOptionsObject.Country>> CountriesArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries;
         
        public static bool GetStatusOnline(int lastSeen , int isShowOnline)
        {
            try
            {
                string time = Methods.Time.TimeAgo(lastSeen);
                bool status  = isShowOnline == 1 && time == Methods.Time.LblJustNow ? true : false;
                return status;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static string GetNameFinal(UserInfoObject dataUser)
        {
            try
            { 
                if (!string.IsNullOrEmpty(dataUser.Fullname))
                    return Methods.FunString.DecodeString(dataUser.Fullname);

                string name = !string.IsNullOrEmpty(dataUser.FirstName) && !string.IsNullOrEmpty(dataUser.LastName)
                    ? dataUser.FirstName + " " + dataUser.LastName
                    : dataUser.Username; 
                return Methods.FunString.DecodeString(name);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Methods.FunString.DecodeString(dataUser?.Username);
            }
        }
         
        public static string GetWorkStatus(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(WorkStatusLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                return name;

                //int index = id - 1;
                //if (index > -1)
                //{
                //    string name = WorkStatusLocal[index];
                //    return name;
                //}
               // return ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            } 
        }

        public static string GetRelationship(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(RelationshipLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                return name ?? ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetEducation(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(EducationLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetEthnicity(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(EthnicityLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetBody(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(BodyLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetHairColor(int id)
        {
            try
            { 
                string name = Methods.FunString.DecodeString(HairColorLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetCharacter(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(CharacterLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetChildren(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(ChildrenLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetFriends(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(FriendsLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetBlogCategories(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(BlogCategoriesLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetLiveWith(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(LiveWithLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetCar(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(CarLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetReligion(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(ReligionLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetSmoke(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(SmokeLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetTravel(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(TravelLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetNotification(GetNotificationsObject.Datum item)
        {
            try
            {
                string text = "";
                GetNotificationsObject.Datum check = null;
                
                switch (item.Type)
                {
                    case "visit":
                        text = Application.Context.GetText(Resource.String.Lbl_VisitYou);
                        check = ListUtils.VisitsList.FirstOrDefault(a => a.Notifier.Id == item.Notifier.Id);
                        if (check == null)
                            ListUtils.VisitsList.Add(item);
                        break;
                    case "like":
                        text = Application.Context.GetText(Resource.String.Lbl_LikeYou);
                        check = ListUtils.LikesList.FirstOrDefault(a => a.Notifier.Id == item.Notifier.Id);
                        if (check == null)
                            ListUtils.LikesList.Add(item);
                        break;
                    case "dislike":
                        text = Application.Context.GetText(Resource.String.Lbl_DislikeYou);
                        check = ListUtils.LikesList.FirstOrDefault(a => a.Notifier.Id == item.Notifier.Id);
                        if (check == null)
                            ListUtils.LikesList.Add(item);
                        break;
                    case "send_gift":
                        text = Application.Context.GetText(Resource.String.Lbl_SendGiftToYou);
                        break;
                    case "got_new_match":
                        text = Application.Context.GetText(Resource.String.Lbl_YouGotMatch);
                          check = ListUtils.MatchList.FirstOrDefault(a => a.Notifier.Id == item.Notifier.Id);
                        if (check == null)
                            ListUtils.MatchList.Add(item);
                        break;
                    default:
                        text = "";
                        break;
                }

                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetGender(int id)
        {
            try
            {
                string text; 
                string name = Methods.FunString.DecodeString(ListUtils.SettingsSiteList.FirstOrDefault()?.Gender?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault()); 
                switch (id)
                {
                    case 4525:
                        text = Application.Context.GetText(Resource.String.Lbl_Male);
                        break;
                    case 4526:
                        text = Application.Context.GetText(Resource.String.Lbl_Female);
                        break;
                    default:
                        text = name;
                        break;
                }
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetPets(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(PetsLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static string GetDrink(int id)
        {
            try
            {
                string name = Methods.FunString.DecodeString(DrinkLocal?.FirstOrDefault(a => a.ContainsKey(id.ToString()))?.Values.FirstOrDefault());
                 return name ?? ""; 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }


        public static string GetCountry(string codeCountry)
        {
            try
            {
                var name = ListUtils.SettingsSiteList.FirstOrDefault()?.Countries?.FirstOrDefault(a => a.ContainsKey(codeCountry))?.Values.FirstOrDefault()?.Name;
                return Methods.FunString.DecodeString(name);

               // var list = CountriesArrayId.ToList();
               // int index = 0;

               // var data = list.FirstOrDefault(a => a.Contains(codeCountry));
               //if (data != null)
               //{
               //    index = list.IndexOf(data);
               //}
                
               // if (index > -1)
               // {
               //     string name = CountriesArray[index];
               //     return name;
               // }
               // return "";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }
         
        public static string GetNotificationsText(string type)
        {
            try
            {
                string text = "";
                if (type == "visit")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_VisitYou);
                }
                else if (type == "like")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_LikeYou);
                }
                else if (type == "dislike")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DislikeYou);
                }
                else if (type == "send_gift")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_SendGiftToYou);
                }
                else if (type == "got_new_match")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_YouGotMatch);
                }
                else if (type == "message")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_MessageNotifications);
                }
                else if (type == "approve_receipt")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_ApproveReceipt);
                }
                else if (type == "disapprove_receipt")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DisapproveReceipt);
                }
                else if (type == "accept_chat_request")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_AcceptChatRequest);
                }
                else if (type == "accept_chdecline_chat_requestat_request")
                {
                    text = Application.Context.GetText(Resource.String.Lbl_DeclineChatRequest);
                }
                
                return text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return "";
            }
        }

        public static void OpenProfile(Activity activity, string eventPage, UserInfoObject item , ImageView image)
        {
            try
            {
                if (item.Id != UserDetails.UserId)
                {
                    var intent = new Intent(activity, typeof(UserProfileActivity));
                    intent.PutExtra("EventPage", eventPage); // Close , Move , likeAndClose , HideButton
                    intent.PutExtra("ItemUser", JsonConvert.SerializeObject(item));
                    if (AppSettings.EnableAddAnimationImageUser)
                    {
                        if (image != null)
                        {
                            ActivityOptionsCompat options = ActivityOptionsCompat.MakeSceneTransitionAnimation((Activity)activity, image, "profileimage");
                            activity.StartActivity(intent, options.ToBundle());
                        }
                        else
                        {
                            activity.StartActivity(intent);
                        }
                    }
                    else
                    {
                        activity.StartActivity(intent);
                    }
                }
                else
                {
                    HomeActivity.GetInstance()?.NavigationTabBar.SetModelIndex(4, true);
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}