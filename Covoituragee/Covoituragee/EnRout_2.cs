using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using static Android.Content.ClipData;

namespace Covoituragee 
{
    [Activity(Label = "EnRout_2")]
    public class EnRout_2 : Activity
    {
        AutoCompleteTextView editTextrout;
        string connectionString = MainActivity.connectionString;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.EnRout_2);

            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(voiture));
            };
            /* var textView1 = FindViewById<TextView>(Resource.Id.textView1);
             var textView2 = FindViewById<TextView>(Resource.Id.textView2);
             var textView3 = FindViewById<TextView>(Resource.Id.textView3);
             var textView4 = FindViewById<TextView>(Resource.Id.textView4);
             var textView5 = FindViewById<TextView>(Resource.Id.textView5);
             var textView6 = FindViewById<TextView>(Resource.Id.textView6);
             var textView7 = FindViewById<TextView>(Resource.Id.textView7);
             var textView8 = FindViewById<TextView>(Resource.Id.textView8);
             var textView9 = FindViewById<TextView>(Resource.Id.textView9);
             var textView10 = FindViewById<TextView>(Resource.Id.textView10);
             var textView11 = FindViewById<TextView>(Resource.Id.textView11);
             var textView12 = FindViewById<TextView>(Resource.Id.textView12);
             var textView13 = FindViewById<TextView>(Resource.Id.textView13);
             var textView14 = FindViewById<TextView>(Resource.Id.textView14);
             var textView15 = FindViewById<TextView>(Resource.Id.textView15);
             var textView16 = FindViewById<TextView>(Resource.Id.textView16); 
             var textView17 = FindViewById<TextView>(Resource.Id.textView17);
             var textView18 = FindViewById<TextView>(Resource.Id.textView18);
             var textView19 = FindViewById<TextView>(Resource.Id.textView19);
             var textView20 = FindViewById<TextView>(Resource.Id.textView20);
             var textView21 = FindViewById<TextView>(Resource.Id.textView21);
             var textView22 = FindViewById<TextView>(Resource.Id.textView22);*/
            int UtilisateurVoitureId = Intent.GetIntExtra("UtilisateurVoitureId", 0);

            string txtdepart = Intent.GetStringExtra("txtdepart");
            string txtdate = Intent.GetStringExtra("date");
            string txttime = Intent.GetStringExtra("time");
            // int date=id

            string txtarriver = Intent.GetStringExtra("arriver");
            string txtprix = Intent.GetStringExtra("prix");
            string txtplace = Intent.GetStringExtra("place");

            string txtportbag = Intent.GetStringExtra("portbag");
            string txtbavare = Intent.GetStringExtra("bavare");
            string txtfumer = Intent.GetStringExtra("fumer");
            string txtanimaux = Intent.GetStringExtra("animaux");
            string txtmusique = Intent.GetStringExtra("musique");
            string txtautre = Intent.GetStringExtra("autre");


            /*textView1.Text = txtdepart;
                        textView2.Text = txttime;
                        textView3.Text = txtdate;
                        textView4.Text = txtarriver;
                        textView5.Text = txtprix;
                        textView6.Text = txtplace;
                        textView7.Text = txtcouleur;
                        textView8.Text = txtmatricule;
                        textView9.Text = txtmarque;
                        textView11.Text = txtmodule;
                        textView10.Text = txtportbag;
                        textView15.Text = txtfumer;

                        //======>===>==>
                        textView19.Text = txtbavare;
                        textView20.Text = txtanimaux;
                        textView21.Text = txtmusique;
                        textView22.Text = txtautre;*/
            editTextrout = FindViewById<AutoCompleteTextView>(Resource.Id.editTextrout);
            string rout = editTextrout.Text;
            {
                Button button2 = FindViewById<Button>(Resource.Id.button3);
                button2.Click += delegate
                {
                    if (string.IsNullOrEmpty(editTextrout.Text)
                  )
                    {
                        // display error message if any fields are missing
                        Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

                    }
                    else
                    {
                        using (var connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            using (var transaction = connection.BeginTransaction())
                            {
                                try
                                {
                                    var commandText = @"BEGIN TRANSACTION;
                                    DECLARE @trajetid INT;
                                    
                                     INSERT INTO trajet(utilisateur_voiture_id,source,destination,dateTrajet,tempsTrajet,nbrPlaces,prix,tailleBagageAutorisee,LieuRencontre) 
    VALUES (@UtilisateurVoitureId,@txtdepart,@txtarriver,@txtdate,@txttime,@txtplace,@txtprix,@txtportbag,@rout);
    SET @trajetid = SCOPE_IDENTITY();
    INSERT INTO preference(trajet_id,bavardage,fumer,animaux,musique,autrePreferences) 
    VALUES (@trajetid,@txtbavare,@txtfumer,@txtanimaux,@txtmusique,@txtautre);;
                                    COMMIT TRANSACTION;";
                                    using (var command = new SqlCommand(commandText, connection, transaction))
                                    {
                                        command.Parameters.AddWithValue("@UtilisateurVoitureId", UtilisateurVoitureId);
                                        command.Parameters.AddWithValue("@txtdepart", txtdepart);
                                        command.Parameters.AddWithValue("@txtarriver", txtarriver);
                                        command.Parameters.AddWithValue("@txtdate", DateTime.Parse(txtdate));
                                        command.Parameters.AddWithValue("@txttime", TimeSpan.Parse(txttime));
                                        command.Parameters.AddWithValue("@txtplace", txtplace);
                                        command.Parameters.AddWithValue("@txtprix", txtprix);
                                        command.Parameters.AddWithValue("@txtportbag", txtportbag);
                                        command.Parameters.AddWithValue("@rout", editTextrout.Text);
                                        command.Parameters.AddWithValue("@txtbavare", txtbavare);
                                        command.Parameters.AddWithValue("@txtfumer", txtfumer);
                                        command.Parameters.AddWithValue("@txtanimaux", txtanimaux);
                                        command.Parameters.AddWithValue("@txtmusique", txtmusique);
                                        command.Parameters.AddWithValue("@txtautre", txtautre);
                                        command.ExecuteNonQuery();
                                    }

                                    transaction.Commit();
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    // Handle exception...
                                    //
                                    Toast.MakeText(this, ex.Message, ToastLength.Short).Show();
                                }
                            }
                        }
                    }



                    StartActivity(typeof(Home));
                    Toast.MakeText(this, "Votre trajet est ajouté avec succès.", ToastLength.Short).Show();


                };



            }
        }
    }
}