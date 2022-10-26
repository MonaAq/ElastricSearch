using ElasticDynamicSearch.Model.Filters;
using ElasticDynamicSerch.UnitTests.Model;
using Microsoft.Extensions.Configuration;
using Nest;
using Filter = ElasticDynamicSearch.Model.Filters.Filter;

namespace ElasticDynamicSerch.UnitTests
{
    public class StatementRepositoryUnitTests
    {
        private readonly string _indexName;
        private readonly IElasticClient _elasticClient;
        private ElasticDynamicSearch.ElasticDynamicSearch _elasticDynamicSearch;
        public readonly IConfigurationRoot _configuration;
        public StatementRepositoryUnitTests()
        {
            var _sutFactory = new ElasticClientFactoryTests();
            ElasticClientFactory e = new ElasticClientFactory(_sutFactory.elasticProperties);
            var elasticServiceClient = new ElasticServiceClientTests();
            _elasticClient = elasticServiceClient.CreateElasticClient();
            _elasticDynamicSearch = new ElasticDynamicSearch.ElasticDynamicSearch(_elasticClient);
            _indexName = "omdp-tepix";
            _configuration = elasticServiceClient._configuration;
            DeleteAllData();
        }

        [Fact]
        public async void GetTepixByText_Should_HasValue()
        {
            // Arrange
            ArrangeDateForGetTepixByindexValueFilter();

            //Act
            var result = await _elasticDynamicSearch.SearchByText<Tepix>("1389099.00",10,0, _indexName);

            //Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async void GetTepixByindexValueFilter_Should_HasValue()
        {
            // Arrange
            ArrangeDateForGetTepixByindexValueFilter();

            Filter filters = new Filter();
            List<Filtering> List = new List<Filtering>();
            List.Add(new AndFilter()
            {
                PropertyName = "indexValue",
                PropertyValue = "1389099.00"
            });
            filters.Filters = List;

            //Act
            var result = await _elasticDynamicSearch.SearchByFilter<Tepix>(filters, _indexName);

            //Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async void GetTepixByDateTimeBetweenFilter_Should_HasValue()
        {
            // Arrange
            ArrangeDateForByDateTimeBetweenFilter();

            Filter filters = new Filter();
            List<Filtering> List = new List<Filtering>();
            List.Add(new BetweenFilter()
            {
                PropertyName = "insertDateTime",
                LeftValue = DateTime.Parse("2022-09-17T08:00:00"),
                RightValue = DateTime.Parse("2022-09-19T08:00:00"),
            });
            filters.Filters = List;

            //Act
            var result = await _elasticDynamicSearch.SearchByFilter<Tepix>(filters, _indexName);

            //Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async void GetTepixByDateTimeFilter_Should_HasValue()
        {
            // Arrange
            ArrangeDateForDateTimeFilter();

            Filter filters = new Filter();
            List<Filtering> List = new List<Filtering>();
            List.Add(new AndFilter()
            {
                PropertyName = "dateTime",
                PropertyValue = "2022-09-18T08:40:00",

            });
            filters.Filters = List;

            //Act
            var result = await _elasticDynamicSearch.SearchByFilter<Tepix>(filters, _indexName);

            //Assert
            Assert.NotNull(result);

        }

        [Fact]
        public async void GetTepixByDateTimeAndMustNotFilter_Should_HasValue()
        {
            // Arrange
            ArrangeDateForMustNotDateTimeFilter();

            Filter filters = new Filter();
            List<Filtering> List = new List<Filtering>();
            List.Add(new AndNotFilter()
            {
                PropertyName = "dateTime",
                PropertyValue = "2022-09-18T08:40:00",
            });
            List.Add(new AndFilter()
            {
                PropertyName = "indexValue",
                PropertyValue = "1389099.00",

            });
            filters.Filters = List;

            //Act
            var result = await _elasticDynamicSearch.SearchByFilter<Tepix>(filters, _indexName);

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Data.Count == 2);

        }

        #region Arrange Data and delete it
        private void ArrangeDateForGetTepixByindexValueFilter()
        {
            var connectionSetting = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticProperties").Get<ElasticProperties>().ElasticNodes[0]));
            var client = new ElasticClient(connectionSetting);
            var tepix = new Tepix(DateTime.Parse("2022-02-01T08:35:00"), "1389099.00");
            client.Index(tepix, i => i.Index(_indexName));
        }

        private void ArrangeDateForByDateTimeBetweenFilter()
        {
            var connectionSetting = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticProperties").Get<ElasticProperties>().ElasticNodes[0]));
            var client = new ElasticClient(connectionSetting);
            var tepix = new Tepix(DateTime.Parse("2022-09-18T08:00:00"), "1389099.00");
            client.Index(tepix, i => i.Index(_indexName));
        }

        private void ArrangeDateForDateTimeFilter()
        {
            var connectionSetting = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticProperties").Get<ElasticProperties>().ElasticNodes[0]));
            var client = new ElasticClient(connectionSetting);
            var tepix = new Tepix(DateTime.Parse("2022-09-18T08:40:00"), "1389099.00");
            client.Index(tepix, i => i.Index(_indexName));
        }

        private void ArrangeDateForMustNotDateTimeFilter()
        {
            List<Tepix> tepixes = new List<Tepix>();
            var connectionSetting = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticProperties").Get<ElasticProperties>().ElasticNodes[0]));
            var client = new ElasticClient(connectionSetting);
            var tepix = new Tepix(DateTime.Parse("2022-09-18T08:40:00"), "1389099.00");
            var tepix1 = new Tepix(DateTime.Parse("2022-09-19T08:40:00"), "1389099.00");
            var tepix2 = new Tepix(DateTime.Parse("2022-09-17T08:40:00"), "1389099.00");
            tepixes.Add(tepix);
            tepixes.Add(tepix1);
            tepixes.Add(tepix2);
            var bulk = new BulkRequest(_indexName) { Operations = new List<IBulkOperation>() };
            tepixes.ForEach(tepix =>
            {
                var bulkIndex = new BulkIndexOperation<Tepix>(tepix)
                {
                    Id = tepix.Id
                };
                bulk.Operations.Add(bulkIndex);
            });
            _elasticClient.BulkAsync(bulk);
        }

        private void DeleteAllData()
        {
            var connectionSetting = new ConnectionSettings(new Uri(_configuration.GetSection("ElasticProperties").Get<ElasticProperties>().ElasticNodes[0]));
            var client = new ElasticClient(connectionSetting);

            var response = client.DeleteByQuery<Tepix>(q => q
            .Query(q => q.QueryString(qs => qs.Query("*")))
            .Index(_indexName)
            );
        }
        #endregion
    }
}
