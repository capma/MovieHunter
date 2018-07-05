using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//Add
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;

namespace MovieHunter.Models
{
    public class ListMovies
    {
        public IEnumerable<MovieResult> MovieNowPlaying { get; set; }
        public IEnumerable<MovieResult> MovieTopRated { get; set; }
        public IEnumerable<MovieResult> MoviePopular { get; set; }
        public IEnumerable<MovieResult> MovieUpcoming { get; set; }
    }
}