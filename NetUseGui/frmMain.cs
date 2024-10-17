using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetUse.NetConfigFile;
using NetUse.Common;
using NetUse.Core;
using System.Security;
using System.Runtime.CompilerServices;
using NetUseGui.Properties;
using System.Runtime;
using System.Reflection;
using Microsoft.Win32;
using System.Globalization;
using System.Text.RegularExpressions;




namespace NetUseGui
{
    public partial class frmMain : Form
    {
        public const string MainWindowTitelBase = "Net Use";
        private string MessageBoxTitle = String.Empty;

        private bool UpdatingGuiActive = false;

        private bool CurrentNetConfigurationFileChanged = false;
        private NetConfigFile CurrentNetConfigurationFile = null;

        private char[] InvalidShareNameChars = new char[] { ' ', '/', ':', '*', '?','"','<','>','|' };
        private char[] InvalidUserNameChars = new char[] { ' ', '"', '/', '\\', '[', ']', ':', ';', '|', '=', ',', '+', '*', '?', '<', '>' };
        private char[] InvalidUserPasswordChars = new char[] { ' ' };



        public frmMain()
        {
            InitializeComponent();


            this.Text = MainWindowTitelBase;
            this.MessageBoxTitle = MainWindowTitelBase;

            this.cbDeviceLetter.Items.Clear();
            this.cbDeviceLetter.Items.AddRange(MakeDeviceNameList());

            ShowMainPanel(false);
        }



        #region GUI Event Handler



        private void btnStartPanel_MakeNew_Click(object sender, EventArgs e)
        {
            this.UpdatingGuiActive = true;

            this.CurrentNetConfigurationFile = null;
            this.CurrentNetConfigurationFileChanged = false;
            this.Text = MainWindowTitelBase;

            InitMainPanelAsConnectToShare(false, true);
            ShowMainPanel(true);

            this.UpdatingGuiActive = false;
        }



        private void btnStartPanel_Open_Click(object sender, EventArgs e)
        {
            CommonResult loadResult = OpenNetConfigurationFile(true);


            if (loadResult.Success)
            {
                this.UpdatingGuiActive = true;

                this.CurrentNetConfigurationFile = loadResult.Data as NetConfigFile;
                this.CurrentNetConfigurationFileChanged = false;
                UpdateMainWindowTitel();

                UpdateMainPanelElements(this.CurrentNetConfigurationFile.Data);
                ShowMainPanel(true);

                this.UpdatingGuiActive = false;
            }
        }



        private void rbConnectShare_CheckedChanged(object sender, EventArgs e)
        {
            if (this.UpdatingGuiActive) { return; }

            this.UpdatingGuiActive = true;


            if (sender == rbConnectShare)
            {
                this.CurrentNetConfigurationFileChanged = true;
                UpdateMainWindowTitel();

                InitMainPanelAsConnectToShare(true, false);
            }
            else if (sender == rbDisconnectShare)
            {
                this.CurrentNetConfigurationFileChanged = true;
                UpdateMainWindowTitel();

                InitMainPanelAsDisconnectFromShare(true, false);
            }

            this.UpdatingGuiActive = false;
        }



        private void onGuiElementChange(object sender, EventArgs e)
        {
            if (this.UpdatingGuiActive) { return; }

            this.UpdatingGuiActive = true;

            this.CurrentNetConfigurationFileChanged = true;
            UpdateMainWindowTitel();

            this.UpdatingGuiActive = false;
        }



        private void tbShareName_Leave(object sender, EventArgs e)
        {
            string usedInvalidChars = string.Empty;


            if (CheckForInvalidChar(tbShareName.Text, false, true, true, InvalidShareNameChars, out usedInvalidChars))
            {
                MessageBox.Show($"Warning: The entered network share name contains invalid characters. Please try again with valid characters. \n\nNot allowed characters: {usedInvalidChars}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                tbShareName.Text = String.Empty;
            }
        }



        private void tbUserName_Leave(object sender, EventArgs e)
        {
            string usedInvalidChars = string.Empty;


            if (CheckForInvalidChar(tbUserName.Text, false, true, true, InvalidUserNameChars, out usedInvalidChars))
            {
                MessageBox.Show($"Warning: The entered user name contains invalid characters. Please try again with valid characters. \n\nNot allowed characters: {usedInvalidChars}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                tbUserName.Text = String.Empty;
            }
        }



        private void tbUserPw_Leave(object sender, EventArgs e)
        {
            string usedInvalidChars = string.Empty;


            if (CheckForInvalidChar(tbUserPw.Text, false, true, true, InvalidUserPasswordChars, out usedInvalidChars))
            {
                MessageBox.Show($"Warning: The entered password contains invalid characters. Please try again with valid characters. \n\nNot allowed characters: {usedInvalidChars}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);

                tbUserPw.Text = String.Empty;
            }
        }



        private void btnMenStrip_MakeNew_Click(object sender, EventArgs e)
        {
            StoreUnsavedData(() =>
                {
                    btnStartPanel_MakeNew_Click(sender, e);
                });
        }



        private void btnMenStrip_Open_Click(object sender, EventArgs e)
        {
            StoreUnsavedData(() =>
                {
                    btnStartPanel_Open_Click(sender, e);
                });
        }



        private void btnMenStrip_Save_Click(object sender, EventArgs e)
        {
            CommonResult writeResult = null;


            if (this.CurrentNetConfigurationFile != null)
            {
                // Write to NetCinfiguration file.
                writeResult = SaveNetConfigurationFile(this.CurrentNetConfigurationFile, BuildNetConfigData(), true);
            }
            else
            {
                // Create new NetCinfiguration file.
                writeResult = SaveNetConfigurationFile(BuildNetConfigData(), true);
            }

            if (writeResult.Success)
            {
                this.UpdatingGuiActive = true;

                this.CurrentNetConfigurationFile = writeResult.Data as NetConfigFile;
                this.CurrentNetConfigurationFileChanged = false;
                UpdateMainWindowTitel();

                this.UpdatingGuiActive = false;
            }
        }



        private void btnMenStrip_SaveAs_Click(object sender, EventArgs e)
        {
            CommonResult writeResult = SaveNetConfigurationFile(BuildNetConfigData(), true);

            if (writeResult.Success)
            {
                this.UpdatingGuiActive = true;

                this.CurrentNetConfigurationFile = writeResult.Data as NetConfigFile;
                this.CurrentNetConfigurationFileChanged = false;
                UpdateMainWindowTitel();

                this.UpdatingGuiActive = false;
            }
        }



        private void btnMenStrip_Exit_Click(object sender, EventArgs e)
        {
            StoreUnsavedData(() =>
            {
                this.UpdatingGuiActive = true;

                Close();

                this.UpdatingGuiActive = false;
            });
        }



        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.UpdatingGuiActive) { return; }

            this.UpdatingGuiActive = true;

            StoreUnsavedData(() =>
            {
                ;
            }, () =>
            {
                e.Cancel = true;
            });

            this.UpdatingGuiActive = false;
        }



        private void frmMain_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


                if (files.Length == 1)
                {
                    NetConfigFile config = new NetConfigFile();


                    if (config.LoadFile(files[0]).Success)
                    {
                        e.Effect = DragDropEffects.Copy;

                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }



        private void frmMain_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);


            if (files.Length == 1)
            {
                StoreUnsavedData(() =>
                {
                    CommonResult loadResult = OpenNetConfigurationFile(files[0], true);


                    if (loadResult.Success)
                    {
                        this.UpdatingGuiActive = true;

                        this.CurrentNetConfigurationFile = loadResult.Data as NetConfigFile;
                        this.CurrentNetConfigurationFileChanged = false;
                        UpdateMainWindowTitel();

                        UpdateMainPanelElements(this.CurrentNetConfigurationFile.Data);
                        ShowMainPanel(true);

                        this.UpdatingGuiActive = false;
                    }
                });
            }
        }



        private void frmMain_Shown(object sender, EventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();


            if (args.Length == 2)
            {
                string netConfigurationFile = args[1].Replace('"', ' ').Trim();


                if (File.Exists(netConfigurationFile))
                {
                    CommonResult loadResult = OpenNetConfigurationFile(netConfigurationFile, true);


                    if (loadResult.Success)
                    {
                        this.UpdatingGuiActive = true;

                        this.CurrentNetConfigurationFile = loadResult.Data as NetConfigFile;
                        this.CurrentNetConfigurationFileChanged = false;
                        UpdateMainWindowTitel();

                        UpdateMainPanelElements(this.CurrentNetConfigurationFile.Data);
                        ShowMainPanel(true);

                        this.UpdatingGuiActive = false;
                    }
                }
            }
        }



        private void btnMenStrip_RunCmd_Click(object sender, EventArgs e)
        {
            CommonResult commonResult;
            NetConfigData netConfigData = BuildNetConfigData();


            // Make some basic checks, before executing the net use command.
            if (rbConnectShare.Checked)
            {
                if (tbShareName.Text.Length < 5)
                {
                    // Minimum 5 chars are expected for share name: "\\x\y"
                    MessageBox.Show("The entered network share name appears to be invalid. Please enter a valid name.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (tbUserName.Text.Length == 0)
                {
                    MessageBox.Show("The entered user name appears to be invalid. Please enter a valid name.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }

                if (tbUserPw.Text.Length == 0)
                {
                    MessageBox.Show("The entered user name appears to be invalid. Please enter a valid name.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }
            else
            {
                if (tbShareName.Text.Length < 5)
                {
                    // Minimum 5 chars are expected for share name: "\\x\y"
                    MessageBox.Show ("The entered network share name appears to be invalid. Please enter a valid name.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
            }

            commonResult = RunCommand(netConfigData);

            if (commonResult.Success)
            {
                MessageBox.Show("The Net Use command was executed without problems.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"The Net Use command was executed with errors!\n\nErrormessage:\n{commonResult.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }



        private void btnMenStrip_RegFileExt_Click(object sender, EventArgs e)
        {
            CommonResult commonResult;


            commonResult = RegisterNetConfigurationFileExtension();

            if (commonResult.Success)
            {
                MessageBox.Show("File extension has been registered.", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                if (commonResult.ErrorCode == CommonResult.ErrorResultCodes.E_UnauthorizedAccessException)
                {
                    MessageBox.Show(commonResult.Message, this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show(commonResult.Message, this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        private void btnMenStrip_About_Click(object sender, EventArgs e)
        {
            frmAboutBox aboutBox = new frmAboutBox();
            aboutBox.ShowDialog();
        }



        #endregion



        #region GUI Element Management



        private void UpdateMainWindowTitel()
        {
            if (this.CurrentNetConfigurationFile == null)
            {
                this.Text = $"{MainWindowTitelBase} {(this.CurrentNetConfigurationFileChanged ? "*" : "")}";
            }
            else
            {
                string configFilename = Path.GetFileNameWithoutExtension(this.CurrentNetConfigurationFile.CurrentFile);


                this.Text = $"{MainWindowTitelBase} [{configFilename}] {(this.CurrentNetConfigurationFileChanged ? "*" : "")}";
            }
        }



        private void ShowMainPanel(bool showMainPanel)
        {
            panelStart.Visible = !showMainPanel;
            panelEdit.Visible = showMainPanel;

            btnMenStrip_Save.Enabled = showMainPanel;
            btnMenStrip_SaveAs.Enabled = showMainPanel;
            btnMenStrip_RunCmd.Enabled = showMainPanel;
        }



        private void UpdateMainPanelElements(NetConfigData netConfigData)
        {
            if (netConfigData.OnlyDisconnect)
            {
                rbDisconnectShare.Checked = true;

                cbDeviceLetter.Text = netConfigData.DeviceName.ToString();
                tbShareName.Text = String.Empty;
                cbDisconnectFirst.Checked = false;

                tbUserName.Text = String.Empty;
                tbUserPw.Text = String.Empty;


                tbShareName.Enabled = false;
                cbDisconnectFirst.Enabled = false;

                tbUserName.Enabled = false;
                tbUserPw.Enabled = false;
            }
            else
            {
                rbConnectShare.Checked = true;

                cbDeviceLetter.Text = netConfigData.DeviceName.ToString();
                tbShareName.Text = netConfigData.ShareName;
                cbDisconnectFirst.Checked = netConfigData.DisconnectFirst;

                tbUserName.Text = netConfigData.UserName;
                tbUserPw.Text = netConfigData.UserPass;


                tbShareName.Enabled = true;
                cbDisconnectFirst.Enabled = true;

                tbUserName.Enabled = true;
                tbUserPw.Enabled = true;
            }

            tbComment.Text = netConfigData.Comments;
        }



        private void InitMainPanelAsConnectToShare(bool keepCurrentValues, bool loadExampleVal)
        {
            rbConnectShare.Checked = true;

            if (keepCurrentValues == false)
            {
                cbDeviceLetter.Text = "Z";
                tbShareName.Text = loadExampleVal ? @"\\server\share" : "";
                cbDisconnectFirst.Checked = true;

                tbUserName.Text = loadExampleVal ? "Username" : "";
                tbUserPw.Text = loadExampleVal ? "Password" : "";

                tbComment.Text = loadExampleVal ? "New NetUse Configuration file" : "";
            }

            tbShareName.Enabled = true;
            cbDisconnectFirst.Enabled = true;

            tbUserName.Enabled = true;
            tbUserPw.Enabled = true;
        }



        private void InitMainPanelAsDisconnectFromShare(bool keepCurrentValues, bool loadExampleVal)
        {
            rbDisconnectShare.Checked = true;

            if (keepCurrentValues == false)
            {
                cbDeviceLetter.Text = "Z";
                tbShareName.Text = String.Empty;
                cbDisconnectFirst.Checked = false;

                tbUserName.Text = String.Empty;
                tbUserPw.Text = String.Empty;

                tbComment.Text = loadExampleVal ? "New NetUse Configuration file" : "";
            }

            tbShareName.Enabled = false;
            cbDisconnectFirst.Enabled = false;

            tbUserName.Enabled = false;
            tbUserPw.Enabled = false;
        }



        private NetConfigData BuildNetConfigData()
        {
            bool conn = rbConnectShare.Checked;
            NetConfigData result = new NetConfigData();

            result.OnlyDisconnect = !rbConnectShare.Checked;

            result.DeviceName = cbDeviceLetter.Text[0];
            result.ShareName = conn ? tbShareName.Text : String.Empty;
            result.DisconnectFirst = conn ? cbDisconnectFirst.Checked : false;

            result.UserName = conn ? tbUserName.Text : String.Empty;
            result.UserPass = conn ? tbUserPw.Text : String.Empty;

            result.Comments = tbComment.Text;

            return result;
        }



        private void StoreUnsavedData(Action afterSaveAction, Action userAbortAction = null)
        {
            if (this.CurrentNetConfigurationFileChanged == true)
            {
                DialogResult result = MessageBox.Show("The settings have been changed but not saved yet!\n\nShould the settings be saved first ?", this.MessageBoxTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);

                switch (result)
                {
                    case DialogResult.Yes:
                        {
                            if (this.CurrentNetConfigurationFile != null)
                            {
                                // Save to current file                                    
                                CommonResult writeResult = SaveNetConfigurationFile(this.CurrentNetConfigurationFile, BuildNetConfigData(), true);

                                if (!writeResult.Success)
                                {
                                    return;
                                }
                            }
                            else
                            {
                                // Save to new file
                                CommonResult writeResult = SaveNetConfigurationFile(BuildNetConfigData(), true);

                                if (!writeResult.Success)
                                {
                                    return;
                                }
                            }

                            this.UpdatingGuiActive = true;

                            afterSaveAction();

                            this.UpdatingGuiActive = false;

                            break;
                        }
                    case DialogResult.No:
                        {
                            this.UpdatingGuiActive = true;

                            afterSaveAction();

                            this.UpdatingGuiActive = false;

                            break;
                        }
                    default:
                        {
                            if (userAbortAction != null)
                            {
                                userAbortAction();
                            }
                            else
                            {
                                return;
                            }

                            break;
                        }
                }
            }
            else
            {
                this.UpdatingGuiActive = true;

                afterSaveAction();

                this.UpdatingGuiActive = false;
            }
        }



        private bool CheckForInvalidChar(string textToCheck, bool allowAsciiLettersAndDigitsOnly, bool checkForInvalidAscii, bool checkForInvalidUnicode, char[] optionalInvalidCharList, out string usedInvalidChars)
        {
            bool hasInvalidChar = false;

            string filteredString = String.Empty;
            //IEnumerable<char> filteredCharList;   // Not usable, because no ".Join" linq extension is available.
            IEnumerable<char> invalidChars;


            usedInvalidChars = String.Empty;

            if (allowAsciiLettersAndDigitsOnly)
            {
                filteredString = new string(textToCheck.Where(c => char.IsLetter(c) || char.IsDigit(c)).ToArray());
                //filteredCharList = textToCheck.Where(c => char.IsLetter(c) || char.IsDigit(c));

                hasInvalidChar |= filteredString.Length != textToCheck.Length ? true : false;
                //hasInvalidChar |= filteredCharList.Count() > 0 ? true : false; 
            }

            if (checkForInvalidAscii)
            {
                filteredString = new string(textToCheck.Where(c => !char.IsControl(c)).ToArray());
                //filteredCharList = textToCheck.Where(c => !char.IsControl(c));

                hasInvalidChar |= filteredString.Length != textToCheck.Length ? true : false;
                //hasInvalidChar |= filteredCharList.Count() > 0 ? true : false;
            }

            if (checkForInvalidUnicode)
            {
                filteredString = new string(textToCheck.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.Format).ToArray());
                //filteredCharList = textToCheck.Where(c => char.GetUnicodeCategory(c) != UnicodeCategory.Format);

                hasInvalidChar |= filteredString.Length != textToCheck.Length ? true : false;
                //hasInvalidChar |= filteredCharList.Count() > 0 ? true : false;
            }

            if (optionalInvalidCharList != null)
            {
                invalidChars = filteredString.Join(optionalInvalidCharList,
                                          inputChar => inputChar,
                                          invalidChar => invalidChar,
                                          (str1, invalidChar) => str1);

                if (invalidChars.Count() > 0)
                {
                    hasInvalidChar |= true;

                    foreach (var invalidChar in invalidChars)
                    {
                        usedInvalidChars += $", \"{invalidChar.ToString()}\"";
                    }
                    usedInvalidChars = usedInvalidChars.Remove(0, 1);
                }
            }

            return hasInvalidChar;

            #region RegEx examples:

            // Example source: https://learn.microsoft.com/en-us/dotnet/standard/base-types/how-to-strip-invalid-characters-from-a-string
            //
            //static string CleanInput(string strIn)
            //{
            //    // Replace invalid characters with empty strings.
            //    try
            //    {
            //        return Regex.Replace(strIn, @"[^\w\.@-]", "",
            //                             RegexOptions.None, TimeSpan.FromSeconds(1.5));
            //    }
            //    // If we timeout when replacing invalid characters,
            //    // we should return Empty.
            //    catch (RegexMatchTimeoutException)
            //    {
            //        return String.Empty;
            //    }
            //}

            // Example source: https://www.csharphelper.com/howtos/howto_remove_non_ascii.html
            //
            //public static string TrimNonAscii(this string value)
            //{
            //    string pattern = "[^ -~]+";
            //    Regex reg_exp = new Regex(pattern);
            //    return reg_exp.Replace(value, "");
            //}

            #endregion
        }



        private string[] MakeDeviceNameList()
        {
            return new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        }



        #endregion



        #region Business logic



        /// <summary>
        /// Show open file dialog window and read the selected NetUse Configuration file.
        /// </summary>
        /// <param name="showMessageDialogOnError">Show an error dialog window on error</param>
        /// <returns>CommonResult object which hosts a NetConfigFile object on success</returns>
        private CommonResult OpenNetConfigurationFile(bool showMessageDialogOnError)
        {
            try
            {
                NetConfigFile configFile = new NetConfigFile();

                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                String currentPath = Path.GetDirectoryName(currentAssembly.Location);

                CommonResult loadResult;


                openFileDialog1.Title = "Open NetUse Configuration file";
                openFileDialog1.Filter = "NetUse Configuration file (*.netcfg)|*.netcfg|All files (*.*)|*.*";
                openFileDialog1.FilterIndex = 0;
                openFileDialog1.DefaultExt = "*.netcfg";
                openFileDialog1.InitialDirectory = currentPath;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.FileName = String.Empty;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    loadResult = configFile.LoadFile(openFileDialog1.FileName);

                    if (loadResult.Success && configFile.CurrentFile != String.Empty)
                    {
                        return CommonResult.MakeSuccess(configFile);
                    }
                    else
                    {
                        if (showMessageDialogOnError)
                        {
                            MessageBox.Show($"An error occurred while loading a NetUse Configuration file!\n\nReason:\n{loadResult.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return loadResult;
                    }
                }
                else
                {
                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_Abort, "Abort by user");
                }
            }
            catch (Exception ex)
            {
                if (showMessageDialogOnError)
                {
                    MessageBox.Show($"An exeption occurred while loading a NetUse Configuration file!\n\nReason:\n{ex.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return CommonResult.MakeExeption(ex.Message);
            }
        }



        /// <summary>
        /// Read a given NetUse Configuration file.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="showMessageDialogOnError">Show an error dialog window on error</param>
        /// <returns>CommonResult object which hosts a NetConfigFile object on success</returns>
        private CommonResult OpenNetConfigurationFile(string filename, bool showMessageDialogOnError)
        {
            try
            {
                NetConfigFile configFile = new NetConfigFile();

                CommonResult loadResult = configFile.LoadFile(filename);

                if (loadResult.Success && configFile.CurrentFile != String.Empty)
                {
                    return CommonResult.MakeSuccess(configFile);
                }
                else
                {
                    if (showMessageDialogOnError)
                    {
                        MessageBox.Show($"An error occurred while loading a NetUse Configuration file!\n\nReason:\n{loadResult.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return loadResult;
                }
            }
            catch (Exception ex)
            {
                if (showMessageDialogOnError)
                {
                    MessageBox.Show($"An exeption occurred while loading a NetUse Configuration file!\n\nReason:\n{ex.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return CommonResult.MakeExeption(ex.Message);
            }
        }



        /// <summary>
        /// Show save file dialog window and write into the selected NetUse Configuration file.
        /// </summary>
        /// <param name="newNetConfigurationData"></param>
        /// <param name="showMessageDialogOnError"></param>
        /// <returns>CommonResult object which hosts a NetConfigFile object on success</returns>
        private CommonResult SaveNetConfigurationFile(NetConfigData newNetConfigurationData, bool showMessageDialogOnError)
        {
            try
            {
                NetConfigFile newNetConfigurationFile = new NetConfigFile();

                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                String currentPath = Path.GetDirectoryName(currentAssembly.Location);

                CommonResult writeResult;


                saveFileDialog1.Title = "Save NetUse Configuration file";
                saveFileDialog1.Filter = "NetUse Configuration file (*.netcfg)|*.netcfg|All files (*.*)|*.*";
                saveFileDialog1.FilterIndex = 0;
                saveFileDialog1.DefaultExt = "*.netcfg";
                saveFileDialog1.InitialDirectory = currentPath;
                saveFileDialog1.RestoreDirectory = true;
                saveFileDialog1.FileName = String.Empty;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    writeResult = newNetConfigurationFile.WriteFile(newNetConfigurationData, saveFileDialog1.FileName, Cryption.CryptionMethod.PlanText, true, true);

                    if (writeResult.Success && newNetConfigurationFile.CurrentFile != String.Empty)
                    {
                        return CommonResult.MakeSuccess(newNetConfigurationFile);
                    }
                    else
                    {
                        if (showMessageDialogOnError)
                        {
                            MessageBox.Show($"An error occurred while writing a new NetUse Configuration file!\n\nReason:\n{writeResult.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        return writeResult;
                    }
                }
                else
                {
                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_Abort, "Abort by user");
                }
            }
            catch (Exception ex)
            {
                if (showMessageDialogOnError)
                {
                    MessageBox.Show($"An exeption occurred while writing data to new NetUse Configuration file!\n\nReason:\n{ex.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return CommonResult.MakeExeption(ex.Message);
            }
        }



        /// <summary>
        /// Write / update a current exist NetConfiguration file.
        /// </summary>
        /// <param name="curNetConfigurationFile"></param>
        /// <param name="newNetConfigurationData"></param>
        /// <param name="showMessageDialogOnError"></param>
        /// <returns>CommonResult object which hosts a NetConfigFile object on success</returns>
        private CommonResult SaveNetConfigurationFile(NetConfigFile curNetConfigurationFile, NetConfigData newNetConfigurationData, bool showMessageDialogOnError)
        {
            try
            {
                CommonResult writeResult = curNetConfigurationFile.WriteFile(newNetConfigurationData, curNetConfigurationFile.CurrentFile, Cryption.CryptionMethod.PlanText, true, true);


                if (writeResult.Success && curNetConfigurationFile.CurrentFile != String.Empty)
                {
                    return CommonResult.MakeSuccess(curNetConfigurationFile);
                }
                else
                {
                    if (showMessageDialogOnError)
                    {
                        MessageBox.Show($"An error occurred while writing a NetUse Configuration file!\n\nReason:\n{writeResult.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    return writeResult;
                }
            }
            catch (Exception ex)
            {
                if (showMessageDialogOnError)
                {
                    MessageBox.Show($"An exeption occurred while writing data to NetUse Configuration file!\n\nReason:\n{ex.Message}", this.MessageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                return CommonResult.MakeExeption(ex.Message);
            }
        }



        /// <summary>
        /// Register our NetConfiguration file in the registry.
        /// </summary>
        /// <returns></returns>
        private CommonResult RegisterNetConfigurationFileExtension()
        {
            try
            {
                RegistryKey regRoot = Registry.ClassesRoot;

                Assembly currentAssembly = Assembly.GetExecutingAssembly();
                String currentPath = Path.GetDirectoryName(currentAssembly.Location);


                using (RegistryKey key1 = regRoot.CreateSubKey("NetCfgFile"))
                {
                    key1.SetValue("", "Net Use Configuration File", RegistryValueKind.String);

                    using (RegistryKey key11 = key1.CreateSubKey("DefaultIcon"))
                    {
                        key11.SetValue("", $"\"{currentAssembly.Location}\",-1", RegistryValueKind.ExpandString);
                    }

                    using (RegistryKey key12 = key1.CreateSubKey("shell"))
                    {
                        using (RegistryKey
                            key121 = key12.CreateSubKey("edit"),
                            key1211 = key121.CreateSubKey("command"))
                        {
                            key1211.SetValue("", $"\"{currentAssembly.Location}\" \"%1\"", RegistryValueKind.ExpandString);
                        }

                        using (RegistryKey
                            key122 = key12.CreateSubKey("open"),
                            key1221 = key122.CreateSubKey("command"))
                        {
                            key1221.SetValue("", $"\"{currentPath}\\netuse.exe\" \"%1\"", RegistryValueKind.ExpandString);
                        }
                    }
                }

                using (RegistryKey key2 = regRoot.CreateSubKey(".netcfg"))
                {
                    key2.SetValue("", "NetCfgFile", RegistryValueKind.String);
                }


                return CommonResult.MakeSuccess();
            }
            catch (UnauthorizedAccessException)
            {
                return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_UnauthorizedAccessException, $"Write access to the Win-Registry was denied. In order for the file extension to be registered, write access is required.\n\nPlease start the program again with administrator rights.");
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption($"An error occurred while registering the file extension.\n\nOriginal reason: {ex.Message}");
            }
        }



        /// <summary>
        /// Execute the Net Use command.
        /// </summary>
        /// <param name="netConfigurationData"></param>
        /// <returns></returns>
        private CommonResult RunCommand(NetConfigData netConfigurationData)
        {
            try
            {
                CommonResult funcResult = null;


                if (netConfigurationData.OnlyDisconnect)
                {
                    return CoreFunc.ExecuteDisconnectNetCommand(netConfigurationData.DeviceName);
                }
                else
                {
                    if (netConfigurationData.DisconnectFirst)
                    {
                        funcResult = CoreFunc.DisconnectNetShare(netConfigurationData.DeviceName);
                    }

                    if (funcResult == null || funcResult.Success)
                    {
                        return CoreFunc.ExecuteConnectNetCommand(netConfigurationData.DeviceName, netConfigurationData.ShareName, netConfigurationData.UserName, netConfigurationData.UserPass);
                    }
                    else
                    {
                        return funcResult;
                    }
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption(ex.Message);
            }
        }



        #endregion

    }
}
