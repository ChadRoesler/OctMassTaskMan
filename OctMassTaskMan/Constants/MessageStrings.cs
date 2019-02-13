namespace OctMassTaskMan.Constants
{
    internal class MessageStrings
    {
        internal const string ErrorFormattingText = "An error occurred during: {0}. Please review the following Error(s):\r\n{1}";

        internal const string GatheringTasks = "Gathering Tasks";

        
        internal const string TaskCancelled = "Task Canceled.";

        internal const string CancelTaskOther = "Canceling: {0} | Description: {1}}";
        internal const string CancelTaskProject = "Canceling: {0} | Project: {1}";
        internal const string ValidateCancelTask = "Are you sure you want to cancel (Y/N)?";
        internal const string SkippingTask = "Skipping Task";
        internal const string CancellingTask = "Canceling Task";

        internal const string RetryTaskOther = "Retrying: {0}";
        internal const string RetryTaskProject = "Retrying: {0} | Project: {1}";

        internal const string RetryActiveTasks = "There are still active tasks, checking again in 1 minute";

        internal const string ProgressComplete = "Task Finished.";
        internal const string ProgressFailed = "Task Finished with Errors, please review the following.";
        internal const string ConsoleComplete = "Press enter to exit...";
        internal const string ProgressCancelled = "Task Canceled.";

        internal const string CmdCancelMessage = @"==============================================
Press Ctrl + c to Cancel the command.
==============================================";

    }
}
