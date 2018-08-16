/*
 * Author: Greg Uctum
 * Template: https://codeshare.co.uk/blog/how-to-create-a-random-password-generator-in-c/
 * Version: 1.0
 * Date: 8/15/2018
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using Password_Generator.CodeShare.Library.Passwords;

namespace Password_Generator
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, EventArgs e)
        {
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
                const string specialCharacters = @"!#$%&*@\";
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