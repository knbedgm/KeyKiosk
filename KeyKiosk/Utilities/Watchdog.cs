using System.Diagnostics;
using System.Timers;
using Timer = System.Timers.Timer;

namespace KeyKiosk.Utilities
{
    public class WatchDog
    {
        object locker = new object();
        long lastPoke;
        Stopwatch stopWatch = new Stopwatch();
        long threshold;
        Action callback;
        Timer watchDogTimer;
        public WatchDog(long threshold, Action callback)
        {
            this.callback = callback;
            this.threshold = threshold;
            var interval = Math.Min(1000, threshold / 3);
            watchDogTimer = new Timer(interval);
            watchDogTimer.Elapsed += new ElapsedEventHandler(WatchDogTimer_Elapsed);
            watchDogTimer.Start();
            stopWatch.Start();
        }


        void WatchDogTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            bool elapsed = false;
            lock (locker)
            {
                if ((stopWatch.ElapsedMilliseconds - lastPoke) > threshold)
                {
                    elapsed = true;
                }
            }

            if (elapsed)
            {
                callback();
            }
        }

        public void Poke()
        {
            //Console.WriteLine("poked");
            lock (locker)
            {
                lastPoke = stopWatch.ElapsedMilliseconds;
            }
        }

        public void Stop()
        {
            watchDogTimer.Stop();
            watchDogTimer.Close();
        }
    }
}
