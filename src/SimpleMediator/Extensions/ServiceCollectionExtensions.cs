using Microsoft.Extensions.DependencyInjection;
using SimpleMediator.Implementation;
using SimpleMediator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SimpleMediator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSimpleMediator(this IServiceCollection services, params object[] args)
        {
            Assembly[] assemblies = ResolveAssemblies(args);

            _ = services.AddSingleton<IMediator, Mediator>();

            RegisterHandlers(services, assemblies, typeof(INotificationHandler<>));
            RegisterHandlers(services, assemblies, typeof(IRequestHandler<,>));

            return services;
        }

        private static Assembly[] ResolveAssemblies(object[] args)
        {
            // Return ALL
            if (args == null || args.Length == 0)
            {
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.FullName))
                    .ToArray();
            }

            // Return all informed (same behavior as above)
            if (args.All(a => a is Assembly))
                return args.Cast<Assembly>().ToArray();

            // Return filtered by namespace (most performatic)
            if (args.All(a => a is string))
            {
                string[] prefixes = args.Cast<string>().ToArray();
                return AppDomain.CurrentDomain
                    .GetAssemblies()
                    .Where(a =>
                        !a.IsDynamic &&
                        !string.IsNullOrWhiteSpace(a.FullName) &&
                        prefixes.Any(p => a.FullName!.StartsWith(p)))
                    .ToArray();
            }

            throw new ArgumentException("Invalid parameters for AddSimpleMediator(). Use: no arguments, Assembly[], or prefix strings.");
        }

        private static void RegisterHandlers(IServiceCollection services, Assembly[] assemblies, Type handlerInterface)
        {
            List<Type> types = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract)
                .ToList();

            foreach (Type? type in types)
            {
                IEnumerable<Type> interfaces = type.GetInterfaces()
                    .Where(i =>
                        i.IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerInterface);

                foreach (Type? iface in interfaces)
                {
                    _ = services.AddTransient(iface, type);
                }
            }
        }
    }
}