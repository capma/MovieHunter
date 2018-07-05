using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Bổ sung
using TMDbLib.Objects.Search;

namespace MovieHunter.Models
{
    public class MovieSearchResult
    {
        public string textToSearch;
        public int totalPages;
        public List<SearchMovie> movieSearchResult;
    }
}