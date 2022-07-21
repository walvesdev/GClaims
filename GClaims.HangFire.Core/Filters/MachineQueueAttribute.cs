using Hangfire.Common;
using Hangfire.States;

namespace GClaims.HangFire.Core.Filters
{
    public class MachineQueueAttribute : JobFilterAttribute, IElectStateFilter
    {
        public bool Enabled { get; set; } = true;

        public void OnStateElection(ElectStateContext context)
        {
            if (!Enabled)
            {
                return;
            }

            if (context.CandidateState is EnqueuedState enqueuedState)
            {
                enqueuedState.Queue = MachineQueueName;
            }
        }

        public static string MachineQueueName => $"{string.Concat(Environment.MachineName.ToLowerInvariant().Where(char.IsLetterOrDigit).Select(p => p.ToString()))}_debug";
    }
}
