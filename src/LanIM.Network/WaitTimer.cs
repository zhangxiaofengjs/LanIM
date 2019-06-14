using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Com.LanIM.Network
{
    class WaitTimer
    {
        private Timer _timer;
        private TimerCallback _callback;

        public static void Start(int waitTime, TimerCallback callback, object state)
        {
            WaitTimer waiter = new WaitTimer();
            waiter._callback = callback;
            waiter._timer = new Timer(new TimerCallback(waiter.CallbackFunc), state, waitTime, waitTime);
        }

        private void CallbackFunc(object state)
        {
            //只运行一次
            this.Stop();

            _callback?.Invoke(state);
        }

        public void Stop()
        {
            _timer.Dispose();
        }
    }
}
