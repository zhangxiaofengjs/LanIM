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
    public class WaitTimer
    {
        private static List<WaitTimer> s_timerCache = new List<WaitTimer>();

        private Timer _timer;
        private TimerCallback _callback;
        private bool _loop = false;
        private long _id = DateTime.Now.Ticks;

        public static long Once(int waitTime, TimerCallback callback, object state)
        {
            WaitTimer waiter = new WaitTimer();
            waiter._callback = callback;
            waiter._timer = new Timer(new TimerCallback(waiter.CallbackFunc), state, waitTime, waitTime);

            return waiter._id;
        }

        public static long Loop(int waitTime, TimerCallback callback, object state)
        {
            WaitTimer waiter = new WaitTimer();
            waiter._callback = callback;
            waiter._loop = true;
            waiter._timer = new Timer(new TimerCallback(waiter.CallbackFunc), state, waitTime, waitTime);

            return waiter._id;
        }

        public static long LoopNow(int waitTime, TimerCallback callback, object state)
        {
            WaitTimer waiter = new WaitTimer();
            waiter._callback = callback;
            waiter._loop = true;
            waiter._timer = new Timer(new TimerCallback(waiter.CallbackFunc), state, 0, waitTime);

            return waiter._id;
        }

        public static void Stop(long id)
        {
            WaitTimer wt = s_timerCache.Find((w) =>
            {
                if (id == w._id)
                {
                    return true;
                }
                return false;
            });

            if(wt != null)
            {
                wt.Stop();
            }
        }

        private void CallbackFunc(object state)
        {
            if (!_loop)
            {
                //只运行一次
                this.Stop();
            }

            _callback?.Invoke(state);
        }

        public void Stop()
        {
            _timer.Dispose();

            s_timerCache.RemoveAll((w) =>
            {
                if(this._id == w._id)
                {
                    return true;
                }
                return false;
            });
        }
    }
}
