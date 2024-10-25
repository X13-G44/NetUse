///<copyright>
/// Copyright (c) 2024 Christian Harscher (alias X13-G44)
///
/// This program is free software: you can redistribute it and/or modify
/// it under the terms of the GNU Affero General Public License as
/// published by the Free Software Foundation, either version 3 of the
/// License, or (at your option) any later version.
///
/// This program is distributed in the hope that it will be useful,
/// but WITHOUT ANY WARRANTY; without even the implied warranty of
/// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
/// GNU Affero General Public License for more details.
///
/// You should have received a copy of the GNU Affero General Public License
/// along with this program.  If not, see <https://www.gnu.org/licenses/>.
///
/// Contact: info@x13-g44.com
/// </copyright>
///
/// <author>Christian Harscher (alias X13-G44)</author>



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace NetUseGui
{
    partial class frmAboutBox : Form
    {
        public frmAboutBox ()
        {
            InitializeComponent ();

            this.Text = String.Format ("About {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = String.Format ("Version {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            //this.labelCompanyName.Text = AssemblyCompany;
            this.labelCompanyName.Text = "https://github.com/x13-44/netuse";

            //this.textBoxDescription.Text = AssemblyDescription;
            this.textBoxDescription.Lines = new List<string> () {
                "With this program solution you can easily use the Micosoft Windows CLI tool “NET” to create network connections.\nThe focus of this solution is the integration into other (third party) programs such as backup tools, batch files, Powershell, ... .\n",
                "The program solution consists of 2 parts:\n",
                "- NetUseGui.exe is used to create and edit configuration files for the control of “NET”.\n",
                "- NetUse.exe is called or integrated by the third party programs to establish the network connection.\n",
                "\n",
                "License:\n",
                "This program is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version. This program is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details. You should have received a copy of the GNU Affero General Public License along with this program. If not, see <https://www.gnu.org/licenses/>.\n",
                "\n",
                "Third Party Licenses:",
                "- Menu icons, Solution icons:",
                "  Axialis Free Icon License (\"https://www.axialis.com\")",
            }.ToArray ();
        }



        #region Assembly attribute accessors



        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly ().GetCustomAttributes (typeof (AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute) attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension (Assembly.GetExecutingAssembly ().CodeBase);
            }
        }



        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
            }
        }



        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly ().GetCustomAttributes (typeof (AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute) attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly ().GetCustomAttributes (typeof (AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute) attributes[0]).Product;
            }
        }



        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly ().GetCustomAttributes (typeof (AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute) attributes[0]).Copyright;
            }
        }



        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly ().GetCustomAttributes (typeof (AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute) attributes[0]).Company;
            }
        }


        #endregion
    }
}
