using System.ComponentModel;
using System.Text.Json;
using MorganStanley.ComposeUI.Messaging.Abstractions.Exceptions;

namespace MorganStanley.ComposeUI.Messaging.Abstractions
{
    public static class MessagingServiceJsonExtensions
    {
        public static ValueTask PublishJsonAsync<TPayload>(this IMessaging messaging, string topic, TPayload payload, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken = default)
        {
            var stringPayload = JsonSerializer.Serialize(payload, jsonSerializerOptions);
            return messaging.PublishAsync(topic, stringPayload, cancellationToken);
        }

        public static async ValueTask<TResult?> InvokeJsonServiceAsync<TPayload, TResult>(
            this IMessaging messaging,
            string serviceName,
            TPayload payload,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken = default)
        {
            var stringPayload = JsonSerializer.Serialize(payload, jsonSerializerOptions);
            var response = await messaging.InvokeServiceAsync(serviceName, stringPayload);

            if (response == null) { return default; }

            return JsonSerializer.Deserialize<TResult>(response, jsonSerializerOptions);
        }

        public static async ValueTask<TResult?> InvokeJsonServiceAsync<TResult>(this IMessaging messaging, string serviceName, JsonSerializerOptions jsonSerializerOptions, CancellationToken cancellationToken = default)
        {
            var response = await messaging.InvokeServiceAsync(serviceName, cancellationToken: cancellationToken);

            return response == null ? default : JsonSerializer.Deserialize<TResult>(response, jsonSerializerOptions);
        }

        public static ValueTask<IAsyncDisposable> RegisterJsonServiceAsync<TRequest, TResult>(
            this IMessaging messaging,
            string serviceName,
            Func<TRequest?, ValueTask<TResult?>> typedHandler,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken = default)
        {
            return messaging.RegisterServiceAsync(serviceName, CreateJsonServiceHandler(typedHandler, jsonSerializerOptions), cancellationToken);
        }

        private static ServiceHandler CreateJsonServiceHandler<TRequest, TResult>(Func<TRequest?, ValueTask<TResult>> realHandler, JsonSerializerOptions jsonSerializerOptions)
        {
            if (typeof(TRequest) == typeof(string))
            {
                throw new MessagingException("NonJsonService", "The handler provided has string as input. This extension does not support that use-case. Use CreateServiceHandler directly to register such a service.");
            }

            return async (payload) =>
            {
                var request = payload == null ? default : JsonSerializer.Deserialize<TRequest>(payload, jsonSerializerOptions);
                var result = await realHandler(request);

                if (result is string str)
                {
                    return str;
                }

                return JsonSerializer.Serialize(result, jsonSerializerOptions);
            };
        }
    }
}
