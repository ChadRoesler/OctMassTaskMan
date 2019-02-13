namespace OctMassTaskMan.Models.Interfaces
{
    public interface IOctopusConnectionSettings
    {
        string Uri { get; set; }
        string ApiKey { get; set; }
    }
}
