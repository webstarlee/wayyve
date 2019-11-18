using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.OS;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using Exception = System.Exception;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class FilterBackgroundFragment : Android.Support.V4.App.Fragment, MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView LanguageIcon, EthnicityIcon, ReligionIcon;
        private EditText EdtLanguage, EdtEthnicity, EdtReligion;
        private string TypeDialog;
        public int IdEthnicity, IdReligion;
        public string Language;

        #endregion

        #region General

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
            GlobalContext = (SearchFilterTabbedActivity)Activity;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            try
            {
                View view = inflater.Inflate(Resource.Layout.FilterBackgroundLayout, container, false);

                InitComponent(view);
                
                AddOrRemoveEvent(true);

                return view;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }

        public override void OnLowMemory()
        {
            try
            {
                GC.Collect(GC.MaxGeneration);
                base.OnLowMemory();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        #endregion

        #region Functions

        private void InitComponent(View view)
        {
            try
            {
                LanguageIcon = view.FindViewById<TextView>(Resource.Id.IconLanguage);
                EdtLanguage = view.FindViewById<EditText>(Resource.Id.LanguageEditText);

                EthnicityIcon = view.FindViewById<TextView>(Resource.Id.IconEthnicity);
                EdtEthnicity = view.FindViewById<EditText>(Resource.Id.EthnicityEditText);
                 
                ReligionIcon = view.FindViewById<TextView>(Resource.Id.IconReligion);
                EdtReligion = view.FindViewById<EditText>(Resource.Id.ReligionEditText);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, LanguageIcon, FontAwesomeIcon.Language);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeRegular, EthnicityIcon, FontAwesomeIcon.TheaterMasks);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, ReligionIcon, FontAwesomeIcon.Church);

                Methods.SetFocusable(EdtLanguage);
                Methods.SetFocusable(EdtEthnicity);
                Methods.SetFocusable(EdtReligion);

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
                    EdtLanguage.Touch += EdtLanguageOnClick;
                    EdtEthnicity.Touch += EdtEthnicityOnClick;
                    EdtReligion.Touch += EdtReligionOnClick;
                } 
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

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
                var dialogList = new MaterialDialog.Builder(Context);

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

        //Ethnicity
        private void EdtEthnicityOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Ethnicity";
                //string[] ethnicityArray = Application.Context.Resources.GetStringArray(Resource.Array.EthnicityArray);
                var ethnicityArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Ethnicity;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (ethnicityArray != null) arrayAdapter.AddRange(ethnicityArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_BodyType));
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

        //Religion
        private void EdtReligionOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Religion";
                //string[] religionArray = Application.Context.Resources.GetStringArray(Resource.Array.ReligionArray);
                var religionArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Religion;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (religionArray != null) arrayAdapter.AddRange(religionArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetText(Resource.String.Lbl_Religion));
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

        #endregion


        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Language")
                {
                    Language = itemString.ToString().ToLower();
                    EdtLanguage.Text = itemString.ToString();
                }
                else if (TypeDialog == "Ethnicity")
                {
                    var ethnicityArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Ethnicity?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdEthnicity = int.Parse(ethnicityArray ?? "1");
                    EdtEthnicity.Text = itemString.ToString();
                }
                else if (TypeDialog == "Religion")
                {
                    var religionArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Religion?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdReligion = int.Parse(religionArray ?? "1");
                    EdtReligion.Text = itemString.ToString();
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