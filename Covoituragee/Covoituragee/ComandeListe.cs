using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Org.Xmlpull.V1.Sax2;
using Refractored.Controls;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using static Android.Content.ClipData;

namespace Covoituragee
{
    [Activity(Label = "ComandeListe")]
    public class ComandeListe : Activity
    {
        string connection = MainActivity.connectionString;
        List<demande> tabletrajets = new List<demande>();
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.ComandeListe);
            ListView listeTrajets = FindViewById<ListView>(Resource.Id.listView1);
            int Utilisateur_id = MainActivity.userId;
            GetTrajets();

            listeTrajets.Adapter = new HomeScreenAdapter(this, tabletrajets);

            if (tabletrajets.Count == 0)
            {
                getEmptyUserTrajets(listeTrajets);
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Visible;

            }
            else
            {
                LinearLayout ly = FindViewById<LinearLayout>(Resource.Id.EmptyTrajets);
                ly.Visibility = ViewStates.Invisible;
                listeTrajets.Visibility = ViewStates.Visible;
                listeTrajets.Adapter = new HomeScreenAdapter(this, tabletrajets);


            }
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

            ImageButton backButton = FindViewById<ImageButton>(Resource.Id.backButton);
            backButton.Click += (sender, args) => { Finish(); };


        }
        public void getEmptyUserTrajets(ListView l)
        {
            l.Visibility = ViewStates.Invisible;


        }

        /*void OnListClickAccep(object sender ,AdapterView.ItemClickEventArgs e)
        {
            var listeTrajets = sender as ListView;
          //  var t = tabletrajets[e.Position];
            Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

            //   Android.Widget.Toast.MakeText(this, t.nom, Android.Widget.ToastLength.Short).Show();
        }*/
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        internal class demande
        {
            public int demande_id;
            public string nom;
            public string prenom;
            public Bitmap imageUtilisateur;

            public string source;
            public string destination;
            public DateTime demandeDate;
            public int nombrePlace;
            public int Utilisateur_id;

            public demande(string nom, string prenom,
                 Bitmap imageUtilisateur, int demande_id,
                 string source, string destination,
                 DateTime demandtDate, int nombrePlace, int Utilisateur_id)
            {
                this.nom = nom;
                this.prenom = prenom;
                this.imageUtilisateur = imageUtilisateur;
                this.source = source;
                this.destination = destination;
                this.demandeDate = demandtDate;
                this.nombrePlace = nombrePlace;
                this.demande_id = demande_id;
                this.Utilisateur_id = Utilisateur_id;

            }

        }
        internal class HomeScreenAdapter : BaseAdapter<demande>
        {
            List<demande> items;
            Activity context;
            private int demande_id;

            public HomeScreenAdapter(Activity context, List<demande> items) : base()
            {
                this.context = context;
                this.items = items;
            }
            public override long GetItemId(int position)
            {
                return position;
            }
            public override demande this[int position]
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.InfoComande, null);
                string fullname = item.prenom + " " + item.nom;
                DateTime dt = item.demandeDate;
                string myDateString = dt.ToString("yyyy - MM - dd");
                view.FindViewById<TextView>(Resource.Id.UserFullNameTextV).Text = fullname;
                view.FindViewById<CircleImageView>(Resource.Id.userImage).SetImageBitmap(item.imageUtilisateur);

                view.FindViewById<TextView>(Resource.Id.sourceTextV).Text = item.source;
                view.FindViewById<TextView>(Resource.Id.destinationTextV).Text = item.destination;
                int RatingAvg = 0;
                int nombreAvis = 0;
                string sql = "select AVG(Rating),count(*)\r\nfrom Rating,demandeTrajet D\r\nwhere demande_id=@demandeId \r\nand Rating.UtilisateurId=D.demandeur_id";
                using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                {
                    using (SqlCommand command = new SqlCommand(sql, con))
                    {
                        command.Parameters.Add("@demandeId", item.demande_id);
                        con.Open();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                if (!reader.IsDBNull(0))
                                {
                                    RatingAvg = reader.GetInt32(0);
                                    nombreAvis = reader.GetInt32(1);

                                }


                            }


                        }
                        con.Close();

                    }

                }
                if (RatingAvg == 0)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(0 avis)";


                }
                else if (RatingAvg == 1)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);

                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(" + nombreAvis + " avis)";

                }
                else if (RatingAvg == 2)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(" + nombreAvis + " avis)";


                }
                else if (RatingAvg == 3)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);
                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(" + nombreAvis + " avis)";


                }
                else if (RatingAvg == 4)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIcone);
                    R5.SetImageResource(Resource.Drawable.starRatingIconeGray);
                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(" + nombreAvis + " avis)";

                }
                else if (RatingAvg == 5)
                {
                    ImageView R1 = view.FindViewById<ImageView>(Resource.Id.R1);
                    ImageView R2 = view.FindViewById<ImageView>(Resource.Id.R2);
                    ImageView R3 = view.FindViewById<ImageView>(Resource.Id.R3);
                    ImageView R4 = view.FindViewById<ImageView>(Resource.Id.R4);
                    ImageView R5 = view.FindViewById<ImageView>(Resource.Id.R5);

                    R1.SetImageResource(Resource.Drawable.starRatingIcone);
                    R2.SetImageResource(Resource.Drawable.starRatingIcone);
                    R3.SetImageResource(Resource.Drawable.starRatingIcone);
                    R4.SetImageResource(Resource.Drawable.starRatingIcone);
                    R5.SetImageResource(Resource.Drawable.starRatingIcone);
                    TextView nbrAvis = view.FindViewById<TextView>(Resource.Id.nbrAvisTV);
                    nbrAvis.Text = "(" + nombreAvis + " avis)";


                }


                view.FindViewById<TextView>(Resource.Id.dateTrajetTextV).Text = myDateString;
                view.FindViewById<TextView>(Resource.Id.NbrSeatsTextV).Text = "(" + item.nombrePlace + " Places)";
                Intent intent = new Intent(view.Context, typeof(Home));
                view.FindViewById<AppCompatButton>(Resource.Id.btnaccepted).Click += (sender, args) =>
                {
                    using (var connections = new SqlConnection(MainActivity.connectionString))
                    {
                        connections.Open();
                        string sql = @"UPDATE demandeTrajet
                                                     SET
                                                     Etat_demande = 'ACCEPTED'
                                                     WHERE demande_id=@demande_id;";
                        using (SqlCommand command = new SqlCommand(sql, connections))
                        {
                            command.Parameters.AddWithValue("@demande_id", item.demande_id);


                            command.ExecuteNonQuery();
                        }
                        connections.Close();

                    }
                    context.Recreate();
                };
                view.FindViewById<AppCompatButton>(Resource.Id.btnrefuse).Click += (sender, args) =>
                {


                    using (var connections = new SqlConnection(MainActivity.connectionString))
                    {
                        connections.Open();

                        string sql = @"UPDATE demandeTrajet
                                                     SET
                                                     Etat_demande ='REFUSED'
                                                     WHERE demande_id=@txtID;";
                        using (SqlCommand command = new SqlCommand(sql, connections))
                        {
                            command.Parameters.AddWithValue("@txtID", item.demande_id);


                            command.ExecuteNonQuery();
                        }
                        connections.Close();

                    }

                    context.Recreate();

                };

                return view;
            }
        }
        public void GetTrajets()
        {
            int Utilisateur_id = MainActivity.userId;

            string sql =
            "SELECT U.nom, U.prenom, U.imageUtilisateur, DT.nombrePlace, T.source, T.destination,T.dateTrajet,DT.demande_id FROM Utilisateur U INNER JOIN DemandeTrajet DT ON U.id_utilisateur = DT.demandeur_id INNER JOIN Trajet T ON DT.trajet_id = T.trajet_id INNER JOIN UtlisateurVoiture UV ON T.utilisateur_voiture_id = UV.utilisateur_voiture_id WHERE UV.id_utilisateur =@id_utilisateur and DT.Etat_demande IS NULL";
            using (SqlConnection con = new SqlConnection(connection))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    command.Parameters.AddWithValue("@id_utilisateur", Utilisateur_id);


                    command.ExecuteNonQuery();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {

                            byte[] imageData = (byte[])reader["imageUtilisateur"];

                            // Convert the image data to a bitmap 
                            Bitmap userpic = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length);


                            tabletrajets.Add(new demande(
                            reader.GetString(0),
                            reader.GetString(1),
                            userpic,

                            reader.GetInt32(7),

                            reader.GetString(4),
                            reader.GetString(5),
                            reader.GetDateTime(6),
                            reader.GetInt32(3),
                            reader.GetInt32(7)

                            ));

                        }
                    }
                    con.Close();
                }



            }
        }










    }
}