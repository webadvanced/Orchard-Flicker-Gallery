using System.Data;
using FlickrGallery.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace FlickrGallery {
    public class Migrations : DataMigrationImpl {

        public int Create() {
            // Creating table FlickrGalleryWidgetRecord
            SchemaBuilder.CreateTable("FlickrGalleryWidgetRecord", table => table
				.ContentPartRecord()
                .Column("MaxImages", DbType.Int32)
                .Column("Mode", DbType.Int32)
                .Column("GalleryID", DbType.String)
                .Column("PhotoSetId", DbType.String)
                .Column("GroupId", DbType.String)
                .Column("Tags", DbType.String)
                .Column("PhotosOfUserId", DbType.String)
                .Column("PhotosUploadedByUserId", DbType.String)
			);

            ContentDefinitionManager.AlterPartDefinition(typeof(FlickrGalleryWidgetPart).Name,
                builder => builder.Attachable());

            ContentDefinitionManager.AlterTypeDefinition("FlickrGalleryWidget", cfg => cfg
                .WithPart("FlickrGalleryWidgetPart")
                .WithPart("WidgetPart")
                .WithPart("CommonPart")
                .WithSetting("Stereotype", "Widget"));
            return 1;
        }

        public int UpdateFrom1() {
            return 2;
        }
    }
}