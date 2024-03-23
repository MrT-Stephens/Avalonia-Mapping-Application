using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Avalonia_Mapping_Application.Services
{
    class DataBaseService
    {
        private MongoClient? client;

        public DataBaseService()
        {
            client = null;
        }

        public void Connect(ref string email, ref string password)
        {
            var settings = MongoClientSettings.FromConnectionString(
                $"mongodb+srv://{email.Split('@')[0]}:{password}@mrt-stephens.x797uqs.mongodb.net/?retryWrites=true&w=majority&appName=MrT-Stephens");

            settings.ServerApi = new ServerApi(ServerApiVersion.V1);

            client = new MongoClient(settings);

            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("GHFHDFJFJDFJJJJJJJJJJDFJDFJFF");
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
