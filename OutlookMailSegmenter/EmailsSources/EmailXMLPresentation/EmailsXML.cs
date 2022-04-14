using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace TMS.Libraries.EmailsSources.XMLPresentation
{
    [Serializable]
    public class EmailsXML
    {

        #region Properties

        //[XmlElement]
        //public string FileFormatVersion { get; set; } = "1.2";

        //[XmlElement]
        //public string AppVersion { get; set; } = Assembly.GetEntryAssembly().GetName().Version.ToString();


        [XmlArray]
        public List<Email> Emails { get; set; } = new List<Email>();

        #endregion

        public static Base FindByID(Guid? ID)
        {
            if (ID == null)
                return null;

            return _Parts.SingleOrDefault(b => b.ID == ID);
        }


        private static List<Base> _Parts = new List<Base>();
        //public static List<Base> Parts => _Parts;

        internal static void Add(Base part)
        {
            _Parts.Add(part);
        }

    }
}
