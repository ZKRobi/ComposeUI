// Morgan Stanley makes this available to you under the Apache License,
// Version 2.0 (the "License"). You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0.
// 
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership. Unless required by applicable law or agreed
// to in writing, software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
// or implied. See the License for the specific language governing permissions
// and limitations under the License.

namespace MorganStanley.ComposeUI.Messaging.Abstractions;

public interface IMessaging
{
    /// <summary>
    /// Gets the client ID of the current connection.
    /// </summary>
    /// <remarks>
    /// The returned value will be <value>null</value> if the client is not connected.
    /// </remarks>
    public string? ClientId { get; }

    /// <summary>
    /// Gets an observable that represents a topic.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="subscriber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<IAsyncDisposable> SubscribeAsync(string topic, 
        MessageHandler subscriber, 
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message to a topic.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="payload"></param>
    /// <param name="options"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask PublishAsync(string topic, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Registers a service by providing a name and handler.
    /// </summary>
    /// <param name="serviceName"></param>
    /// <param name="subscriber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public ValueTask<IAsyncDisposable> RegisterServiceAsync(string serviceName, ServiceHandler subscriber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Invoke a named service.
    /// </summary>
    /// <param name="serviceName">The name of the service registered to Messaging via RegisterServiceAsync</param>
    /// <param name="payload">Data to pass on to the service</param>
    /// <param name="cancellationToken"></param>
    /// <returns>The response from the service</returns>
    public ValueTask<string?> InvokeServiceAsync(string serviceName, string? payload = null, CancellationToken cancellationToken = default);
}