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



namespace NetUse.Common
{
    /// <summary>
    /// Common class to handle functions results.
    /// </summary>
    public class CommonResult
    {
        public enum ErrorResultCodes
        {
            /// <summary>
            /// GeneGeneralrel success
            /// </summary>
            Success = 0,

            /// <summary>
            /// General (user) abort error
            /// </summary>
            E_Abort = (int.MinValue + 2),

            /// <summary>
            /// General exception error
            /// </summary>
            E_Exeption = (int.MinValue + 1),

            /// <summary>
            /// General unknown error
            /// </summary>
            E_Unknown = int.MinValue,



            #region Program::CheckArgAndLoadNetCfgFile

            E_CntOfStartParam = -1,
            E_CfgFileNotFound = -2,

            #endregion

            #region NetConfigFile::LoadFile

            E_FileNotExists = -10,
            E_FileRead = -11,
            E_FileReadParsing = -12,
            E_Deserialize = -13,

            #endregion

            #region NetConfigFile::WriteFile

            E_FileExists = -20,
            E_ConvertToFileStorageItem = -21,
            E_FileOpenReadWrite = -22,
            E_Serialize = -23,

            #endregion

            #region CoreFunc::ExecuteCommand_WinApi, CoreFunc::ExecuteCommand_Cli

            E_NetUse_Cli = -100,
            E_NetUse_ShellApi = -101,

            #endregion

            #region CoreFunc::ConnectNetShare 

            E_NotAllowedToDisconnect = -110,

            #endregion

            #region CoreFunc::DisconnectNetShare 

            E_ConDeviceIsNoShare = -120,
            E_ConDeviceStillPresent = -121,

            #endregion

            #region frmMain::RegisterNetConfigurationFileExtension

            E_UnauthorizedAccessException = -200,

            #endregion

            #region frmMain::CheckNetworkFolderText

            E_NetFolderNameToShort = -210,
            E_NetFolderNameEndString = -211,
            E_NetFolderNameStartString = -212,
            E_NetFolderNameInvalidChar = -213,

            #endregion

            #region frmMain::CheckUsernameText

            E_UserNameToShort = -220,
            E_UserNameToLong = -221,
            E_UserNameInvalidChar = -222,

            #endregion

            #region frmMain::CheckUserpasswordText

            E_UserPwToShort = -230,
            E_UserPwInvalidChar = -231,

            #endregion
        }



        /// <summary>
        /// Over all result status.
        /// </summary>
        public bool Success { get; private set; }

        /// <summary>
        /// An optional error code. Zero is for success.
        /// </summary>
        public ErrorResultCodes ErrorCode { get; private set; }

        /// <summary>
        /// When set, a (fatal) exception has been occurred.
        /// </summary>
        public bool ExceptionOccurred { get; private set; }

        /// <summary>
        /// Result message. Usually an error description for the user.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Result data from the returning function.
        /// </summary>
        public Object Data { get; private set; }



        public CommonResult ()
        {
            this.Success = false;
            this.ErrorCode = ErrorResultCodes.E_Unknown;
            this.ExceptionOccurred = false;
            this.Message = string.Empty;
            this.Data = null;
        }



        public CommonResult (bool success, ErrorResultCodes errorCode, bool exeptionOccurred, string message, Object data) : base ()
        {
            this.Success = success;
            this.ErrorCode = errorCode;
            this.ExceptionOccurred = exeptionOccurred;
            this.Message = message;
            this.Data = data;
        }



        /// <summary>
        /// Create a "Success" CommonResult object.
        /// </summary>
        /// <param name="data">Returned data object from function</param>
        /// <param name="message">Returned message from function</param>
        /// <returns></returns>
        public static CommonResult MakeSuccess (Object data = null, string message = "")
        {
            return new CommonResult (true, 0, false, message, data);
        }



        /// <summary>
        /// Create a "Error" CommonResult object.
        /// </summary>
        /// <param name="errorCode">General error code</param>
        /// <param name="message">Error message</param>
        /// <param name="data">Returned data object from function</param>
        /// <returns></returns>
        public static CommonResult MakeError (ErrorResultCodes errorCode, string message, Object data = null)
        {
            return new CommonResult (false, errorCode, false, message, data);
        }



        /// <summary>
        /// Create a "Exception" CommonResult object.
        /// </summary>
        /// <param name="message">Error message</param>
        /// <returns></returns>
        public static CommonResult MakeExeption (string message)
        {
            return new CommonResult (false, ErrorResultCodes.E_Exeption, true, message, null);
        }
    }
}
