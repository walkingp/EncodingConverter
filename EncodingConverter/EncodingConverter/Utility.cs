using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EncodingConverter
{
    class Utility
    {
        /// <summary>
        /// 用FileStream写文件
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void FileStreamWriteFile(string path, string str)
        {
            byte[] byData;
            char[] charData;
            try
            {
                FileStream nFile = new FileStream(path, FileMode.Open);
                //获得字符数组
                charData = str.ToCharArray();
                //初始化字节数组
                byData = new byte[charData.Length];
                //将字符数组转换为正确的字节格式
                Encoder enc = Encoding.UTF8.GetEncoder();
                enc.GetBytes(charData, 0, charData.Length, byData, 0, true);
                nFile.Seek(0, SeekOrigin.Begin);
                nFile.Write(byData, 0, byData.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// FileStream读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string FileStreamReadFile(string filePath)
        {
            byte[] data = new byte[100];
            char[] charData = new char[100];
            try
            {
                FileStream file = new FileStream(filePath, FileMode.Open);
                //文件指针指向0位置
                file.Seek(0, SeekOrigin.Begin);
                //读入两百个字节
                file.Read(data, 0, 200);
                //提取字节数组
                Decoder dec = Encoding.UTF8.GetDecoder();
                dec.GetChars(data, 0, data.Length, charData, 0);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Convert.ToString(charData);
        }
        public static void SetWriteble(string path)
        {
            //设置文件为可写
            FileAttributes att = File.GetAttributes(path);
            if ((att & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
            {
                att = RemoveAttribute(att, FileAttributes.ReadOnly);
                File.SetAttributes(path, att);
            }
        }
        private static FileAttributes RemoveAttribute(FileAttributes attributes, FileAttributes attributesToRemove)
        {
            return attributes & ~attributesToRemove;
        }
        /// <summary>
        /// 复制文件夹（及文件夹下所有子文件夹和文件）
        /// </summary>
        /// <param name="sourcePath">待复制的文件夹路径</param>
        /// <param name="destinationPath">目标路径</param>
        public static void CopyDirectory(String sourcePath, String destinationPath)
        {
            DirectoryInfo info = new DirectoryInfo(sourcePath);
            Directory.CreateDirectory(destinationPath);
            foreach (FileSystemInfo fsi in info.GetFileSystemInfos())
            {
                String destName = Path.Combine(destinationPath, fsi.Name);

                if (fsi is System.IO.FileInfo)          //如果是文件，复制文件
                    File.Copy(fsi.FullName, destName);
                else                                    //如果是文件夹，新建文件夹，递归
                {
                    Directory.CreateDirectory(destName);
                    CopyDirectory(fsi.FullName, destName);
                }
            }
        }
        /// <summary>
        /// 获取文件编码方式
        /// </summary>
        /// <param name="filename">文件路径</param>
        /// <returns>编码方式</returns>
        public static Encoding GetFileEncodeType(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);
            Byte[] buffer = br.ReadBytes(6);
            br.Close();
            fs.Close();
            if (buffer[0] >= 0xEF)//has BOM
            {
                if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF)
                {
                    return System.Text.Encoding.UTF8;
                }
                else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                {
                    return System.Text.Encoding.Unicode;
                }
                else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                {
                    return System.Text.Encoding.BigEndianUnicode;
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            else//has no BOM
            {
                /*
                 * 这里有问题，如果当前文件编码是无BOM的utf-8，会被认为是ANSI编码，搜索了很多资料，暂时无法解决*/
                /*using (StreamReader sr = new StreamReader(filename, Encoding.Default, true))
                {
                    char[] b = new char[3];
                    int n = sr.Read(b, 0, 3);
                    sr.Close();
                    Encoding enc = new UTF8Encoding(true);
                    byte[] p = enc.GetPreamble();
                    if (n==3 && b[0] == p[0] && b[1] == p[1] && b[2] == p[2])
                    {
                        return new UTF8Encoding(false);
                    }
                }*/

                return System.Text.Encoding.Default;
            }
        }
    }
}
