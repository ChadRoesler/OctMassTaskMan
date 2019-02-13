using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OctMassTaskMan.Constants;
using OctMassTaskMan.Enums;
using OctMassTaskMan.Models.CommandLine;
using OctMassTaskMan.Models.Interfaces;
using OctMassTaskMan.Workers;
using CommandLine;
using log4net;



namespace OctMassTaskMan.Models.Utillities
{
    internal class ConsoleExecution
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static CancellationTokenSource CancelSource { get; set; }

        internal async static Task ConsoleTask(string[] arguments)
        {
            var verbCommand = new VerbCommands();
            string invokedVerb = string.Empty; ;
            object invokedVerbInstance = new object();

            if (!Parser.Default.ParseArguments(arguments, verbCommand, (verb, subObtions) =>
            {
                invokedVerb = verb;
                invokedVerbInstance = subObtions;
            }))
            {
                Environment.Exit(Parser.DefaultExitCodeFail);
            }
            else
            {
                try
                {
                    ConsoleProgress<Dictionary<ProgressType, string>> progressDictionary = new ConsoleProgress<Dictionary<ProgressType, string>>(value =>
                    {
                        if (value.ContainsKey(ProgressType.Output) && !string.IsNullOrWhiteSpace(value[ProgressType.Output]))
                        {
                            Log.Info(value[ProgressType.Output]);
                        }
                        if (value.ContainsKey(ProgressType.Error))
                        {
                            Log.Error(value[ProgressType.Error]);
                        }
                        if (value.ContainsKey(ProgressType.Warning))
                        {
                            Log.Warn(value[ProgressType.Warning]);
                        }
                    });
                    CancelSource = new CancellationTokenSource();
                    var cancelToken = CancelSource.Token;

                    ConsoleCommands parsedVerb;
                    var taskWatcher = new TaskWatcher();
                    if (Enum.TryParse(invokedVerb, true, out parsedVerb))
                    {
                        Log.Warn(MessageStrings.CmdCancelMessage);
                        Console.CancelKeyPress += (sender, eventargs) =>
                        {
                            CancelSource.Cancel();
                            eventargs.Cancel = true;
                        };

                        switch (parsedVerb)
                        {
                            case ConsoleCommands.Retry:
                                var retry = (RetryCommands)invokedVerbInstance;
                                try
                                {
                                    retry.OctRepository = new OctopusConnectionWorker((IOctopusConnectionSettings)retry).UserConnection();
                                    var retryWorker = new RetryWorker((IOctopusSettings)retry, (IRetrySettings)retry);
                                    await retryWorker.RetryTasks(progressDictionary, taskWatcher, cancelToken);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message);
                                    Log.Error(retry.GetUsage());
                                    Environment.Exit(1);
                                }
                                break;
                            case ConsoleCommands.Cancel:
                                var cancel = (CancelCommands)invokedVerbInstance;
                                try
                                {
                                    cancel.OctRepository = new OctopusConnectionWorker((IOctopusConnectionSettings)cancel).UserConnection();
                                    var cancelWorker = new CancelWorker((IOctopusSettings)cancel, (ICancelSettings)cancel);
                                    await cancelWorker.CancelTasks(progressDictionary, taskWatcher, cancelToken);
                                }
                                catch (Exception ex)
                                {
                                    Log.Error(ex.Message);
                                    Log.Error(cancel.GetUsage());
                                    Environment.Exit(1);
                                }
                                break;
                            default:
                                Log.Info(verbCommand.GetUsage());
                                break;
                        }
                    }
                    else
                    {
                        Log.Error(verbCommand.GetUsage());
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
