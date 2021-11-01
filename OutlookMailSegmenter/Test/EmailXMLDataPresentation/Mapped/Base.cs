using System;
using System.Xml.Serialization;

using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    public class Base
    {

        #region Init

        public Base() { }

        public Base(IEmailPart origin)
        {

            if (origin.Body != null)
                this.Body = new Body(origin.Body);

            this.ID = origin.ID;
            this.ParentID = origin.Parent?.ID;

        }

        #endregion

        #region Properties

        public Body Body { get; set; }

        public Guid ID { get; set; }

        public Guid? ParentID { get; set; }

        [XmlIgnore]
        public Base Parent => AllEmailParts.FindByID(this.ParentID);

        #endregion

    }
}
