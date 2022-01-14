using System;
using System.Threading;
using System.Threading.Tasks;
using DumDum.Interfaces;
using Microsoft.Extensions.Hosting;

namespace DumDum.Services
{
    public  class RecureHostedService : IHostedService
    {

        public Task ExecutingTask;
        public CancellationTokenSource Cts;
        private ITimeService TimeService { get; set; }

        public RecureHostedService(ITimeService timeService)
        {
            TimeService = timeService;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            ExecutingTask = ExecuteAsync(Cts.Token);
            return ExecutingTask.IsCompleted ? ExecutingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (ExecutingTask == null)
            {
                return;
            }

            Cts.Cancel();
            await Task.WhenAny(ExecutingTask, Task.Delay(-1, cancellationToken));
            cancellationToken.ThrowIfCancellationRequested();
        }

        public virtual async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("Hello world");
               // TimeService.UpdateAllKingdomsEvents();
                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken);
            }
        }

    }
}
