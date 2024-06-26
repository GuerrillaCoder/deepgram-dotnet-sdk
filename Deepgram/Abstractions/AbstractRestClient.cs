﻿// Copyright 2021-2024 Deepgram .NET SDK contributors. All Rights Reserved.
// Use of this source code is governed by a MIT license that can be found in the LICENSE file.
// SPDX-License-Identifier: MIT

using Deepgram.Encapsulations;
using Deepgram.Models.Authenticate.v1;

namespace Deepgram.Abstractions;

public abstract class AbstractRestClient
{
    /// <summary>
    ///  HttpClient created by the factory
    internal HttpClient _httpClient;

    /// <summary>
    /// get the logger of category type of _clientName
    /// </summary>
    internal ILogger _logger => LogProvider.GetLogger(this.GetType().Name);

    /// <summary>
    /// Constructor that take the options and a httpClient
    /// </summary>
    /// <param name="deepgramClientOptions"><see cref="_deepgramClientOptions"/>Options for the Deepgram client</param>

    internal AbstractRestClient(string apiKey = "", DeepgramClientOptions? options = null)
    {
        options ??= new DeepgramClientOptions();
        _httpClient = HttpClientFactory.Create();
        _httpClient = HttpClientFactory.ConfigureDeepgram(_httpClient, apiKey, options);
    }

    /// <summary>
    /// GET Rest Request
    /// </summary>
    /// <typeparam name="T">Type of class of response expected</typeparam>
    /// <param name="uriSegment">request uri Endpoint</param>
    /// <returns>Instance of T</returns>
    public virtual async Task<T> GetAsync<T>(string uriSegment, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Get, QueryParameterUtil.AppendQueryParameters(uriSegment, addons));

            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await RequestContentUtil.DeserializeAsync<T>(response);
            return result;
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "GET", ex);
            throw;
        }
    }

    /// <summary>
    /// Post method 
    /// </summary>
    /// <typeparam name="T">Class type of what return type is expected</typeparam>
    /// <param name="uriSegment">Uri for the api including the query parameters</param> 
    /// <param name="content">HttpContent as content for HttpRequestMessage</param>  
    /// <returns>Instance of T</returns>
    public virtual async Task<(Dictionary<string, string>, MemoryStream)> PostFileAsync<T>(string uriSegment, HttpContent content, List<string> keys, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, QueryParameterUtil.AppendQueryParameters(uriSegment, addons)) { Content = content };
            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();

            var result = new Dictionary<string, string>();

            for (int i = 0; i < response.Headers.Count(); i++) 
            {
                var key = response.Headers.ElementAt(i).Key.ToLower();
                var value = response.Headers.GetValues(key).FirstOrDefault() ?? "";

                var index = key.IndexOf("x-dg-");
                if (index == 0)
                {
                    var newKey = key.Substring(5);
                    if (keys.Contains(newKey))
                    {
                        result.Add(newKey, value);
                        continue;
                    }
                }
                index = key.IndexOf("dg-");
                if (index == 0)
                {
                    var newKey = key.Substring(3);
                    if (keys.Contains(newKey))
                    {
                        result.Add(newKey, value);
                        continue;
                    }
                }
                if (keys.Contains(key))
                {
                    result.Add(key, value);
                }
            }

            if (keys.Contains("content-type"))
            {
                result.Add("content-type", response.Content.Headers.ContentType?.MediaType ?? "");
            }

            MemoryStream stream = new MemoryStream();
            await response.Content.CopyToAsync(stream);

            return (result, stream);
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "POST", ex);
            throw;
        }
    }

    /// <summary>
    /// Post method 
    /// </summary>
    /// <typeparam name="T">Class type of what return type is expected</typeparam>
    /// <param name="uriSegment">Uri for the api including the query parameters</param> 
    /// <param name="content">HttpContent as content for HttpRequestMessage</param>  
    /// <returns>Instance of T</returns>
    public virtual async Task<T> PostAsync<T>(string uriSegment, HttpContent content, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Post, QueryParameterUtil.AppendQueryParameters(uriSegment, addons)) { Content = content };
            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await RequestContentUtil.DeserializeAsync<T>(response);

            return result;
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "POST", ex);
            throw;
        }

    }


    /// <summary>
    /// Delete Method for use with calls that do not expect a response
    /// </summary>
    /// <param name="uriSegment">Uri for the api including the query parameters</param> 
    public virtual async Task DeleteAsync(string uriSegment, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, QueryParameterUtil.AppendQueryParameters(uriSegment, addons));
            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "DELETE", ex);
            throw;
        }
    }

    /// <summary>
    /// Delete method that returns the type of response specified
    /// </summary>
    /// <typeparam name="T">Class Type of expected response</typeparam>
    /// <param name="uriSegment">Uri for the api including the query parameters</param>      
    /// <returns>Instance  of T or throws Exception</returns>
    public virtual async Task<T> DeleteAsync<T>(string uriSegment, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, QueryParameterUtil.AppendQueryParameters(uriSegment, addons));
            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await RequestContentUtil.DeserializeAsync<T>(response);

            return result;
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "DELETE ASYNC", ex);
            throw;
        }
    }

    /// <summary>
    /// Patch method call that takes a body object
    /// </summary>
    /// <typeparam name="T">Class type of what return type is expected</typeparam>
    /// <param name="uriSegment">Uri for the api including the query parameters</param>  
    /// <returns>Instance of T</returns>
    public virtual async Task<T> PatchAsync<T>(string uriSegment, StringContent content, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
#if NETSTANDARD2_0
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), QueryParameterUtil.AppendQueryParameters(uriSegment, addons)) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);
#else
            var request = new HttpRequestMessage(HttpMethod.Patch, QueryParameterUtil.AppendQueryParameters(uriSegment, addons)) { Content = content };
            var response = await _httpClient.SendAsync(request, cancellationToken);

#endif
            response.EnsureSuccessStatusCode();
            var result = await RequestContentUtil.DeserializeAsync<T>(response);
            return result;

        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "PATCH", ex);
            throw;
        }

    }

    /// <summary>
    /// Put method call that takes a body object
    /// </summary>
    /// <typeparam name="T">Class type of what return type is expected</typeparam>
    /// <param name="uriSegment">Uri for the api</param>   
    /// <returns>Instance of T</returns>
    public virtual async Task<T> PutAsync<T>(string uriSegment, StringContent content, CancellationToken cancellationToken = default, Dictionary<string, string>? addons = null)
    {
        try
        {
            var req = new HttpRequestMessage(HttpMethod.Put, QueryParameterUtil.AppendQueryParameters(uriSegment, addons))
            {
                Content = content
            };
            var response = await _httpClient.SendAsync(req, cancellationToken);
            response.EnsureSuccessStatusCode();
            var result = await RequestContentUtil.DeserializeAsync<T>(response);

            return result;
        }
        catch (Exception ex)
        {
            Log.Exception(_logger, "PUT", ex);
            throw;
        }
    }


    /// <summary>
    /// Allow for the setting of a HttpClient timeout, timeout will change for any future calls 
    /// calls that are currently running will not be affected
    /// </summary>
    /// <param name="timeout"></param>
    public void SpecifyTimeOut(TimeSpan timeout)
    {
        _httpClient.Timeout = timeout;
    }
}

