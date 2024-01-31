using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Covoituragee
{
    public class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
    {
        public const string TAG = "DatePickerFragment";

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            // Use the current date as the default date in the picker
            DateTime currentDate = DateTime.Now;
            int year = currentDate.Year;
            int month = currentDate.Month - 1; // DatePickerDialog uses 0-based months
            int day = currentDate.Day;

            // Create a new instance of DatePickerDialog and return it
            DatePickerDialog dialog = new DatePickerDialog(Activity, this, year, month, day);
            return dialog;
        }

        public void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            // Display the selected date in the button text
            DateTime selectedDate = new DateTime(year, month + 1, dayOfMonth);

            // Calculate age based on the selected date of birth
            DateTime today = DateTime.Today;
            int age = today.Year - selectedDate.Year;
            if (selectedDate > today.AddYears(-age))
                age--;

            // Restrict access for minors (age below 18)
            if (age < 18)
            {
                Toast.MakeText(Activity, "Vous devez avoir 18 ans pour vous inscrire", ToastLength.Long).Show();
                // Disable sign up or log out the user, depending on your app logic
                // ...
            }
            else
            {
                // Set the button text only if the user is not a minor
                Button datePickerButton = Activity.FindViewById<Button>(Resource.Id.datePickerButton);
                datePickerButton.Text = selectedDate.ToString("yyyy-MM-dd");
            }
        }

    }

}