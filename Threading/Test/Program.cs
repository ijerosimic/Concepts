using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class Program
    {
        static async Task Main()
        {
            var c = new Incrementor();

            var threadSafe = new List<Task>();
            var threadUnsafe = new List<Task>();
            var deadlocked = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                threadSafe.Add(c.IncrementWithLockAndUnlock());
            }

            Console.WriteLine("THREAD SAFE \n");
            await Task.WhenAll(threadSafe);

            for (int i = 0; i < 10; i++)
            {
                threadUnsafe.Add(c.IncrementWithoutLock());
            }
            
            Console.WriteLine("\nTHREAD UNSAFE \n");
            await Task.WhenAll(threadUnsafe);

            for (int i = 0; i < 10; i++)
            {
                deadlocked.Add(c.IncrementWithoutUnlock());
            }

            Console.WriteLine(" \nDEADLOCKED \n");
            await Task.WhenAll(deadlocked);
        }
    }

    public class Incrementor
    {
        int i = 0;
        int y = 0;
        int z = 0;
        readonly object locker = new object();

        public async Task IncrementWithLockAndUnlock()
        {
            try
            {
                await Task.Delay(1000);
                Monitor.Enter(locker);
                i += 1;
                Console.WriteLine("Thread ID {0} incremented i. Current value: {1}", Thread.CurrentThread.ManagedThreadId, i);
            }
            finally
            {
                Monitor.Exit(locker);
            }
        }

        public async Task IncrementWithoutLock()
        {
            await Task.Delay(1000);
            y += 1;
            Console.WriteLine("Thread ID {0} incremented i. Current value: {1}", Thread.CurrentThread.ManagedThreadId, y);
        }

        public async Task IncrementWithoutUnlock()
        {
            await Task.Delay(1000);
            Monitor.Enter(locker);
            z += 1;
            Console.WriteLine("Thread ID {0} incremented i. Current value: {1}", Thread.CurrentThread.ManagedThreadId, z);
        }
    }
}