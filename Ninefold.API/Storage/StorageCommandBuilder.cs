﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Reflection;
using Ninefold.Core;

namespace Ninefold.Storage
{
    public class StorageCommandBuilder : IStorageCommandBuilder
    {
        private const BindingFlags PropertyFilters = BindingFlags.Public | BindingFlags.Instance;

        public HttpWebRequest GenerateRequest(IStorageCommandRequest request, string userId, string requestMethod)
        {
            Validator.ValidateObject(request, new ValidationContext(request, null, null), true);
            
            var webRequest = (HttpWebRequest) WebRequest.Create(request.Resource);
            webRequest.Method = requestMethod.ToString();
            webRequest.ContentType = "application/octet-stream";

            var requestType = request.GetType();
            var properties = requestType.GetProperties(PropertyFilters)
                .Where(
                    p =>
                    (p.GetValue(request, null) != null) &&
                    (!string.IsNullOrWhiteSpace(p.GetValue(request, null).ToString())))
                .Where(p => p.GetCustomAttributes(false).OfType<HeaderAttribute>().Where(h => h.Serialise).Any())
                .Select(p => new
                                 {
                                     Name = p.GetCustomAttributes(false).OfType<HeaderAttribute>().Select(h => h.Name).First(),
                                     Value = p.GetValue(request, null).ToString()
                                 }).ToList();

            properties.Add(new { Name = "x-emc-date", Value = DateTime.UtcNow.ToString("r") });
            properties.Add(new { Name = "x-emc-uid", Value = userId });
            
            foreach (var property in properties)
            {
                webRequest.Headers.Add(property.Name, property.Value);
            }
            
            return webRequest;
        }

    }
}
