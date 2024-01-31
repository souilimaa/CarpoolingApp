using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Text.Format;
using Android.Views;
using Android.Widget;
using Java.Sql;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Covoituragee
{
    [Activity(Label = "ajoute_trajet")]
    public class ajoute_trajet : Activity
    {
        EditText editTextdate;
        EditText editTextTime;
        AutoCompleteTextView  editTextarriver;
        AutoCompleteTextView editTextDepart;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ajouter_trajet);
            string date = DateTime.Now.Date.ToString();


            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //Image im1=protected override bool OnBackButtonPressed();
            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(Home));
            };

            


            EditText button1 = FindViewById<EditText>(Resource.Id.editTextdate);
            button1.Click += delegate
            {
                DatePickerFragmentHomes frag = DatePickerFragmentHomes.NewInstance(delegate (DateTime time)
                {
                    button1.Text = time.ToShortDateString();
                });
                frag.Show(FragmentManager, DatePickerFragmentHomes.TAG);
            };
            EditText buttontime = FindViewById<EditText>(Resource.Id.editTextTime);
            buttontime.Click += delegate
            {
                TimePickerFragment fragt = TimePickerFragment.NewInstance(delegate (string time)
                {
                    buttontime.Text = time;
                });
                fragt.Show(FragmentManager, TimePickerFragment.TAG);
            };
            AutoCompleteTextView textView = FindViewById<AutoCompleteTextView>(Resource.Id.editTextDepart);
            string[] COUNTRIES = new string[] {
 " Casablanca","Rabat","Marrakech","Fès","Tanger","Agadir","Meknès","Oujda","Kenitra","Safi","Essaouira","El Jadida","Taroudant","Taza",
"Nador","Khouribga","Berkane","Ksar El Kebir","Settat","Mohammedia"};
            var adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES);

            textView.Adapter = adapter;

            AutoCompleteTextView textView1 = FindViewById<AutoCompleteTextView>(Resource.Id.editTextarriver);
            string[] COUNTRIES1 = new string[]
            {
  "Casablanca","Rabat","Marrakech","Fès","Tanger","Agadir","Meknès","Oujda","Kenitra","Safi","Essaouira","El Jadida","Taroudant","Taza",
"Nador","Khouribga","Berkane","Ksar El Kebir","Settat","Mohammedia"};
            var adapter1 = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES1);

            textView1.Adapter = adapter1;
        
    //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!






    editTextdate = FindViewById<EditText>(Resource.Id.editTextdate);
            editTextTime = FindViewById<EditText>(Resource.Id.editTextTime);
            editTextDepart = FindViewById<AutoCompleteTextView>(Resource.Id.editTextDepart);
            editTextarriver = FindViewById<AutoCompleteTextView>(Resource.Id.editTextarriver);

            string edit = editTextarriver.Text;


            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += delegate
            {




                if (string.IsNullOrEmpty(editTextdate.Text) ||
                string.IsNullOrEmpty(editTextTime.Text) ||
                string.IsNullOrEmpty(editTextarriver.Text)
  )


                {
                    // display error message if any fields are missing
                    Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

                }
                else if (!(COUNTRIES1.Contains(editTextarriver.Text)))
                {
                    Toast.MakeText(this, "le nom de ville d'arriver non trouvable", ToastLength.Short).Show();

                }
                else if (!(COUNTRIES.Contains(editTextDepart.Text)))
                {
                    Toast.MakeText(this, "le nom de ville de depart non trouvable", ToastLength.Short).Show();

                }


                else
                {
                    var intent = new Intent(this, typeof(place_prix));

                    intent.PutExtra("date", editTextdate.Text);
                    intent.PutExtra("time", editTextTime.Text);
                    intent.PutExtra("depart", editTextDepart.Text);
                    intent.PutExtra("arriver", editTextarriver.Text);


                    StartActivity(intent);


                }




            };
}
    }
}