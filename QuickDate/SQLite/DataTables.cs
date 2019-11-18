using QuickDateClient.Classes.Common;
using QuickDateClient.Classes.Global;
using SQLite;

namespace QuickDate.SQLite
{
    public class DataTables
    {
        public class LoginTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public string UserId { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string AccessToken { get; set; }
            public string Cookie { get; set; }
            public string Email { get; set; }
            public string Status { get; set; }
            public string Lang { get; set; }
            public string DeviceId { get; set; }
        }

        public class SettingsTb  : GetOptionsObject.DataOptions
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }

            public new string Height { get; set; }
            public new string Notification { get; set; }
            public new string Gender { get; set; }
            public new string BlogCategories { get; set; }
            public new string Countries { get; set; }
            public new string HairColor { get; set; }
            public new string Travel { get; set; }
            public new string Drink { get; set; }
            public new string Smoke { get; set; }
            public new string Religion { get; set; }
            public new string Car { get; set; }
            public new string LiveWith { get; set; }
            public new string Pets { get; set; }
            public new string Friends { get; set; }
            public new string Children { get; set; }
            public new string Character { get; set; }
            public new string Body { get; set; }
            public new string Ethnicity { get; set; }
            public new string Education { get; set; }
            public new string WorkStatus { get; set; }
            public new string Relationship { get; set; }
            public new string Language { get; set; }
        }

        public class InfoUsersTb  : UserInfoObject
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }
             
            public new string ProfileCompletionMissing { get; set; }
            public new string Mediafiles { get; set; }
            public new string Likes { get; set; }
            public new string  Blocks { get; set; }
            public new string Payments  { get; set; }
            public new string Reports { get; set; }
            public new string Visits { get; set; }
        }
         
        public class GiftsTb  
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }
             
            public int IdGifts { get; set; }
            public string File { get; set; }
        }

        public class StickersTb
        {
            [PrimaryKey, AutoIncrement]
            public int AutoIdStickers { get; set; }
             
            public int IdStickers { get; set; }
            public string File { get; set; }
        }

        public class LastChatTb
        {
            [PrimaryKey, AutoIncrement] public int AutoIdLastChat { get; set; }

            public int Id { get; set; }
            public int Owner { get; set; }
            public string UserDataJson { get; set; }
            public int Seen { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public string Sticker { get; set; }
            public string Time { get; set; }
            public string CreatedAt { get; set; }
            public string UserId { get; set; }
            public int NewMessages { get; set; }
            public string MessageType { get; set; }
            public string ToId { get; set; }
            public string FromId { get; set; }
        }
         
        public class MessageTb
        {
            [PrimaryKey, AutoIncrement] public int AutoIdMessage { get; set; }

            public int Id { get; set; }
            public string FromName { get; set; }
            public string FromAvater { get; set; }
            public string ToName { get; set; }
            public string ToAvater { get; set; }
            public int FromId { get; set; }
            public int ToId { get; set; }
            public string Text { get; set; }
            public string Media { get; set; }
            public int FromDelete { get; set; }
            public int ToDelete { get; set; }
            public string Sticker { get; set; }
            public string CreatedAt { get; set; }
            public int Seen { get; set; }
            public string Type { get; set; }
            public string MessageType { get; set; }
        }

        public class FavoriteUsersTb : UserInfoObject
        {
            [PrimaryKey, AutoIncrement]
            public int AutoId { get; set; }
             
            public new string ProfileCompletionMissing { get; set; }
            public new string Mediafiles { get; set; }
            public new string Likes { get; set; }
            public new string Blocks { get; set; }
            public new string Payments { get; set; }
            public new string Reports { get; set; }
            public new string Visits { get; set; }

        } 
    }
} 