using DistributedLoopDetector;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;

namespace SPS.DistributedLoopDetector.Extensions
{
    public static class HttpClientExtension
    {
        //
        // Summary:
        //     Send a DELETE request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //     -or- The requestUri is not an absolute URI. -or- System.Net.Http.HttpClient.BaseAddress
        //     is not set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> DeleteDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.DeleteAsync(requestUri);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            return ret!;
        }

        //
        // Summary:
        //     Send a DELETE request to the specified Uri with a cancellation token as an asynchronous
        //     operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //     -or- The requestUri is not an absolute URI. -or- System.Net.Http.HttpClient.BaseAddress
        //     is not set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> DeleteDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.DeleteAsync(requestUri, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a DELETE request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //     -or- The requestUri is not an absolute URI. -or- System.Net.Http.HttpClient.BaseAddress
        //     is not set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> DeleteDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.DeleteAsync(requestUri);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a DELETE request to the specified Uri with a cancellation token as an asynchronous
        //     operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //     -or- The requestUri is not an absolute URI. -or- System.Net.Http.HttpClient.BaseAddress
        //     is not set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> DeleteDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.DeleteAsync(requestUri, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with an HTTP completion option as an
        //     asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   completionOption:
        //     An HTTP completion option value that indicates when the operation should be considered
        //     completed.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpCompletionOption completionOption, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, completionOption);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with an HTTP completion option and a
        //     cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   completionOption:
        //     An HTTP completion option value that indicates when the operation should be considered
        //     completed.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, completionOption, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with a cancellation token as an asynchronous
        //     operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with an HTTP completion option as an
        //     asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   completionOption:
        //     An HTTP completion option value that indicates when the operation should be considered
        //     completed.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpCompletionOption completionOption, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, completionOption);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with an HTTP completion option and a
        //     cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   completionOption:
        //     An HTTP completion option value that indicates when the operation should be considered
        //     completed.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, completionOption, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a GET request to the specified Uri with a cancellation token as an asynchronous
        //     operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> GetDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.GetAsync(requestUri, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Sends a GET request to the specified Uri and return the response body as a byte
        //     array in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<byte[]> GetByteArrayDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            byte[]? ret;
            try
            {
                ret = await httpClient.GetByteArrayAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Sends a GET request to the specified Uri and return the response body as a byte
        //     array in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<byte[]> GetByteArrayDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            byte[]? ret;
            try
            {
                ret = await httpClient.GetByteArrayAsync(requestUri, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a byte
        //     array in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<byte[]> GetByteArrayDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            byte[]? ret;
            try
            {
                ret = await httpClient.GetByteArrayAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a byte
        //     array in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<byte[]> GetByteArrayDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            byte[]? ret;
            try
            {
                ret = await httpClient.GetByteArrayAsync(requestUri, cancellationToken);

            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a stream
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<Stream> GetStreamDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            Stream? ret;
            try
            {
                ret = await httpClient.GetStreamAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a stream
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public static async Task<Stream> GetStreamDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            Stream? ret;
            try
            {
                ret = await httpClient.GetStreamAsync(requestUri, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a stream
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<Stream> GetStreamDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            Stream? ret;
            try
            {
                ret = await httpClient.GetStreamAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a stream
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri is null.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<Stream> GetStreamDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            Stream? ret;
            try
            {
                ret = await httpClient.GetStreamAsync(requestUri, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<string> GetStringDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            string? ret;
            try
            {
                ret = await httpClient.GetStringAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri is null.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<string> GetStringDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            string? ret;
            try
            {
                ret = await httpClient.GetStringAsync(requestUri, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<string> GetStringDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            string? ret;
            try
            {
                ret = await httpClient.GetStringAsync(requestUri);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Send a GET request to the specified Uri and return the response body as a string
        //     in an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   cancellationToken:
        //     The cancellation token to cancel the operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The requestUri is null.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation (or timeout for .NET Framework only).
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<string> GetStringDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            string? ret;
            try
            {
                ret = await httpClient.GetStringAsync(requestUri, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Sends a PATCH request to a Uri designated as a string as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public static async Task<HttpResponseMessage> PatchDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PatchAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Sends a PATCH request with a cancellation token to a Uri represented as a string
        //     as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public static async Task<HttpResponseMessage> PatchDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PatchAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Sends a PATCH request as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public static async Task<HttpResponseMessage> PatchDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PatchAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Sends a PATCH request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        public static async Task<HttpResponseMessage> PatchDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PatchAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a POST request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PostDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PostAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a POST request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PostDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PostAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a POST request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PostDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PostAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a POST request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PostDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PostAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a PUT request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PutDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PutAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a PUT request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PutDLoopDAsync(this System.Net.Http.HttpClient httpClient, string? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PutAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a PUT request to the specified Uri as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PutDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PutAsync(requestUri, content);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send a PUT request with a cancellation token as an asynchronous operation.
        //
        // Parameters:
        //   requestUri:
        //     The Uri the request is sent to.
        //
        //   content:
        //     The HTTP request content sent to the server.
        //
        //   cancellationToken:
        //     A cancellation token that can be used by other objects or threads to receive
        //     notice of cancellation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.InvalidOperationException:
        //     The requestUri must be an absolute URI or System.Net.Http.HttpClient.BaseAddress
        //     must be set.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> PutDLoopDAsync(this System.Net.Http.HttpClient httpClient, Uri? requestUri, HttpContent? content, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(httpClient.DefaultRequestHeaders, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.PutAsync(requestUri, content, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Sends an HTTP request with the specified request.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        // Returns:
        //     An HTTP response message.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.NotSupportedException:
        //     The HTTP version is 2.0 or higher or the version policy is set to System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher.
        //     -or- The custom class derived from System.Net.Http.HttpContent does not override
        //     the System.Net.Http.HttpContent.SerializeToStream(System.IO.Stream,System.Net.TransportContext,System.Threading.CancellationToken)
        //     method. -or- The custom System.Net.Http.HttpMessageHandler does not override
        //     the System.Net.Http.HttpMessageHandler.Send(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)
        //     method.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, or server certificate validation.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     If the System.Threading.Tasks.TaskCanceledException exception nests the System.TimeoutException:
        //     The request failed due to timeout.
        public static HttpResponseMessage SendDLoopD(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = httpClient.Send(request);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Sends an HTTP request.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   completionOption:
        //     One of the enumeration values that specifies when the operation should complete
        //     (as soon as a response is available or after reading the response content).
        //
        // Returns:
        //     The HTTP response message.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.NotSupportedException:
        //     The HTTP version is 2.0 or higher or the version policy is set to System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher.
        //     -or- The custom class derived from System.Net.Http.HttpContent does not override
        //     the System.Net.Http.HttpContent.SerializeToStream(System.IO.Stream,System.Net.TransportContext,System.Threading.CancellationToken)
        //     method. -or- The custom System.Net.Http.HttpMessageHandler does not override
        //     the System.Net.Http.HttpMessageHandler.Send(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)
        //     method.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, or server certificate validation.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     If the System.Threading.Tasks.TaskCanceledException exception nests the System.TimeoutException:
        //     The request failed due to timeout.
        public static HttpResponseMessage SendDLoopD(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = httpClient.Send(request, completionOption);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Sends an HTTP request with the specified request, completion option and cancellation
        //     token.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   completionOption:
        //     One of the enumeration values that specifies when the operation should complete
        //     (as soon as a response is available or after reading the response content).
        //
        //   cancellationToken:
        //     The token to cancel the operation.
        //
        // Returns:
        //     The HTTP response message.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.NotSupportedException:
        //     The HTTP version is 2.0 or higher or the version policy is set to System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher.
        //     -or- The custom class derived from System.Net.Http.HttpContent does not override
        //     the System.Net.Http.HttpContent.SerializeToStream(System.IO.Stream,System.Net.TransportContext,System.Threading.CancellationToken)
        //     method. -or- The custom System.Net.Http.HttpMessageHandler does not override
        //     the System.Net.Http.HttpMessageHandler.Send(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)
        //     method.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, or server certificate validation.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     The request was canceled. -or- If the System.Threading.Tasks.TaskCanceledException
        //     exception nests the System.TimeoutException: The request failed due to timeout.
        public static HttpResponseMessage SendDLoopD(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = httpClient.Send(request, completionOption, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
#if !NETCOREAPP3_1 
        //
        // Summary:
        //     Sends an HTTP request with the specified request and cancellation token.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   cancellationToken:
        //     The token to cancel the operation.
        //
        // Returns:
        //     The HTTP response message.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.NotSupportedException:
        //     The HTTP version is 2.0 or higher or the version policy is set to System.Net.Http.HttpVersionPolicy.RequestVersionOrHigher.
        //     -or- The custom class derived from System.Net.Http.HttpContent does not override
        //     the System.Net.Http.HttpContent.SerializeToStream(System.IO.Stream,System.Net.TransportContext,System.Threading.CancellationToken)
        //     method. -or- The custom System.Net.Http.HttpMessageHandler does not override
        //     the System.Net.Http.HttpMessageHandler.Send(System.Net.Http.HttpRequestMessage,System.Threading.CancellationToken)
        //     method.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, or server certificate validation.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     The request was canceled. -or- If the System.Threading.Tasks.TaskCanceledException
        //     exception nests the System.TimeoutException: The request failed due to timeout.
        public static HttpResponseMessage SendDLoopD(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = httpClient.Send(request, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }
#endif
        //
        // Summary:
        //     Send an HTTP request as an asynchronous operation.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> SendDLoopDAsync(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.SendAsync(request);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send an HTTP request as an asynchronous operation.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   completionOption:
        //     When the operation should complete (as soon as a response is available or after
        //     reading the whole response content).
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> SendDLoopDAsync(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.SendAsync(request, completionOption);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send an HTTP request as an asynchronous operation.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   completionOption:
        //     When the operation should complete (as soon as a response is available or after
        //     reading the whole response content).
        //
        //   cancellationToken:
        //     The cancellation token to cancel operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> SendDLoopDAsync(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.SendAsync(request, completionOption, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

        //
        // Summary:
        //     Send an HTTP request as an asynchronous operation.
        //
        // Parameters:
        //   request:
        //     The HTTP request message to send.
        //
        //   cancellationToken:
        //     The cancellation token to cancel operation.
        //
        // Returns:
        //     The task object representing the asynchronous operation.
        //
        // Exceptions:
        //   T:System.ArgumentNullException:
        //     The request is null.
        //
        //   T:System.InvalidOperationException:
        //     The request message was already sent by the System.Net.Http.HttpClient instance.
        //
        //   T:System.Net.Http.HttpRequestException:
        //     The request failed due to an underlying issue such as network connectivity, DNS
        //     failure, server certificate validation or timeout.
        //
        //   T:System.Threading.Tasks.TaskCanceledException:
        //     .NET Core and .NET 5.0 and later only: The request failed due to timeout.
        public static async Task<HttpResponseMessage> SendDLoopDAsync(this System.Net.Http.HttpClient httpClient, HttpRequestMessage request, CancellationToken cancellationToken, IHttpContextAccessor httpContextAccessor)
        {
            LoopDetectorHandler.GetLoopIdFromItemsAddToHeader(request.Headers, httpContextAccessor.HttpContext.Items);
            HttpResponseMessage? ret;
            try
            {
                ret = await httpClient.SendAsync(request, cancellationToken);

                if ((ret != null) && ((ret.StatusCode == HttpStatusCode.RequestTimeout) || (ret.StatusCode == HttpStatusCode.GatewayTimeout)))
                {
                    if (httpContextAccessor?.HttpContext?.Request?.Headers != null)
                    {
                        httpContextAccessor.HttpContext.Request.Headers.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                    }
                }
            }
            catch (TaskCanceledException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }
            catch (TimeoutException)
            {
                if (httpContextAccessor?.HttpContext?.Items != null)
                {
                    httpContextAccessor.HttpContext.Items.TryAdd(HttpStatusCode.RequestTimeout.ToString(), true.ToString());
                }
                throw;
            }

            return ret!;
        }

    }
}
