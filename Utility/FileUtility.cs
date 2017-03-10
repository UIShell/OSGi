namespace UIShell.OSGi.Utility
{
    using System;
    using System.IO;

    internal class FileUtility
    {
        public static void CopyFilesDirs(string srcDir, string dstDir, bool overrideExist)
        {
            foreach (string str in Directory.GetFiles(srcDir))
            {
                string path = Path.Combine(dstDir, Path.GetFileName(str));
                string sourceFileName = Path.Combine(srcDir, Path.GetFileName(str));
                if (overrideExist || !File.Exists(path))
                {
                    File.Copy(sourceFileName, path, overrideExist);
                }
            }
            foreach (string str4 in Directory.GetDirectories(srcDir))
            {
                Directory.CreateDirectory(Path.Combine(dstDir, Path.GetFileName(str4)));
                CopyFilesDirs(Path.Combine(srcDir, Path.GetFileName(str4)), Path.Combine(dstDir, Path.GetFileName(str4)), overrideExist);
            }
        }

        public static string EvaluateRelativePath(string mainDirPath, string absoluteFilePath)
        {
            string[] strArray = mainDirPath.Trim(new char[] { Path.DirectorySeparatorChar }).Split(new char[] { Path.DirectorySeparatorChar });
            string[] strArray2 = absoluteFilePath.Trim(new char[] { Path.DirectorySeparatorChar }).Split(new char[] { Path.DirectorySeparatorChar });
            int num = 0;
            for (int i = 0; i < Math.Min(strArray.Length, strArray2.Length); i++)
            {
                if (!strArray[i].ToLower().Equals(strArray2[i].ToLower()))
                {
                    break;
                }
                num++;
            }
            if (num == 0)
            {
                return absoluteFilePath;
            }
            string str = string.Empty;
            for (int j = num; j < strArray.Length; j++)
            {
                if (j > num)
                {
                    str = str + Path.DirectorySeparatorChar;
                }
                str = str + "..";
            }
            if (str.Length == 0)
            {
                str = ".";
            }
            for (int k = num; k < strArray2.Length; k++)
            {
                str = str + ((string) Path.DirectorySeparatorChar) + strArray2[k];
            }
            return str;
        }
    }
}

