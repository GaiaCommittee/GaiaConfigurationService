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
        /// Subscriber to the channels of Redis server.
        /// </summary>
        private readonly ISubscriber Subscriber;
        
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
        public ConfigurationClient(string unit_name, uint port = 6379, string ip = "127.0.0.1") :
            this(unit_name, ConnectionMultiplexer.Connect($"{ip}:{port.ToString()}"))
        {}

        /// <summary>
        /// Reuse the connection to a Redis server.
        /// </summary>
        /// <param name="unit_name">Name of the configuration unit to bind.</param>
        /// <param name="connection">Connection to the Redis server.</param>
        public ConfigurationClient(string unit_name, IConnectionMultiplexer connection)
        {
            UnitName = unit_name;
            Database = connection.GetDatabase();
            Subscriber = connection.GetSubscriber();
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
        /// Get the value of a configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to get.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>String value of the configuration item, or default value if it does not exist.</returns>
        public string Get(string item_name, string default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? result.ToString() : default_value;
        }

        /// <summary>
        /// Get the value of a integer configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>Value of the item or default value if the item is empty.</returns>
        public int GetInteger(string item_name, int default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? (int)result : default_value;
        }
        
        /// <summary>
        /// Get the value of a long integer configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>Value of the item or default value if the item is empty.</returns>
        public long GetLong(string item_name, long default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? (long)result : default_value;
        }

        /// <summary>
        /// Get the value of a float configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>Value of the item or default value if the item is empty.</returns>
        public double GetFloat(string item_name, double default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? (double)result : default_value;
        }
        
        /// <summary>
        /// Get the value of a double configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>Value of the item or default value if the item is empty.</returns>
        public double GetDouble(string item_name, double default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? (double)result : default_value;
        }

        /// <summary>
        /// Get the value of a boolean configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item.</param>
        /// <param name="default_value">Default value to return if given configuration item is empty.</param>
        /// <returns>Value of the item or default value if the item is empty.</returns>
        public bool GetBoolean(string item_name, bool default_value)
        {
            var result = Database.StringGet($"configurations/{UnitName}/{item_name}");
            return result.HasValue ? (bool)result : default_value;
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

        /// <summary>
        /// Set the value of a integer configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, int value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a unsigned integer configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, uint value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a long integer configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, long value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a unsigned long configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, ulong value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a float configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, float value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a double configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, double value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }
        /// <summary>
        /// Set the value of a boolean configuration item.
        /// </summary>
        /// <param name="item_name">Name of the configuration item to set or add.</param>
        /// <param name="value">Value of the configuration item.</param>
        public void Set(string item_name, bool value)
        {
            Database.StringSet($"configurations/{UnitName}/{item_name}", value);
        }

        /// <summary>
        /// Reload the configuration from the JSON file to the Redis server.
        /// </summary>
        public void Reload()
        {
            Subscriber.Publish("configurations/load", UnitName);
        }

        /// <summary>
        /// Apply the configuration in the Redis server to a JSON file.
        /// </summary>
        public void Apply()
        {
            Subscriber.Publish("configurations/save", UnitName);
        }
    }
}