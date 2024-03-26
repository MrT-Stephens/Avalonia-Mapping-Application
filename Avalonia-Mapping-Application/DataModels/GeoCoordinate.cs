using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Avalonia_Mapping_Application.DataModels
{
    public class GeoCoordinate
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Latitude")]
        public double Latitude { get; set; }

        [BsonElement("Longitude")]
        public double Longitude { get; set; }
    }
}
