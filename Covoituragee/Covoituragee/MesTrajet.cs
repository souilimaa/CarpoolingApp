using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.AppCompat.Widget;
using System;
using System.Data;
using System.Data.SqlClient;
using Android.Content;
using Google.Android.Material.Tabs;
using System.Collections.Generic;
using Android.Views;
using System.Drawing;
using Refractored.Controls;
using Bitmap = Android.Graphics.Bitmap;
using Android.Graphics.Drawables;
using System.Runtime.Remoting.Contexts;
using Java.Lang;
using static Android.Icu.Text.Transliterator;
using static Android.Widget.AdapterView;
using static Xamarin.Essentials.Platform;
using Intent = Android.Content.Intent;
using Java.Nio.Channels;
using static AndroidX.RecyclerView.Widget.RecyclerView;
using static Android.Views.WindowInsets;
using static Android.Content.ClipData;
using System.Threading.Tasks;
using Dalvik.Annotation;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;

using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using Dalvik.Annotation;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using static Android.Renderscripts.Sampler;

namespace Covoituragee
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme")]
    public class MesTrajet : AppCompatActivity
    {
        private static string transactionId = " ";
        int Utilisateur_id = MainActivity.userId;
        static public decimal montant;
        List<MesTrajets> tabletrajets = new List<MesTrajets>();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.MesTrajet);


























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



            GetTrajets(Utilisateur_id);
            ListView TrajetListView = FindViewById<ListView>(Resource.Id.TrajetsListView);

            if (tabletrajets.Count == 0)
            {
                getEmptyUserTrajets(TrajetListView);
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Visible;

            }
            else
            {
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Invisible;
                TrajetListView.Visibility = ViewStates.Visible;
                TrajetListView.Adapter = new HomeScreenAdapter(this, tabletrajets);


            }
            AppCompatButton trajetsOffertsBtn = FindViewById<AppCompatButton>(Resource.Id.trajetsOfferts);
            trajetsOffertsBtn.Click += (sender, args) =>
            {
                Intent intent = new Intent(this, typeof(TrajetsOfferts));

                intent.PutExtra("UtilisateurId", Utilisateur_id);
                StartActivity(intent);

            };



        }



        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        public void getEmptyUserTrajets(ListView l)
        {
            l.Visibility = ViewStates.Invisible;


        }

        public void GetTrajets(int utilisateur_id)
        {
            string sql = "\r\nselect nom,prenom,imageUtilisateur,UV.id_utilisateur,marque,model,T.trajet_id,source,destination,prix,dateTrajet,Status,nombrePlace\r\nfrom Utilisateur U,trajet T,UtlisateurVoiture UV,voiture V,demandeTrajet DT\r\nwhere DT.demandeur_id=@Utilisateur_id and DT.Etat_demande='ACCEPTED'\r\nand T.trajet_id=DT.trajet_id\r\nand UV.utilisateur_voiture_id=T.utilisateur_voiture_id\r\nand U.id_utilisateur=UV.id_utilisateur\r\nand V.voiture_id=UV.voiture_id";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@Utilisateur_id", MainActivity.userId);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);
                            string status = "NoUpdate";
                            if (reader.IsDBNull(11))
                            {
                                status = "NoUpdate";
                            }
                            else
                            {
                                status = reader.GetString(11);
                            }

                            tabletrajets.Add(new MesTrajets(reader.GetString(0), reader.GetString(1), userpic,
                            reader.GetInt32(3),
                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetInt32(6),
                            reader.GetString(7),
                            reader.GetString(8),
                            reader.GetDouble(9),
                            reader.GetDateTime(10),
                            status,
                            reader.GetInt32(12)
                            ));

                        }
                    }
                    con.Close();
                }



            }


        }

        public static int GetRattingValueForButton(int utilisateurID, int RatedUtilisateurID)
        {
            int utilisateurId = utilisateurID;
            int RUtilisateurID = RatedUtilisateurID;
            string sql = "select count(*) \r\nfrom Rating\r\nwhere UtilisateurID=@utilisateurID and RatedByUtilisateurID=@RutilisateurID";
            int res = 0;
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = sql;
                    command.Parameters.AddWithValue("@utilisateurID", utilisateurId);
                    command.Parameters.AddWithValue("@RutilisateurID", RUtilisateurID);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            res = (reader.GetInt32(0));
                        }
                    }
                }



                con.Close();

                return res;
            }

        }

        public class HomeScreenAdapter : BaseAdapter<MesTrajets>
        {
            List<MesTrajets> items;
            Activity context;
            public HomeScreenAdapter(Activity context, List<MesTrajets> items)
                : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override MesTrajets this[int position]
            {
                get { return items[position]; }
            }
            public override int Count
            {
                get { return items.Count; }
            }
            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                var item = items[position];
                View view = convertView;
                if (view == null) // no view to re-use, create new
                    view = context.LayoutInflater.Inflate(Resource.Layout.UserTrajets, null);
                string fullname = item.prenom + " " + item.nom;
                string carModel = item.marque + " | " + item.model;
                string prix = item.prix * item.nombrePlace + " Dhs";
                DateTime dt = item.trajetDate;
                string myDateString = dt.ToString("yyyy-MM-dd ");
                view.FindViewById<TextView>(Resource.Id.CarMarkModeleTextV).Text = carModel;
                view.FindViewById<TextView>(Resource.Id.TrajetPriceTextV).Text = prix;
                view.FindViewById<TextView>(Resource.Id.sourceTextV).Text = item.source;
                view.FindViewById<TextView>(Resource.Id.destinationTextV).Text = item.destination;
                view.FindViewById<TextView>(Resource.Id.UserFullNameTextV).Text = fullname;
                view.FindViewById<CircleImageView>(Resource.Id.userImage).SetImageBitmap(item.imageUtilisateur);
                view.FindViewById<TextView>(Resource.Id.dateTrajetTextV).Text = myDateString;
                view.FindViewById<TextView>(Resource.Id.NbrSeatsTextV).Text = "(" + item.nombrePlace + " Place(s))";

                if (item.status == "Started")
                {
                    view.FindViewById<TextView>(Resource.Id.btnMesTrajetsStatus).Text = "Commencé";
                    view.FindViewById<TextView>(Resource.Id.btnEvaluerConducteur).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Visibility = ViewStates.Gone;

                }
                else if (item.status == "Picked Up")
                {
                    view.FindViewById<TextView>(Resource.Id.btnMesTrajetsStatus).Text = "Pris les Passagers";
                    view.FindViewById<TextView>(Resource.Id.btnEvaluerConducteur).Visibility = ViewStates.Gone;
                    view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Visibility = ViewStates.Gone;


                }
                else if (item.status == "Completed")
                {
                    view.FindViewById<TextView>(Resource.Id.btnMesTrajetsStatus).Text = "Terminé";
                    int r = MesTrajet.GetRattingValueForButton(item.DriverID, MainActivity.userId);
                    if (r == 0)
                    {
                        view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerConducteur).Visibility = ViewStates.Visible;
                    }
                    view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Visibility = ViewStates.Gone;
                }

                else if (item.status == "NoUpdate")
                {
                    view.FindViewById<TextView>(Resource.Id.btnMesTrajetsStatus).Text = "Accepté";


                    string sql = "select count(*) from paiement where trajet_id=@trajetId and Passager_id=@userId\r\n";
                    int res = 0;
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        con.Open();

                        using (SqlCommand command = new SqlCommand())
                        {
                            command.Connection = con;
                            command.CommandText = sql;

                            command.Parameters.AddWithValue("@trajetId",item.TrajetID);
                            command.Parameters.AddWithValue("@userId", MainActivity.userId);

                            using (SqlDataReader reader = command.ExecuteReader())
                            {

                                while (reader.Read())
                                {
                                    res = (reader.GetInt32(0));
                                }
                            }
                        }



                        con.Close();

                    }


                    if (res != 0)
                    {
                        view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Visibility = ViewStates.Gone;

                    }



                    if (res == 0) {
                        view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Visibility = ViewStates.Visible;
                            }






                    view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerConducteur).Visibility = ViewStates.Gone;

                }

                view.FindViewById<AppCompatButton>(Resource.Id.btnEvaluerConducteur).Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(RateDriver));
                    intent.PutExtra("DriverID", item.DriverID);
                    intent.PutExtra("UtilisateurId", MainActivity.userId);
                    view.Context.StartActivity(intent);
                };


                view.Click += (sender, args) =>
            {
                Intent intent = new Intent(view.Context, typeof(trajetDetails));
                intent.PutExtra("trajetId", item.TrajetID);
                intent.PutExtra("UtilisateurId", MainActivity.userId);
                intent.PutExtra("DriverID", item.DriverID);
                view.Context.StartActivity(intent);
            };


                view.FindViewById<AppCompatButton>(Resource.Id.paypalbutton).Click += async (sender, e) =>
                {
                    decimal myDecimal = new decimal(System.Math.Round(item.prix*0.090));
                    MainActivity.montant = myDecimal;
                    try
                    {
                        // Créer la commande
                        var order = await MainActivity.CreateOrder();

                        // Rediriger l'utilisateur vers la page de paiement PayPal
                        var approvalUrl = order["links"][1]["href"].ToString();
                        Intent browserIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(approvalUrl));
                        context.StartActivity(browserIntent);

                        Console.WriteLine("1");

                        // Attendre que le client ait cliqué sur "review order"
                        var orderId = order["id"].ToString();
                        while (true)
                        {
                            var orderDetails = await MainActivity.GetOrderDetails(orderId);
                            if (orderDetails["status"].ToString() == "APPROVED")
                            {
                                break;
                            }
                            await Task.Delay(1000);
                        }

                        // Capturer le paiement
                        var captureData = await MainActivity.CapturePayment(orderId);

                        // TODO: stocker les informations de paiement telles que l'ID de transaction
                         transactionId = captureData["purchase_units"][0]["payments"]["captures"][0]["id"].ToString();
                        Console.WriteLine(transactionId.ToString());










                        using (SqlConnection conn = new SqlConnection(MainActivity.connectionString))
                        {
                            conn.Open();

                            // Create your insert query with parameter placeholders
                            string query = "INSERT INTO paiement (trajet_id, Passager_id,transactionId) VALUES (@param1,@param2,@param3)";

                            // Create a SqlCommand object with the query and connection
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                // Add the parameter values to the command
                                cmd.Parameters.AddWithValue("@param2", MainActivity.userId);
                                cmd.Parameters.AddWithValue("@param1", item.TrajetID);
                                cmd.Parameters.AddWithValue("@param3", transactionId);


                                // Execute the command
                                cmd.ExecuteNonQuery();

                                
                            }
                            conn.Close();
                        }




















                    }
                    catch (System.Exception ex)
                    {
                        // Gérer les erreurs
                        Console.WriteLine("Error: " + ex.Message);
                    }
                };







                return view;
            }
        }

        public class MesTrajets
        {
            public string nom;
            public string prenom;
            public Bitmap imageUtilisateur;
            public int DriverID;
            public string marque;
            public string model;
            public int TrajetID;
            public string source;
            public string destination;
            public double prix;
            public DateTime trajetDate;
            public string status;
            public int nombrePlace;
            public MesTrajets(string nom, string prenom,
                Bitmap imageUtilisateur, int DriverID, string marque,
                string model, int TrajetID, string source, string destination, double prix, DateTime trajetDate, string status, int nombrePlace)
            {
                this.nom = nom;
                this.prenom = prenom;
                this.imageUtilisateur = imageUtilisateur;
                this.marque = marque;
                this.model = model;
                this.TrajetID = TrajetID;
                this.source = source;
                this.destination = destination;
                this.prix = prix;
                this.trajetDate = trajetDate;
                this.status = status;
                this.nombrePlace = nombrePlace;
                this.DriverID = DriverID;
            }

        }


    }
}