using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Serialization;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    [Serializable]
    public class EmailsData
    {

        #region Properties

        //[XmlElement]
        //public string FileFormatVersion { get; set; } = "1.2";

        //[XmlElement]
        //public string AppVersion { get; set; } = Assembly.GetEntryAssembly().GetName().Version.ToString();


        [XmlArray]
        public List<Email> Emails { get; set; } = new List<Email>();

        #endregion

    }
}
