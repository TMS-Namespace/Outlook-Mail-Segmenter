using System.Collections.Generic;

namespace TMS.Libraries.EmailSegmentation.SegmentationEngineCore.Helpers
{
    public static class KeyWords
    {
        #region Fields

        // From, to etc... keywords in different languages
        public static string fromKW = string.Join("|", new List<string>() { "from", "от", "من" });
        public static string toKW = string.Join("|", new List<string>() { "to", "Кому", "إلى" });
        public static string ccKW = string.Join("|", new List<string>() { "cc", "Копия", "نسخة" });
        public static string subjectKW = string.Join("|", new List<string>() { "subject", "Тема", "الموضوع" });
        public static string sentKW = string.Join("|", new List<string>() { "sent", "date", "Отправлено", "التاريخ" });
        public static string importantKW = string.Join("|", new List<string>() { "importance", "Важность", "من" });

        public static string closingKW = string.Join("|", new List<string>() { "best regards", "sincerely yours", "с уважением", "Regards" });

        #endregion

    }
}
