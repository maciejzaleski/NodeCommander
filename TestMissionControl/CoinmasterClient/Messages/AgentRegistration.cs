﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stratis.CoinmasterClient.Client.Dispatchers;
using Stratis.CoinmasterClient.Client.Handlers;

namespace Stratis.CoinmasterClient.Messages
{
    public class AgentRegistration : IMessage
    {
        public event ResponseHandler.DispatherCallback DispatherResponseReceived;
        public ClientConnection Client { get; set; }

        public Guid CorrelationId { get; set; }

        public AgentRegistration()
        {
            CorrelationId = Guid.NewGuid();
        } 

        public void OnDispatherResponseReceived(DispatherResponse response)
        {
            DispatherResponseReceived?.Invoke(response);
        }
    }
}
