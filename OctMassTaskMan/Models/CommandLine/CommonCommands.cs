using CommandLine;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Models.Interfaces;

namespace OctMassTaskMan.Models.CommandLine
{
    abstract public class CommonCommands: IOctopusConnectionSettings
    {
        [Option('u', "uri", HelpText = CommandStrings.UriDescription, Required = true)]
        public string Uri { get; set; }

        [Option('a', "apiKey", HelpText = CommandStrings.ApiKeyDescription, Required = true)]
        public string ApiKey { get; set; }
    }
}
