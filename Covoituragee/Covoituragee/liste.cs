using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Covoituragee
{
    [Activity(Label = "liste")]
    public class liste : Activity
    {
        List<demande> tabletrajets = new List<demande>();
        static string txtdate;
        static string txttime;
        static string txtdepart;
        static string txtarriver;
        static string txtprix;
        static string txtplace;
        static string txtportbag;
        static string txtbavare;
        static string txtfumer;
        static string txtanimaux;
        static string txtmusique;
        static string txtautre;

        protected override void OnCreate(Bundle savedInstanceState)
        {

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource

            SetContentView(Resource.Layout.liste);
            ImageView im1 = FindViewById<ImageView>(Resource.Id.imageView1);
            im1.Click += delegate
            {
                StartActivity(typeof(voiture));
            };
            ListView listeTrajets = FindViewById<ListView>(Resource.Id.listView1);

            listeTrajets.Adapter = new HomeScreenAdapter(this, tabletrajets);
            /**/
            txtdate = Intent.GetStringExtra("date");
            txttime = Intent.GetStringExtra("time");
            txtdepart = Intent.GetStringExtra("depart");
            txtarriver = Intent.GetStringExtra("arriver");
            txtprix = Intent.GetStringExtra("prix");
            txtplace = Intent.GetStringExtra("place");
            txtportbag = Intent.GetStringExtra("portbag");
            txtbavare = Intent.GetStringExtra("bavare");
            txtfumer = Intent.GetStringExtra("fumer");
            txtanimaux = Intent.GetStringExtra("animaux");
            txtmusique = Intent.GetStringExtra("musique");
            txtautre = Intent.GetStringExtra("autre");

            GetTrajets();


        }


        /* void OnListClickAccep(object sender ,AdapterView.ItemClickEventArgs e)
         {
             var listeTrajets = sender as ListView;

             var intent = new Intent(this, typeof(place_prix));


             intent.PutExtra("arriver", voiture_id);


             StartActivity(intent);

             //  var t = tabletrajets[e.Position];
             //Toast.MakeText(this, "verifier votre information", ToastLength.Short).Show();

             //   Android.Widget.Toast.MakeText(this, t.nom, Android.Widget.ToastLength.Short).Show();
         }*/
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        internal class demande
        {
            public string marque;
            public string model;
            public string modulAnn;
            public string Matricule;
            public string color;
            public int Utilisateur_voiture_id;
            public string txtdepart;


            public demande(string marque, string model, string modulAnn,
             string Matricule, string color, int Utilisateur_voiture_id)
            {
                this.marque = marque;
                this.model = model;
                this.modulAnn = modulAnn;
                this.Matricule = Matricule;
                this.color = color;
                this.Utilisateur_voiture_id = Utilisateur_voiture_id;
            }

        }
        internal class HomeScreenAdapter : BaseAdapter<demande>
        {
            List<demande> items;
            Activity context;

            public HomeScreenAdapter(Activity context, List<demande> items)
                : base()
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
                    view = context.LayoutInflater.Inflate(Resource.Layout.Donner, null);
                string MaeMod = item.marque + "|" + item.model;


                view.FindViewById<TextView>(Resource.Id.CarMarkModeleTextV).Text = MaeMod;

                view.FindViewById<TextView>(Resource.Id.ModuleAnnéTextV).Text = item.modulAnn;
                view.FindViewById<TextView>(Resource.Id.MatriculeTextV).Text = item.Matricule;
                view.FindViewById<TextView>(Resource.Id.colorTextV).Text = item.color;

                view.FindViewById<AppCompatButton>(Resource.Id.btnaccepted).Click += (sender, args) =>
                {
                    Intent intent = new Intent(view.Context, typeof(EnRout_2));

                    intent.PutExtra("UtilisateurVoitureId", item.Utilisateur_voiture_id);

                    intent.PutExtra("date", txtdate);
                    intent.PutExtra("time", txttime);

                    intent.PutExtra("txtdepart", txtdepart);
                    intent.PutExtra("arriver", txtarriver);
                    intent.PutExtra("prix", txtprix);
                    intent.PutExtra("place", txtplace);
                    intent.PutExtra("portbag", txtportbag);
                    intent.PutExtra("bavare", txtbavare);
                    intent.PutExtra("musique", txtmusique);
                    intent.PutExtra("fumer", txtfumer);
                    intent.PutExtra("autre", txtautre);
                    intent.PutExtra("animaux", txtanimaux);

                    view.Context.StartActivity(intent);
                };/**/


                return view;
            }
        }
        public void GetTrajets()
        {
            string txtdepart = Intent.GetStringExtra("depart");
            string sql =
            "select marque,Model,anneeModel,Matricule,Couleur,UV.utilisateur_voiture_id\r\n            from UtlisateurVoiture UV,voiture V\r\n            where V.voiture_id=UV.voiture_id and UV.id_utilisateur='9'";
            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {

                    //   command.ExecuteNonQuery();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {



                            // Convert the image data to a bitmap 


                            tabletrajets.Add(new demande(

                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            reader.GetString(4),
                            reader.GetInt32(5)


                            ));


                        }
                    }
                    con.Close();
                }


            }
        }/**/
    }
}