using System;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Models.Interfaces;
using Octopus.Client;

namespace OctMassTaskMan.Workers
{
    internal class OctopusConnectionWorker
    {
        IOctopusConnectionSettings sessionOctopusConnectionSettings;
        internal OctopusConnectionWorker(IOctopusConnectionSettings octopusConnectionSettings)
        {
            sessionOctopusConnectionSettings = octopusConnectionSettings;
        }

        internal OctopusRepository UserConnection()
        {
            var repository = new OctopusRepository(new OctopusServerEndpoint(sessionOctopusConnectionSettings.Uri, sessionOctopusConnectionSettings.ApiKey));
            try
            {
                repository.Users.GetCurrent();
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ErrorStrings.InvalidApiKey, ex.Message));
            }
            return repository;
        }
    }
}
