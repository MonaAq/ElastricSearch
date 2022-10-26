﻿using Microsoft.Extensions.Configuration;

namespace ElasticDynamicSerch.UnitTests
{
    public class ElasticClientFactoryTests
    {
        public readonly IConfigurationRoot _configuration;
        public readonly ElasticProperties elasticProperties;

        public ElasticClientFactoryTests()
        {
            _configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.UnitTest.json", true).Build();
            elasticProperties = _configuration.GetSection("ElasticProperties").Get<ElasticProperties>();
        }
    }
}
