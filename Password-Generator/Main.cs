/*
* Author: Greg Uctum
* Note: Started off from a template, but changed paramaters to suit my needs. Also added in additional features.
* Version: 1.3
* Initial Date: 8/15/2018
* Updated: 8/31/2018
*/

using System;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Security.Principal;
using Password_Generator.CodeShare.Library.Passwords;

namespace Password_Generator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            this.ActiveControl = txtName;
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            if (txtName.Text == "")
            {
                MessageBox.Show("This field needs to be filled in!");
                return;
            }

            bool includeLowercase = true;
            bool includeUppercase = true;
            bool includeNumeric = true;
            bool includeSpecial = true;
            bool includeSpaces = false;
            int lengthOfPassword = 17;

            string password = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

            while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, password))
            {
                password = PasswordGenerator.GeneratePassword(true, true, true, true, false, lengthOfPassword);
            }

            lblPassword.Text = password;

            // Gets username of the logged in user. This allows me to deploy this across multiple systems if I so desire.

            WindowsIdentity user = WindowsIdentity.GetCurrent();
            string userName = user.Name.Substring(user.Name.LastIndexOf("\\") + 1);

            string path = @"C:\Users\" + userName + @"\Documents\Keys.txt"; // Somewhat more secure file name..? This was pretty much to see if I could.

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(txtName.Text + ": " + password);
                }
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(txtName.Text + ": " + password);
            }

        }


        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }

    namespace CodeShare.Library.Passwords
    {
        public static class PasswordGenerator
        {
            public static string GeneratePassword(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, int lengthOfPassword)
            {
                const int maximumIdenticalConsecutiveChars = 2;
                const string lowercaseCharacters = "abcdefghijklmnopqrstuvwxyz";
                const string uppercaseCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                const string numericCharacters = "0123456789";
                const string specialCharacters = @"!#&*@\";
                const string spaceCharacter = " ";
                const int passwordLengthMin = 8;
                const int passwordLengthMax = 30;

                if (lengthOfPassword < passwordLengthMin || lengthOfPassword > passwordLengthMax)
                {
                    return "Password length must be between 8 and 30.";
                }

                string characterSet = "";

                if (includeLowercase)
                {
                    characterSet += lowercaseCharacters;
                }

                if (includeUppercase)
                {
                    characterSet += uppercaseCharacters;
                }

                if (includeNumeric)
                {
                    characterSet += numericCharacters;
                }

                if (includeSpecial)
                {
                    characterSet += specialCharacters;
                }

                if (includeSpaces)
                {
                    characterSet += spaceCharacter;
                }

                char[] password = new char[lengthOfPassword];
                int characterSetLength = characterSet.Length;

                System.Random random = new System.Random();
                for (int characterPosition = 0; characterPosition < lengthOfPassword; characterPosition++)
                {
                    password[characterPosition] = characterSet[random.Next(characterSetLength - 1)];

                    bool moreThanTwoIdenticalInARow =
                        characterPosition > maximumIdenticalConsecutiveChars
                        && password[characterPosition] == password[characterPosition - 1]
                        && password[characterPosition - 1] == password[characterPosition - 2];

                    if (moreThanTwoIdenticalInARow)
                    {
                        characterPosition--;
                    }
                }

                return string.Join(null, password);
            }

            public static bool PasswordIsValid(bool includeLowercase, bool includeUppercase, bool includeNumeric, bool includeSpecial, bool includeSpaces, string password)
            {
                const string regexLowercase = @"[a-z]";
                const string regexUppercase = @"[A-Z]";
                const string regexNumeric = @"[\d]";
                const string regexSpecial = @"([!#$%&*@\\])+";
                const string regexSpace = @"([ ])+";

                bool lowerCaseIsValid = !includeLowercase || (includeLowercase && Regex.IsMatch(password, regexLowercase));
                bool upperCaseIsValid = !includeUppercase || (includeUppercase && Regex.IsMatch(password, regexUppercase));
                bool numericIsValid = !includeNumeric || (includeNumeric && Regex.IsMatch(password, regexNumeric));
                bool symbolsAreValid = !includeSpecial || (includeSpecial && Regex.IsMatch(password, regexSpecial));
                bool spacesAreValid = !includeSpaces || (includeSpaces && Regex.IsMatch(password, regexSpace));

                return lowerCaseIsValid && upperCaseIsValid && numericIsValid && symbolsAreValid && spacesAreValid;
            }
        }
    }
}