namespace OctMassTaskMan.Constants
{
    internal class CommandStrings
    {
        internal const string CancelDescription = "Cancels all tasks found.";
        internal const string RetryDescription = "Retrys all interupted tasks found.";
        internal const string VerboseDescription = "Prints info as it occurs.\r\n";
        internal const string UriDescription = "Octopus Server Uri used for logging into Octopus.\r\n";
        internal const string ApiKeyDescription = "Api Key used for logging into Octopus.\r\n";
        internal const string AutoApporveDescription = "Automatically Cancel all Tasks witout user approval.\r\n";
        internal const string ContinuousDefinition = "Continuously check for tasks to retry until all tasks are complete.\r\n";
        internal const string InterruptionNoteDefinition = "Note for interruptions.\r\n";
    }
}
