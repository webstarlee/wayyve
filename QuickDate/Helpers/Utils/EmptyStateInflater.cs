using System;
using Android.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using QuickDate.Helpers.Fonts;

namespace QuickDate.Helpers.Utils
{
   public class EmptyStateInflater
   {
        public Button EmptyStateButton;
        public TextView EmptyStateIcon;
        public TextView DescriptionText;
        public TextView TitleText;

        public enum Type
        {
            NoConnection,
            NoSearchResult,
            SomeThingWentWrong,
            NoUsers,
            NoMatches,
            NoNotifications,
            NoMessage,
            NoBlock,
            NoArticle,
        }

        public void InflateLayout(View inflated , Type type)
        {
            try
            {
                
                EmptyStateIcon = (TextView)inflated.FindViewById(Resource.Id.emtyicon);
                TitleText = (TextView)inflated.FindViewById(Resource.Id.headText);
                DescriptionText = (TextView)inflated.FindViewById(Resource.Id.seconderyText);
                EmptyStateButton = (Button)inflated.FindViewById(Resource.Id.button);
                
                if (type == Type.NoConnection)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.IosThunderstormOutline);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoConnection_Button);
                }
                else if (type == Type.NoSearchResult)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Search);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_NoSearchResult_Button);
                }
                else if (type == Type.SomeThingWentWrong)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Close);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_DescriptionText);
                    EmptyStateButton.Text = Application.Context.GetText(Resource.String.Lbl_SomThingWentWrong_Button);
                }
                else if (type == Type.NoMatches)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Pin);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMoreUsers);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility =ViewStates.Gone;
                } 
                else if (type == Type.NoUsers)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMoreUsers);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility =ViewStates.Gone;
                } 
                else if (type == Type.NoNotifications)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.AndroidNotifications);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoNotification_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoNotification_DescriptionText);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoMessage)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Chatbox);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoMessage_TitleText);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_NoMessage_DescriptionText) + " " + AppSettings.ApplicationName;
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoBlock)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.IonIcons, EmptyStateIcon, IonIconsFonts.Person);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_NoBlockUsers);
                    DescriptionText.Text = " ";
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
                else if (type == Type.NoArticle)
                {
                    FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EmptyStateIcon, FontAwesomeIcon.FileAlt);
                    EmptyStateIcon.SetTextSize(ComplexUnitType.Dip, 45f);
                    TitleText.Text = Application.Context.GetText(Resource.String.Lbl_Empty_Article);
                    DescriptionText.Text = Application.Context.GetText(Resource.String.Lbl_Start_Article);
                    EmptyStateButton.Visibility = ViewStates.Gone;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}