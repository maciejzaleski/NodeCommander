﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratis.CoinmasterClient.Messages
{
    public enum MessageType
    {
        ClientRegistration,
        AgentRegistration,
        AgentHealthcheck,

        NodeStatistics,
        NodeConfiguration,

        ActionRequest,
        ResourceFromClient,
        ResourceFromAgent,



    }
}
