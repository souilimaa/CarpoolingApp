using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using static Java.Util.Jar.Attributes;

namespace Covoituragee
{
    [Activity(Label = "ChangeUserInfosActivity")]

    public class ChangeUserInfosActivity : Activity
    {
        private Button cdatePickerButton;




        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.change_user_infos);
            // Create your application here
            ImageView arrowImageView = FindViewById<ImageView>(Resource.Id.arrow);
            arrowImageView.Click += (sender, e) => {

                Intent intent = new Intent(this, typeof(ProfileActivity));
                intent.PutExtra("id_utilisateur", MainActivity.userId);
                StartActivity(intent);
            };


            cdatePickerButton = FindViewById<Button>(Resource.Id.cdatePickerButton);
            if (cdatePickerButton != null)
            {
                cdatePickerButton.Click += OnDatePickerButtonClick;
            }
            Button saveButton = FindViewById<Button>(Resource.Id.btn_save_infos);
            saveButton.Click += SaveButton_Click;

            RadioGroup radioGroup = FindViewById<RadioGroup>(Resource.Id.cradioGroup);
            radioGroup.CheckedChange += RadioGroup_CheckedChange;
            EditText firstnameEditText = FindViewById<EditText>(Resource.Id.firstnameEditText);
            EditText lastnameEditText = FindViewById<EditText>(Resource.Id.lastnameEditText);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.semailEditText);
            EditText passwordEditText = FindViewById<EditText>(Resource.Id.cpasswordEditText);
            RadioButton maleRadioButton = FindViewById<RadioButton>(Resource.Id.cradioButtonFemale);
            RadioButton femaleRadioButton = FindViewById<RadioButton>(Resource.Id.cradioButtonMale);
            EditText phonenumberEditText = FindViewById<EditText>(Resource.Id.cphonenumberEditText);



            string sql = $"SELECT nom,prenom,DateNaissance,sexe,email,MotPasse,NumTel FROM Utilisateur WHERE id_utilisateur ={MainActivity.userId}";

            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // display user's information
                            string nom = reader.GetString(reader.GetOrdinal("nom"));
                            firstnameEditText.Text = nom;
                            string prenom = reader.GetString(reader.GetOrdinal("prenom"));
                            lastnameEditText.Text = prenom;
                            string datenaissance = reader.GetString(reader.GetOrdinal("DateNaissance"));
                            cdatePickerButton.Text = datenaissance;
                            string genre = reader.GetString(reader.GetOrdinal("sexe"));
                            if (genre == "Homme")
                            {
                                maleRadioButton.Checked = true;
                            }
                            else if (genre == "Femme")
                            {
                                femaleRadioButton.Checked = true;
                            }
                            string email = reader.GetString(reader.GetOrdinal("email"));
                            emailEditText.Text = email;
                            string password = reader.GetString(reader.GetOrdinal("MotPasse"));
                            passwordEditText.Text = password;
                            string phonenumber = reader.GetString(reader.GetOrdinal("NumTel"));
                            phonenumberEditText.Text = phonenumber;
                        }
                        else
                        {
                            // handle case when user is not found
                            // display an error message, or redirect to login/signup page
                        }
                    }
                }

                con.Close();
            }


        }



        private void SaveButton_Click(object sender, EventArgs e)
        {
            EditText firstnameEditText = FindViewById<EditText>(Resource.Id.firstnameEditText);
            EditText lastnameEditText = FindViewById<EditText>(Resource.Id.lastnameEditText);
            EditText emailEditText = FindViewById<EditText>(Resource.Id.semailEditText);
            EditText passwordEditText = FindViewById<EditText>(Resource.Id.cpasswordEditText);
            RadioButton maleRadioButton = FindViewById<RadioButton>(Resource.Id.cradioButtonFemale);
            Button cdatePickerButton = FindViewById<Button>(Resource.Id.cdatePickerButton);
            EditText phonenumberEditText = FindViewById<EditText>(Resource.Id.cphonenumberEditText);



            string phoneNumber = phonenumberEditText.Text.Trim();
            if (!Regex.IsMatch(phoneNumber, @"^(06|07)\d{8}$"))
            {
                // display error message if phone number is not valid
                Toast.MakeText(this, "Format du numéro de téléphone n'est pas valide", ToastLength.Short).Show();
                return;
            }
            string email = emailEditText.Text.Trim();
            if (!IsValidEmail(email))
            {
                // display error message if email is not valid
                Toast.MakeText(this, "Format de l'e-mail n'est pas valide", ToastLength.Short).Show();
                return;
            }

            string password = passwordEditText.Text;
            if (password.Length < 8 ||
                !Regex.IsMatch(password, @"[a-zA-Z]") ||
                !Regex.IsMatch(password, @"\d") ||
                !Regex.IsMatch(password, @"[\W_]"))
            {
                // display error message if password is not valid
                Toast.MakeText(this, "Le mot de passe doit comporter au moins 8 caractères, des lettres et des chiffres, et au moins un caractère spécial.", ToastLength.Short).Show();
                return;
            }
            string sql = $"UPDATE Utilisateur SET DateNaissance='{cdatePickerButton.Text}' ,nom = '{firstnameEditText.Text}', prenom = '{lastnameEditText.Text}', email = '{emailEditText.Text}', MotPasse = '{passwordEditText.Text}', sexe = '{(maleRadioButton.Checked ? "Male" : "Female")}',NumTel='{phonenumberEditText.Text}' WHERE id_utilisateur = {MainActivity.userId}";

            using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
            {
                con.Open();

                using (SqlCommand command = new SqlCommand(sql, con))
                {
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 1)
                    {
                        // display success message, or redirect to profile page
                    }
                    else
                    {
                        // handle case when user is not found
                        // display an error message, or redirect to login/signup page
                    }
                }

                con.Close();
            }
            // Redirect user to profile page after successful save
            Intent intent = new Intent(this, typeof(ProfileActivity));
            intent.PutExtra("id_utilisateur", MainActivity.userId);
            StartActivity(intent);
        }
        private void OnDatePickerButtonClick(object sender, EventArgs e)
        {
            Console.WriteLine($"cdatePickerButton is null? {cdatePickerButton == null}");

            CDatePickerFragment fragment = new CDatePickerFragment();
            fragment.Show(this.FragmentManager, CDatePickerFragment.TAG);
        }

        private void RadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int selectedId = e.CheckedId;

            if (selectedId == Resource.Id.cradioButtonMale)
            {
                // Male radio button is selected
            }
            else if (selectedId == Resource.Id.cradioButtonFemale)
            {
                // Female radio button is selected
            }
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // use the MailAddress class to validate the email syntax
                MailAddress addr = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}