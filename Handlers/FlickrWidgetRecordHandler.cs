using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Orchard.ContentManagement.Handlers;
using Orchard.Data;
using FlickrGallery.Models;

namespace FlickrGallery.Handlers
{
    public class FlickrGalleryWidgetRecordHandler : ContentHandler
    {
        public FlickrGalleryWidgetRecordHandler(IRepository<FlickrGalleryWidgetRecord> repository)
        {
            Filters.Add(StorageFilter.For(repository));
        }
    }
}