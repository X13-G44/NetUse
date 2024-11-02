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



//#define WORKAROUND_DISCONNECTED_DETECTION
#define USE_WIN32_API



using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Configuration;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using NetUse.Common;
using System.Runtime.InteropServices;



namespace NetUse.Core
{
    static public class CoreFunc
    {
        /// <summary>
        /// This function tries to connect to a network share.
        /// The function also check the device name state; if it is used by the os and must released first.
        /// </summary>
        /// <param name="allowDisconnect">Set the parameter to release the (possible) assigned device name first</param>
        /// <param name="deviceName"></param>
        /// <param name="shareName"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        static public CommonResult ConnectNetShare (bool allowDisconnect, char deviceName, string shareName, string userName, string userPass)
        {
            try
            {
                CommonResult funcResult = null;


                if (IsDeviceNameUsed (deviceName) == false)
                {
                    // Connect network share.
#if USE_WIN32_API
                    return ExecuteCommand_WinApi (false, deviceName, shareName, userName, userPass);
#else
                    return ExecuteCommand_Cli (false, deviceName, shareName, userName, userPass);
#endif
                }
                else
                {
                    // The device name is used. Try to disconnect the assigned network share first - if allowed by configuration.

                    if (allowDisconnect == false)
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NotAllowedToDisconnect, $"Device name \"{deviceName}:\\\" is already assigned");
                    }
                    else
                    {
                        // Disconnect the network share.
                        funcResult = DisconnectNetShare (deviceName);

                        if (funcResult.Success != true)
                        {
                            return funcResult;
                        }
                    }

#if USE_WIN32_API
                    return ExecuteCommand_WinApi (false, deviceName, shareName, userName, userPass);
#else
                    return ExecuteCommand_Cli (false, deviceName, shareName, userName, userPass);
#endif
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption (ex.Message);
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
                CommonResult funcResult = null;
                DriveType driveType;


                if (IsDeviceNameUsed (deviceName, out driveType))
                {
                    // We found an active device with the given device letter/name. 
                    // Try to disconnect when it is a network share.

                    if (driveType != DriveType.Network)
                    {
                        // We can only disconnect network share devices.
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_ConDeviceIsNoShare, $"The device name \"{deviceName}:\\\" is already assigned to a none network device");
                    }

                    // Disconnect active share.
#if USE_WIN32_API
                    funcResult = ExecuteCommand_WinApi (true, deviceName, "", "", "");
#else
                    funcResult = ExecuteCommand_Cli (true, deviceName, "", "", "");
#endif

                    if (funcResult.Success)
                    {
#warning The check after disconnecting a network share failed. The underlaying function DriveInfo.GetDrives() returns still the unconnected device name.
                        // WORKAROUND CODE /////////////////////////////////////////////////////
#if WORKAROUND_DISCONNECTED_DETECTION

                        // Time for the OS to disconnect / close the connection.
                        Task.Delay (2000).Wait ();
                        return CommonResult.MakeSuccess (null, $"Connected share \"{deviceName}:\\\" was disconnected (unconfirmed)");

                        // ORIGINAL CODE /////////////////////////////////////////////////////
#else

                        // Try to check the disconnection up to 5sec.
                        for (int recheckLoopCnt = 0; recheckLoopCnt < 10; recheckLoopCnt++)
                        {
                            // Time for the OS to disconnect / close the connection.
                            Task.Delay (500).Wait ();

                            // Make a new check...
                            if (IsDeviceNameUsed (deviceName) == false)
                            {
                                return CommonResult.MakeSuccess (null, $"Connected share \"{deviceName}:\\\" was disconnected");
                            }
                        }

                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_ConDeviceStillPresent, $"The device name \"{deviceName}:\\\" is after net use disconnect command still present");

#endif
                        // END WORKAROUND /////////////////////////////////////////////////////
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



        /// <summary>
        /// This function tries to disconnect all network shares.
        /// </summary>
        /// <returns></returns>
        static public CommonResult DisconnectAllNetShare ()
        {
            try
            {
#if USE_WIN32_API
                CommonResult funcResult;

                DriveInfo[] allDrives = DriveInfo.GetDrives ();
                var deviceQuerry = allDrives.Where (d => d.IsReady && d.DriveType == DriveType.Network);


                foreach (var drive in deviceQuerry)
                {
                    funcResult = ExecuteCommand_WinApi (true, drive.Name[0], "", "", "");

                    if (funcResult.Success == false)
                    {
                        return funcResult;
                    }
                }

                return CommonResult.MakeSuccess (null);
#else
                // Disconnect all active shares.
                return ExecuteCommand_Cli (true, ' ', "", "", "")
#endif
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption (ex.Message);
            }
        }



        /// <summary>
        /// Check if the given device name is already assigned by the OS to a resource.
        /// </summary>
        /// <param name="deviceName">Device name like 'z'</param>
        /// <returns>True, if the given device name is already used</returns>
        static public bool IsDeviceNameUsed (char deviceName)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives ();
            DriveInfo deviceQuerry = allDrives.FirstOrDefault (d => d.Name[0] == deviceName && d.IsReady);


            return (deviceQuerry != null) ? true : false;
        }



        /// <summary>
        /// Check if the given device name is already assigned by the OS to a resource.
        /// </summary>
        /// <param name="deviceName">Device name like 'z'</param>
        /// <param name="driveType">Device type of the assigned device name</param>
        /// <returns>True, if the given device name is already used</returns>
        static public bool IsDeviceNameUsed (char deviceName, out DriveType driveType)
        {
            DriveInfo[] allDrives = DriveInfo.GetDrives ();
            DriveInfo deviceQuerry = allDrives.FirstOrDefault (d => d.Name[0] == deviceName && d.IsReady);


            driveType = DriveType.Unknown;

            if (deviceQuerry != null)
            {
                driveType = deviceQuerry.DriveType;
                return true;
            }

            return false;
        }



        #region Function based on the NET USE command line.



        /// <summary>
        /// Execute the Net Use command to connect / disconnect a network share.
        /// The function use the command line tool NET USE.
        /// The function also allows to disconnect all connected shares. To do this, param deviceName must be set to ' ' and param disconnectOnly must be also set.
        /// </summary>
        /// <param name="disconnectOnly"></param>
        /// <param name="deviceName"></param>
        /// <param name="shareName"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        static private CommonResult ExecuteCommand_Cli (bool disconnectOnly, char deviceName, string shareName, string userName, string userPass)
        {
            Process prc = new Process ();
            string errorStreamText = "";
            string stdStreamText = "";
            string argString = "";


            try
            {
                if (disconnectOnly == false)
                {
                    argString = $"use {deviceName}: {shareName} /persistent:no /user:{userName} {userPass}";
                }
                else
                {
                    if (deviceName != ' ')
                    {
                        argString = $"use {deviceName}: /delete";
                    }
                    else
                    {
                        argString = $"use * /delete /yes";
                    }
                }

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
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse_Cli, $"An error occurred while disconnect device name\"{deviceName}:\\\". Original error message: \"{msgString}\"", prc.ExitCode);
                    }
                    else
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse_Cli, $"An error occurred while connecting to network sharing \"{shareName}\". Original error message: \"{msgString}\"", prc.ExitCode);
                    }
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption ($"A critical error occurred while connecting to network sharing. Original error message: \"{ex.Message}\"");
            }
        }



        #endregion



        #region Function based o Win32 API - mpr.dll (WNet)



        //[DllImport ("mpr.dll", CharSet = CharSet.Auto)]
        [DllImport ("mpr.dll")]
        private static extern int WNetAddConnection2 (NetResource netResource, string password, string username, int flags);



        //[DllImport ("mpr.dll", CharSet = CharSet.Auto)]
        [DllImport ("mpr.dll")]
        private static extern int WNetCancelConnection2 (string name, int flags, bool force);



        [StructLayout (LayoutKind.Sequential)]
        private class NetResource
        {
            public ResourceScope Scope;
            public ResourceType ResourceType;
            public ResourceDisplayType DisplayType;
            public int Usage;
            public string LocalName;
            public string RemoteName;
            public string Comment;
            public string Provider;
        }



        private enum ResourceScope : int
        {
            Connected = 1,
            GlobalNetwork,
            Remembered,
            Recent,
            Context
        };



        private enum ResourceType : int
        {
            Any = 0,
            Disk = 1,
            Print = 2,
            Reserved = 8,
        }



        private enum ResourceDisplayType : int
        {
            Generic = 0x0,
            Domain = 0x01,
            Server = 0x02,
            Share = 0x03,
            File = 0x04,
            Group = 0x05,
            Network = 0x06,
            Root = 0x07,
            Shareadmin = 0x08,
            Directory = 0x09,
            Tree = 0x0a,
            Ndscontainer = 0x0b
        }



        /// <summary>
        /// Execute the Net Use command to connect / disconnect a network share.
        /// The function use the Win32 API (WNet).
        /// Sources:
        /// https://learn.microsoft.com/de-de/windows/win32/api/winnetwk/nf-winnetwk-wnetaddconnection2a
        /// </summary>
        /// <param name="disconnectOnly"></param>
        /// <param name="deviceName"></param>
        /// <param name="shareName"></param>
        /// <param name="userName"></param>
        /// <param name="userPass"></param>
        /// <returns></returns>
        static private CommonResult ExecuteCommand_WinApi (bool disconnectOnly, char deviceName, string shareName, string userName, string userPass)
        {
            try
            {
                if (disconnectOnly)
                {
                    var result = WNetCancelConnection2 ($"{deviceName}:", 0, true);


                    if (result == 0)
                    {
                        return CommonResult.MakeSuccess ();
                    }
                    else
                    {
                        string errorMessage = new Win32Exception (Marshal.GetLastWin32Error ()).Message;


                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse_ShellApi, $"An error occurred while disconnect device name\"{deviceName}:\\\". Error code: {result} {errorMessage}", result);
                    }
                }
                else
                {
                    NetResource netResource = new NetResource
                    {
                        Scope = ResourceScope.GlobalNetwork,
                        ResourceType = ResourceType.Disk,
                        DisplayType = ResourceDisplayType.Share,
                        RemoteName = shareName,
                        LocalName = $"{deviceName}:"
                    };

                    int result = WNetAddConnection2 (
                        netResource,
                        userPass,
                        userName,
                        0);

                    if (result == 0)
                    {
                        return CommonResult.MakeSuccess ();
                    }
                    else
                    {
                        string errorMessage = new Win32Exception (Marshal.GetLastWin32Error ()).Message;


                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_NetUse_ShellApi, $"An error occurred while connecting to network sharing \"{shareName}\". Error code: {result} {errorMessage}", result);
                    }
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption ($"A critical error occurred while connecting to network sharing. Original error message: \"{ex.Message}\"");
            }
        }



        #endregion
    }
}
