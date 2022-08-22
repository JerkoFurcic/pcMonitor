using System;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;



namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            PerformanceCounter cpuCounter;
            PerformanceCounter ramCounter;

            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            


            Console.WriteLine("Computer CPU Utilization rate:" + cpuCounter.NextValue() + "%");
            Console.WriteLine("The computer can use memory:" + ramCounter.NextValue() + "MB");
            Console.WriteLine("Total memory:" + FormatSize(GetTotalPhys()));
            Console.WriteLine("Has been used:" + FormatSize(GetUsedPhys()));
            Console.ReadKey();
            Console.WriteLine();

            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("Computer CPU Utilization rate:" + cpuCounter.NextValue() + " %");
                Console.WriteLine("The computer can use memory:" + ramCounter.NextValue() + "MB");
                Console.WriteLine("Total memory:" + FormatSize(GetTotalPhys()));
                Console.WriteLine("Has been used:" + FormatSize(GetUsedPhys()));
                Console.WriteLine();

                if ((int)cpuCounter.NextValue() > 80)
                {
                    System.Threading.Thread.Sleep(1000 * 60);
                }
            }
        }

        #region Obtain memory information API
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatusEx(ref MEMORY_INFO mi);

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_INFO
        {
            public uint dwLength; //Current structure size
            public uint dwMemoryLoad; //Current memory utilization
            public ulong ullTotalPhys; //Total physical memory size
            public ulong ullAvailPhys; //Available physical memory size
            public ulong ullTotalPageFile; //Total Exchange File Size
            public ulong ullAvailPageFile; //Total Exchange File Size
            public ulong ullTotalVirtual; //Total virtual memory size
            public ulong ullAvailVirtual; //Available virtual memory size
            public ulong ullAvailExtendedVirtual; //Keep this value always zero
        }
        #endregion

        #region Formatting capacity size
        private static string FormatSize(double size)
        {
            double d = (double)size;
            int i = 0;
            while ((d > 1024) && (i < 5))
            {
                d /= 1024;
                i++;
            }
            string[] unit = { "B", "KB", "MB", "GB", "TB" };
            return (string.Format("{0} {1}", Math.Round(d, 2), unit[i]));
        }
        #endregion

        #region Get the current memory usage
       public static MEMORY_INFO GetMemoryStatus()
        {
            MEMORY_INFO mi = new MEMORY_INFO();
            mi.dwLength = (uint)System.Runtime.InteropServices.Marshal.SizeOf(mi);
            GlobalMemoryStatusEx(ref mi);
            return mi;
        }
        #endregion

        #region Get the current available physical memory size
        public static ulong GetAvailPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return mi.ullAvailPhys;
        }
        #endregion

        #region Get the current memory size used
        public static ulong GetUsedPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return (mi.ullTotalPhys - mi.ullAvailPhys);
        }
        #endregion

        #region Get the current total physical memory size
        public static ulong GetTotalPhys()
        {
            MEMORY_INFO mi = GetMemoryStatus();
            return mi.ullTotalPhys;
        }
        #endregion
    }
}

