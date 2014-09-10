using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeoLauncher
{
    [Serializable]
    public class Info
    {
        [System.Xml.Serialization.XmlElement("Version")]
        public int Version { get; set; }

        [System.Xml.Serialization.XmlElement("DownloadUrl")]
        public string DownloadUrl { get; set; }
    }
}
