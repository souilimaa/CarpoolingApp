using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using System;
using System.Data.SqlClient;
using System.Data;
using Android.Preferences;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Mail;
using System.Drawing;
using System.IO;
using Android.Graphics;








namespace Covoituragee
{
    [Activity(Label = "SignUpActivity")]
    public class SignUpActivity : Activity
    {

        bool isFormComplete = false;

        EditText lastnameEditText, firstnameEditText, semailEditText, spasswordEditText, cinEditText, phonenumberEditText;
        RadioButton radioButtonMale, radioButtonFemale;

        private Button datePickerButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.signUp);

            lastnameEditText = FindViewById<EditText>(Resource.Id.lastnameEditText);
            firstnameEditText = FindViewById<EditText>(Resource.Id.firstnameEditText);
            semailEditText = FindViewById<EditText>(Resource.Id.semailEditText);
            spasswordEditText = FindViewById<EditText>(Resource.Id.spasswordEditText);
            cinEditText = FindViewById<EditText>(Resource.Id.cinEditText);
            phonenumberEditText = FindViewById<EditText>(Resource.Id.phonenumberEditText);
            radioButtonMale = FindViewById<RadioButton>(Resource.Id.radioButtonMale);
            radioButtonFemale = FindViewById<RadioButton>(Resource.Id.radioButtonFemale);


            Button signUpButton = FindViewById<Button>(Resource.Id.sbtn_sign_in);
            signUpButton.Click += delegate
            {
                // validate form fields
                if (string.IsNullOrEmpty(lastnameEditText.Text) ||
                    string.IsNullOrEmpty(firstnameEditText.Text) ||
                    string.IsNullOrEmpty(semailEditText.Text) ||
                    string.IsNullOrEmpty(spasswordEditText.Text) ||
                    string.IsNullOrEmpty(cinEditText.Text) ||
                    string.IsNullOrEmpty(phonenumberEditText.Text) ||
                    string.IsNullOrEmpty(datePickerButton.Text) ||
                    (!radioButtonMale.Checked && !radioButtonFemale.Checked))
                {
                    // display error message if any fields are missing
                    Toast.MakeText(this, "Veuillez remplir tous les champs", ToastLength.Short).Show();
                    isFormComplete = false;
                }
                if (!IsValidEmail(semailEditText.Text))
                {
                    // display error message if email syntax is invalid
                    Toast.MakeText(this, "Adresse électronique invalide", ToastLength.Short).Show();
                    isFormComplete = false;
                    return;
                }
                // validate password
                if (spasswordEditText.Text.Length < 8 ||
                    !Regex.IsMatch(spasswordEditText.Text, @"[a-zA-Z]") ||
                    !Regex.IsMatch(spasswordEditText.Text, @"\d") ||
                    !Regex.IsMatch(spasswordEditText.Text, @"[\W_]"))
                {
                    // display error message if password is invalid
                    Toast.MakeText(this, "Le mot de passe doit comporter au moins 8 caractères, des lettres et des chiffres, et au moins un caractère spécial.", ToastLength.Short).Show();
                    isFormComplete = false;
                    return;
                }

                else
                {
                    isFormComplete = true;

                    //database connection 
                    using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                        con.Open();

                        // check if email already exists
                        string checkEmailSql = "SELECT COUNT(*) FROM Utilisateur WHERE email = @email";
                        using (SqlCommand checkEmailCmd = new SqlCommand(checkEmailSql, con))
                        {
                            checkEmailCmd.Parameters.AddWithValue("@email", semailEditText.Text);
                            int emailCount = (int)checkEmailCmd.ExecuteScalar();
                            if (emailCount > 0)
                            {
                                // display error message if email already exists
                                Toast.MakeText(this, "L'e-mail existe déjà", ToastLength.Short).Show();
                                isFormComplete = false;
                                return;
                            }
                        }
                        // check if cin already exists
                        string checkCinSql = "SELECT COUNT(*) FROM Utilisateur WHERE cin = @cin";
                        using (SqlCommand checkCinCmd = new SqlCommand(checkCinSql, con))
                        {
                            checkCinCmd.Parameters.AddWithValue("@cin", cinEditText.Text);
                            int cinCount = (int)checkCinCmd.ExecuteScalar();
                            if (cinCount > 0)
                            {
                                // display error message if cin already exists
                                Toast.MakeText(this, "Le CIN existe déjà", ToastLength.Short).Show();
                                isFormComplete = false;
                                return;
                            }
                        }

                        // check if phone number follows Moroccan phone number specifications
                        string phoneNumber = phonenumberEditText.Text.Trim();
                        if (!Regex.IsMatch(phoneNumber, @"^(06|07)\d{8}$"))
                        {
                            // display error message if phone number is not valid
                            Toast.MakeText(this, "Le format du numéro de téléphone n'est pas valide", ToastLength.Short).Show();
                            isFormComplete = false;
                            return;
                        }

                        // check if CIN is valid
                        string cin = cinEditText.Text.Trim();
                        if (cin.Length < 1 || cin.Length > 7)
                        {
                            // display error message if CIN is not within length range
                            Toast.MakeText(this, "Le CIN doit contenir de 1 à 7 caractères", ToastLength.Short).Show();
                            isFormComplete = false;
                            return;
                        }
                        if (!Regex.IsMatch(cin, @"^[a-zA-Z][a-zA-Z0-9]{5}\d?$"))
                        {
                            // display error message if CIN format is not valid
                            Toast.MakeText(this, "Le format CIN n'est pas valide", ToastLength.Short).Show();
                            isFormComplete = false;
                            return;
                        }
                        // First, retrieve the image from the drawable folder as a Bitmap
                        Android.Graphics.Bitmap bmp = BitmapFactory.DecodeResource(Resources, Resource.Drawable.profile_image);

                        // Convert the Bitmap to a byte array
                        byte[] imageBytes;
                        using (MemoryStream stream = new MemoryStream())
                        {
                            bmp.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 0, stream);
                            imageBytes = stream.ToArray();
                        }

                        string sql = "INSERT INTO Utilisateur (nom, prenom, email, MotPasse, cin, NumTel, DateNaissance, sexe,imageUtilisateur) VALUES (@nom, @prenom, @email, @MotPasse, @cin, @NumTel, @DateNaissance, @sexe,@imageBytes)";
                        using (SqlCommand command = new SqlCommand(sql, con))
                        {
                            command.Parameters.AddWithValue("@nom", lastnameEditText.Text);
                            command.Parameters.AddWithValue("@prenom", firstnameEditText.Text);
                            command.Parameters.AddWithValue("@email", semailEditText.Text);
                            command.Parameters.AddWithValue("@MotPasse", spasswordEditText.Text);
                            command.Parameters.AddWithValue("@cin", cinEditText.Text);
                            command.Parameters.AddWithValue("@NumTel", phonenumberEditText.Text);
                            command.Parameters.AddWithValue("@DateNaissance", datePickerButton.Text);
                            string genre = "";
                            if (radioButtonMale.Checked)
                            {
                                genre = "Femme";
                            }
                            else if (radioButtonFemale.Checked)
                            {
                                genre = "Homme";
                            }
                            command.Parameters.AddWithValue("@sexe", genre);
                            command.Parameters.Add("@imageBytes", SqlDbType.VarBinary).Value = imageBytes;

                            command.ExecuteNonQuery();
                        }
                        con.Close();
                    }
                }
            };
            Button button = FindViewById<Button>(Resource.Id.sbtn_sign_in);
            button.Click += delegate
            {
                if (isFormComplete)
                {
                    // get user ID from database
                    //int userId = 0;
                    //using (SqlConnection con = new SqlConnection(MainActivity.connectionString))
                    {
                      //  con.Open();
                     //   string sql = "SELECT id_utilisateur FROM Utilisateur WHERE email = @email";
                     //   using (SqlCommand command = new SqlCommand(sql, con))
                       // {
                          //  command.Parameters.AddWithValue("@email", semailEditText.Text);
                          //  userId = (int)command.ExecuteScalar();
                        }
                       // con.Close();
                   // }
                    //MainActivity.userId = userId;
                    // store the user ID in a shared preference
                   // ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                   // ISharedPreferencesEditor editor = prefs.Edit();
                   // editor.PutInt("id_utilisateur", userId);
                   // editor.Apply();

                    // start ProfileActivity and pass user ID as extra
                    Intent intent = new Intent(this, typeof(LoginActivity));
                    StartActivity(intent);
                }
                else
                {
                    // display error message if form is not complete
                    Toast.MakeText(this, "Veuillez remplir tous les champs", ToastLength.Short).Show();
                }



            };





            datePickerButton = FindViewById<Button>(Resource.Id.datePickerButton);
            datePickerButton.Click += OnDatePickerButtonClick;

            RadioGroup radioGroup = FindViewById<RadioGroup>(Resource.Id.radioGroup);
            radioGroup.CheckedChange += RadioGroup_CheckedChange;

            // Get a reference to the TextView
            TextView textView = FindViewById<TextView>(Resource.Id.text_view2);

            // Set a click listener for the TextView
            textView.Click += TextView_Click;


        }

        private void OnDatePickerButtonClick(object sender, EventArgs e)
        {
            DatePickerFragment fragment = new DatePickerFragment();
            fragment.Show(FragmentManager, DatePickerFragment.TAG);
        }

        private void RadioGroup_CheckedChange(object sender, RadioGroup.CheckedChangeEventArgs e)
        {
            int selectedId = e.CheckedId;

            if (selectedId == Resource.Id.radioButtonMale)
            {
                // Male radio button is selected
            }
            else if (selectedId == Resource.Id.radioButtonFemale)
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



        private void TextView_Click(object sender, EventArgs e)
        {
            // Create a new Intent to start the RegistrationActivity
            Intent intent = new Intent(this, typeof(LoginActivity));

            // Start the new activity
            StartActivity(intent);
        }
    }
}