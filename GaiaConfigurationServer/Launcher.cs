using System;
using System.Threading;
using Microsoft.Extensions.CommandLineUtils;
using ServiceStack.Text;

namespace Gaia.ConfigurationService
{
    /// <summary>
    /// Launcher is used to manage service instances.
    /// </summary>
    public class Launcher
    {
        /// <summary>
        /// Entrance point with argument parsing.
        /// </summary>
        /// <param name="arguments"></param>
        public static void Main(string[] arguments)
        {
            // Prepare command line arguments parser.
            var application = new CommandLineApplication();
            var option_host = application.Option("-h | --host <address>", 
                "set the ip address of the Redis server to connect.",
                CommandOptionType.SingleValue);
            var option_port = application.Option("-p | --port <number>", 
                "set the port number of the Redis server to connect.",
                CommandOptionType.SingleValue);
            var option_path = application.Option("-d | --directory <path>",
                "path of the configurations directory",
                CommandOptionType.SingleValue);
            application.HelpOption("-? | --help");
            
            application.OnExecute(() =>
            {
                bool crashed = false;
                // Loop until launcher normally exited.
                do
                {
                    try
                    {
                        var launcher = new Launcher();
                        crashed = false;
                        Console.WriteLine("Launching configuration service...");
                        launcher.Launch(
                            option_path.HasValue() ? option_path.Value() : "./Configurations",
                            option_port.HasValue() ? Convert.ToUInt32(option_port.Value()) : 6379,
                            option_host.HasValue() ? option_host.Value() : "127.0.0.1"
                        );
                    }
                    catch (Exception error)
                    {
                        crashed = true;
                        Console.WriteLine("Configuration service crashed. Restart in 1 seconds.");
                        error.PrintDump();
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                } while (crashed);

                return 0;
            });

            // Parse command line arguments and then perform the action.
            application.Execute(arguments);
        }

        /// <summary>
        /// Launch the configuration service.
        /// </summary>
        /// <param name="path">The path of the configuration folder.</param>
        /// <param name="port">Port of the Redis server.</param>
        /// <param name="ip">IP address of the Redis server.</param>
        /// <exception cref="Exception">When failed to connect to the Redis server or service aborted.</exception>
        private void Launch(string path = "./Configurations", uint port = 6379, string ip = "127.0.0.1")
        {
            Console.WriteLine("Configuration service initializing...");

            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }

            var mapper = new Mapper(path, port, ip);

            if (!mapper.IsConnected)
            {
                throw new Exception(
                    $"Configuration service failed to connect to the database on {ip}:{port.ToString()}");
            }

            Console.WriteLine($"Configuration service online: database on {ip}:{port.ToString()} connected.");

            while (mapper.LifeFlag)
            {
                Thread.Sleep(TimeSpan.FromSeconds(3));
            }
            
            Console.WriteLine("Configuration service normally stopped.");
        }
    }
}