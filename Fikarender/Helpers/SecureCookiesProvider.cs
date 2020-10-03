using Microsoft.AspNetCore.Http;
using System;

public interface ISecureCookiesProvider
{
    void Add(HttpContext context, string token, string value, DateTimeOffset expire, bool isEssential);
    bool Contains(HttpContext context, string token);
    string GetValue(HttpContext context, string token);
    void Remove(HttpContext context, string token);
}

namespace Helpers
{
    public class SecureCookiesProvider : ISecureCookiesProvider
    {
        private readonly IProtectionProvider _protectionProvider;

        public SecureCookiesProvider(IProtectionProvider protectionProvider)
        {
            _protectionProvider = protectionProvider;
        }

        public void Add(HttpContext context, string token, string value, DateTimeOffset expire, bool isEssential)
        {
            value = _protectionProvider.Encrypt(value);
            context.Response.Cookies.Append(token, value, getCookieOptions(context, expire, isEssential));
        }

        public bool Contains(HttpContext context, string token)
        {
            return context.Request.Cookies.ContainsKey(token);
        }

        public string GetValue(HttpContext context, string token)
        {
            string cookieValue;
            if (!context.Request.Cookies.TryGetValue(token, out cookieValue))
            {
                return null;
            }
            return _protectionProvider.Decrypt(cookieValue);
        }

        public void Remove(HttpContext context, string token)
        {
            if (context.Request.Cookies.ContainsKey(token))
            {
                context.Response.Cookies.Delete(token);
            }
        }

        /// <summary>
        /// Expires at the end of the browser's session.
        /// </summary>
        private CookieOptions getCookieOptions(HttpContext context, DateTimeOffset expire, bool isEssential)
        {
            return new CookieOptions
            {
                IsEssential = true,
                Expires = expire,
                HttpOnly = true,
                Path = context.Request.PathBase.HasValue ? context.Request.PathBase.ToString() : "/",
                Secure = context.Request.IsHttps
            };
        }
    }
}
