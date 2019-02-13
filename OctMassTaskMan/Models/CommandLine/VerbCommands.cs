using CommandLine;
using CommandLine.Text;
using OctMassTaskMan.Constants;

namespace OctMassTaskMan.Models.CommandLine
{
    public class VerbCommands
    {
        public VerbCommands()
        {
            CancelCmd = new CancelCommands();
            RetryCmd = new RetryCommands();
        }

        [VerbOption("Retry", HelpText = CommandStrings.RetryDescription)]
        public RetryCommands RetryCmd { get; set; }

        [VerbOption("Cancel", HelpText = CommandStrings.CancelDescription)]
        public CancelCommands CancelCmd {get; set;}

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }

        public string GetUsage()
        {
            return HelpText.AutoBuild(this);
        }
    }
}
