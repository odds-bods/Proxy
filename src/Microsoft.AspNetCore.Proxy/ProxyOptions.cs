// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Http;

namespace Microsoft.AspNetCore.Proxy
{
    /// <summary>
    /// Proxy Options
    /// </summary>
    public class ProxyOptions
    {
        /// <summary>
        /// Destination uri scheme
        /// </summary>
        public string Scheme { get; set; }

        /// <summary>
        /// Destination uri host
        /// </summary>
        public HostString Host { get; set; }

        /// <summary>
        /// Destination uri path base to which current Path will be appended
        /// </summary>
        public PathString PathBase { get; set; }

        /// <summary>
        /// Query string parameters to append to each request
        /// </summary>
        public QueryString AppendQuery { get; set; }

        /// <summary>
        /// Infers the scheme and host from the original path i.e. localhost/https/www.foo.com/bar equates to https://www.foo.com/bar
        /// </summary>
        public bool UseDynamicSchemeAndHost { get; set; }
    }
}
