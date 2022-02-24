﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using LocalCollector.Processes;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ProcessExplorer.Entities
{
    public enum Status
    {
        Running = 1,
        Stopped = 0
    }

    public class ProcessInfo
    {
        public ProcessInfoManager infoGenerator;
        public ProcessInfo(int processId, ProcessInfoManager manager)
            : this(Process.GetProcessById(processId), manager)
        {

        }
        public ProcessInfo(Process process, ProcessInfoManager manager)
        {
            Data = new ProcessInfoDto();
            infoGenerator = manager;
            process.Refresh();
            if (process != null && process.Id != 0)
            {
                try
                {
                    Data.StartTime = process.StartTime.ToString("yyyy.mm.dd. hh:mm:s");
                    Data.ProcessorUsageTime = process.TotalProcessorTime;
                    Data.PhysicalMemoryUsageBit = process.WorkingSet64;
                    UserProcessorTime = process.UserProcessorTime;
                    Data.ProcessPriorityClass = process.PriorityClass.ToString();
                    Data.VirtualMemorySize = process.VirtualMemorySize64;

                    var list = new List<ProcessThreadInfoDto>();
                    foreach (ProcessThread processThread in process.Threads)
                    {
                        list.Add(new ProcessThreadInfo(processThread).Data);
                    }
                    Data.Threads = list;
                    Data.ProcessStatus = process.HasExited == false ? Status.Running.ToString() : Status.Stopped.ToString();
                }
                catch (Exception)
                {
                    Data.ProcessStatus = Status.Stopped.ToString();
                }
                finally
                {
                    Data.PID = process.Id;
                    Data.ProcessName = process.ProcessName;
                    Data.PriorityLevel = process.BasePriority;
                    PagedMemoryUsage = process.PagedMemorySize64;
                    NonPagedMemoryUsage = process.NonpagedSystemMemorySize64;
                    PagedSystemMemoryUsage = process.PagedSystemMemorySize64;
                    NonPagedSystemMemoryUsage = process.NonpagedSystemMemorySize64;
                    Data.PrivateMemoryUsage = process.PrivateMemorySize64;
                    IsLinux = (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux));
                    Data.Children = infoGenerator.GetChildProcesses(process);
                    Data.ParentId = infoGenerator.GetParentId(process);
                    Data.MemoryUsage = infoGenerator.GetMemoryUsage(process);
                    Data.ProcessorUsage = infoGenerator.GetCPUUsage(process);
                }
            }
        }

        public ProcessInfoDto? Data { get; set; }
        public bool IsLinux { get; internal set; } = false;
        public long? PagedMemoryUsage { get; internal set; } = default;
        public long? NonPagedMemoryUsage { get; internal set; } = default;
        public long? PagedSystemMemoryUsage { get; internal set; } = default;
        public long? NonPagedSystemMemoryUsage { get; internal set; } = default;
        public TimeSpan? UserProcessorTime { get; internal set; } = default;
    }

    public class ProcessThreadInfo
    {
        public ProcessThreadInfoDto Data { get; set; }
        public ProcessThreadInfo(ProcessThread processThread)
        {
            Data = new ProcessThreadInfoDto();
            if (processThread != null)
            {
                Data.StartTime = processThread.StartTime.ToString();
                Data.PriorityLevel = processThread.CurrentPriority;
                Data.Id = processThread.Id;
                Data.Status = processThread.ThreadState.ToString();
                Data.ProcessorUsageTime = processThread.TotalProcessorTime;
                Data.WaitReason = processThread.WaitReason.ToString();
            }
        }
    }

    public class ProcessThreadInfoDto
    {
        public string? StartTime { get; internal set; } = default;
        public int? PriorityLevel { get; internal set; } = default;
        public int? Id { get; internal set; } = default;
        public string? Status { get; internal set; } = default;
        public TimeSpan? ProcessorUsageTime { get; internal set; } = default;
        public string? WaitReason { get; internal set; } = default;
    }

    public class ProcessInfoDto
    {
        public string? StartTime { get; internal set; } = default;
        public TimeSpan? ProcessorUsageTime { get; internal set; } = default;
        public long PhysicalMemoryUsageBit { get; internal set; } = default;
        public string? ProcessName { get; internal set; } = default;
        public int PID { get; internal set; } = default;
        public int PriorityLevel { get; internal set; } = default;
        public string ProcessPriorityClass { get; internal set; } = default;
        public List<ProcessThreadInfoDto>? Threads { get; set; } = new List<ProcessThreadInfoDto>();
        public long VirtualMemorySize { get; internal set; } = default;
        public int? ParentId { get; internal set; } = null;
        public long PrivateMemoryUsage { get; internal set; } = default;
        public string ProcessStatus { get; internal set; } = Status.Running.ToString();
        public List<ProcessInfoDto>? Children { get; internal set; } = new List<ProcessInfoDto>();
        public float MemoryUsage { get; internal set; } = default;
        public float ProcessorUsage { get; internal set; } = default;
    }
}
