using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Text;

namespace WebNotifications.Helper
{
    public class ExportToFile
    {
        public static void DownLoadAsExcel(string fileName, string content)
        {
            OutputContentToClient(fileName, content, ".xls", "application/ms-excel");
        }

        public static void DownLoadAsConfig(string fileName, string content)
        {
            OutputContentToClient(fileName, content, ".config", "application/txt");
        }

        /// <summary>        
        /// output content to client        
        /// </summary>        
        /// <param name="fileName">fileName</param>        
        /// <param name="content">content</param>        
        public static void OutputContentToClient(string fileName, string content, string suffix, string contentType)
        {
            if (!string.IsNullOrEmpty(content))
            {
                //fileName = HttpUtility.UrlEncode(fileName + DateTime.Now + ".csv");          
                // fileName = fileName + DateTime.Now.ToString(dateFormat) + ".csv";                
                fileName = fileName + suffix;
                byte[] buffer = Encoding.UTF8.GetBytes(content);
                buffer = Encoding.UTF8.GetPreamble().Concat(buffer).ToArray(); HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + CodeHelper.GetEncodeStr(fileName));               
                HttpContext.Current.Response.ContentType = contentType; HttpContext.Current.Response.AddHeader("Content-Length", buffer.Length.ToString()); HttpContext.Current.Response.BinaryWrite(buffer);
                HttpContext.Current.Response.Flush();
                HttpContext.Current.Response.Close();
            }
        }

        public static bool DownloadFile(string fileFullPath)
        {
            if (!File.Exists(fileFullPath)) return false;

            bool res = false;
            string fileName = Path.GetFileName(fileFullPath);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(fileFullPath);

            using (FileStream fileStream = new FileStream(fileFullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                //Reads file as binary values                
                BinaryReader fileBinaryReader = new BinaryReader(fileStream);
                //Check whether file exists in specified location                
                try
                {
                    long startBytes = 0;
                    string lastUpdateTiemStamp = File.GetLastWriteTimeUtc(fileFullPath).ToString("r"); string etagEncodedData = HttpUtility.UrlEncode(fileName, Encoding.UTF8) + lastUpdateTiemStamp;
                    HttpContext.Current.Response.Clear(); HttpContext.Current.Response.Buffer = false;
                    HttpContext.Current.Response.AddHeader("Accept-Ranges", "bytes"); HttpContext.Current.Response.AppendHeader("ETag", "\"" + etagEncodedData + "\""); HttpContext.Current.Response.AppendHeader("Last-Modified", lastUpdateTiemStamp); HttpContext.Current.Response.ContentType = "application/octet-stream";
                    //HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(fileInfo.Name));                    
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + CodeHelper.GetEncodeStr(fileInfo.Name));
                    //HttpContext.Current.Response.AddHeader("Content-Length", (fileInfo.Length - startBytes).ToString());                    
                    HttpContext.Current.Response.AddHeader("Connection", "Keep-Alive"); HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
                    //Send data                    
                    fileBinaryReader.BaseStream.Seek(startBytes, SeekOrigin.Begin);
                    //Dividing the data in 1024 bytes package                    
                    int maxCount = (int)Math.Ceiling((fileInfo.Length - startBytes + 0.0) / 1024);
                    //Download in block of 1024 bytes                    
                    int i;
                    for (i = 0; i < maxCount && HttpContext.Current.Response.IsClientConnected; i++)
                    {
                        HttpContext.Current.Response.BinaryWrite(fileBinaryReader.ReadBytes(1024));
                        HttpContext.Current.Response.Flush();
                    }
                    //if blocks transfered not equals total number of blocks                   
                    if (i < maxCount) res = false;
                    res = true;
                }
                catch
                {
                    res = false;
                }
                finally
                {
                    HttpContext.Current.Response.Close();
                    fileBinaryReader.Close();
                    fileStream.Close();
                }
            }
            return res;
        }
    }
}