using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace EricUtility
{
    public class IOUtility
    {
        private static List<String> GenerateFileList(string Dir)
        {
            List<String> content = new List<String>();
            foreach (string file in Directory.GetFiles(Dir)) // add each file in directory
                content.Add(file);
            foreach (string dirs in Directory.GetDirectories(Dir)) // recursive
            {
                content.AddRange(GenerateFileList(dirs));
            }
            if (content.Count == 0)
                content.Add(String.Format("{0}/", Dir));
            return content; // return file list
        }

        public static void CopyFolderAndContent(string srcFolderPath, string destFolderPath)
        {
            DirectoryInfo srcInfo = new DirectoryInfo(srcFolderPath);
            DirectoryInfo[] srcDirs = srcInfo.GetDirectories();
            foreach (DirectoryInfo dir in srcDirs)
            {
                Directory.CreateDirectory(Path.Combine(destFolderPath, dir.Name));
                CopyFolderAndContent(Path.Combine(srcFolderPath, dir.Name), Path.Combine(destFolderPath, dir.Name));
            }
            FileInfo[] srcFiles = srcInfo.GetFiles();
            foreach (FileInfo file in srcFiles)
            {
                try
                {
                    file.CopyTo(Path.Combine(destFolderPath, file.Name), true);
                }
                catch
                {
                    LogManager.Log(LogLevel.Error, "IOUtility.CopyFolderAndContent",String.Format("Impossible de copier {0}.", file.Name));
                }
            }
        }

        public static string CreateTemporaryFolder(string rootPath, string filename)
        {
            string tempFolderName = filename.Split('.')[0];
            string tempFolderRoot = Path.Combine(rootPath, tempFolderName);
            if (Directory.Exists(tempFolderRoot))
                Directory.Delete(tempFolderRoot, true);
            Directory.CreateDirectory(tempFolderRoot);
            return tempFolderRoot;
        }
        public static string CreateHierarchicFolder(string rootPath, string path)
        {
            string tempFolderPath = rootPath;
            string[] hierarchie = path.Split('/');
            for (int i = 0; i < hierarchie.Length; ++i)
            {
                tempFolderPath = Path.Combine(tempFolderPath, hierarchie[i]);
                if (!Directory.Exists(tempFolderPath))
                    Directory.CreateDirectory(tempFolderPath);
            }
            return tempFolderPath;
        }
        public static void RemoveHierarchicFolder(string rootPath, string path)
        {
            string fullPath = Path.Combine(rootPath, path);
            bool canDelete = true;
            while (canDelete && fullPath.Length > rootPath.Length)
            {
                DirectoryInfo di = new DirectoryInfo(fullPath);
                if (di.GetDirectories().Length > 0 || di.GetFiles().Length > 0)
                    canDelete = false;
                else
                {
                    Directory.Delete(fullPath);
                    fullPath = di.Parent.FullName;
                }
            }
        }
    }
}
