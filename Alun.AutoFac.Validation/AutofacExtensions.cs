using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Autofac.Core;

namespace Autofac
{
    public static class AutofacExtensions
    {
        /// <summary>
        /// Lops  through all the services
        /// and tries to instantiate them. Any problems will be reported using assertFail.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="assertFail">called when problems are found</param>
        public static void AssertConfigurationIsValid(this IContainer container, Action<string> assertFail)
        {
            var problems = FindConfigurationProblems(container);
            if(problems.Count > 0)
                assertFail(
                    string.Join(
                    Environment.NewLine, 
                    problems.Select(ex => ex.Message).ToArray()));
        }

        /// <summary>
        /// Calls the other version of DebugAssertConfigurationIsValid using
        /// Debug.Assert as assertFail
        /// </summary>
        /// <param name="container">The container.</param>
        public static void AssertConfigurationIsValid(this IContainer container)
        {
            AssertConfigurationIsValid(container, s => Debug.Assert(false, s));
        }

        /// <summary>
        /// Tries to find any configuration problems
        /// by going through each of the services registered and trying to resolve them. 
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>A list of exceptions from resolve attemps or an empty list if no problems were found.</returns>
        public static IList<Exception> FindConfigurationProblems(this IContainer container)
        {
            var services = container
                .ComponentRegistry
                .Registrations
                .SelectMany(x => x.Services)
                .OfType<TypedService>()
                .Where(x => !x.ServiceType.Name.StartsWith("Autofac"))
                .ToList();
            var exceptions = new List<Exception>();
            foreach (var typedService in services)
            {
                try
                {
                    container.Resolve(typedService.ServiceType);
                }
                catch (DependencyResolutionException ex)
                {
                    exceptions.Add(ex);
                }
            }
            return exceptions;
        }
    }
}
