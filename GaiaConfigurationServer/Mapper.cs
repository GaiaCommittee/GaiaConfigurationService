using System;
using System.IO;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.Text;
using StackExchange.Redis;

namespace Gaia.ConfigurationService
{
    /// <summary>
    /// Mapper will map data of all json files in a specific directory to Redis.
    /// </summary>
    public class Mapper : IDisposable
    {
        private readonly string DirectoryPath;

        private readonly IConnectionMultiplexer Connection;
        private readonly IServer Server;
        private readonly IDatabase Database;

        public bool LifeFlag;
        
        public Mapper(string path = "./Configurations", uint port = 6379, string ip = "127.0.0.1")
        {
            LifeFlag = true;
            DirectoryPath = path;
            Connection = ConnectionMultiplexer.Connect($"{ip}:{port.ToString()}");
            Server = Connection.GetServer(Connection.GetEndPoints()[0]);
            Database = Connection.GetDatabase();

            var subscriber= Connection.GetSubscriber();

            subscriber.Subscribe("configurations/command/load", (channel, value) =>
            {
                Load(value.ToString());
            });
            subscriber.Subscribe("configurations/command/save", (channel, value) =>
            {
                Save(value.ToString());
            });
            subscriber.Subscribe("configurations/command/stop", (channel, value) =>
            {
                LifeFlag = false;
            });
            subscriber.Subscribe("configurations/command/restart", (channel, value) =>
            {
                LifeFlag = false;
                throw new Exception(
                    "Restart command received, current thread has been aborted by this exception.");
            });
            
            LoadAll();
        }

        /// <summary>
        /// Save all configuration key-values when dispose.
        /// </summary>
        public void Dispose()
        {
            SaveAll();
        }

        /// <summary>
        /// Check whether this mapper is connected to the Redis server or not.
        /// </summary>
        /// <returns>Connected or not.</returns>
        public bool IsConnected => Connection.IsConnected;

        /// <summary>
        /// Load a configuration unit from JSON file into Redis database.
        /// </summary>
        /// <param name="configuration_name">The name of the configuration to load, without file postfix.</param>
        private void Load(string configuration_name)
        {
            var json_path = Path.Combine(DirectoryPath, configuration_name + ".json");
            if (!File.Exists(json_path)) return;

            using var reader = new StreamReader(json_path);
            var document = JsonObject.Parse(reader.ReadToEnd());
            foreach (var pair in document)
            {
                Database.StringSet($"configurations/{configuration_name}/{pair.Key}", pair.Value);
            }
        }

        /// <summary>
        /// Save a configuration unit from Redis database into the correspondi
        /// </summary>
        /// <param name="configuration_name"></param>
        private void Save(string configuration_name)
        {
            // Scan all keys of this configuration unit.
            var keys = Server.Keys(pattern: $"configurations/{configuration_name}/*");
            
            // Construct the JSON configuration document.
            var document = new JsonObject();
            foreach (var key in keys)
            {
                document.Add(key.ToString().Split("/")[^1], Database.StringGet(key));
            }
            
            // Save the JSON document into a file.
            var json_path = Path.Combine(DirectoryPath, configuration_name + ".json");
            using var writer = new StreamWriter(json_path);
            writer.Write(document.ToJson());
            writer.Close();
        }

        /// <summary>
        /// Load all JSON files in the configuration folder.
        /// </summary>
        private void LoadAll()
        {
            var directory_data = new DirectoryInfo(DirectoryPath);
            foreach (var file in directory_data.GetFiles("*.json"))
            {
                Load(file.Name.Replace(".json", ""));
            }
        }

        /// <summary>
        /// Save all configuration key-value pairs into JSON files.
        /// </summary>
        private void SaveAll()
        {
            // Query all configuration key-value pairs.
            var keys = Server.Keys(pattern: $"configurations/*");
            // Classify keys into documents.
            var documents = new Dictionary<string, JsonObject>();
            foreach (var key in keys)
            {
                if (!documents.ContainsKey(key))
                {
                    documents.Add(key, new JsonObject());
                }

                var key_parts = key.ToString().Split("/");
                if (key_parts.Length < 3) continue;
                documents[key].Add(key_parts[^1], Database.StringGet(key));
            }
            // Save all documents.
            foreach (var pair in documents)
            {
                // Save the JSON document into a file.
                var key_parts = pair.Key.Split("/");
                if (key_parts.Length < 3) continue;
                var json_path = Path.Combine(DirectoryPath, key_parts[^2] + ".json");
                using var writer = new StreamWriter(json_path);
                writer.Write(pair.Value.ToJson());
                writer.Close();
            }
        }
    }
}