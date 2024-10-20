using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace NetUse.NetConfigFile
{
    /// <summary>
    /// Object is used to store the raw data into the NetConfiguration file.
    /// </summary>
    [Serializable]
    public class FileStorageItem
    {
        public const UInt32 CLASSVERSION = 0x00010001;  // Ver 1.1



        /// <summary>
        /// Version of this class.
        /// </summary>
        public UInt32 ClassVersion { get; set; }


        /// <summary>
        /// Only disconnect a mapped share from devicename.
        /// </summary>
        public bool OnlyDisconnect { get; set; }


        /// <summary>
        /// Method of data encryption.
        /// </summary>
        public int EncryptionMethod { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public char DeviceName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ShareName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UserPass { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Comments { get; set; }


        /// <summary>
        /// Check and disconnect a mapped share from devicename.
        /// </summary>
        public bool DisconnectFirst { get; set; }



        public FileStorageItem ()
        {
            this.ClassVersion = 0;

            this.OnlyDisconnect = false;

            this.EncryptionMethod = 0;
            this.DeviceName = 'Z';
            this.ShareName = String.Empty;
            this.UserName = String.Empty;
            this.UserPass = String.Empty;

            this.Comments = String.Empty;

            this.DisconnectFirst = true;
        }



        public FileStorageItem (bool onlyDisconnect,
            int encryptIonMethod,
            char deviceName,
            string shareName,
            string userName,
            string userPass,
            string comments,
            bool disconnectFirst) : base ()
        {
            this.ClassVersion = CLASSVERSION;

            this.OnlyDisconnect = onlyDisconnect;

            this.EncryptionMethod = encryptIonMethod;
            this.DeviceName = deviceName;
            this.ShareName = shareName;
            this.UserName = userName;
            this.UserPass = userPass;

            this.Comments = comments;

            this.DisconnectFirst = disconnectFirst;
        }
    }
}
