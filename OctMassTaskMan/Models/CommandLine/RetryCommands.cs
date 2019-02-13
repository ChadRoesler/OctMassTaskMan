using CommandLine;
using CommandLine.Text;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Models.Interfaces;
using Octopus.Client;

namespace OctMassTaskMan.Models.CommandLine
{
    public class RetryCommands : CommonCommands, IRetrySettings, IOctopusConnectionSettings, IOctopusSettings
    {
        public OctopusRepository OctRepository { get; set; }

        [Option('c', "continuous", DefaultValue = false, HelpText = CommandStrings.ContinuousDefinition)]
        public bool Continuous { get; set; }

        [Option('n', "interruptionNote", DefaultValue = "MassRetry", HelpText = CommandStrings.InterruptionNoteDefinition)]
        public string InterruptionNote { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
