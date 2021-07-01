using StackExchange.Redis;

namespace Gaia.ConfigurationService
{
    /// <summary>
    /// Configuration service client, provides functions for get and set the value of configuration items.
    /// </summary>
    public class ConfigurationClient
    {
        /// <summary>
        /// Connection to the Redis server.
        /// </summary>
        private readonly IDatabase Database;
        /// <summary>
        /// Name of the configuration unit.
        /// </summary>
        private readonly string UnitName;
        
        /// <summary>
        /// Connect to the given Redis server and bind to the given configuration unit.
        /// It will create a configuration unit if the corresponding unit does not exist.
        /// </summary>
        /// <param name="unit_name">Name of the configuration unit.</param>
        /// <param name="port">Port of the Redis server.</param>
        /// <param name="ip">IP address of the Redis server.</param>
        public ConfigurationClient(string unit_name, uint port = 6379, string ip = "127.0.0.1")
        {
            UnitName = unit_name;
            
            var connection = ConnectionMultiplexer.Connect($"{ip}:{port.ToString()}");
            Database = connection.GetDatabase();
        }

        /// <summary>
        /// Get the value of a configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to get.</param>
        /// <returns>String value of the configuration item, or empty if it does not exist.</returns>
        public string Get(string item_name)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? result.ToString() : "";
        }

        /// <summary>
        /// Set the value of a configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="item_value">String value of the configuration item.</param>
        public void Set(string item_name, string item_value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", item_value);
        }
    }
}