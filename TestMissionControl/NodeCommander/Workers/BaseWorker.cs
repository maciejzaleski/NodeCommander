﻿using System;
using System.Threading.Tasks;
using System.Timers;

namespace Stratis.NodeCommander.Workers
{
    public abstract class BaseWorker
    {
        protected Timer _jobScheduler;
        public delegate void StateChangeHandler(object source, StateChangeHandlerEventArgs args);
        public event StateChangeHandler StateChange;
        protected double _interval;
        protected bool DisabledByDefault = false;
        

        protected object QueryLock = new object();


        public Double Interval
        {
            get { return _interval; }
            set
            {
                _interval = value;
                _jobScheduler.Interval = value;
            }
        }

        public BaseWorker(double interval)
        {
            _jobScheduler = new Timer();
            _jobScheduler.Elapsed += PerformWork;
            _jobScheduler.AutoReset = true;
            Interval = interval;

            OnStateChange(this, WorkerState.Stopped);
        }

        protected void OnStateChange(object source, WorkerState state)
        {
            StateChangeHandlerEventArgs args = new StateChangeHandlerEventArgs();
            args.State = state;

            if (StateChange != null) StateChange.Invoke(source, args);
        }

        public void Start()
        {
            Reset();
            _jobScheduler.Interval = 100;

            if (!DisabledByDefault)
            {
                _jobScheduler.Start();
            }

            OnStateChange(this, WorkerState.Idle);
        }



        public void Stop()
        {
            _jobScheduler.Stop();
            OnStateChange(this, WorkerState.Stopped);
        }

        public abstract void Reset();

        public void Tick(bool suspendTimer = false)
        {
            _jobScheduler.Stop();
            _jobScheduler.Interval = 100;

            if (!suspendTimer) _jobScheduler.Start();
            else PerformWork(this, null);
        }


        private async void PerformWork(object sender, ElapsedEventArgs e)
        {
            _jobScheduler.Interval = _interval;
            _jobScheduler.Stop();
            OnStateChange(this, WorkerState.Running);

            var queryTask = RunQuery();
            await queryTask;

            OnStateChange(this, WorkerState.Idle);
            _jobScheduler.Start();
        }

        protected abstract Task RunQuery();
    }
}
