using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Add more
using TMDbLib.Objects.General;

namespace MovieHunter.Models
{
    public class AllMovies
    {
        public string currentMovieType;
        public string currentMovieTypeName;
        public int totalResults;
        public int currentPage;
        public int totalPages;
        public IEnumerable<MovieResult> currentAllMovies;
    }
}