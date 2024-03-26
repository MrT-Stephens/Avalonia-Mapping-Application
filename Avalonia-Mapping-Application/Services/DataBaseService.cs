using Avalonia_Mapping_Application.DataModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia_Mapping_Application.Services
{
    class DataBaseService
    {
        private MongoClient? client;
        private IMongoCollection<GeoCoordinate>? collection;

        public DataBaseService()
        {
            client = null;
            collection = null;
        }

        public Task<bool> Connect(string username, string password)
        {
            return Task.Run(() =>
            {
                var settings = MongoClientSettings.FromConnectionString(
                    $"mongodb+srv://{WebUtility.UrlEncode(username)}:{WebUtility.UrlEncode(password)}@mrt-stephens.x797uqs.mongodb.net/?retryWrites=true&w=majority&appName=MrT-Stephens");

                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                client = new MongoClient(settings);

                try
                {
                    var database = client.GetDatabase("AvaloniaMappingApplication");
                    collection = database.GetCollection<GeoCoordinate>("GeoCoordinates");
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            });
        }

        public async Task<List<GeoCoordinate>> GetGeoCoordinates()
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            return (await collection.FindAsync(Builders<GeoCoordinate>.Filter.Empty)).ToList();
        }

        public async void AddGeoCoordinate(GeoCoordinate geoCoordinate)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            await collection.InsertOneAsync(geoCoordinate);
        }

        public async void UpdateGeoCoordinate(GeoCoordinate geoCoordinate)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            var filter = Builders<GeoCoordinate>.Filter.Eq("ID", geoCoordinate.ID);
            await collection.ReplaceOneAsync(filter, geoCoordinate);
        }

        public async void DeleteGeoCoordinate(string id)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            var filter = Builders<GeoCoordinate>.Filter.Eq("ID", id);
            await collection.DeleteOneAsync(filter);
        }

        public void Disconnect()
        {
            collection = null;
            client = null;
        }
    }
}
