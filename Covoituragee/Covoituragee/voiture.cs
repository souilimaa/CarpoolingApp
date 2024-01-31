using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Covoituragee
{
    [Activity(Label = "voiture")]
    public class voiture : Activity
    {
        AutoCompleteTextView editTextCouleurV, editTextMatriculeV, editTextModuleV, editTextMarqueV, editTextModuleVir;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.voiture);

            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(ref_bag));
            };

            AutoCompleteTextView textView = FindViewById<AutoCompleteTextView>(Resource.Id.editTextMarqueV);
            string[] COUNTRIES = new string[] {
 " KIA","VOLVO","DACIA","FIRARI","AUDI","Mercedes","Bentley","BMW ","Fiat ","Opel","Hyundai","TOYOTA","SKODA","JEEP",
"NISSAN","VOLKSWAGEN","PEUGEOT","CITRWIEN","Ford","SEAT"};
            var adapter = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES);

            textView.Adapter = adapter;

            AutoCompleteTextView textView1 = FindViewById<AutoCompleteTextView>(Resource.Id.editTextCouleurV);
            string[] COUNTRIES1 = new string[] {
 " blanc ","noir ","gris foncé","gris argent","bleu","rouge ","marron ","beige  ","vert  ","jaune "};
            var adapter1 = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIES1);

            textView1.Adapter = adapter1;

            editTextCouleurV = FindViewById<AutoCompleteTextView>(Resource.Id.editTextCouleurV);
            editTextMatriculeV = FindViewById<AutoCompleteTextView>(Resource.Id.editTextMatriculeV);
            editTextModuleV = FindViewById<AutoCompleteTextView>(Resource.Id.editTextModuleV);
            editTextMarqueV = FindViewById<AutoCompleteTextView>(Resource.Id.editTextMarqueV);
            editTextModuleVir = FindViewById<AutoCompleteTextView>(Resource.Id.editTextModuleVir);
            string txtdate = Intent.GetStringExtra("date");
            string txttime = Intent.GetStringExtra("time");
            string txtdepart = Intent.GetStringExtra("depart");
            string txtarriver = Intent.GetStringExtra("arriver");
            string txtprix = Intent.GetStringExtra("prix");
            string txtplace = Intent.GetStringExtra("place");
            string txtportbag = Intent.GetStringExtra("portbag");
            string txtbavare = Intent.GetStringExtra("bavare");
            string txtfumer = Intent.GetStringExtra("fumer");
            string txtanimaux = Intent.GetStringExtra("animaux");
            string txtmusique = Intent.GetStringExtra("musique");
            string txtautre = Intent.GetStringExtra("autre");
            if (editTextMatriculeV.Text == " KIA")
            {
                AutoCompleteTextView textViewmarque = FindViewById<AutoCompleteTextView>(Resource.Id.editTextModuleVir);
                string[] COUNTRIESmar = new string[] {
 " Picanto ","ceed ","sonet ","sportage","K5","carnival ","K2500 "};
                var adaptermar = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIESmar);

                textViewmarque.Adapter = adaptermar;
            }
            if (editTextMatriculeV.Text == " DACIA")
            {
                AutoCompleteTextView textViewmarque = FindViewById<AutoCompleteTextView>(Resource.Id.editTextModuleVir);
                string[] COUNTRIESmar = new string[] {
 " spring ","duster ","logan ","sandero sreetway","sandero sreepway","lodgy "};
                var adaptermar = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIESmar);

                textViewmarque.Adapter = adaptermar;
            }
            if (editTextMatriculeV.Text == " AUDI")
            {
                AutoCompleteTextView textViewmarque = FindViewById<AutoCompleteTextView>(Resource.Id.editTextModuleVir);
                string[] COUNTRIESmar = new string[] {
 " A3 ","A4 ","A5 ","A6","A7","A8 ","Q2","Q3"};
                var adaptermar = new ArrayAdapter<String>(this, Resource.Layout.list_item, COUNTRIESmar);

                textViewmarque.Adapter = adaptermar;
            }

            Button button1 = FindViewById<Button>(Resource.Id.button3);
            button1.Click += delegate
            {
                var intent = new Intent(this, typeof(liste));


                intent.PutExtra("date", txtdate);
                intent.PutExtra("time", txttime);
                intent.PutExtra("depart", txtdepart);
                intent.PutExtra("arriver", txtarriver);
                intent.PutExtra("prix", txtprix);
                intent.PutExtra("place", txtplace);
                intent.PutExtra("portbag", txtportbag);
                intent.PutExtra("bavare", txtbavare);
                intent.PutExtra("musique", txtmusique);
                intent.PutExtra("fumer", txtfumer);
                intent.PutExtra("autre", txtautre);
                intent.PutExtra("animaux", txtanimaux);



                StartActivity(intent);

            };

            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += delegate
            {
                if (string.IsNullOrEmpty(editTextCouleurV.Text) ||

             string.IsNullOrEmpty(editTextModuleV.Text) ||
             string.IsNullOrEmpty(editTextMarqueV.Text) ||
             string.IsNullOrEmpty(editTextModuleVir.Text)

             )
                {
                    // display error message if any fields are missing
                    Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

                }
                else if (!(COUNTRIES.Contains(editTextMarqueV.Text)))
                {
                    Toast.MakeText(this, "le nom de ville d'arriver non trouvable", ToastLength.Short).Show();

                }
                else if (!(COUNTRIES1.Contains(editTextCouleurV.Text)))
                {
                    Toast.MakeText(this, "la couleur non trouvable", ToastLength.Short).Show();

                }


                /* else if  (!isValidMatricule)
                  {
                      Toast.MakeText(this, "verifier le matricule", ToastLength.Short).Show();
                  }*/
                else
                {
                    var intent = new Intent(this, typeof(enrout_place));

                    intent.PutExtra("couleur", editTextCouleurV.Text);
                    intent.PutExtra("matricule", editTextMatriculeV.Text);
                    intent.PutExtra("module", editTextModuleV.Text);
                    intent.PutExtra("marque", editTextMarqueV.Text);
                    intent.PutExtra("modulever", editTextModuleVir.Text);
                    intent.PutExtra("date", txtdate);
                    intent.PutExtra("time", txttime);
                    intent.PutExtra("depart", txtdepart);
                    intent.PutExtra("arriver", txtarriver);
                    intent.PutExtra("prix", txtprix);
                    intent.PutExtra("place", txtplace);
                    intent.PutExtra("portbag", txtportbag);
                    intent.PutExtra("bavare", txtbavare);
                    intent.PutExtra("musique", txtmusique);
                    intent.PutExtra("fumer", txtfumer);
                    intent.PutExtra("autre", txtautre);
                    intent.PutExtra("animaux", txtanimaux);



                    StartActivity(intent);

                }
            };




        }
    }
}