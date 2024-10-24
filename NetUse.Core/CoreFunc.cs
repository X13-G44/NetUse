using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetUse.Common;



namespace NetUse.Core
{
    static public class CoreFunc
    {
        /// <summary>
        /// Execute the Net Use command with connect parameter.
        /// No checks will be performed about the current connection state (eg. already connected, ...)
        /// </summary>
        /// <param name="deviceName"></param>
        /// <param name="shareName"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        static public CommonResult ExecuteConnectNetCommand (char deviceName, string shareName, string userName, string userPass)
        {
            return ExecuteNetCommand (false, deviceName, shareName, userName, userPass);
        }



        /// <summary>
        /// Execute the Net Use command with disconnect parameter.
        /// No checks will be performed about the current connection state (eg. already connected, ...)
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        static public CommonResult ExecuteDisconnectNetCommand (char deviceName)
        {
            return ExecuteNetCommand (true, deviceName, "", "", "");
        }



        /// <summary>
        /// Execute the Net Use command.
        /// </summary>
        /// <param name="disconnectOnly"></param>
        /// <param name="deviceName"></param>
        /// <param name="shareName"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        static private CommonResult ExecuteNetCommand (bool disconnectOnly, char deviceName, string shareName, string userName, string userPass)
        {
            Process prc = new Process ();
            string errorStreamText = "";
            string stdStreamText = "";
            string argString = "";


            try
            {
                argString = disconnectOnly ? $"use {deviceName}: /delete" : $"use {deviceName}: {shareName} /persistent:no /user:{userName} {userPass}";

                prc.StartInfo.FileName = "net";
                prc.StartInfo.Arguments = argString;
                prc.StartInfo.UseShellExecute = false;
                prc.StartInfo.RedirectStandardError = true;
                prc.StartInfo.RedirectStandardOutput = true;
                prc.ErrorDataReceived += new DataReceivedEventHandler ((sender, e) => { errorStreamText += e.Data; });
                prc.OutputDataReceived += new DataReceivedEventHandler ((sender, e) => { stdStreamText += e.Data; });

                prc.Start ();

                // Start async reading from the streams. See https://learn.microsoft.com/de-de/dotnet/api/system.diagnostics.processstartinfo.redirectstandardoutput?view=netframework-4.8&devlangs=csharp&f1url=%3FappId%3DDev17IDEF1%26l%3DDE-DE%26k%3Dk(System.Diagnostics.ProcessStartInfo.RedirectStandardOutput)%3Bk(TargetFrameworkMoniker-.NETFramework%2CVersion%253Dv4.8)%3Bk(DevLang-csharp)%26rd%3Dtrue
                prc.BeginErrorReadLine ();
                prc.BeginOutputReadLine ();

                prc.WaitForExit ();


                if (prc.ExitCode == 0)
                {
                    return CommonResult.MakeSuccess (stdStreamText);
                }
                else
                {
                    string msgString = "";


                    if (!String.IsNullOrEmpty (errorStreamText))
                    {
                        msgString += errorStreamText;
                    }

                    if (!String.IsNullOrEmpty (stdStreamText))
                    {
                        msgString += " " + stdStreamText;
                    }


                    if (disconnectOnly)
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse, $"An error occurred while disconnect device \"{deviceName}:\\\". Original error message: \"{msgString}\"", prc.ExitCode);
                    }
                    else
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse, $"An error occurred while connecting to network sharing \"{shareName}\". Original error message: \"{msgString}\"", prc.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption ($"A critical error occurred while connecting to network sharing. Original error message: \"{ex.Message}\"");
            }
        }



        /// <summary>
        /// This function tries to disconnect a network share.
        /// A check about the current connection state is performed.
        /// </summary>
        /// <param name="deviceName">Device name (in upper char)</param>
        /// <returns></returns>
        static public CommonResult DisconnectNetShare (char deviceName)
        {
            try
            {
                DriveInfo[] allDrives = DriveInfo.GetDrives ();
                DriveInfo deviceQuerry = allDrives.FirstOrDefault (d => d.Name[0] == deviceName && d.IsReady);
                CommonResult funcResult = null;


                if (deviceQuerry != null)
                {
                    // We found an active device with the given device letter/name. 
                    // Try to disconnect when it is a network share.

                    if (deviceQuerry.DriveType != DriveType.Network)
                    {
                        // We can only disconnect network share devices.
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_ConDeviceIsNoShare, $"The device name \"{deviceName}:\\\" is already assigned to a none network device");
                    }

                    // Disconnect active share.
                    funcResult = ExecuteDisconnectNetCommand (deviceName);

                    if (funcResult.Success)
                    {
                        // /////////////////////////////////////////////////////
#warning @@@@@
                        //#warning The check don't works. Function DriveInfo.GetDrives makes trouble!
                        // Time for the OS to disconnect / close the connection.
                        Task.Delay (2000).Wait ();
                        return CommonResult.MakeSuccess (null, $"Connected share \"{deviceName}:\\\" was disconnected (unconfirmed)");

                        //// Try to check the disconnection up to 5sec.
                        //for (int recheckLoopCnt = 0; recheckLoopCnt < 5; recheckLoopCnt++)
                        //{
                        //    // Time for the OS to disconnect / close the connection.
                        //    Task.Delay (1500).Wait ();

                        //    // Make a second check...
                        //    allDrives = DriveInfo.GetDrives ();
                        //    deviceQuerry = allDrives.FirstOrDefault (d => d.Name[0] == deviceName && d.IsReady);

                        //    if (deviceQuerry != null)
                        //    {
                        //        return CommonResult.MakeSuccess (null, $"Connected share \"{deviceName}:\\\" was disconnected");
                        //    }
                        //}

                        // /////////////////////////////////////////////////////

                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_ConDeviceStillPresent, $"The device name \"{deviceName}:\\\" is after net use disconnect command still present");
                    }
                    else
                    {
                        return funcResult;
                    }
                }
                else
                {
                    // We found no active device with the given device letter/name. 

                    return CommonResult.MakeSuccess (null, $"No connected share with name \"{deviceName}:\\\" was found");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption (ex.Message);
            }
        }
    }
}
