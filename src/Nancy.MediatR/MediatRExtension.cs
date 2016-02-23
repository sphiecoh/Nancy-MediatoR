using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Nancy.MediatR
{
    public static class MediatRExtension
    {
        public static IEnumerable<object> GetRequiredServices(this IServiceProvider provider, Type serviceType)
        {
            return (IEnumerable<object>)provider.GetRequiredService(typeof(IEnumerable<>).MakeGenericType(serviceType));
        }
    }
}
