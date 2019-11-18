using System;
using System.Collections.Generic;
using System.Linq;
using AFollestad.MaterialDialogs;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Java.Lang;
using QuickDate.Helpers.Fonts;
using QuickDate.Helpers.Utils;
using Exception = System.Exception;

namespace QuickDate.Activities.SearchFilter.Fragment
{
    public class MoreFragment : Android.Support.V4.App.Fragment, MaterialDialog.IInputCallback ,MaterialDialog.IListCallback, MaterialDialog.ISingleButtonCallback
    {
        #region  Variables Basic

        private SearchFilterTabbedActivity GlobalContext;

        private TextView InterestIcon, EducationIcon, PetsIcon;
        private EditText EdtInterest, EdtEducation, EdtPets;
        private string TypeDialog;
        public string Interest;
        public int IdEducation, IdPets;

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
                View view = inflater.Inflate(Resource.Layout.FilterMoreLayout, container, false);

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
                InterestIcon = view.FindViewById<TextView>(Resource.Id.IconInterest);
                EdtInterest = view.FindViewById<EditText>(Resource.Id.InterestEditText);

                EducationIcon = view.FindViewById<TextView>(Resource.Id.IconEducation);
                EdtEducation = view.FindViewById<EditText>(Resource.Id.EducationEditText);

                PetsIcon = view.FindViewById<TextView>(Resource.Id.IconPets);
                EdtPets = view.FindViewById<EditText>(Resource.Id.PetsEditText);

                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, InterestIcon, FontAwesomeIcon.Tasks);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, EducationIcon, FontAwesomeIcon.GraduationCap);
                FontUtils.SetTextViewIcon(FontsIconFrameWork.FontAwesomeLight, PetsIcon, FontAwesomeIcon.Cat);

                Methods.SetFocusable(EdtInterest);
                Methods.SetFocusable(EdtEducation);
                Methods.SetFocusable(EdtPets);

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
                    EdtInterest.Touch += EdtInterestOnClick;
                    EdtEducation.Touch += EdtEducationOnClick;
                    EdtPets.Touch += EdtPetsOnClick;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        #endregion

        #region Events

        //Interest
        private void EdtInterestOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                TypeDialog = "Interest";
                var dialog = new MaterialDialog.Builder(Context);
                dialog.Title(GetString(Resource.String.Lbl_Interest));
                dialog.Input(Resource.String.Lbl_EnterTextInterest, 0, false, this);
                dialog.InputType(InputTypes.TextFlagImeMultiLine);
                dialog.PositiveText(GetText(Resource.String.Lbl_Submit)).OnPositive(this);
                dialog.NegativeText(GetText(Resource.String.Lbl_Cancel)).OnNegative(this);
                dialog.AlwaysCallSingleChoiceCallback();
                dialog.Build().Show();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        //Education
        private void EdtEducationOnClick(object sender, View.TouchEventArgs e)
        {
            try
            {
                if (e.Event.Action != MotionEventActions.Down) return;
                TypeDialog = "Education";
                //string[] educationArray = Application.Context.Resources.GetStringArray(Resource.Array.EducationArray);
                var educationArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Education;

                var arrayAdapter = new List<string>();
                var dialogList = new MaterialDialog.Builder(Context);

                if (educationArray != null) arrayAdapter.AddRange(educationArray.Select(item => Methods.FunString.DecodeString(item.Values.FirstOrDefault())));

                dialogList.Title(GetString(Resource.String.Lbl_EducationLevel));
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
                var dialogList = new MaterialDialog.Builder(Context);

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


        #endregion


        #region MaterialDialog

        public void OnSelection(MaterialDialog p0, View p1, int itemId, ICharSequence itemString)
        {
            try
            {
                if (TypeDialog == "Education")
                {
                    var educationArray = ListUtils.SettingsSiteList.FirstOrDefault()?.Education?.FirstOrDefault(a => a.ContainsValue(itemString.ToString()))?.Keys.FirstOrDefault();
                    IdEducation = int.Parse(educationArray ?? "1");
                    EdtEducation.Text = itemString.ToString();
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

        public void OnInput(MaterialDialog p0, ICharSequence p1)
        {
            try
            {
                var strName = p1.ToString();
                if (!string.IsNullOrEmpty(strName))
                {
                    if (p1.Length() <= 0) return;

                    Interest = strName;
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