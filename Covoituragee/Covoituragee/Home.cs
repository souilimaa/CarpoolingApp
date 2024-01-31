using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace Covoituragee
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class Home : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Home);

            LinearLayout homeLayout = FindViewById<LinearLayout>(Resource.Id.homeLayout);
            homeLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(Home));
                StartActivity(intent);

            };

            LinearLayout trajetLayout = FindViewById<LinearLayout>(Resource.Id.trajetLayout);
            trajetLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(MesTrajet));

                intent.PutExtra("UtilisateurId", MainActivity.userId);
                StartActivity(intent);
            };

            LinearLayout searchLayout = FindViewById<LinearLayout>(Resource.Id.searchLayout);
            searchLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(ComandeListe));
                StartActivity(intent);

            };



            LinearLayout profilLayout = FindViewById<LinearLayout>(Resource.Id.profilLayout);
            profilLayout.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(ProfileActivity));
                intent.PutExtra("id_utilisateur", MainActivity.userId);
                StartActivity(intent);
            };


            EditText editTextdaterev = FindViewById<EditText>(Resource.Id.editTextdaterev);
            AutoCompleteTextView editTextdepartrev = FindViewById<AutoCompleteTextView>(Resource.Id.editTextdepartrev);
            AutoCompleteTextView editTextarriverrev = FindViewById<AutoCompleteTextView>(Resource.Id.editTextarriverrev);

            //  ImageView Demande = FindViewById<ImageView>(Resource.Id.searchImage);
            // Demande.Click += delegate
            // {
            //     StartActivity(typeof(ComandeListe));
            // };

            Button button2 = FindViewById<Button>(Resource.Id.button2);
            button2.Click += delegate
            {
                StartActivity(typeof(ajoute_trajet));
            };


            EditText button1 = FindViewById<EditText>(Resource.Id.editTextdaterev);
            button1.Click += delegate
            {
                DatePickerFragmentHomes frag = DatePickerFragmentHomes.NewInstance(delegate (DateTime time)
                {
                    button1.Text = time.ToString("yyyy-MM-dd");
                });
                frag.Show(FragmentManager, DatePickerFragmentHomes.TAG);
            };
            AutoCompleteTextView textView = FindViewById<AutoCompleteTextView>(Resource.Id.editTextdepartrev);
            string[] COUNTRIES = new string[] {
 " Casablanca","Rabat","Marrakech","Fès","Tanger","Agadir","Meknès","Oujda","Kenitra","Safi","Essaouira","El Jadida","Taroudant","Taza",
"Nador","Khouribga","Berkane","Ksar El Kebir","Settat","Mohammedia"};
            var adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES);

            textView.Adapter = adapter;

            AutoCompleteTextView textView1 = FindViewById<AutoCompleteTextView>(Resource.Id.editTextarriverrev);
            string[] COUNTRIES1 = new string[] {
  " Casablanca","Rabat","Marrakech","Fès","Tanger","Agadir","Meknès","Oujda","Kenitra","Safi","Essaouira","El Jadida","Taroudant","Taza",
"Nador","Khouribga","Berkane","Ksar El Kebir","Settat","Mohammedia"};
            var adapter1 = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES1);

            textView1.Adapter = adapter1;

            Button button = FindViewById<Button>(Resource.Id.button3);
            button.Click += delegate {
                if (string.IsNullOrEmpty(editTextdaterev.Text) ||
              string.IsNullOrEmpty(editTextdepartrev.Text) ||
              string.IsNullOrEmpty(editTextarriverrev.Text))
                {
                    Toast.MakeText(this, "Tous les champs sont obligatoires!", ToastLength.Short).Show();
                }
                else
                {
                    var intent = new Intent(this, typeof(TrajetsListViewRecherche));
                    intent.PutExtra("date", editTextdaterev.Text);
                    intent.PutExtra("depart", editTextdepartrev.Text);
                    intent.PutExtra("arriver", editTextarriverrev.Text);

                    StartActivity(intent);
                }
            };

        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}