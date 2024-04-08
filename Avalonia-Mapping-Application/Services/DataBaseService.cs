using Avalonia_Mapping_Application.DataModels;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia_Mapping_Application.Services
{
    class DataBaseService
    {
        // The MongoDB client which will be used to connect to the database
        private MongoClient? client;

        // The collection which will be used to interact with the database, this is the GeoCoordinates collection
        private IMongoCollection<GeoCoordinate>? collection;

        // Initializes the DataBaseService
        public DataBaseService()
        {
            client = null;
            collection = null;
        }

        // Connects to the database using the provided username and password
        public Task<bool> Connect(string username, string password)
        {
            return Task.Run(() =>
            {
                // The connection string which will be used to connect to the database, concatenates the username and password which are URL encoded
                var settings = MongoClientSettings.FromConnectionString(
                    $"mongodb+srv://{WebUtility.UrlEncode(username)}:{WebUtility.UrlEncode(password)}@mrt-stephens.x797uqs.mongodb.net/?retryWrites=true&w=majority&appName=MrT-Stephens");

                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                client = new MongoClient(settings);

                try
                {
                    // Gets the GeoCoordinates collection from the AvaloniaMappingApplication database
                    var database = client.GetDatabase("AvaloniaMappingApplication");
                    collection = database.GetCollection<GeoCoordinate>("GeoCoordinates");
                }
                catch (Exception)
                {
                    return false; // Returns false if the connection was unsuccessful
                }

                return true; // Returns true if the connection was successful
            });
        }

        // Gets all the GeoCoordinates from the database
        public async Task<List<GeoCoordinate>> GetGeoCoordinates()
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            return (await collection.FindAsync(Builders<GeoCoordinate>.Filter.Empty)).ToList();
        }

        // Adds a GeoCoordinate to the database
        public async void AddGeoCoordinate(GeoCoordinate geoCoordinate)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            await collection.InsertOneAsync(geoCoordinate);
        }

        public async Task AddManyGeoCoordinates(List<GeoCoordinate> geoCoordinates)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            await collection.InsertManyAsync(geoCoordinates);
        }

        // Updates a GeoCoordinate in the database, from the provided GeoCoordinate
        public async void UpdateGeoCoordinate(GeoCoordinate geoCoordinate)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            var filter = Builders<GeoCoordinate>.Filter.Eq("ID", geoCoordinate.ID);
            await collection.ReplaceOneAsync(filter, geoCoordinate);
        }

        // Deletes a GeoCoordinate from the database, from the provided ID
        public async void DeleteGeoCoordinate(string id)
        {
            if (collection == null)
            {
                throw new Exception("Database not connected");
            }

            var filter = Builders<GeoCoordinate>.Filter.Eq("ID", id);
            await collection.DeleteOneAsync(filter);
        }

        // Disconnects from the database
        public void Disconnect()
        {
            collection = null;
            client = null;
        }
    }
}
