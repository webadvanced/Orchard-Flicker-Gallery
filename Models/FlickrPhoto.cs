namespace FlickrGallery.Models
{
    public class FlickrPhoto
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string OwnerName { get; set; }
        public string Secret { get; set; }
        public string Server { get; set; }
        public string Farm { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPublic { get; set; }
        public bool IsFriend { get; set; }
        public bool IsFamily { get; set; }
        public bool HasComment { get; set; }

        // SQUARE 75x75
        public string Url_SQ { get; set; }
        public int Height_SQ { get; set; }
        public int Width_SQ { get; set; }

        // THUMBNAIL 100 on longest side
        public string Url_T { get; set; }
        public int Height_T { get; set; }
        public int Width_T { get; set; }

        // SMALL 240 on longest side
        public string Url_S { get; set; }
        public int Height_S { get; set; }
        public int Width_S { get; set; }

        // MEDIUM 500 on longest side
        public string Url_M { get; set; }
        public int Height_M { get; set; }
        public int Width_M { get; set; }

        // MEDIUM 640 on longest side
        public string Url_Z { get; set; }
        public int Height_Z { get; set; }
        public int Width_Z { get; set; }

        // LARGE 1024 on longest side
        public string Url_L { get; set; }
        public int Height_L { get; set; }
        public int Width_L { get; set; }

        // ORIGINAL original image, either a jpg, gif or png, depending on source format
        public string Url_O { get; set; }
        public int Height_O { get; set; }
        public int Width_O { get; set; }

    }
}