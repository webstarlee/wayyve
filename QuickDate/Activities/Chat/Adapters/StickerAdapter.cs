using System;
using System.Collections.ObjectModel;
using System.Linq;
using Android.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.CacheLoaders;
using QuickDate.Helpers.Utils;
using QuickDateClient;
using QuickDateClient.Classes.Common;

namespace QuickDate.Activities.Chat.Adapters
{
    public class StickerAdapter : RecyclerView.Adapter
    {
        public event EventHandler<StickerAdapterClickEventArgs> OnItemClick;
        public event EventHandler<StickerAdapterClickEventArgs> OnItemLongClick;
        private readonly Activity ActivityContext;
        private ObservableCollection<DataFile> StickerList = new ObservableCollection<DataFile>();

        public StickerAdapter(Activity context)
        {
            try
            {
                ActivityContext = context;
                HasStableIds = true;
                GetSticker();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            try
            {
                //Setup your layout here >> Sticker_view
                View itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.Style_StickerView, parent, false);
                var vh = new StickerAdapterViewHolder(itemView, Click, LongClick);
                return vh;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            try
            { 
                if (viewHolder is StickerAdapterViewHolder holder)
                {
                    var item = StickerList[position];
                    if (item != null)
                    {
                        var imageSplit = item.File.Split('/').Last();
                        var getImage = Methods.MultiMedia.GetMediaFrom_Disk(Methods.Path.FolderDiskSticker, imageSplit);
                        if (getImage != "File Dont Exists")
                        {
                            GlideImageLoader.LoadImage(ActivityContext,item.File, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        }
                        else
                        {
                            var url = item.File.Contains("media3.giphy.com/");
                            if (url)
                            {
                                item.File = item.File.Replace(Client.WebsiteUrl, "");
                            }

                            Methods.MultiMedia.DownloadMediaTo_DiskAsync(Methods.Path.FolderDiskSticker, item.File);
                            GlideImageLoader.LoadImage(ActivityContext,item.File, holder.Image, ImageStyle.CenterCrop, ImagePlaceholders.Drawable);
                        } 
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void GetSticker()
        {
            try
            {
                StickerList = new ObservableCollection<DataFile>(ListUtils.StickersList.Where(a => !a.File.Contains(".gif")).ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
       public override int ItemCount => StickerList?.Count ?? 0;

        public DataFile GetItem(int position)
        {
            return StickerList[position];
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

        void Click(StickerAdapterClickEventArgs args) => OnItemClick?.Invoke(this, args);
        void LongClick(StickerAdapterClickEventArgs args) => OnItemLongClick?.Invoke(this, args);
    }

    public class StickerAdapterViewHolder : RecyclerView.ViewHolder
    {
        #region Variables Basic

        public View MainView { get;  set; }
        public ImageView Image { get; set; }

        #endregion

        public StickerAdapterViewHolder(View itemView, Action<StickerAdapterClickEventArgs> clickListener, Action<StickerAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            try
            {
                MainView = itemView;

                //Get values
                Image = itemView.FindViewById<ImageView>(Resource.Id.stickerImage);
                
                //Event
                itemView.Click += (sender, e) => clickListener(new StickerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
                itemView.LongClick += (sender, e) => longClickListener(new StickerAdapterClickEventArgs { View = itemView, Position = AdapterPosition });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class StickerAdapterClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}