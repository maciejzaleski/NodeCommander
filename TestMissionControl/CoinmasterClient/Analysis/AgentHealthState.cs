﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratis.CoinmasterClient.Analysis
{
    public class AgentHealthState
    {
        public Guid WorkerId { get; set; }
        public int UpdateCount { get; set; }
        public DateTime LastUpdateTimestamp { get; set; }
        public int MemoryUsageMb { get; set; }
        public int ThreadCount { get; set; }

        public AgentHealthState()
        {
            LastUpdateTimestamp = DateTime.MinValue;
        }

        public AgentHealthState(Guid workerId) : this()
        {
            WorkerId = workerId;
        }
    }
}
