// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.AspNetCore.Proxy
{
    /// <summary>
    /// Proxy Middleware
    /// </summary>
    public class ProxyMiddleware
    {
        private const int DefaultWebSocketBufferSize = 4096;

        private readonly RequestDelegate _next;
        private readonly ProxyOptions _options;

        private static readonly string[] NotForwardedWebSocketHeaders = new[] { "Connection", "Host", "Upgrade", "Sec-WebSocket-Key", "Sec-WebSocket-Version" };

        public ProxyMiddleware(RequestDelegate next, IOptions<ProxyOptions> options)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (options.Value.Scheme == null)
            {
                throw new ArgumentException("Options parameter must specify scheme.", nameof(options));
            }
            if (!options.Value.Host.HasValue)
            {
                throw new ArgumentException("Options parameter must specify host.", nameof(options));
            }

            _next = next;
            _options = options.Value;
        }

        public Task Invoke(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            Uri uri = TryBuildDynamicUri(context);

            if (uri == null)
            {
                uri = new Uri(UriHelper.BuildAbsolute(_options.Scheme, _options.Host, _options.PathBase, context.Request.Path, context.Request.QueryString.Add(_options.AppendQuery)));
            }

            return context.ProxyRequest(uri);
        }

        private Uri TryBuildDynamicUri(HttpContext context)
        {
            if (!_options.UseDynamicSchemeAndHost)
            {
                return null;
            }
            
            var steps = GetPathSteps(3, context);

            if (steps == null)
            {
                return null;
            }

            var scheme = steps[0];
            var host = steps[1];
            var path = steps[2];

            path = path == null ? null : $"/{path.TrimStart('/')}";

            return new Uri(UriHelper.BuildAbsolute(scheme, new HostString(host, scheme == "https" ? 443 : 80), _options.PathBase, new PathString(path), context.Request.QueryString.Add(_options.AppendQuery)));
        }

        private string[] GetPathSteps(int stepCount, HttpContext context)
        {
            var pathTrimmed = context.Request.Path.Value.TrimStart('/');
            var pathTrimmedSplit = pathTrimmed.Split('/');

            if (pathTrimmedSplit.Length < 2)
            {
                return null;
            }

            var steps = new string[stepCount];
            steps[0] = pathTrimmedSplit[0];
            steps[1] = pathTrimmedSplit[1];

            if (pathTrimmedSplit.Length > 2)
            {
                var startIndex = steps[0].Length + steps[1].Length + 2;
                steps[2] = pathTrimmed.Substring(startIndex, pathTrimmed.Length - startIndex);
            }

            return steps;
        }
    }
}
