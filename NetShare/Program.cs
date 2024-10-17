using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using NetUse.Common;
using NetUse.NetConfigFile;
using NetUse.Core;



namespace NetUse
{
    internal class Program
    {
        static int Main(string[] args)
        {
            NetConfigData netConfigData = null;
            CommonResult funcResult = null;


            try
            {
                // Process commandlin parameter.
                funcResult = CheckArgAndLoadNetCfgFile(args);

                // Start working when configuration file was load.
                if (funcResult.Success && funcResult.Data is NetConfigFile.NetConfigFile && (funcResult.Data as NetConfigFile.NetConfigFile).Data != null)
                {
                    netConfigData = (funcResult.Data as NetConfigFile.NetConfigFile).Data as NetConfigData;

                    if (netConfigData.OnlyDisconnect)
                    {
                        funcResult = CoreFunc.ExecuteDisconnectNetCommand(netConfigData.DeviceName);
                    }
                    else
                    {
                        if (netConfigData.DisconnectFirst)
                        {
                            funcResult = CoreFunc.DisconnectNetShare(netConfigData.DeviceName);
                        }

                        if (funcResult == null || funcResult.Success)
                        {
                            funcResult = CoreFunc.ExecuteConnectNetCommand(netConfigData.DeviceName, netConfigData.ShareName, netConfigData.UserName, netConfigData.UserPass);
                        }
                    }
                }

                Console.WriteLine(funcResult.Message);
                return (int)funcResult.ErrorCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"A critical error occurred. Original error message: \"{ex.Message}\"");
                return (int)CommonResult.ErrorResultCodes.E_Exeption;
            }
        }



        /// <summary>
        /// Process the start parameter and try to load the NetConfiguration file.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>On sucess, a NetConfigFile object will be returned</returns>
        static CommonResult CheckArgAndLoadNetCfgFile(string[] args)
        {
            string netConfigFileName = String.Empty;
            NetConfigFile.NetConfigFile netConfigFile = new NetConfigFile.NetConfigFile();

            CommonResult loadCfgFileResult = null;


            try
            {
                if (args.Length == 2)
                {
                    netConfigFileName = args[1].Remove('"', ' ').Trim();


                    // We have to locations to serach for the NetConfiguration file:
                    // 1st: Folder of the current running process / program,
                    // 2nd: At the (possible) given path of the file.

                    #region Search in the folder of the current running process / program.

                    string netUsePath = Path.GetFileName(args[0]);
                    string netConfigFilePath = Path.GetFileName(netConfigFileName);


                    // If no path of the given NetConfiguration file is present, we asume that the file is located in the 
                    // folder of the current process.
                    if (netUsePath != String.Empty && netConfigFilePath == String.Empty)
                    {
                        foreach (string file in Directory.GetFiles(netUsePath))
                        {
                            if (file.ToLower() == netConfigFileName)
                            {
                                netConfigFileName = netUsePath + file;
                            }
                        }
                    }
                    else
                    {
                        // There is a path of the given NetConfiguration file is present. The full file path is present.
                    }

                    #endregion

                    loadCfgFileResult = netConfigFile.LoadFile(netConfigFileName);  // Note: The function preform a file exists operation.

                    if (loadCfgFileResult.Success)
                    {
                        return CommonResult.MakeSuccess(netConfigFile);
                    }
                    else
                    {
                        return loadCfgFileResult;
                    }
                }
                else
                {
                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_CntOfStartParam, "Invalid number of start parameters");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption($"A critical error occurred while try to read the start parameter string, open and read the NetConfiguration file. Original error message: \"{ex.Message}\"");
            }
        }
    }
}
