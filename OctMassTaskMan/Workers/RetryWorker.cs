using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Enums;
using OctMassTaskMan.Models.Interfaces;
using OctMassTaskMan.Models.Utillities;
using OctopusHelpers;
using OctopusHelpers.Enums;

namespace OctMassTaskMan.Workers
{
    class RetryWorker
    {
        IOctopusSettings sessionOctopusSettings;
        IRetrySettings sessionRetrySettings;

        private Dictionary<string, string> Errors;

        internal RetryWorker(IOctopusSettings octopusSettings, IRetrySettings retrySettings)
        {
            sessionOctopusSettings = octopusSettings;
            sessionRetrySettings = retrySettings;
        }

        internal Task RetryTasks(IProgress<Dictionary<ProgressType, string>> progress, TaskWatcher taskWatch, CancellationToken cancelToken)
        {
            Errors = new Dictionary<string, string>();
            return Task.Run(() =>
            {
                try
                {
                    taskWatch.Running = true;
                    var waitCounter = 0;
                    var progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                    progressRetryTaskDictionary[ProgressType.Output] = MessageStrings.GatheringTasks;
                    progress.Report(progressRetryTaskDictionary);

                    var activeTasks = sessionOctopusSettings.OctRepository.Tasks.GetAllActive();
                    var pendingTasks = activeTasks.Where(t => t.HasPendingInterruptions);
                    var interruptionNote = string.Format(ResourceStrings.InterruptionNote, sessionOctopusSettings.OctRepository.Users.GetCurrent().DisplayName, sessionRetrySettings.InterruptionNote);

                    foreach (var pendingTask in pendingTasks)
                    {
                        var retryMessage = string.Empty;
                        if (pendingTask.Name == ResourceStrings.DeployTaskName)
                        {
                            var deployment = DeploymentHelper.GetDeploymentFromTask(sessionOctopusSettings.OctRepository, pendingTask);
                            var project = ProjectHelper.GetProjectById(sessionOctopusSettings.OctRepository, deployment.ProjectId);

                            retryMessage = string.Format(MessageStrings.RetryTaskProject, pendingTask.Id, project.Name);
                        }
                        else
                        {
                            retryMessage = string.Format(MessageStrings.RetryTaskOther, pendingTask.Id, pendingTask.Description);
                        }
                        progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                        progressRetryTaskDictionary[ProgressType.Output] = retryMessage;
                        progress.Report(progressRetryTaskDictionary);

                        var interruption = InterruptionHelper.GetPendingInterruption(sessionOctopusSettings.OctRepository, pendingTask);
                        InterruptionHelper.InterruptionReponse(sessionOctopusSettings.OctRepository, interruption, InterruptionResponse.Retry, interruptionNote);
                        while(waitCounter < 10)
                        {
                            Thread.Sleep(1000);
                            if (cancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            waitCounter++;
                        }
                    }
                    if(sessionRetrySettings.Continuous)
                    {
                        progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                        progressRetryTaskDictionary[ProgressType.Output] = MessageStrings.RetryActiveTasks;
                        progress.Report(progressRetryTaskDictionary);
                        waitCounter = 0;
                        while (waitCounter < 60)
                        {
                            Thread.Sleep(1000);
                            if (cancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            waitCounter++;
                        }
                        progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                        progressRetryTaskDictionary[ProgressType.Output] = MessageStrings.GatheringTasks;
                        progress.Report(progressRetryTaskDictionary);
                        activeTasks = sessionOctopusSettings.OctRepository.Tasks.GetAllActive();
                    }
                    while (activeTasks.Count() > 0 && sessionRetrySettings.Continuous)
                    {
                        pendingTasks = activeTasks.Where(t => t.HasPendingInterruptions);
                        interruptionNote = string.Format(ResourceStrings.InterruptionNote, sessionOctopusSettings.OctRepository.Users.GetCurrent().DisplayName, sessionRetrySettings.InterruptionNote);
                        foreach (var pendingTask in pendingTasks)
                        {
                            var retryMessage = string.Empty;
                            if (pendingTask.Name == ResourceStrings.DeployTaskName)
                            {
                                var deployment = DeploymentHelper.GetDeploymentFromTask(sessionOctopusSettings.OctRepository, pendingTask);
                                var project = ProjectHelper.GetProjectById(sessionOctopusSettings.OctRepository, deployment.ProjectId);

                                retryMessage = string.Format(MessageStrings.RetryTaskProject, pendingTask.Id, project.Name);
                            }
                            else
                            {
                                retryMessage = string.Format(MessageStrings.RetryTaskOther, pendingTask.Id, pendingTask.Description);
                            }
                            progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                            progressRetryTaskDictionary[ProgressType.Output] = retryMessage;
                            progress.Report(progressRetryTaskDictionary);
                            var interruption = InterruptionHelper.GetPendingInterruption(sessionOctopusSettings.OctRepository, pendingTask);
                            InterruptionHelper.InterruptionReponse(sessionOctopusSettings.OctRepository, interruption, InterruptionResponse.Retry, interruptionNote);
                            while (waitCounter < 10)
                            {
                                Thread.Sleep(1000);
                                if (cancelToken.IsCancellationRequested)
                                {
                                    break;
                                }
                                waitCounter++;
                            }
                        }
                        if (cancelToken.IsCancellationRequested)
                        {
                            break;
                        }
                        progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                        progressRetryTaskDictionary[ProgressType.Output] = MessageStrings.RetryActiveTasks;
                        progress.Report(progressRetryTaskDictionary);
                        waitCounter = 0;
                        while (waitCounter < 60)
                        {
                            Thread.Sleep(1000);
                            if (cancelToken.IsCancellationRequested)
                            {
                                break;
                            }
                            waitCounter++;
                        }
                        progressRetryTaskDictionary = new Dictionary<ProgressType, string>();
                        progressRetryTaskDictionary[ProgressType.Output] = MessageStrings.GatheringTasks;
                        progress.Report(progressRetryTaskDictionary);
                        activeTasks = sessionOctopusSettings.OctRepository.Tasks.GetAllActive();
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
