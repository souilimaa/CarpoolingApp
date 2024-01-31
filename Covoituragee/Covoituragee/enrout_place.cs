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
using System.Text;

namespace Covoituragee
{
    [Activity(Label = "enrout_place")]
    public class enrout_place : Activity
    {
        AutoCompleteTextView editTextrout;
        public static readonly string connectionString =MainActivity.connectionString;
        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            // Create your application here

            SetContentView(Resource.Layout.enrout_place);
            int Utilisateur_id = MainActivity.userId;

            Button button3 = FindViewById<Button>(Resource.Id.button1);
            button3.Click += delegate
            {
                StartActivity(typeof(Home));
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

            string txtdepart = Intent.GetStringExtra("depart");
            var txtdate = Intent.GetStringExtra("date");

            var txttime = Intent.GetStringExtra("time");
            // int date=id

            string txtarriver = Intent.GetStringExtra("arriver");
            string txtprix = Intent.GetStringExtra("prix");
            string txtplace = Intent.GetStringExtra("place");
            string txtcouleur = Intent.GetStringExtra("couleur");
            string txtmatricule = Intent.GetStringExtra("matricule");
            string txtmodule = Intent.GetStringExtra("module");
            string txtmarque = Intent.GetStringExtra("marque");
            string txtmodulever = Intent.GetStringExtra("modulever");

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
                                DECLARE @CarId int;
                                DECLARE @UtilisateurVoitureId INT;
                                DECLARE @trajetid INT;
                                INSERT INTO voiture(marque,Model,anneeModel,Matricule,Couleur) VALUES (@txtmarque,@txtmodulever,@txtmodule,@txtmatricule, @txtcouleur);
                                SET @CarId = SCOPE_IDENTITY();
                                INSERT INTO UtlisateurVoiture(id_utilisateur, voiture_id) VALUES (@Utilisateur_id, @CarId);
                                SET @UtilisateurVoitureId = SCOPE_IDENTITY();
                                INSERT INTO Trajet(utilisateur_voiture_id,source,destination,dateTrajet,TempsTrajet,nbrPlaces,prix,tailleBagageAutorisee,LieuRencontre) VALUES 
                                (@UtilisateurVoitureId,@txtdepart,
                                 @txtarriver,@txtdate,@txttime,@txtplace,@txtprix,@txtportbag,@rout);
                                SET @trajetid = SCOPE_IDENTITY();
                                INSERT INTO preference(trajet_id,bavardage,fumer,animaux,musique,autrePreferences) VALUES (@trajetid,@txtbavare,@txtfumer,@txtanimaux,@txtmusique,@txtautre);
                                COMMIT TRANSACTION;";
                                    using (SqlCommand commandes = new SqlCommand(commandText, connection))
                                    {
                                        commandes.Parameters.AddWithValue("@txtmarque", txtmarque);
                                        commandes.Parameters.AddWithValue("@txtmodulever", txtmodulever);
                                        commandes.Parameters.AddWithValue("@txtmodule", txtmodule);
                                        commandes.Parameters.AddWithValue("@txtmatricule", txtmatricule);
                                        commandes.Parameters.AddWithValue("@txtcouleur", txtcouleur);
                                        commandes.Parameters.AddWithValue("@Utilisateur_id", Utilisateur_id);

                                        commandes.Parameters.AddWithValue("@txtdepart", txtdepart);
                                        commandes.Parameters.AddWithValue("@txtarriver", txtarriver);
                                        commandes.Parameters.AddWithValue("@txtdate", DateTime.Parse(txtdate));
                                        commandes.Parameters.AddWithValue("@txttime", TimeSpan.Parse(txttime));
                                        commandes.Parameters.AddWithValue("@txtplace", txtplace);
                                        commandes.Parameters.AddWithValue("@txtprix", txtprix);
                                        commandes.Parameters.AddWithValue("@txtportbag", txtportbag);
                                        commandes.Parameters.AddWithValue("@rout", editTextrout.Text);


                                        commandes.Parameters.AddWithValue("@txtbavare", txtbavare);
                                        commandes.Parameters.AddWithValue("@txtfumer", txtfumer);
                                        commandes.Parameters.AddWithValue("@txtanimaux", txtanimaux);
                                        commandes.Parameters.AddWithValue("@txtmusique", txtmusique);
                                        commandes.Parameters.AddWithValue("@txtautre", txtautre);





                                        var command = commandes;
                                        command.Transaction = transaction; command.ExecuteNonQuery();
                                    }

                                    transaction.Commit();

                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    // Handle exception...
                                    Toast.MakeText(this, $"An error occurred: {ex.Message}", ToastLength.Long).Show();
                                }
                            }
                        }

                        StartActivity(typeof(Home));
                        Toast.MakeText(this, "Votre trajet est ajouté avec succès", ToastLength.Short).Show();



                    }

                };




            
        }
    }
}