using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System.ComponentModel;

namespace FlickrGallery.Models
{
    public class FlickrGalleryWidgetRecord : ContentPartRecord
    {
        public virtual int MaxImages { get; set; }
        public virtual int Mode { get; set; }
        public virtual string GalleryId { get; set; }
        public virtual string PhotoSetId { get; set; }
        public virtual string PhotosOfUserId { get; set; }
        public virtual string PhotosUploadedByUserId { get; set; }
        public virtual string GroupId { get; set; }
        public virtual string Tags { get; set; }
    }

    public enum GalleryMode : int
    {
        [Description("Most Recent")]
        MostRecent = 1,
        [Description("Photo Set")]
        PhotoSet = 2,
        [Description("Gallery")]
        Gallery = 3,
        [Description("Group Photos")]
        GroupPhotos = 4,
        [Description("Photos Of User")]
        PhotosOfUser = 5,
        [Description("Photos Uploaded by User")]
        PhotosUploadedByUser = 6,
        [Description("Search Tags")]
        SearchTags = 7,
       
    }

    public class FlickrGalleryWidgetPart : ContentPart<FlickrGalleryWidgetRecord>
    {
        [Required]
        [DisplayName("Source of images")]
        public GalleryMode Mode
        {
            get { return (GalleryMode)Record.Mode; }
            set { Record.Mode = (int)value; }
        }


        [Required]
        [DefaultValue(10)]
        [DisplayName("Number of Images to display (Max 100)")]
        public int MaxImages
        {
            get { return Record.MaxImages; }
            set { Record.MaxImages = value; }
        }

        [DisplayName("Gallery Id")]
        public string GalleryId
        {
            get { return Record.GalleryId; }
            set { Record.GalleryId = value; }
        }

        [DisplayName("Photo Set Id")]
        public string PhotoSetId
        {
            get { return Record.PhotoSetId; }
            set { Record.PhotoSetId = value; }
        }

        [DisplayName("Group Id")]
        public string GroupId
        {
            get { return Record.GroupId; }
            set { Record.GroupId = value; }
        }

        [DisplayName("User Id (NSID)")]
        public string PhotosOfUserId
        {
            get { return Record.PhotosOfUserId; }
            set { Record.PhotosOfUserId = value; }
        }

        [DisplayName("User Id (NSID)")]
        public string PhotosUploadedByUserId
        {
            get { return Record.PhotosUploadedByUserId; }
            set { Record.PhotosUploadedByUserId = value; }
        }

        [DisplayName("Tags (use a comma to separate multiple tags)")]
        public string Tags
        {
            get { return Record.Tags; }
            set { Record.Tags = value; }
        }
    }
}