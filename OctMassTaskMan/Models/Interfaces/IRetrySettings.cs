namespace OctMassTaskMan.Models.Interfaces
{
    public interface IRetrySettings
    {
        bool Continuous { get; set; }
        string InterruptionNote { get; set; }
    }
}
