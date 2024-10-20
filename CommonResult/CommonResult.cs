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

            E_CntOfStartParam = -5,
            E_CfgFileNotFound = -6,

            #endregion

            #region CoreFunc::ExecuteNetCommand

            E_NetUse = -10,

            #endregion

            #region NetConfigFile::LoadFile

            E_FileNotExists = -15,
            E_FileRead = -16,
            E_FileReadParsing = -17,
            E_Deserialize = -18,

            #endregion

            #region NetConfigFile::WriteFile

            E_FileExists = -25,
            E_ConvertToFileStorageItem = -26,
            E_FileOpenReadWrite = -27,
            E_Serialize = -28,

            #endregion

            #region Program::DisconnectDevice 

            E_ConDeviceIsNoShare = -35,
            E_ConDeviceStillPresent = -36,

            #endregion

            #region frmMain::RegisterNetConfigurationFileExtension

            E_UnauthorizedAccessException = -40,

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
