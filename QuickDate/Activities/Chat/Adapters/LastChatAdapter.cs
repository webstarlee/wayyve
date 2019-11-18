using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Android.App;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Bumptech.Glide;
using Bumptech.Glide.Load.Engine;
using Bumptech.Glide.Request;
using Java.Util;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Model;
using QuickDate.Helpers.Utils;
using QuickDateClient.Classes.Chat;
using IList = System.Collections.IList;
using Object = Java.Lang.Object;

namespace QuickDate.Activities.Chat.Adapters
{
    public class LastChatAdapter : RecyclerView.Adapter, ListPreloader.IPreloadModelProvider
    {
        private readonly Activity ActivityContext;

        public ObservableCollection<GetConversationListObject.DataConversation> UserList = new ObservableCollection<GetConversationListObject.DataConversation>();
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemClick;
        public event EventHandler<LastChatAdapterClickEventArgs> OnItemLongClick;

        private IOnClickListenerSelected ClickListener;
        private readonly SparseBooleanArray SelectedItems;
        private int CurrentSelectedIdx = -1;

        public LastChatAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                SelectedItems = new SparseBooleanArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void SetOnClickListener(IOnClickListenerSelected onClickListener)
        {
            ClickListener = onClickListener;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            {
                if (viewHolder is LastChatAdapterViewHolder holder)
                {
                    var item = UserList[position];
                    if (item != null)
                    { 
                        Initialize(holder, item);

                        holder.LytParent.Activated = SelectedItems.Get(position, false);

                        holder.LytParent.Click += delegate
                        {
                            try
                            {
                                if (ClickListener == null) return;

                                ClickListener.ItemClick(holder.MainView, item, position);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };

                        holder.LytParent.LongClick += delegate
                        {
                            try
                            {
                                if (ClickListener == null) return;

                                ClickListener.ItemLongClick(holder.MainView, item, position);

                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e);
                            }
                        };

                        ToggleCheckedIcon(holder, position);
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private void Initialize(LastChatAdapterViewHolder holder, GetConversationListObject.DataConversation item)
        {
            try
            {
                GlideImageLoader.LoadImage(ActivityContext, item.User.Avater, holder.ImageAvatar, ImageStyle.CircleCrop, ImagePlaceholders.Drawable);

                string name = Methods.FunString.DecodeString(QuickDateTools.GetNameFinal(item.User));
                if (holder.TxtUsername.Text != name)
                {
                    holder.TxtUsername.Text = name;
                }

                //If message contains Media files 
                switch (item.MessageType)
                {
                    case "text":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Gone;
                        holder.TxtLastMessages.Text = item.Text.Contains("http")
                            ? Methods.FunString.SubStringCutOf(item.Text, 30)
                            : Methods.FunString.DecodeString(Methods.FunString.SubStringCutOf(item.Text, 30))
                            ?? ActivityContext.GetText(Resource.String.Lbl_SendMessage);
                        break;
                    }
                    case "media":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon,IonIconsFonts.Images);
                        holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendImageFile);
                        break;
                    }
                    case "sticker" when item.Sticker.Contains(".gif"):
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, holder.LastMessagesIcon,FontAwesomeIcon.Gift);
                        holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendGifFile);
                        break;
                    }
                    case "sticker":
                    {
                        holder.LastMessagesIcon.Visibility = ViewStates.Visible;
                        FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, holder.LastMessagesIcon,IonIconsFonts.Happy);
                        holder.TxtLastMessages.Text = Application.Context.GetText(Resource.String.Lbl_SendStickerFile);
                        break;
                    }
                }

                //last seen time  
                holder.TxtTimestamp.Text = Methods.Time.ReplaceTime(item.Time);
                 
                //Check read message
                if (item.ToId != UserDetails.UserId && item.FromId == UserDetails.UserId)
                {
                    if (item.Seen == 0)
                    {
                        holder.ImageColor.Visibility = ViewStates.Invisible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                    else
                    {
                        holder.ImageColor.Visibility = ViewStates.Visible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }
                else
                {
                    if (item.Seen == 0)
                    {
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Bold);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Bold);

                        var drawable = TextDrawable.TextDrawable.TextDrawbleBuilder.BeginConfig().FontSize(25).EndConfig().BuildRound("N", Color.ParseColor(AppSettings.MainColor));
                        holder.ImageColor.SetImageDrawable(drawable);
                        holder.ImageColor.Visibility = ViewStates.Visible;
                    }
                    else
                    {
                        holder.ImageColor.Visibility = ViewStates.Invisible;
                        holder.TxtUsername.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                        holder.TxtLastMessages.SetTypeface(Typeface.Default, TypefaceStyle.Normal);
                    }
                }

                //if (item.NewMessages <= 0)
                //{
                //    holder.ImageColor.Visibility = ViewStates.Invisible;
                //}
                //else
                //{
                //    var drawable = TextDrawable.TextDrawable.TextDrawbleBuilder.BeginConfig().FontSize(25).EndConfig().BuildRound(item.NewMessages.ToString(), Color.ParseColor(AppSettings.MainColor));
                //    holder.ImageColor.SetImageDrawable(drawable);
                //    holder.ImageColor.Visibility = ViewStates.Visible;
                //} 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Style_HContact_view
                var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_LastChatView, parent, false);
                var vh = new LastChatAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override int ItemCount => UserList?.Count ?? 0;
         
        
        #region Toolbar & Selected

        private void ToggleCheckedIcon(LastChatAdapterViewHolder holder, int position)
        {
            try
            {
                if (SelectedItems.Get(position, false))
                {
                    holder.LytImage.Visibility = ViewStates.Gone;
                    holder.LytChecked.Visibility = ViewStates.Visible;
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
                else
                {
                    holder.LytChecked.Visibility = ViewStates.Gone;
                    holder.LytImage.Visibility = ViewStates.Visible;
                    if (CurrentSelectedIdx == position) ResetCurrentItems();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private void ResetCurrentItems()
        {
            try
            {
                CurrentSelectedIdx = -1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public int GetSelectedItemCount()
        {
            return SelectedItems.Size();
        }

        public List<int> GetSelectedItems()
        {
            List<int> items = new List<int>(SelectedItems.Size());
            for (int i = 0; i < SelectedItems.Size(); i++)
            {
                items.Add(SelectedItems.KeyAt(i));
            }
            return items;
        }

        public void RemoveData()
        {
            try
            { 
                ResetCurrentItems();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ClearSelections()
        {
            try
            {
                SelectedItems.Clear();
                NotifyDataSetChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void ToggleSelection(int pos)
        {
            try
            {
                CurrentSelectedIdx = pos;
                if (SelectedItems.Get(pos, false))
                {
                    SelectedItems.Delete(pos);
                }
                else
                {
                    SelectedItems.Put(pos, true);
                }
                NotifyItemChanged(pos);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        public GetConversationListObject.DataConversation GetItem(int position)
        {
            return UserList[position];
        }

        public override long GetItemId(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        public override int GetItemViewType(int position)
        {
            try
            {
                return position;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return 0;
            }
        }

        private void Click(LastChatAdapterClickEventArgs args)
        {
            OnItemClick?.Invoke(this, args);
        }

        private void LongClick(LastChatAdapterClickEventArgs args)
        {
            OnItemLongClick?.Invoke(this, args);
        }

        public IList GetPreloadItems(int p0)
        {
            try
            {
                var d = new List<string>();
                var item = UserList[p0];

                if (item == null)
                    return Collections.SingletonList(p0);

                if (item.User.Avater != "")
                {
                    d.Add(item.User.Avater);
                    return d;
                }

                return d;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Collections.SingletonList(p0);
            }
        }

        public RequestBuilder GetPreloadRequestBuilder(Object p0)
        {
            return Glide.With(ActivityContext).Load(p0.ToString())
                .Apply(new RequestOptions().CircleCrop().SetDiskCacheStrategy(DiskCacheStrategy.All));
        }
    }

    public class LastChatAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get; private set; }

        public RelativeLayout LytParent { get; private set; }
        public TextView TxtUsername { get; private set; }
        public TextView LastMessagesIcon { get; private set; }
        public TextView TxtLastMessages { get; private set; }
        public TextView TxtTimestamp { get; private set; }
        public ImageView ImageAvatar { get; private set; }
        public ImageView ImageColor { get; private set; }

        public RelativeLayout LytChecked { get; private set; }
        public RelativeLayout LytImage { get; private set; }

        #endregion

        public LastChatAdapterViewHolder(View itemView, Action<LastChatAdapterClickEventArgs> clickListener, Action<LastChatAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                LytParent = (RelativeLayout)MainView.FindViewById(Resource.Id.main);
                TxtUsername = (TextView)MainView.FindViewById(Resource.Id.Txt_Username);
                LastMessagesIcon = (AppCompatTextView)MainView.FindViewById(Resource.Id.LastMessages_icon);
                TxtLastMessages = (TextView)MainView.FindViewById(Resource.Id.Txt_LastMessages);
                TxtTimestamp = (TextView)MainView.FindViewById(Resource.Id.Txt_timestamp);
                ImageAvatar = (ImageView)MainView.FindViewById(Resource.Id.ImageAvatar);

                ImageColor = (ImageView)MainView.FindViewById(Resource.Id.image_view);

                LytChecked = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_checked);
                LytImage = (RelativeLayout)MainView.FindViewById(Resource.Id.lyt_image);


                //Create an Event
                itemView.Click += (sender, e) => clickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new LastChatAdapterClickEventArgs { View = itemView, Position = AdapterPosition });

                //Dont Remove this code #####
                FontUtils.SetFont(TxtUsername, Fonts.SfRegular);
                FontUtils.SetFont(TxtLastMessages, Fonts.SfMedium);
                //#####
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class LastChatAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}