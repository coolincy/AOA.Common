using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AOA.Common.Utility
{

    /// <summary>
    /// 代码执行计时器，Vista及2008之后版本使用
    /// </summary>
    public static class CodeTimer
    {

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool QueryThreadCycleTime(IntPtr threadHandle, ref ulong cycleTime);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetCurrentThread();

        private static ulong GetCycleCount()
        {
            ulong cycleCount = 0;
            QueryThreadCycleTime(GetCurrentThread(), ref cycleCount);
            return cycleCount;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void Initialize()
        {
            Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;
            Thread.CurrentThread.Priority = ThreadPriority.Highest;
            Time("", 1, () => { });

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
        public static void Time(string name, uint iteration, Action action)
        {
            if (String.IsNullOrEmpty(name) || action == null)
                return;

            // 1. Print name
            ConsoleColor currentForeColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(name);

            // 2. Record the latest GC counts
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            int[] gcCounts = new int[GC.MaxGeneration + 1];
            for (int i = 0; i <= GC.MaxGeneration; i++)
                gcCounts[i] = GC.CollectionCount(i);

            // 3. Run action
            Stopwatch watch = new Stopwatch();
            watch.Start();
            ulong cycleCount = GetCycleCount();

            for (int i = 0; i < iteration; i++)
                action();

            ulong cpuCycles = GetCycleCount() - cycleCount;
            watch.Stop();

            // 4. Print CPU
            Console.ForegroundColor = currentForeColor;
            Console.WriteLine(string.Format("\tTime Elapsed:\t{0:N0}ms", watch.ElapsedMilliseconds));
            Console.WriteLine(string.Format("\tTime Elapsed (one time):{0:N0}ms", watch.ElapsedMilliseconds / iteration));
            Console.WriteLine(string.Format("\tCPU Cycles:\t{0:N0}", cpuCycles));
            Console.WriteLine(string.Format("\tCPU Cycles (one time):\t{0:N0}", cpuCycles / iteration));

            // 5. Print GC
            for (int i = 0; i <= GC.MaxGeneration; i++)
            {
                int count = GC.CollectionCount(i) - gcCounts[i];
                Console.WriteLine(string.Format("\tGen {0}: \t\t{1}", i, count));
            }

            Console.WriteLine();
        }

    }

}
