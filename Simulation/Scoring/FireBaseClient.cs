using FireSharp;
using FireSharp.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarSimulation.Scoring
{
    class FireBaseClient
    {
        private FirebaseClient client;

        public FireBaseClient(string secret, string url)
        {
            var config = new FirebaseConfig
            {
                AuthSecret = secret,
                BasePath = url
            };

            client = new FirebaseClient(config);
        }

        public async Task<T> SaveAsync<T>(string location, object data)
        {
            var response = await client.SetAsync(location, data);
            return response.ResultAs<T>();
        }

        public async Task<T> LoadAsync<T>(string location)
        {
            var response = await client.GetAsync(location);
            return response.ResultAs<T>();
        }
    }
}
