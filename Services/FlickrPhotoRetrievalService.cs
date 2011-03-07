using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Xml;
using System.Xml.Linq;
using JetBrains.Annotations;
using FlickrGallery.Contracts.Services;
using FlickrGallery.Models;
using Orchard.Caching;
using System.Threading;

namespace FlickrGallery.Services
{
    [UsedImplicitly]
    public class FlickrPhotoRetrievalService
        : IFlickrPhotoRetrievalService
    {
        string FlickrAPIKey = "8f3c6f6cae93c660bf41cd8ef37ca6b0";
        string FlickrAPISecret = "2c1e1e2b0972b9d6";
        string FlickrAPIUrl = "http://api.flickr.com/services/rest/?method={0}&api_key={1}";


        protected readonly string CacheKeyPrefix = "flickrPhotos_";
        protected ICacheManager CacheManager { get; private set; }
        protected ISignals Signals { get; private set; }
        protected Timer Timer { get; private set; }

        public FlickrPhotoRetrievalService(ICacheManager cacheManager, ISignals signals)
        {
            this.CacheManager = cacheManager;
            this.Signals = signals;
        }

        public List<FlickrPhoto> GetPhotos(FlickrGalleryWidgetPart part)
        {
            if(part.DisableCaching)
            {
                return RetrievePhotos(part).Take(part.MaxImages).ToList();
            }
            else
            {
                var cacheKey = CacheKeyPrefix + (int)part.Mode;
                switch (part.Mode)
                {
                    case GalleryMode.MostRecent:
                        break;
                    case GalleryMode.PhotoSet:
                        cacheKey += part.PhotoSetId;
                        break;
                    case GalleryMode.Gallery:
                        cacheKey += part.GalleryId;
                        break;
                    case GalleryMode.PhotosOfUser:
                        cacheKey += part.PhotosOfUserId;
                        break;
                    case GalleryMode.PhotosUploadedByUser:
                        cacheKey += part.PhotosUploadedByUserId;
                        break;
                    case GalleryMode.GroupPhotos:
                        cacheKey += part.GroupId;
                        break;
                    case GalleryMode.SearchTags:
                        cacheKey += part.Tags;
                        break;
                    default:
                        break;
                }

                return CacheManager.Get(cacheKey, ctx =>
                {
                    ctx.Monitor(Signals.When(cacheKey));
                    Timer = new Timer(t => Signals.Trigger(cacheKey), part, TimeSpan.FromMinutes(part.CacheDuration), TimeSpan.FromMilliseconds(-1));
                    return RetrievePhotos(part).Take(part.MaxImages).ToList();
                });
            }
        }

        private List<FlickrPhoto> RetrievePhotos(FlickrGalleryWidgetPart part)
        {
            List<FlickrPhoto> photos;
            switch (part.Mode)
            {
                case GalleryMode.MostRecent:
                    photos = GetAllRecent();
                    break;
                case GalleryMode.PhotoSet:
                    photos = GetPhotosOfPhotoSet(part.PhotoSetId);
                    break;
                case GalleryMode.Gallery:
                    photos = GetPhotosOfGallery(part.GalleryId);
                    break;
                case GalleryMode.PhotosOfUser:
                    photos = GetPhotosOfUser(part.PhotosOfUserId);
                    break;
                case GalleryMode.PhotosUploadedByUser:
                    photos = GetUserPublicPhotos(part.PhotosUploadedByUserId);
                    break;
                case GalleryMode.GroupPhotos:
                    photos = GetPhotosOfGroup(part.GroupId);
                    break;
                case GalleryMode.SearchTags:
                    photos = GetPhotosByTags(part.Tags);
                    break;
                default:
                    photos = new List<FlickrPhoto>();
                    break;
            }

            return photos.ToList();
        }

        private List<FlickrPhoto> GetAllRecent()
        {
            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.photos.getRecent", new Dictionary<string, string>());

            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetPhotosOfPhotoSet(string photoSetId)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("photoset_id", photoSetId);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.photosets.getPhotos", AdditionalQueryStringParameters);

            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetPhotosOfGallery(string galleryId)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("gallery_id", galleryId);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.galleries.getPhotos", AdditionalQueryStringParameters);
            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetPhotosOfUser(string userId)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("user_id", userId);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.people.getPhotosOf", AdditionalQueryStringParameters);

            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetUserPublicPhotos(string userId)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("user_id", userId);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.people.getPublicPhotos", AdditionalQueryStringParameters);

            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetPhotosOfGroup(string groupId)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("group_id", groupId);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.groups.pools.getPhotos", AdditionalQueryStringParameters);

            return DeserializePhotos(response);
        }

        private List<FlickrPhoto> GetPhotosByTags(string tags)
        {
            var AdditionalQueryStringParameters = new Dictionary<string, string>();
            AdditionalQueryStringParameters.Add("tags", tags);

            string response = GetResponseFromFlickr(FlickrAPIUrl, FlickrAPIKey, "flickr.photos.search", AdditionalQueryStringParameters);
            return DeserializePhotos(response);
        }

        private string GetResponseFromFlickr(string flickApiUrl, string flickrApiKey, string method, Dictionary<string, string> additionalQueryStringParameters)
        {
            var extras = "url_sq,url_z,description,owner_name";
            var flickrClient = new WebClient();
            var request = new System.Text.StringBuilder();

            request.Append(string.Format(flickApiUrl, method, flickrApiKey) + "&extras=" + extras);
            foreach (KeyValuePair<string, string> kvp in additionalQueryStringParameters)
                request.Append("&" + kvp.Key + "=" + kvp.Value);

            return flickrClient.DownloadString(request.ToString());
        }

        private List<FlickrPhoto> DeserializePhotos(string response)
        {
            var photos = new List<FlickrPhoto>();

            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(response);

            var xmlPhotos = xmlDoc.GetElementsByTagName("photo");
            for (int i = 0; i < xmlPhotos.Count; i++)
            {
                var xmlPhoto = xmlPhotos.Item(i);
                var Photo = new FlickrPhoto();

                Photo.Id = xmlPhoto.Attributes["id"] != null ? xmlPhoto.Attributes["id"].Value : "";
                Photo.Owner = xmlPhoto.Attributes["owner"] != null ? xmlPhoto.Attributes["owner"].Value : "";
                Photo.OwnerName = xmlPhoto.Attributes["ownername"] != null ? xmlPhoto.Attributes["ownername"].Value : "";
                Photo.Secret = xmlPhoto.Attributes["secret"] != null ? xmlPhoto.Attributes["secret"].Value : "";
                Photo.Server = xmlPhoto.Attributes["server"] != null ? xmlPhoto.Attributes["server"].Value : "";
                Photo.Farm = xmlPhoto.Attributes["farm"] != null ? xmlPhoto.Attributes["farm"].Value : "";
                Photo.Title = xmlPhoto.Attributes["title"] != null ? xmlPhoto.Attributes["title"].Value : "";
                Photo.Description = xmlPhoto.Attributes["description"] != null ? xmlPhoto.Attributes["description"].Value : "";
                Photo.IsPrimary = xmlPhoto.Attributes["isprimary"] != null ? xmlPhoto.Attributes["isprimary"].Value == "1" ? true : false : false;
                Photo.IsPublic = xmlPhoto.Attributes["ispublic"] != null ? xmlPhoto.Attributes["ispublic"].Value == "1" ? true : false : false;
                Photo.IsFriend = xmlPhoto.Attributes["isfriend"] != null ? xmlPhoto.Attributes["isfriend"].Value == "1" ? true : false : false;
                Photo.IsFamily = xmlPhoto.Attributes["isfamily"] != null ? xmlPhoto.Attributes["isfamily"].Value == "1" ? true : false : false;
                Photo.HasComment = xmlPhoto.Attributes["has_comment"] != null ? xmlPhoto.Attributes["has_comment"].Value == "1" ? true : false : false;

                Photo.Url_SQ = xmlPhoto.Attributes["url_sq"] != null ? xmlPhoto.Attributes["url_sq"].Value : "";
                Photo.Height_SQ = xmlPhoto.Attributes["height_sq"] != null ? Int32.Parse(xmlPhoto.Attributes["height_sq"].Value) : 0;
                Photo.Width_SQ = xmlPhoto.Attributes["width_sq"] != null ? Int32.Parse(xmlPhoto.Attributes["width_sq"].Value) : 0;

                Photo.Url_T = xmlPhoto.Attributes["url_t"] != null ? xmlPhoto.Attributes["url_t"].Value : "";
                Photo.Height_T = xmlPhoto.Attributes["height_t"] != null ? Int32.Parse(xmlPhoto.Attributes["height_t"].Value) : 0;
                Photo.Width_T = xmlPhoto.Attributes["width_t"] != null ? Int32.Parse(xmlPhoto.Attributes["width_t"].Value) : 0;

                Photo.Url_S = xmlPhoto.Attributes["url_s"] != null ? xmlPhoto.Attributes["url_s"].Value : "";
                Photo.Height_S = xmlPhoto.Attributes["height_s"] != null ? Int32.Parse(xmlPhoto.Attributes["height_s"].Value) : 0;
                Photo.Width_S = xmlPhoto.Attributes["width_s"] != null ? Int32.Parse(xmlPhoto.Attributes["width_s"].Value) : 0;

                Photo.Url_M = xmlPhoto.Attributes["url_m"] != null ? xmlPhoto.Attributes["url_m"].Value : "";
                Photo.Height_M = xmlPhoto.Attributes["height_m"] != null ? Int32.Parse(xmlPhoto.Attributes["height_m"].Value) : 0;
                Photo.Width_M = xmlPhoto.Attributes["width_m"] != null ? Int32.Parse(xmlPhoto.Attributes["width_m"].Value) : 0;

                Photo.Url_Z = xmlPhoto.Attributes["url_z"] != null ? xmlPhoto.Attributes["url_z"].Value : "";
                Photo.Height_Z = xmlPhoto.Attributes["height_z"] != null ? Int32.Parse(xmlPhoto.Attributes["height_z"].Value) : 0;
                Photo.Width_Z = xmlPhoto.Attributes["width_z"] != null ? Int32.Parse(xmlPhoto.Attributes["width_z"].Value) : 0;

                Photo.Url_L = xmlPhoto.Attributes["url_l"] != null ? xmlPhoto.Attributes["url_l"].Value : "";
                Photo.Height_L = xmlPhoto.Attributes["height_l"] != null ? Int32.Parse(xmlPhoto.Attributes["height_l"].Value) : 0;
                Photo.Width_L = xmlPhoto.Attributes["width_l"] != null ? Int32.Parse(xmlPhoto.Attributes["width_l"].Value) : 0;

                Photo.Url_O = xmlPhoto.Attributes["url_o"] != null ? xmlPhoto.Attributes["url_o"].Value : "";
                Photo.Height_O = xmlPhoto.Attributes["height_o"] != null ? Int32.Parse(xmlPhoto.Attributes["height_o"].Value) : 0;
                Photo.Width_O = xmlPhoto.Attributes["width_o"] != null ? Int32.Parse(xmlPhoto.Attributes["width_o"].Value) : 0;

                photos.Add(Photo);
            }

            return photos;
        }
    }
}
