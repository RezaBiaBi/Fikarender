using Fikarender.Models;
using Fikarender.Data;
using System;
using System.Linq;

namespace Helpers
{
    public interface ILinkTools
    {
        string GenerateShortLink(int? length, ShortLinkType type);
        string EncodePersianLink(string text);
        string DecodePersianLink(string text);
    }

    public class LinkTools : ILinkTools
    {
        public string GenerateShortLink(int? length, ShortLinkType type)
        {
            if (!length.HasValue)
            {
                length = 5;
            }
            string key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, length.Value);
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                while (db.ShortLink.Any(a => a.ShortKey.Equals(key)))
                {
                    key = Guid.NewGuid().ToString().Replace("-", "").Substring(0, length.Value);
                }
            }
            return key;
        }

        public string EncodePersianLink(string text)
        {
            return text.Trim().Replace(' ', '-');
        }

        public string DecodePersianLink(string text)
        {
            return text.Trim().Replace('-', ' ');
        }
    }
}
