using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using Ionic.Zip;
using NeoLauncher.Properties;

namespace NeoLauncher
{
    public partial class Form1 : Form
    {
        public Info Info;

        public WebClient InfoDownloader = new WebClient { Proxy = null };
        public WebClient UpdateDownloader = new WebClient { Proxy = null };

        private FileInfo DownloadFile, TV_CD_DVD;
        private DirectoryInfo Root;

        public Form1()
        {
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            WebRequest.DefaultWebProxy = null;
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DownloadFile = new FileInfo(Path.GetTempFileName());
            TV_CD_DVD = new FileInfo("tv_cd_dvd.exe");
            Root = TV_CD_DVD.Directory.Parent.Parent;

#if !DEBUG
            if (!TV_CD_DVD.Exists)
            {
                MessageBox.Show("tv_cd_dvd.exe not found\nPlease place this launcher into same directory as your game executable!");
                Close();
                return;
            }
#endif
            webBrowser1.Url = new Uri(Settings.Default.NewsUrl, UriKind.Absolute);
            //webBrowser1.Url = new Uri("http://tribesrevengeance.com", UriKind.Absolute);


            InfoDownloader.DownloadProgressChanged += ShowProgress;
            UpdateDownloader.DownloadProgressChanged += ShowProgress;

            InfoDownloader.DownloadStringCompleted += InfoDownloaded;
            ResetProgressBar();
            InfoDownloader.DownloadStringAsync(new Uri(Settings.Default.InfoUrl));
        }

        private void ShowProgress(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            long recvBytes = downloadProgressChangedEventArgs.BytesReceived;
            long totalBytes = downloadProgressChangedEventArgs.TotalBytesToReceive;

            double percent = 1.0 * recvBytes / totalBytes * 100.0;
            int num = (int)Math.Round(percent);

            if (num < 0 || num > 100) num = 100;
            progressBar1.Value = num;
        }

        private void InfoDownloaded(object sender, DownloadStringCompletedEventArgs downloadStringCompletedEventArgs)
        {
            progressBar1.Value = 100;
            var str = downloadStringCompletedEventArgs.Result;

            var serializer = new XmlSerializer(typeof(Info));
            using (var stream = new StringReader(str))
            {
                Info = serializer.Deserialize(stream) as Info;
            }

            if (Info.Version > Settings.Default.CurrentVersion) Update();
            else button1.Enabled = true;
        }

        void Update()
        {
            UpdateDownloader.DownloadFileCompleted += UpdateDownloadCompleted;
            ResetProgressBar();
            UpdateDownloader.DownloadFileAsync(new Uri(Info.DownloadUrl), DownloadFile.FullName);
        }

        private void UpdateDownloadCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            progressBar1.Value = 100;

            Extract();

            Settings.Default.CurrentVersion = Info.Version;
            Settings.Default.Save();
            button1.Enabled = true;
        }

        private void ResetProgressBar()
        {
            progressBar1.Value = 0;
        }

        private void webBrowser1_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            /*if (e.Url.AbsoluteUri != Settings.Default.NewsUrl)
            {
                e.Cancel = true;
                Process.Start(e.Url.ToString());
            }*/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start(TV_CD_DVD.FullName, "-console");
            Close();
        }

        void Extract()
        {
            using (ZipFile zip = ZipFile.Read(DownloadFile.FullName))
            {
                foreach (var file in zip)
                {
                    file.Extract(Root.FullName, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }
    }
}
