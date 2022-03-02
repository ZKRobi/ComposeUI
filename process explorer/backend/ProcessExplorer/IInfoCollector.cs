﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

using LocalCollector;
using ProcessExplorer.Entities;
using ProcessExplorer.Processes;
using System.Collections.Concurrent;

namespace ProcessExplorer
{
    public interface IInfoCollector
    {
        #region Properties
        public ConcurrentDictionary<string, InfoAggregatorDto>? Information { get; set; }
        public IProcessMonitor? ProcessMonitor { get; set; }
        #endregion

        #region Methods
        public void AddInformation(string assembly, InfoAggregatorDto info);
        public void Remove(string assembly);
        public void SetProcessMonitorMainPID(int pid);
        public SynchronizedCollection<ProcessInfoDto>? GetProcesses();
        public void SetSubribeUrl(string url);
        #endregion
    }
}
