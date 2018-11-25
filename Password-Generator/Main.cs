/*
* Author: Greg Uctum
* Note: Started off from a template, but changed paramaters to suit my needs. Also added in additional features.
* Version: 1.4
* Initial Date: 8/15/2018
* Updated: 11/24/2018
*/

using System;
using System.IO;
using System.Windows.Forms;
using System.Security.Principal;

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
                MessageBox.Show("The Name field needs to be filled in!");
                txtName.Select();
                return;
            }

            if (txtLength.Text == "")
            {
                MessageBox.Show("This Character field needs to be filled in!");
                txtLength.Select();
                return;
            }

            bool includeLowercase = true;
            bool includeUppercase = true;
            bool includeNumeric = true;
            bool includeSpecial = true;
            bool includeSpaces = false;
            int lengthOfPassword;
            // Convert user input from the text box into 
            int length = Int32.Parse(txtLength.Text);
            lengthOfPassword = length;

            string password = PasswordGenerator.GeneratePassword(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, lengthOfPassword);

            while (!PasswordGenerator.PasswordIsValid(includeLowercase, includeUppercase, includeNumeric, includeSpecial, includeSpaces, password))
            {
                password = PasswordGenerator.GeneratePassword(true, true, true, true, false, lengthOfPassword);
            }

            lblPassword.Text = password;

            // Gets username of the logged in user. This allows me to deploy this across multiple systems if I so desire.

            WindowsIdentity user = WindowsIdentity.GetCurrent();
            string userName = user.Name.Substring(user.Name.LastIndexOf("\\") + 1);

            string path = @"C:\Users\" + userName + @"\Documents\Keys.txt";

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
}