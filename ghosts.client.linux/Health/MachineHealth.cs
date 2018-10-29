﻿// Copyright 2017 Carnegie Mellon University. All Rights Reserved. See LICENSE.md file for terms.

using System;
using System.Diagnostics;
using System.IO;
using Ghosts.Domain;
using Ghosts.Domain.Code;
using NLog;

namespace ghosts.client.linux.Health
{
    public static class MachineHealth
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public static ResultHealth.MachineStats Run()
        {
            var stats = new ResultHealth.MachineStats();
            try
            {
                stats.Memory = GetMemory();
                stats.Cpu = GetCpu();
                stats.DiskSpace = GetDiskSpace();
                _log.Trace($"MEMORY: {stats.Memory} CPU: {stats.Cpu} DISK: {stats.DiskSpace}");
            }
            catch (Exception ex)
            {
                _log.Debug($"Health tasks failing: {ex}");
            }
            return stats;
        }

        public static float GetMemory()
        {
            float totalMemory = 0;
            foreach (var process in Process.GetProcesses())
            {
                totalMemory += process.PrivateMemorySize64;
            }
            return totalMemory;
        }

        public static float GetCpu()
        {
            var proc = Process.GetCurrentProcess();
            var cpu = proc.TotalProcessorTime;
            foreach (var process in Process.GetProcesses())
            {
                    //Console.WriteLine("Proc {0,30}  CPU {1,-20:n} msec", process.ProcessName, cpu.TotalMilliseconds);
            }
            return cpu.Ticks;
        }

        public static float GetDiskSpace()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == Path.GetPathRoot(ApplicationDetails.InstalledPath))
                {
                    return 1 - Convert.ToSingle(drive.AvailableFreeSpace) / Convert.ToSingle(drive.TotalSize);
                }
            }
            return -1;
        }
    }
}
