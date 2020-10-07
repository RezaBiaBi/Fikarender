using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace Helpers
{
    public static class ImageValidator
    {
        public static bool IsImage(this IFormFile courseImageFile)
        {
            try
            {
                var image = System.Drawing.Image.FromStream(courseImageFile.OpenReadStream());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
