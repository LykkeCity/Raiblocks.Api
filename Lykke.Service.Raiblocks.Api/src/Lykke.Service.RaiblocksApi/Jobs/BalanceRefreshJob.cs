using Common.Log;
using Lykke.JobTriggers.Triggers.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Service.RaiblocksApi.Jobs
{
    public class BalanceRefreshJob
    {
        private ILog _log;

        public BalanceRefreshJob(ILog log)
        {
            _log = log;
        }

        [TimerTrigger("00:00:10")]
        public async Task RefreshBalances()
        {
            await _log.WriteMonitorAsync("", $"Env: {Program.EnvInfo}", "RefreshBalances");
            throw new NotImplementedException();
        }
    }
}
