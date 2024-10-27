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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using static NetUse.NetConfigFile.Cryption;
using NetUse.Common;



namespace NetUse.NetConfigFile
{
    /// <summary>
    /// Class to provide access to the NetConfiguration files.
    /// </summary>
    public class NetConfigFile
    {
        /// <summary>
        /// Filename from the currently used NetConfiguration file.
        /// Empty, when no file was load.
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        /// Data from the currently used NetConfiguration file.
        /// NULL, when no file was load.
        /// </summary>
        public NetConfigData Data { get; private set; }

        /// <summary>
        /// Encryption method from the currently used NetConfiguration file.
        /// PlainText, when no file was load.
        /// </summary>
        public Cryption.EncryptionMethod CryptIonMethod { get; private set; }



        /// <summary>
        /// Try to load a NetConfiguration file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public CommonResult LoadFile (string filename)
        {
            this.CurrentFile = "";
            this.Data = null;
            this.CryptIonMethod = EncryptionMethod.PlanText;

            try
            {
                if (File.Exists (filename))
                {
                    XmlSerializer serializer = new XmlSerializer (typeof (FileStorageItem));
                    FileStorageItem fileStorageItem;
                    NetConfigData storageItem;
                    EncryptionMethod cryptionMethod;


                    try
                    {
                        using (FileStream fs = new FileStream (filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                fileStorageItem = (FileStorageItem) serializer.Deserialize (fs);
                            }
                            catch (Exception ex)
                            {
                                return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_Deserialize, $"Could not de-serialize Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                            }

                            storageItem = NetConfigData.FromFileStorageItem (fileStorageItem, out cryptionMethod);

                            if (storageItem != null)
                            {
                                this.Data = storageItem;
                                this.CurrentFile = filename;
                                this.CryptIonMethod = cryptionMethod;

                                return CommonResult.MakeSuccess ();
                            }

                            return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_FileReadParsing, $"Could not parse Net Use Configuration file content. Maybe incompatible version. Check for new version from of this application");
                        }
                    }
                    catch (Exception ex)
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_FileRead, $"Could not open and read Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                    }
                }
                else
                {
                    return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_FileNotExists, $"Net Use Configuration file \"{filename}\" not exists");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption (ex.Message);
            }
        }



        /// <summary>
        /// Create and write a NetConfiguration file.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filename"></param>
        /// <param name="cryptionMethod"></param>
        /// <param name="allowFileOverride"></param>
        /// <param name="loadFile"></param>
        /// <returns></returns>
        public CommonResult WriteFile (NetConfigData data, string filename, Cryption.EncryptionMethod cryptionMethod, bool allowFileOverride, bool loadFile)
        {
            try
            {
                if (allowFileOverride || File.Exists (filename) == false)
                {
                    XmlSerializer serializer = new XmlSerializer (typeof (FileStorageItem));
                    FileStorageItem fileStorageItem;


                    fileStorageItem = data.ToFileStorageItem (cryptionMethod);

                    if (fileStorageItem != null)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream (filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                try
                                {
                                    serializer.Serialize (fs, fileStorageItem);
                                }
                                catch (Exception ex)
                                {
                                    return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_Serialize, $"Data preparing failed. Reason: {ex.Message}");
                                }

                                if (loadFile)
                                {
                                    this.Data = data;
                                    this.CurrentFile = filename;
                                    this.CryptIonMethod = cryptionMethod;
                                }

                                return CommonResult.MakeSuccess ();
                            }

                        }
                        catch (Exception ex)
                        {
                            return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_FileOpenReadWrite, $"Could not create and write Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                        }
                    }
                    else
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_ConvertToFileStorageItem, $"Data preparing failed");
                    }
                }
                else
                {
                    return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_FileExists, $"Net Use Configuration file \"{filename}\" already exists");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption (ex.Message);
            }
        }



        public NetConfigFile ()
        {
            this.CurrentFile = "";
            this.Data = null;
            this.CryptIonMethod = EncryptionMethod.PlanText;
        }
    }
}
