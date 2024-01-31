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
    [Activity(Label = "ref_bag")]
    public class ref_bag : Activity
    {
        RadioButton radioButtonNObagage, radioButtonGrand, radioButtonPetit;
        CheckBox checkBoxBavar, checkBoxFumer, checkBoxAnimaux, checkBoxMusique;
        AutoCompleteTextView editTextautre;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ref_bag);

            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(place_prix));
            };

            radioButtonNObagage = FindViewById<RadioButton>(Resource.Id.radioButtonNObagage);
            radioButtonGrand = FindViewById<RadioButton>(Resource.Id.radioButtonGrand);
            radioButtonPetit = FindViewById<RadioButton>(Resource.Id.radioButtonPetit);
            checkBoxBavar = FindViewById<CheckBox>(Resource.Id.checkBoxBavar);
            checkBoxFumer = FindViewById<CheckBox>(Resource.Id.checkBoxFumer);
            checkBoxAnimaux = FindViewById<CheckBox>(Resource.Id.checkBoxAnimaux);
            checkBoxMusique = FindViewById<CheckBox>(Resource.Id.checkBoxMusique);
            editTextautre = FindViewById<AutoCompleteTextView>(Resource.Id.editTextautre);

            string txtdate = Intent.GetStringExtra("date");
            string txttime = Intent.GetStringExtra("time");
            string txtdepart = Intent.GetStringExtra("depart");
            string txtarriver = Intent.GetStringExtra("arriver");
            string txtprix = Intent.GetStringExtra("prix");
            string txtplace = Intent.GetStringExtra("place");


            Button button = FindViewById<Button>(Resource.Id.button1);
            button.Click += delegate
            {

                if ((!radioButtonNObagage.Checked && !radioButtonGrand.Checked && !radioButtonPetit.Checked)

              )
                {
                    // display error message if any fields are missing
                    Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

                }
                else
                {

                    var intent = new Intent(this, typeof(voiture));
                    intent.PutExtra("date", txtdate);
                    intent.PutExtra("time", txttime);
                    intent.PutExtra("depart", txtdepart);
                    intent.PutExtra("arriver", txtarriver);
                    intent.PutExtra("prix", txtprix);
                    intent.PutExtra("place", txtplace);
                    intent.PutExtra("autre", editTextautre.Text);
                    string bagage = "";
                    if (radioButtonNObagage.Checked)
                    {
                        bagage = "No bagage";
                    }
                    else if (radioButtonGrand.Checked)
                    {
                        bagage = "Grand";

                    }
                    else if (radioButtonPetit.Checked)
                    {
                        bagage = "petit";

                    }
                    intent.PutExtra("portbag", bagage);
                    string etat = "";
                    if (checkBoxBavar.Checked)
                    {
                        etat = "oui";
                    intent.PutExtra("bavare", etat);

                    }
                    else
                    {
                        etat = "non";
                        intent.PutExtra("bavare", etat);
                    }
                    if (checkBoxMusique.Checked)
                    {
                        etat = "oui";
                    intent.PutExtra("musique", etat);

                    }
                    else
                    {
                        etat = "non";
                        intent.PutExtra("musique", etat);
                    }

                    if (checkBoxFumer.Checked)
                    {
                        etat = "oui";
                    intent.PutExtra("fumer", etat);

                    }
                    else
                    {
                        etat = "non";
                        intent.PutExtra("fumer", etat);
                    }
                    
                    if (checkBoxAnimaux.Checked)
                    {
                        etat = "oui";
                    intent.PutExtra("animaux", etat);

                    }
                    else
                    {
                        etat = "non";
                        intent.PutExtra("animaux", etat);
                    }


                    StartActivity(intent);


                }
            };
        }
    }
}