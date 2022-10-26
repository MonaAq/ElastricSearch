using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElasticDynamicSerch.UnitTests.Model
{
    public class Tepix
    {
        /// <summary>
        /// fill properties in cunstractor for new tepix object
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="indexValue"></param>
        public Tepix(DateTime datetime, String indexValue)
        {
            Id = CreateId(datetime);
            IndexValue = indexValue;
            DateTime = datetime;
            InsertDateTime = DateTime.Now;
        }
        public DateTime InsertDateTime { get; set; }
        public DateTime DateTime { get; set; }
        public string IndexValue { get; set; }
        public string Id { get; set; }

        /// <summary>
        /// generate id with datetime
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static string CreateId(DateTime date)
        {
            return new StringBuilder()
            .Append(date.Year)
            .Append('-')
            .Append(date.Month)
            .Append('-')
            .Append(date.Day)
            .Append('-')
            .Append(date.Hour)
            .Append('-')
            .Append(date.Minute)
            .Append('-')
            .Append(date.Second)
            .ToString();
        }
    }
}
