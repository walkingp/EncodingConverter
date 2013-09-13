using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace EncodingConverter
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
            InitialControls();
        }
        private void InitialControls()
        {
            cbTarget.DataSource = new BindingSource(TargetEncodingDict,null);
            cbTarget.DisplayMember = "Key";
            cbTarget.ValueMember = "Key";
            cbTarget.SelectedItem = cbTarget.Items[0];
        }
        private Dictionary<string, Encoding> TargetEncodingDict
        {
            get
            {
                return new Dictionary<string, Encoding>
                {
                    {"UTF-8",Encoding.UTF8},
                    {"UTF-8(with BOM)",new UTF8Encoding(false)},
                    {"Unicode",Encoding.Unicode}
                };
            }
        }
        private void btnSelPath_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog=new FolderBrowserDialog();
            DialogResult dr = dialog.ShowDialog();
            if (dr.Equals(DialogResult.OK) && !string.IsNullOrEmpty(dialog.SelectedPath))
            {
                string path=dialog.SelectedPath;
                txtPath.Text = path;
                
                listView.Items.Clear();
                listView.Tag = 0;//By path
                bwSearchFile.RunWorkerAsync(path);
            }
        }
        /// <summary>
        /// 检查后缀是否符合
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private bool CheckExtension(string fileName)
        {
            string[] allowExtentions = txtFilter.Text.Replace("*", "").ToLower().Split(';');
            var list = allowExtentions.Where(s => s != "");
            foreach (string ext in list)
            {
                if (fileName.ToLower().EndsWith(ext))
                {
                    return true;
                }
            }
            return false;
        }
        void ListFiles(FileSystemInfo info)
        {
            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;
            //不是目录 
            if (dir == null) return;
            int fileCount = 0;
            FileSystemInfo[] files = dir.GetFileSystemInfos();
            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //是文件 
                if (file != null)
                {
                    if (!CheckExtension(file.Name))
                    {
                        bwSearchFile.ReportProgress(0, file.FullName + " passed.");
                        continue;
                    }
                    DelegateAddListViewItem del=new DelegateAddListViewItem(AddListViewItem);
                    this.Invoke(del, file);
                    ++fileCount;
                    bwSearchFile.ReportProgress(0, file.FullName + " found.");
                    //del(file);
                    //对于子目录，进行递归调用 
                }
                else
                {
                    ListFiles(files[i]);
                }
            }
            bwSearchFile.ReportProgress(100, fileCount + " files found.");
        }

        delegate void DelegateAddListViewItem(FileInfo file);
        void AddListViewItem(FileInfo file)
        {
            ListViewItem lv = new ListViewItem();
            lv.Text = file.Name;
            lv.SubItems.Add((file.Length / 1024.0).ToString("F2"));
            Encoding enc = Utility.GetFileEncodeType(file.FullName);
            lv.SubItems.Add(enc.EncodingName);
            lv.SubItems.Add(file.FullName);
            lv.SubItems.Add("...");
            listView.Items.Add(lv);
        }

        delegate void DelegateConvertEncode();
        void ConvertEncode()
        {
            if (chkBak.Checked)
            {
                if (listView.Tag.Equals(0))
                {
                    string sourcePath = txtPath.Text;
                    string descPath = DateTime.Now.ToString("yyyyMMddhhmmss") + "\\" + sourcePath.Substring(sourcePath.LastIndexOf("\\"));
                    lblOpen.Tag = descPath;
                    Utility.CopyDirectory(sourcePath, descPath);
                }
            }

            int succCount=0;
            for (int i = 0, len = listView.Items.Count; i < len; i++)
            {
                string path = listView.Items[i].SubItems[3].Text;
                if (Utility.GetFileEncodeType(path) == Encoding.UTF8)
                {
                    listView.Items[i].SubItems[4].Text = "跳过";
                    continue;
                }

                if (listView.Tag.Equals(1))
                {
                    string descPath=Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory,DateTime.Now.ToString("yyyyMMdd"));
                    //descPath = Path.Combine(descPath, "bak");
                    if(!Directory.Exists(descPath)){
                        Directory.CreateDirectory(descPath);
                    }
                    descPath = Path.Combine(descPath, Path.GetFileName(path));
                    try
                    {
                        if (!File.Exists(descPath))
                        {
                            //File.Delete(descPath);
                            File.Copy(path, descPath);
                        }
                    }
                    catch
                    {
                       
                    }
                }
                FileInfo file = new FileInfo(path);
                if (!file.Exists)
                {
                    continue;
                }
                bwConvertFile.ReportProgress(0, "Converting file: " + path);
                //Set files into writeable
                Utility.SetWriteble(path);
                try
                {
                    string content = File.ReadAllText(path,Utility.GetFileEncodeType(path));

                    Encoding targetEncoding = TargetEncodingDict[cbTarget.SelectedValue.ToString()];
                    using (StreamWriter sw = new StreamWriter(path, false,targetEncoding))
                    {
                        sw.WriteLine(content);
                        sw.Close();
                    }

                    listView.Items[i].SubItems[2].Text = Utility.GetFileEncodeType(path).EncodingName;
                    listView.Items[i].SubItems[4].Text = "√";
                    ++succCount;
                }
                catch
                {
                    listView.Items[i].SubItems[4].Text = "×";
                }
            }
            bwConvertFile.ReportProgress(100, succCount + " files converted complete.");
        }

        private void bwSearchFile_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = e.Argument.ToString();
            ListFiles(new DirectoryInfo(path));
        }

        private void bwSearchFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DelegateShowStatusText del = new DelegateShowStatusText(ShowStatusText);
            this.Invoke(del, e.UserState.ToString());
        }

        private void bwSearchFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lblStatus.Text = "Searching complete";
        }

        private void bwConvertFile_DoWork(object sender, DoWorkEventArgs e)
        {
            DelegateConvertEncode del = new DelegateConvertEncode(ConvertEncode);
            this.Invoke(del);
        }

        private void bwConvertFile_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DelegateShowStatusText del = new DelegateShowStatusText(ShowStatusText);
            this.Invoke(del, e.UserState.ToString());
        }

        private void bwConvertFile_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //DelegateShowStatusText del = new DelegateShowStatusText(ShowStatusText);
            //this.Invoke(del, e.UserState.ToString());
            MessageBox.Show("Convert completed!", "Encoding Converter", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            bwConvertFile.RunWorkerAsync();
        }

        delegate void DelegateShowStatusText(string text);
        void ShowStatusText(string text)
        {
            lblStatus.Text = text;
        }

        private void lblOpen_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string path=System.AppDomain.CurrentDomain.BaseDirectory;
            if (lblOpen.Tag != null)
            {
                 path=lblOpen.Tag.ToString();
            }
            System.Diagnostics.Process.Start(path);
        }

        private void listView_DragDrop(object sender, DragEventArgs e)
        {
            string[] fileArr = e.Data.GetData(DataFormats.FileDrop,false) as string[];

            listView.Tag = 1;
            foreach (string filePath in fileArr)
            {
                if (!CheckExtension(filePath))
                {
                    lblStatus.Text = string.Concat(filePath, " passed");
                    continue;
                }

                FileInfo file = new FileInfo(filePath);
                DelegateAddListViewItem del = new DelegateAddListViewItem(AddListViewItem);
                this.Invoke(del, file);
            }
        }

        private void listView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Link;
            else e.Effect = DragDropEffects.None;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            listView.Items.Clear();
        }

        private void lnkImport_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter="Text file(*.txt)|*.txt|All files(*.*)|*.*";
            listView.Tag = 1;

            if (DialogResult.OK == dialog.ShowDialog())
            {
                string path = dialog.FileName;
                using (StreamReader sr = new StreamReader(path, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if(!string.IsNullOrEmpty(line)){
                            FileInfo file = new FileInfo(line);
                            if (file.Exists)
                            {
                                if (!CheckExtension(file.Name))
                                {
                                    lblStatus.Text = string.Concat(file.Name, " passed");
                                    continue;
                                }

                                DelegateAddListViewItem del = new DelegateAddListViewItem(AddListViewItem);
                                this.Invoke(del, file);
                            }
                        }
                    }
                    sr.Close();
                }
            }
        }

        private void lblRefresh_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            List<string> listPath = new List<string>();
            for (int i = 0, len = listView.Items.Count; i < len; i++)
            {
                listPath.Add(listView.Items[i].SubItems[3].Text);
            }
            listView.Items.Clear();
            foreach (string filePath in listPath)
            {
                FileInfo file = new FileInfo(filePath);
                DelegateAddListViewItem del = new DelegateAddListViewItem(AddListViewItem);
                this.Invoke(del, file);
            }
        }
    }
}
