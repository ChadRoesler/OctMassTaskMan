using Octopus.Client;

namespace OctMassTaskMan.Models.Interfaces
{
    public interface IOctopusSettings
    {
        OctopusRepository OctRepository { get; set; }
    }
}
