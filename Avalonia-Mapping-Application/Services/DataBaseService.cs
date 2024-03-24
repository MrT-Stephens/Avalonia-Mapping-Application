using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Avalonia_Mapping_Application.Services
{
    class DataBaseService
    {
        private MongoClient? client;

        public DataBaseService()
        {
            client = null;
        }

        public Task<bool> Connect(string email, string password)
        {
            return Task.Run(() =>
            {
                var settings = MongoClientSettings.FromConnectionString(
                    $"mongodb+srv://{WebUtility.UrlEncode(email.Split('@')[0])}:{WebUtility.UrlEncode(password)}@mrt-stephens.x797uqs.mongodb.net/?retryWrites=true&w=majority&appName=MrT-Stephens");

                settings.ServerApi = new ServerApi(ServerApiVersion.V1);

                client = new MongoClient(settings);

                try
                {
                    var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                }
                catch (Exception)
                {
                    return false;
                }

                return true;
            });
        }

        public void Disconnect()
        {
            client = null;
        }
    }
}
