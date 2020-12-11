using Buildersoft.Andy.X.Client.Configurations;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Buildersoft.Andy.X.Client
{
    public class Writer<TEntity> where TEntity : class
    {
        private readonly HttpClient _client;
        private readonly ILogger _logger;
        private readonly AndyXOptions _andyXOptions;

        public Writer(AndyXClient andyClient)
        {
            _andyXOptions = AndyXOptions.Create(andyClient.GetAndyXOptions());

            _logger = _andyXOptions.Logger.GetLoggerFactory().CreateLogger<Writer<TEntity>>();


            _client = new HttpClient(_andyXOptions.HttpClientHandler);
            if (_andyXOptions.Token != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant-Authorization", $"Bearer {_andyXOptions.Token}");

            if (_andyXOptions.Tenant != "")
                _client.DefaultRequestHeaders.Add("x-andy-x-tenant", _andyXOptions.Tenant);
        }

        public Writer<TEntity> Component(string component)
        {
            _andyXOptions.Component = component;
            return this;
        }

        public Writer<TEntity> Book(string book)
        {
            _andyXOptions.Book = book;
            return this;
        }

        public Writer<TEntity> Schema(SchemaTypes schemaType)
        {
            _andyXOptions.WriterOptions.SchemaType = schemaType;
            return this;
        }

        public Writer<TEntity> WriterType(WriterTypes writerType)
        {
            _andyXOptions.WriterOptions.WriterType = writerType;
            return this;
        }
    }
}
