using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers;


namespace TMS.Libraries.EmailSegmentation.Segmentor.Segments
{
    public class BodySegment : EmailChunk
    {

        #region Init


        // for locking previous static collection
        private static Mutex mutex = new Mutex();

        //private BodySegment Origin;

        public BodySegment(EmailChunk parent, string originalHTML) : base(parent)//, originalHTML)
        {
            //Origin = origin;
            this.OriginalHTML = originalHTML;

            // In what below, we may modify same collection in parallel, so we need to mutex-lock it to prevent errors
            if (Factory.CheckForIdenticalBodySegments)
            {
                if (Factory.ProcessInParallel)
                    mutex.WaitOne();

                // calc hash
                SHA256 shaHash = SHA256.Create();
                var hash = GetSha256Hash(shaHash, Text);

                // look if this chunk obtained before
                this.BaseBodySegment = Factory.AllBodies.SingleOrDefault(c => c.Hash == hash);

                // if this is a new unique chunk, save hash
                if (this.BaseBodySegment == null)
                    Hash = hash;
            }


            Factory.AllBodies.Add(this);


            // unlock
            if (Factory.CheckForIdenticalBodySegments & Factory.ProcessInParallel)
                mutex.ReleaseMutex();

        }

        #endregion

        #region Properties

        internal string Hash { get; private set; }

        public BodySegment BaseBodySegment { get; private set; }

        private string _HTML;
        public string HTML
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_HTML))
                    _HTML = Cleaners.CleanHTML(this.OriginalHTML);


                return _HTML;
            }
        }

        private string _Text;
        public string Text
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_Text))
                    _Text = Cleaners.StripTextFromHTML(HTML);


                return _Text;
            }
        }



        // hide body from base
        private new BodySegment Body { get; set; }


        List<string> _EmailAddresses;
        public List<string> EmailAddresses
        {
            get
            {
                if (_EmailAddresses == null)
                    _EmailAddresses = InfoParsers.ParseEmailAddresses(this.HTML);

                return _EmailAddresses;
            }
        }

        List<string> _Phones;
        public List<string> InternationalPhones
        {
            get
            {
                if (_Phones == null)
                    _Phones = InfoParsers.ParseInternationalPhones(this.HTML);

                return _Phones;
            }
        }


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
