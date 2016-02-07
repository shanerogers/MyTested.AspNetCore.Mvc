﻿namespace MyTested.Mvc.Internal.Application
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.DependencyInjection;
    using Utilities.Validators;
    using Microsoft.AspNetCore.Mvc.Internal;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.Extensions.Options;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Provides global application services.
    /// </summary>
    public class TestServiceProvider
    {
        private static IDictionary<Type, IOutputFormatter> outputFormatters
            = new Dictionary<Type, IOutputFormatter>();

        /// <summary>
        /// Gets the current service provider.
        /// </summary>
        /// <value>Type of IServiceProvider.</value>
        public static IServiceProvider Current => TestApplication.Services;

        /// <summary>
        /// Gets required service. Throws exception if such is not found or there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance GetRequiredService<TInstance>()
        {
            ServiceValidator.ValidateServices();
            var service = Current.GetService<TInstance>();
            ServiceValidator.ValidateServiceExists(service);
            return service;
        }

        /// <summary>
        /// Gets service. Returns null if such is not found. Throws exception if there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance GetService<TInstance>()
        {
            ServiceValidator.ValidateServices();
            return Current.GetService<TInstance>();
        }

        /// <summary>
        /// Gets output formatter from the registered MVC options.
        /// </summary>
        /// <typeparam name="TFormatter">Type of formatter to get.</typeparam>
        /// <returns>Formatter of TFormatter type.</returns>
        public static TFormatter GetOutputFormatter<TFormatter>()
            where TFormatter : IOutputFormatter
        {
            var typeOfFormatter = typeof(TFormatter);
            if (!outputFormatters.ContainsKey(typeOfFormatter))
            {
                var mvcOptions = GetRequiredService<IOptions<MvcOptions>>();

                var formatter = mvcOptions.Value?.OutputFormatters?.FirstOrDefault(f => f.GetType() == typeOfFormatter);
                ServiceValidator.ValidateServiceExists(formatter);

                outputFormatters.Add(typeOfFormatter, formatter);
            }

            return (TFormatter)outputFormatters[typeOfFormatter];
        }

        /// <summary>
        /// Gets collection of services. Returns null if no service of this type is not found. Throws exception if there are no registered services.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static IEnumerable<TInstance> GetServices<TInstance>()
        {
            ServiceValidator.ValidateServices();
            return Current.GetServices<TInstance>();
        }

        /// <summary>
        /// Tries to get service. Returns null if such is not found or no services are registered.
        /// </summary>
        /// <typeparam name="TInstance">Type of requested service.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance TryGetService<TInstance>()
            where TInstance : class
        {
            return Current.GetService<TInstance>();
        }

        /// <summary>
        /// Tries to create instance of the provided type. Returns null if not successful.
        /// </summary>
        /// <typeparam name="TInstance">Type to create.</typeparam>
        /// <returns>Instance of TInstance type.</returns>
        public static TInstance TryCreateInstance<TInstance>()
            where TInstance : class
        {
            try
            {
                var typeActivatorCache = GetRequiredService<ITypeActivatorCache>();
                return typeActivatorCache.CreateInstance<TInstance>(Current, typeof(TInstance));
            }
            catch
            {
                return null;
            }
        }
    }
}