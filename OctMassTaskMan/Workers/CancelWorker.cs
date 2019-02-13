using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OctMassTaskMan.Enums;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Models.Interfaces;
using OctMassTaskMan.Models.Utillities;
using OctopusHelpers;
using Octopus.Client.Model;


namespace OctMassTaskMan.Workers
{
    public class CancelWorker
    {
        IOctopusSettings sessionOctopusSettings;
        ICancelSettings sessionCancelSettings;

        private Dictionary<string, string> Errors;

        public CancelWorker(IOctopusSettings octopusSettings, ICancelSettings cancelSettings)
        {
            sessionOctopusSettings = octopusSettings;
            sessionCancelSettings = cancelSettings;
        }

        public Task CancelTasks(IProgress<Dictionary<ProgressType, string>> progress, TaskWatcher taskWatch, CancellationToken cancelToken)
        {
            Errors = new Dictionary<string, string>();
            return Task.Run(() =>
            {
                try
                {
                    taskWatch.Running = true;

                    var progressCancelTaskDictionary = new Dictionary<ProgressType, string>();
                    progressCancelTaskDictionary[ProgressType.Output] = MessageStrings.GatheringTasks;
                    progress.Report(progressCancelTaskDictionary);

                    var runningTasks = sessionOctopusSettings.OctRepository.Tasks.GetAllActive();
                    foreach (var runningTask in runningTasks)
                    {
                        var cancelationMessage = string.Empty;
                        if(runningTask.Name == ResourceStrings.DeployTaskName)
                        {
                            var deployment = DeploymentHelper.GetDeploymentFromTask(sessionOctopusSettings.OctRepository, runningTask);
                            var project = ProjectHelper.GetProjectById(sessionOctopusSettings.OctRepository, deployment.ProjectId);
                      
                            cancelationMessage = string.Format(MessageStrings.CancelTaskProject, runningTask.Id, project.Name);
                        }
                        else
                        {
                            cancelationMessage = string.Format(MessageStrings.CancelTaskOther, runningTask.Id, runningTask.Description);
                        }
                        if (cancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (!sessionCancelSettings.AutoApprove)
                        {
                            progressCancelTaskDictionary = new Dictionary<ProgressType, string>();
                            progressCancelTaskDictionary[ProgressType.Output] = cancelationMessage;
                            progressCancelTaskDictionary[ProgressType.Warning] = MessageStrings.ValidateCancelTask;
                            progress.Report(progressCancelTaskDictionary);
                            var consoleKey = Console.ReadKey(true);
                            while (consoleKey.Key != ConsoleKey.Y && consoleKey.Key != ConsoleKey.N && (consoleKey.Key != ConsoleKey.C && consoleKey.Modifiers != ConsoleModifiers.Control))
                            {
                                var progressInvalidKeyTaskDictionary = new Dictionary<ProgressType, string>();
                                progressInvalidKeyTaskDictionary[ProgressType.Warning] = MessageStrings.ValidateCancelTask;
                                progress.Report(progressInvalidKeyTaskDictionary);
                                consoleKey = Console.ReadKey(true);
                            }
                            if (consoleKey.Key == ConsoleKey.Y)
                            {
                                sessionOctopusSettings.OctRepository.Tasks.Cancel(runningTask);
                                var progressCancelTaskCancelDictionary = new Dictionary<ProgressType, string>();
                                progressCancelTaskCancelDictionary[ProgressType.Output] = MessageStrings.CancellingTask;
                                progress.Report(progressCancelTaskCancelDictionary);
                            }
                            else if(consoleKey.Key == ConsoleKey.N)
                            {
                                var progressCancelTaskSkipDictionary = new Dictionary<ProgressType, string>();
                                progressCancelTaskSkipDictionary[ProgressType.Output] = MessageStrings.SkippingTask;
                                progress.Report(progressCancelTaskSkipDictionary);
                            }
                            else if(consoleKey.Key == ConsoleKey.C && consoleKey.Modifiers == ConsoleModifiers.Control)
                            {
                                if (cancelToken.IsCancellationRequested)
                                {
                                    break;
                                }
                            }
                            if (cancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                        else
                        {
                            progressCancelTaskDictionary = new Dictionary<ProgressType, string>();
                            progressCancelTaskDictionary[ProgressType.Output] = cancelationMessage;
                            progress.Report(progressCancelTaskDictionary);
                            sessionOctopusSettings.OctRepository.Tasks.Cancel(runningTask);
                            if (cancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                        }
                    }
                    var progressCompleteDictionary = new Dictionary<ProgressType, string>();
                    if (cancelToken.IsCancellationRequested)
                    {
                        progressCompleteDictionary[ProgressType.Output] = MessageStrings.ProgressCancelled;
                        progress.Report(progressCompleteDictionary);
                        taskWatch.Running = false;
                    }
                    else
                    {
                        progressCompleteDictionary[ProgressType.Output] = MessageStrings.ProgressComplete;
                        progress.Report(progressCompleteDictionary);
                        taskWatch.Running = false;
                    }

                }
                catch(Exception ex)
                {
                    var progressCriticalErrorDictionary = new Dictionary<ProgressType, string>();
                    progressCriticalErrorDictionary[ProgressType.Error] = string.Format(MessageStrings.ErrorFormattingText, ex.Message);
                    progressCriticalErrorDictionary[ProgressType.Output] = MessageStrings.ProgressFailed;
                    progress.Report(progressCriticalErrorDictionary);
                    taskWatch.Running = false;
                }
            }, cancelToken);
        }
    }
}
