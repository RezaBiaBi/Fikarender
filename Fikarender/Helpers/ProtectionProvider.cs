using Microsoft.AspNetCore.DataProtection;
using System;
using System.Text;

public interface IProtectionProvider
{
    string Decrypt(string inputText);
    string Encrypt(string inputText);
}

namespace Helpers
{
    public class ProtectionProvider : IProtectionProvider
    {
        private readonly IDataProtector _dataProtector;

        public ProtectionProvider(IDataProtectionProvider dataProtectionProvider)
        {
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(ProtectionProvider).FullName);
        }

        public string Decrypt(string inputText)
        {
            var inputBytes = Convert.FromBase64String(inputText);
            var bytes = _dataProtector.Unprotect(inputBytes);
            return Encoding.UTF8.GetString(bytes);
        }

        public string Encrypt(string inputText)
        {
            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var bytes = _dataProtector.Protect(inputBytes);
            return Convert.ToBase64String(bytes);
        }
    }
}