﻿/* Morgan Stanley makes this available to you under the Apache License, Version 2.0 (the "License"). You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0. See the NOTICE file distributed with this work for additional information regarding copyright ownership. Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License. */

namespace ProcessExplorer.Entities.Connections
{
    public class ConnectionMonitor 
    {
        public ConnectionMonitorDto Data { get; set; } = new ConnectionMonitorDto();
        public ConnectionMonitor()
        {
            Data.Connections = new List<ConnectionDto>();
        }
        public ConnectionMonitor(List<ConnectionDto> connections)
            => this.Data.Connections = connections;
        public void AddConnection(ConnectionDto connectionInfo)
            => Data.Connections.Add(connectionInfo);
        public void RemoveConnection(ConnectionDto connectionInfo)
            => Data?.Connections.Remove(connectionInfo);
        public void ChangeElement(ConnectionDto connection)
        {
            var temp = Data.Connections.ToList();
            var element = temp.FindIndex(conn => conn.Id == connection.Id);
            if (element != default)
            {
                temp[element] = connection;
                Data.Connections = temp;
            }
            else
            {
                AddConnection(connection);
            }
        }
        public ConnectionDto? GetConnection(ConnectionDto connection) 
            => Data.Connections.Where(conn => conn.Equals(connection)).FirstOrDefault();

        public List<ConnectionDto>? GetConnections()
            => Data.Connections;

        public void StatusChanged(ConnectionDto conn)
        {
            throw new NotImplementedException(); //triggering event tbc
        }
    }

    public class ConnectionMonitorDto
    {
        public List<ConnectionDto>? Connections { get; set; } = new List<ConnectionDto>();
    }
}
