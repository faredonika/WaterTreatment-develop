using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WaterTreatment.Web.Models
{
    public class SearchResult<T>
    {
        public SearchResult()
        {
            Results = Enumerable.Empty<T>();
            Total = 0;
        }

        public IEnumerable<T> Results;
        public int Total;

    }

}