using System;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using ChatTailorAI.Shared.Services.Common;

namespace ChatTailorAI.Services.WinUI.Dispatching
{
    public class DispatcherService : IDispatcherService
    {
        private readonly DispatcherQueue _dispatcherQueue;

        public DispatcherService()
        {
            // Access the current DispatcherQueue for the UI thread
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread()
                ?? throw new InvalidOperationException("DispatcherQueue is not available on the current thread.");
        }

        public async Task RunOnUIThreadAsync(Action action)
        {
            // Use the DispatcherQueue to invoke the action on the UI thread
            var taskCompletionSource = new TaskCompletionSource();

            var enqueued = _dispatcherQueue.TryEnqueue(() =>
            {
                try
                {
                    action();
                    taskCompletionSource.SetResult();
                }
                catch (Exception ex)
                {
                    taskCompletionSource.SetException(ex);
                }
            });

            if (!enqueued)
            {
                throw new InvalidOperationException("Failed to enqueue the action on the UI thread.");
            }

            await taskCompletionSource.Task;
        }
    }
}
