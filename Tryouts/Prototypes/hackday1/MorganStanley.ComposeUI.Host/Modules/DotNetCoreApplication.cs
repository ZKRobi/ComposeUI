﻿/*
* Morgan Stanley makes this available to you under the Apache License,
* Version 2.0 (the "License"). You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0.
*
* See the NOTICE file distributed with this work for additional information
* regarding copyright ownership. Unless required by applicable law or agreed
* to in writing, software distributed under the License is distributed on an
* "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
* or implied. See the License for the specific language governing permissions
* and limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using ComposeUI.Messaging.Client;
using MorganStanley.ComposeUI.Interfaces;

namespace MorganStanley.ComposeUI.Host.Modules;

internal class DotNetCoreApplication : IApplication
{
    private Process _process = new Process();
    private readonly string _path;

    public DotNetCoreApplication(string path)
    {
        _path = path;
    }

    public Task<bool> ClosingRequested()
    {
        return Task.FromResult(true);
    }

    public Task Initialize(IMessageRouter messageRouter)
    {
        _process = new Process();
        _process.StartInfo.UseShellExecute = false;
        
        var location = Path.GetFullPath(_path);
        _process.StartInfo.FileName = "dotnet";
        _process.StartInfo.ArgumentList.Add(location);
        _process.StartInfo.ArgumentList.Add("ws://localhost:5000/ws");
        _process.StartInfo.WorkingDirectory = Path.GetDirectoryName(location);
        _process.Start();
        return Task.CompletedTask;
    }

    public void Render(ContentPresenter target)
    {
        
    }

    public async Task Teardown()
    {
        _process.CloseMainWindow();
        await _process.WaitForExitAsync();
    }
}