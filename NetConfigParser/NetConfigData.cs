
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



namespace NetUse.NetConfigFile
{
    /// <summary>
    /// Data of a NetConfiguration file.
    /// </summary>
    public class NetConfigData
    {
        /// <summary>
        /// Only disconnect a mapped share from devicename.
        /// </summary>
        public bool OnlyDisconnect { get; set; }

        public char DeviceName { get; set; }
        public string ShareName { get; set; }
        public string UserName { get; set; }
        public string UserPass { get; set; }

        public string Comments { get; set; }

        /// <summary>
        /// Check and disconnect a mapped share from devicename.
        /// </summary>
        public bool DisconnectFirst { get; set; }



        public NetConfigData ()
        {
            this.OnlyDisconnect = false;
            this.DeviceName = 'Z';
            this.ShareName = "";
            this.UserName = "";
            this.UserPass = "";
            this.Comments = "";
            this.DisconnectFirst = true;
        }



        /// <summary>
        /// Convert a FileStorageItem object into a NetConfigData object.
        /// </summary>
        /// <param name="fileStorageItem">A FileStorageItem to convert from</param>
        /// <param name="cryptionMethod">Used encryption method in the FileStorageItem object</param>
        /// <returns>New NetConfigData object</returns>
        internal static NetConfigData FromFileStorageItem (FileStorageItem fileStorageItem, out Cryption.EncryptionMethod cryptionMethod)
        {
            cryptionMethod = Cryption.EncryptionMethod.PlanText;

            try
            {
                NetConfigData result = new NetConfigData ();


                if (fileStorageItem != null)
                {
                    if (fileStorageItem.ClassVersion >= 0x00010001)
                    {
                        // Add here deeper "ClassVersion" processing, if need.
                        // if (fileStorageItem.ClassVersion == 0xabcdabcd) { ... }

                        result.OnlyDisconnect = fileStorageItem.OnlyDisconnect;
                        result.DeviceName = fileStorageItem.DeviceName;
                        result.ShareName = fileStorageItem.ShareName;
                        result.Comments = fileStorageItem.Comments;
                        result.DisconnectFirst = fileStorageItem.DisconnectFirst;

                        if ((Cryption.EncryptionMethod) fileStorageItem.EncryptionMethod != Cryption.EncryptionMethod.PlanText)
                        {
                            // Add here deeper user credentials decryption, when implemented.

                            throw new NotImplementedException ("Decryption is not implemented jet.");
                        }
                        else
                        {
                            result.UserName = fileStorageItem.UserName;
                            result.UserPass = fileStorageItem.UserPass;
                        }
                        cryptionMethod = (Cryption.EncryptionMethod) fileStorageItem.EncryptionMethod;

                        return result;
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }



        /// <summary>
        /// Convert a NetConfigData object into a FileStorageItem object.
        /// </summary>
        /// <param name="storageItem">NetConfigData to use</param>
        /// <param name="cryptionMethod">Select the user credentials encryption method</param>
        /// <returns>New FileStorageItem object</returns>
        internal FileStorageItem ToFileStorageItem (Cryption.EncryptionMethod cryptionMethod)
        {
            try
            {
                if (cryptionMethod != Cryption.EncryptionMethod.PlanText)
                {
                    // Add here deeper user credentials encryption, when implemented.

                    throw new NotImplementedException ("Encryption is not implemented jet.");
                }

                return new FileStorageItem (
                    this.OnlyDisconnect,
                    (int) cryptionMethod,
                    this.DeviceName,
                    this.ShareName,
                    this.UserName,
                    this.UserPass,
                    this.Comments,
                    this.DisconnectFirst);
            }
            catch
            {
                return null;
            }
        }
    }
}
