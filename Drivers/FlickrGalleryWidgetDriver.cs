using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using FlickrGallery.Models;
using FlickrGallery.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using System.Net;
using System.Web.Caching;
using FlickrGallery.Contracts.Services;

namespace FlickrGallery.Drivers
{
    public class FlickrGalleryWidgetDriver 
        : ContentPartDriver<FlickrGalleryWidgetPart>
    {
        protected IFlickrPhotoRetrievalService FlickrPhotoRetrievalService { get; private set; }

        public FlickrGalleryWidgetDriver(IFlickrPhotoRetrievalService flickrPhotoRetrievalService)
        {
            this.FlickrPhotoRetrievalService = flickrPhotoRetrievalService;
        }

        // GET
        protected override DriverResult Display(
            FlickrGalleryWidgetPart part, string displayType, dynamic shapeHelper)
        {
            return ContentShape("Parts_FlickrGalleryWidget",
                () => shapeHelper.Parts_FlickrGalleryWidget(
                    Photos: FlickrPhotoRetrievalService.GetPhotos(part),
                    MaxImages: part.MaxImages,
                    Mode: part.Mode
                    ));
        }

        // GET
        protected override DriverResult Editor(FlickrGalleryWidgetPart part, dynamic shapeHelper)
        {
            return ContentShape("Parts_FlickrGalleryWidget_Edit",
                () => shapeHelper.EditorTemplate(
                    TemplateName: "Parts/FlickrGalleryWidget",
                    Model: part,
                    Prefix: Prefix));
        }

        // POST
        protected override DriverResult Editor(
            FlickrGalleryWidgetPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            HttpContext.Current.Response.Write("POST: " + part.Mode);
            updater.TryUpdateModel(part, Prefix, null, null);
            return Editor(part, shapeHelper);
        }
    }
}