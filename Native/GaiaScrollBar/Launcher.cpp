#include <iostream>
#include <thread>
#include <boost/program_options.hpp>
#include <opencv2/opencv.hpp>
#include <GaiaInspectionReader/GaiaInspectionReader.hpp>

int main(int arguments_count, char** arguments)
{
    using namespace Gaia::InspectionService;
    using namespace boost::program_options;

    options_description options("Options");

    options.add_options()
            ("help,?", "show help message.")
            ("host,h", value<std::string>()->default_value("127.0.0.1"),
             "IP address of the Redis server.")
            ("port,p", value<unsigned int>()->default_value(6379),
             "Port of the Redis server.")
            ("unit,u", value<std::string>(),
             "name of the unit to watch")
            ("variable,v", value<std::string>(), "name of the variable to watch.")
            ("frequency,f", value<unsigned int>(), "query frequency, aka. query times per second.")
            ("max,m", value<unsigned int>()->default_value(255),
                    "max value for the scroll bar.");

    variables_map variables;
    store(parse_command_line(arguments_count, arguments, options), variables);
    notify(variables);

    if (variables.count("help"))
    {
        std::cout << options << std::endl;
        return 0;
    }

    sw::redis::Redis connection("tcp://" + variables["host"].as<std::string>() + ":"
        + std::to_string(variables["port"].as<unsigned int>()));

    std::string unit_name;
    if (!variables.count("unit"))
    {
        std::cout << "Input unit name: ";
        std::cin >> unit_name;
    }
    else
    {
        unit_name = variables["unit"].as<std::string>();
    }
    std::string variable_name;
    if (!variables.count("variable"))
    {
        std::cout << "Input variable name: ";
        std::cin >> variable_name;
    }
    else
    {
        variable_name = variables["variable"].as<std::string>();
    }

    unsigned int frequency = 1;
    if (variables.count("frequency"))
    {
        frequency = variables["frequency"].as<unsigned int>();
    }

    unsigned int max = 255;
    if (variables.count("max"))
    {
        max = variables["max"].as<unsigned int>();
    }

    std::string item_name = "configurations/" + unit_name + "/" + variable_name;

    cv::namedWindow("Gaia Inspection - " + unit_name);

    int value  = std::stoi(connection.get(item_name).value_or("0"));

    cv::createTrackbar(variable_name, "Gaia Inspection - " + unit_name, &value, static_cast<int>(max));
    while (cv::waitKey(1000 / static_cast<int>(frequency)) != 27)
    {
        connection.set(item_name, std::to_string(value));
    }
}