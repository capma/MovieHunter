using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Add more
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
//using TMDbLib.Objects.General;
using TMDbLib.Objects.Discover;
using TMDbLib.Objects.Search;

namespace MovieHunter.Models
{
    public class AdvancedSearch
    {
        [Display(Name = "Year:")]
        public int year { get; set; }

        [Display(Name = "Sort by:")]
        public DiscoverMovieSortBy sortBy { get; set; }

        [Display(Name = "Genres:")]
        public int[] Genres { get; set; }

        public int totalPages { get; set; }

        public List<SearchMovie> movieSearchResult { get; set; }

        public int yearSave { get; set; }
        public string sortBySave { get; set; }

        public int[] genresSave { get; set; }
    }

}
