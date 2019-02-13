using CommandLine;
using CommandLine.Text;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Models.Interfaces;
using Octopus.Client;

namespace OctMassTaskMan.Models.CommandLine
{
    public class CancelCommands : CommonCommands, ICancelSettings, IOctopusConnectionSettings, IOctopusSettings
    {
        public OctopusRepository OctRepository { get; set; }

        [Option('x', "autoApprove", DefaultValue = false, HelpText = CommandStrings.AutoApporveDescription)]
        public bool AutoApprove { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
