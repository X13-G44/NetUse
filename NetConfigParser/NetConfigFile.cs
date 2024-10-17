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
        /// Filename from the curently used NetConfiguration file.
        /// Empty, when no file was load.
        /// </summary>
        public string CurrentFile { get; private set; }

        /// <summary>
        /// Data from the curently used NetConfiguration file.
        /// NULL, when no file was load.
        /// </summary>
        public NetConfigData Data { get; private set; }

        /// <summary>
        /// Cryption method from the curently used NetConfiguration file.
        /// PlainText, when no file was load.
        /// </summary>
        public Cryption.CryptionMethod CryptionMethod { get; private set; }



        /// <summary>
        /// Try to load a NetConfiguration file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public CommonResult LoadFile(string filename)
        {
            this.CurrentFile = "";
            this.Data = null;
            this.CryptionMethod = CryptionMethod.PlanText;

            try
            {
                if (File.Exists(filename))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FileStorageItem));
                    FileStorageItem fileStorageItem;
                    NetConfigData storageItem;
                    CryptionMethod cryptionMethod;


                    try
                    {
                        using (FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            try
                            {
                                fileStorageItem = (FileStorageItem)serializer.Deserialize(fs);
                            }
                            catch (Exception ex)
                            {
                                return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_Deserialize, $"Could not deserialize Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                            }

                            storageItem = NetConfigData.FromFileStorageItem(fileStorageItem, out cryptionMethod);

                            if (storageItem != null)
                            {
                                this.Data = storageItem;
                                this.CurrentFile = filename;
                                this.CryptionMethod = cryptionMethod;

                                return CommonResult.MakeSuccess();
                            }

                            return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_FileReadParsing, $"Could not parse Net Use Configuration file content. Maybe incompatible version");
                        }
                    }
                    catch (Exception ex)
                    {
                        return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_FileRead, $"Could not open and read Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                    }
                }
                else
                {
                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_FileNotExists, $"Net Use Configuration file \"{filename}\" not exists");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption(ex.Message);
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
        public CommonResult WriteFile(NetConfigData data, string filename, Cryption.CryptionMethod cryptionMethod, bool allowFileOverride, bool loadFile)
        {
            try
            {
                if (allowFileOverride || File.Exists(filename) == false)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(FileStorageItem));
                    FileStorageItem fileStorageItem;


                    fileStorageItem = data.ToFileStorageItem(cryptionMethod);

                    if (fileStorageItem != null)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite))
                            {
                                try
                                {
                                    serializer.Serialize(fs, fileStorageItem);
                                }
                                catch (Exception ex)
                                {
                                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_Serialize, $"Data preparing failed. Reason: {ex.Message}");
                                }

                                if (loadFile)
                                {
                                    this.Data = data;
                                    this.CurrentFile = filename;
                                    this.CryptionMethod = cryptionMethod;
                                }

                                return CommonResult.MakeSuccess();
                            }

                        }
                        catch (Exception ex)
                        {
                            return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_FileOpenReadWrite, $"Could not create and write Net Use Configuration file \"{filename}\". Reason: {ex.Message}");
                        }
                    }
                    else
                    {
                        return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_ConvertToFileStorageItem, $"Data preparing failed");
                    }
                }
                else
                {
                    return CommonResult.MakeError(CommonResult.ErrorResultCodes.E_FileExists, $"Net Use Configuration file \"{filename}\" already exists");
                }
            }
            catch (Exception ex)
            {
                return CommonResult.MakeExeption(ex.Message);
            }
        }



        public NetConfigFile()
        {
            this.CurrentFile = "";
            this.Data = null;
            this.CryptionMethod = CryptionMethod.PlanText;
        }
    }
}
