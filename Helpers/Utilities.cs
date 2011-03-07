using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Reflection;

namespace FlickrGallery.Helpers
{
    public class Utilities
    {
        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes != null && attributes.Length > 0) 
                return attributes[0].Description;
            else 
                return value.ToString();
        }

        public static string GeneratePhotoUrl(string userId, string photoId)
        {
            string url = "http://www.flickr.com/photos/{0}/{1}";
            return string.Format(url, userId, photoId);
        }
    }
}