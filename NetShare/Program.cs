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
using System.Reflection;



namespace NetUse
{
    internal class Program
    {
        static int Main (string[] args)
        {
            NetConfigData netConfigData;
            CommonResult funcResult;


            Console.WriteLine ($"{AssemblyTitle} (v{AssemblyVersion}) - {AssemblyCopyright}\n");

            try
            {
                // Process command line parameter.
                funcResult = CheckArgAndLoadNetCfgFile (args);

                // Start working when configuration file was load.
                if (funcResult.Success && funcResult.Data is NetConfigFile.NetConfigFile && (funcResult.Data as NetConfigFile.NetConfigFile).Data != null)
                {
                    netConfigData = (funcResult.Data as NetConfigFile.NetConfigFile).Data as NetConfigData;

                    if (netConfigData.OnlyDisconnect)
                    {
                        funcResult = CoreFunc.DisconnectNetShare (netConfigData.DeviceName);
                    }
                    else
                    {
                        if (netConfigData.DisconnectFirst)
                        {
                            funcResult = CoreFunc.DisconnectNetShare (netConfigData.DeviceName);
                        }

                        if (funcResult == null || funcResult.Success)
                        {
                            funcResult = CoreFunc.ExecuteConnectNetCommand (netConfigData.DeviceName, netConfigData.ShareName, netConfigData.UserName, netConfigData.UserPass);
                        }
                    }
                }

                Console.WriteLine (funcResult.Message);
                return (int) funcResult.ErrorCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine ($"A critical error occurred. Original error message: \"{ex.Message}\"");
                return (int) CommonResult.ErrorResultCodes.E_Exeption;
            }
        }



        /// <summary>
        /// Process the start parameter and try to load the NetConfiguration file.
        /// </summary>
        /// <param name="args"></param>
        /// <returns>On success, a NetConfigFile object will be returned</returns>
        static CommonResult CheckArgAndLoadNetCfgFile (string[] args)
        {
            string netConfigFileName;
            NetConfigFile.NetConfigFile netConfigFile = new NetConfigFile.NetConfigFile ();

            CommonResult loadCfgFileResult = null;


            try
            {
                if (args.Length == 1)
                {
                    // Note: User can select the Net Use Configuration file by two ways:
                    // 1st: Only the filename of the Net Use Configuration file without file extension.
                    //      --> The file must be located in the same folder as our running assembly does.
                    // 2nd: A full path to the Net Use Configuration file.

                    netConfigFileName = args[0].Replace ('"', ' ').Trim ().ToLower ();

                    string[] configFiles = Directory.GetFiles (AssemblyDirectory, "*.netcfg");

                    foreach (var configFile in configFiles)
                    {
                        if (Path.GetFileNameWithoutExtension (configFile).ToLower () == netConfigFileName)
                        {
                            netConfigFileName = configFile;
                            break;
                        }
                    }

                    if (File.Exists (netConfigFileName))
                    {
                        loadCfgFileResult = netConfigFile.LoadFile (netConfigFileName);

                        if (loadCfgFileResult.Success)
                        {
                            return CommonResult.MakeSuccess (netConfigFile);
                        }
                        else
                        {
                            return loadCfgFileResult;
                        }
                    }
                    else
                    {
                        return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_CfgFileNotFound,
                            $"The selected Net Use Configuration file \"{netConfigFileName}\" was not found.\n\n" +
                            $"Use:\n" +
                            $"\t{AssemblyFileName} configFile_without_extension\n" +
                            $"\t{AssemblyFileName} \"C:\\path\\configFile.netcfg\"\n\n" +
                            $"\tNote: In the first example, the Configuration file must be located in the same folder as {AssemblyFileName} does.");
                    }
                }
                else
                {
                    return CommonResult.MakeError (CommonResult.ErrorResultCodes.E_CntOfStartParam,
                        $"Invalid number of start parameters!\n\n" +
                        $"Use:\n" +
                        $"\t{AssemblyFileName} configFile_without_extension\n" +
                        $"\t{AssemblyFileName} \"C:\\path\\configFile.netcfg\"\n\n" +
                        $"\tNote: In the first example, the Configuration file must be located in the same folder as {AssemblyFileName} does.");

                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption ($"A critical error occurred while try to read the start parameter string, open and read the NetConfiguration file. Original error message: \"{ex.Message}\"");
            }
        }




        #region Assembly attribute accessors



        static public string AssemblyFileName
        {
            get
            {
                return Path.GetFileName (Assembly.GetExecutingAssembly ().Location);
            }
        }



        static public string AssemblyDirectory
        {
            get
            {
                return Path.GetDirectoryName (Assembly.GetExecutingAssembly ().Location);
            }
        }



        static public string AssemblyTitle
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



        static public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly ().GetName ().Version.ToString ();
            }
        }



        static public string AssemblyDescription
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



        static public string AssemblyProduct
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



        static public string AssemblyCopyright
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



        static public string AssemblyCompany
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
