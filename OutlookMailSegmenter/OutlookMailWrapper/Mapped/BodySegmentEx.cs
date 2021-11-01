using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

using TMS.Libraries.ClassicalEmailSegmenter;
using TMS.Libraries.OutlookMailWrapper.Helpers;

namespace TMS.Libraries.OutlookMailWrapper
{
    public class BodySegmentEx : BaseSegmentEx
    {

        #region Init


        // for locking previous static collection
        private static Mutex mutex = new Mutex();

        private BodySegment Origin;

        public BodySegmentEx(BodySegment origin, IEmailPart parent) : base(origin, parent)
        {
            Origin = origin;

            // In what below, we may modify same collection in parallel, so we need to mutex-lock it to prevent errors
            if (Outlook.CheckForIdenticalBodySegments)
            {
                if (Outlook.ProcessInParallel)
                    mutex.WaitOne();

                // calc hash
                SHA256 shaHash = SHA256.Create();
                var hash = GetSha256Hash(shaHash, Origin.Text);

                // look if this chunk obtained before
                this.BaseBodySegment = Outlook.AllBodies.SingleOrDefault(c => c.Hash == hash);

                // if this is a new unique chunk, save hash
                if (this.BaseBodySegment == null)
                    Hash = hash;
            }


            Outlook.AllBodies.Add(this);


            // unlock
            if (Outlook.CheckForIdenticalBodySegments & Outlook.ProcessInParallel)
                mutex.ReleaseMutex();

        }

        #endregion

        #region Properties

        internal string Hash { get; private set; }

        public BodySegmentEx BaseBodySegment { get; private set; }

        public string HTML => (BaseBodySegment == null) ? Origin.HTML : null;

        public string Text => (BaseBodySegment == null) ? Origin.Text : null;


        // hide body from base
        private new BodySegmentEx Body { get; set; }


        public List<string> EmailAddresses => (BaseBodySegment == null) ? Origin.EmailAddresses : null;

        public List<string> InternationalPhones => (BaseBodySegment == null) ? Origin.InternationalPhones : null;


        // the collection that will hold unique base chunks, that will be referenced by any repeated email part, like repeated signatures
        //internal static List<BodySegmentEx> BaseBodySegments;

        #endregion

        #region Help methods

        private string GetSha256Hash(SHA256 shaHash, string input)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        #endregion

    }
}
