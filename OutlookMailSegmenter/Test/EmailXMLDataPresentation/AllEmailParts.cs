using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TMS.Libraries.EmailXMLDataPresentation
{
    public static class AllEmailParts
    {
        public static Base FindByID(Guid? ID)
        {
            if (ID == null)
                return null;

            return _Parts.SingleOrDefault(b => b.ID == ID);
        }


        private static List<Base> _Parts = new List<Base>();
        //public static List<Base> Parts => _Parts;

        public static void Add(Base part)
        {
            _Parts.Add(part);
        }

    }
}
