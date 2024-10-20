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
            //this.textBoxDescription.Text = AssemblyDescription;

            this.textBoxDescription.Lines = new List<string> () {
                "This program, in conjunction with NetUse.exe, offers a simple graphical user interface to easily use the Microsoft Windows internal console program.\"NET\".",
                "The solution is designed so that NetUse.exe is integrated and called by other tools (for example: Powershell, Batch files, backup tools, ...).",
                " ",
                "Third Party Licenses:",
                "- Menu and program ,icons:",
                "\tIcons by Axialis",
                "\t Axialis Free Icon License (\"https://www.axialis.com\")",
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
