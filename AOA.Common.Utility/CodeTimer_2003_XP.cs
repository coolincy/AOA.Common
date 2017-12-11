using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 代码执行计时器，XP及2003之前版本使用
    /// </summary>
    public static class CodeTimer_2003_XP
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool GetThreadTimes(IntPtr hThread, out long lpCreationTime, out long lpExitTime, out long lpKernelTime, out long lpUserTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        /// <summary>
        /// 测试方法委托
        /// </summary>
        public delegate void ActionDelegate();

        private static long GetCurrentThreadTimes()
        {
            long l;
            long kernelTime, userTimer;
            GetThreadTimes(GetCurrentThread(), out l, out l, out kernelTime, out userTimer);
            return kernelTime + userTimer;
        }

        static CodeTimer_2003_XP()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;

            // 测试代码

            //// 采用委托
            //CodeTimer.Time("Thread Sleep", 1, delegate() { Thread.Sleep(3000); });
            //CodeTimer.Time("Empty Method", 10000000, delegate() { });
            //string a = "";
            //CodeTimer.Time("String Concat", 100000, delegate() { a += "a"; });
            //StringBuilder ab = new StringBuilder();
            //CodeTimer.Time("StringBuilder Conca", 100000, delegate() { ab.Append("a"); });

            //// 采用匿名方法
            //CodeTimer.Time("Thread Sleep", 1, () => { Thread.Sleep(3000); });
            //CodeTimer.Time("Empty Method", 10000000, () => { });
            //string s = "";
            //CodeTimer.Time("String Concat", 100000, () => { s += "a"; });
            //StringBuilder sb = new StringBuilder();
            //CodeTimer.Time("StringBuilder", 100000, () => { sb.Append("a"); });
        }

        /// <summary>
        /// 执行计时器
        /// </summary>
        /// <param name="name"></param>
        /// <param name="iteration"></param>
        /// <param name="action"></param>
        public static void Time(string name, uint iteration, ActionDelegate action)
        {
            if (String.IsNullOrEmpty(name) || action == null)
                return;

            // 1. Print name
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 2. Record the latest GC counts
            //GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            GC.Collect(GC.MaxGeneration);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
                gcCounts[i] = GC.CollectionCount(i);

            // 3. Run action
            Stopwatch watch = new Stopwatch();
            watch.Start();
            long ticksFst = GetCurrentThreadTimes(); //100 nanosecond one tick

            for (int i = 0; i < iteration; i++)
                action();

            long ticks = GetCurrentThreadTimes() - ticksFst;
            watch.Stop();

            // 4. Print CPU
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine(String.Format("\tTime Elapsed:\t\t{0:N0}ms", watch.ElapsedMilliseconds));
            Console.WriteLine(String.Format("\tTime Elapsed (one time):{0:N0}ms", watch.ElapsedMilliseconds / iteration));
            Console.WriteLine(String.Format("\tCPU time:\t\t{0:N0}ns", ticks * 100));
            Console.WriteLine(String.Format("\tCPU time (one time):\t{0:N0}ns", ticks * 100 / iteration));

            // 5. Print GC
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine(String.Format("\tGen {0}: \t\t\t{1}", i, count));
            }

            Console.WriteLine();
        }

    }

}