using System.Collections.Generic;
using FlickrGallery.Models;
using Orchard;

namespace FlickrGallery.Contracts.Services
{
    public interface IFlickrPhotoRetrievalService
        : IDependency
    {
        List<FlickrPhoto> GetPhotos(FlickrGalleryWidgetPart part);
    }
}