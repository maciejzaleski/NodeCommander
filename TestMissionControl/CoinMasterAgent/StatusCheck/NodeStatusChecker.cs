﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using NLog;
using Stratis.CoinmasterClient.Analysis;
using Stratis.CoinmasterClient.Network;

namespace Stratis.CoinMasterAgent.StatusCheck
{
    public class NodeStatusChecker
    {
        private CountdownEvent signalingEvent = new CountdownEvent(1);
        private NodeNetwork localNodes;
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private Dictionary<Guid, NodeLog> nodeLogWorkers = new Dictionary<Guid, NodeLog>();
        private Dictionary<Guid, NodeOperation> nodeOperationWorkers = new Dictionary<Guid, NodeOperation>();
        private Dictionary<Guid, NodeDeployment> nodeDeploymentWorkers = new Dictionary<Guid, NodeDeployment>();
        private Dictionary<Guid, NodeProcess> nodeProcessWorkers = new Dictionary<Guid, NodeProcess>();

        public NodeStatusChecker(NodeNetwork localNodes)
        {
            this.localNodes = localNodes;
        }

        public void Start()
        {
            Thread updateThread = new Thread(async ()  =>
            {

                while (true)
                {
                    //ToDo: Pause the loop if there are no active clients
                    foreach (SingleNode node in localNodes.NetworkNodes.Values)
                    {
                        await UpdateData(node);
                    }
                        

                    Thread.Sleep(3000);
                    if (signalingEvent.CurrentCount > 0) signalingEvent.Signal();
                }
            });
            updateThread.Start();
        }

        private async Task UpdateData(SingleNode node)
        {
            try
            {
                if (node.NodeDeploymentState == null) node.NodeDeploymentState = new NodeDeploymentState();

                NodeDeployment nodeDeployment;
                if (node.NodeDeploymentState.WorkerId != Guid.Empty && nodeOperationWorkers.ContainsKey(node.NodeDeploymentState.WorkerId))
                {
                    nodeDeployment = nodeDeploymentWorkers[node.NodeDeploymentState.WorkerId];
                }
                else
                {
                    Guid newWorkerGuid = Guid.NewGuid();

                    nodeDeployment = new NodeDeployment(newWorkerGuid, node);
                    nodeDeploymentWorkers.Add(newWorkerGuid, nodeDeployment);
                }
                node.NodeDeploymentState = nodeDeployment.State;
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot get NodeDeploymentState", ex);
            }

            try
            {
                if (node.NodeOperationState == null) node.NodeOperationState = new NodeOperationState();

                NodeOperation nodeOperation;
                if (node.NodeOperationState.WorkerId != Guid.Empty && nodeOperationWorkers.ContainsKey(node.NodeOperationState.WorkerId))
                {
                    nodeOperation = nodeOperationWorkers[node.NodeOperationState.WorkerId];
                }
                else
                {
                    Guid newWorkerGuid = Guid.NewGuid();

                    nodeOperation = new NodeOperation(newWorkerGuid, node);
                    nodeOperationWorkers.Add(newWorkerGuid, nodeOperation);
                }
                node.NodeOperationState = nodeOperation.State;
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot get GetNodeOperationState", ex);
            }

            try
            {
                if (node.NodeLogState == null) node.NodeLogState = new NodeLogState();

                NodeLog nodeLog;
                if (node.NodeLogState.WorkerId != Guid.Empty && nodeLogWorkers.ContainsKey(node.NodeLogState.WorkerId))
                {
                    nodeLog = nodeLogWorkers[node.NodeLogState.WorkerId];
                }
                else
                {
                    Guid newWorkerGuid = Guid.NewGuid();

                    nodeLog = new NodeLog(newWorkerGuid);
                    nodeLogWorkers.Add(newWorkerGuid, nodeLog);
                }
                node.NodeLogState = nodeLog.State;
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot get NodeLogState", ex);
            }

            try
            {
                if (node.NodeLogState == null) node.NodeLogState = new NodeLogState();

                NodeProcess nodeProcess;
                if (node.NodeLogState.WorkerId != Guid.Empty && nodeProcessWorkers.ContainsKey(node.NodeProcessState.WorkerId))
                {
                    nodeProcess = nodeProcessWorkers[node.NodeProcessState.WorkerId];
                }
                else
                {
                    Guid newWorkerGuid = Guid.NewGuid();

                    nodeProcess = new NodeProcess(newWorkerGuid, node);
                    nodeProcessWorkers.Add(newWorkerGuid, nodeProcess);
                }
                node.NodeProcessState = nodeProcess.State;
            }
            catch (Exception ex)
            {
                logger.Error($"Cannot get NodeProcessState", ex);
            }
        }

        public NodeNetwork GetUpdate()
        {
            signalingEvent.Wait();
            signalingEvent.Reset();
            return localNodes;
        }
        
    }
}
