﻿using System;
using System.Xml.Serialization;
using TMS.Libraries.EmailSegmentation.Segmentor;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    public class Base
    {

        #region Init

        public Base() { }

        public Base(EmailChunk origin)
        {
            this.Origin = origin;

            if (origin.Body != null)
                this.Body = new Body(origin.Body);

            this.ID = origin.ID;
            this.ParentID = origin.Parent?.ID;

        }

        #endregion

        #region Properties

        private Body _Body ;
        public Body Body { get {

                if (Origin != null && _Body==null)
                    _Body = new Body( Origin.Body);

                return _Body;

            } set { _Body = value; } }

        private Guid _ID;
        public Guid ID
        {
            get
            {

                if (Origin != null && _ID == Guid.Empty)
                    _ID = Origin.ID;

                return _ID;

            }
            set { _ID = value; }
        }

        public Guid? ParentID { get; set; }

        [XmlIgnore]
        public Base Parent => AllEmailParts.FindByID(this.ParentID);

        [XmlIgnore]
        public EmailChunk Origin { get; set; }
        #endregion

    }
}
