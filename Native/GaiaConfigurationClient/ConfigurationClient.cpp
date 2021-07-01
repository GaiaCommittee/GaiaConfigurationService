#include "ConfigurationClient.hpp"

#include <sstream>

namespace Gaia::ConfigurationService
{
    /// Combine the unit name and item name to get the key name of this configuration item.
    std::string ConfigurationClient::MakeKeyName(const std::string &unit_name, const std::string &item_name)
    {
        std::stringstream key_builder;
        key_builder << "configurations/" << unit_name << "/" << item_name;
        return key_builder.str();
    }

    ConfigurationClient::ConfigurationClient(const std::string &unit, unsigned int port, const std::string &ip)
    {
        // Configure the connection and connect to the Redis server.
        sw::redis::ConnectionOptions options;
        options.port = static_cast<int>(port);
        options.host = ip;
        options.socket_timeout = std::chrono::milliseconds(100);

        Connection = std::make_unique<sw::redis::Redis>(options);
    }

    /// Get the string value of the given configuration item.
    std::optional<std::string> ConfigurationClient::Get(const std::string& name)
    {
        return Connection->get(MakeKeyName(UnitName, name));
    }

    /// Update or add the value of the given configuration item.
    void ConfigurationClient::Set(const std::string &name, const std::string &value)
    {
        Connection->set(MakeKeyName(UnitName, name), value);
    }

    /// Reload the configuration from the JSON file into the Redis server.
    void ConfigurationClient::Reload()
    {
        Connection->publish("configurations/load", UnitName);
    }

    /// Apply the configuration in the Redis server to a JSON file.
    void ConfigurationClient::Apply()
    {
        Connection->publish("configurations/save", UnitName);
    }
}