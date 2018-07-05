using System;  
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

//Add more
using MovieHunter.Models;
using TMDbLib;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.Discover;
using System.ComponentModel;

namespace MovieHunter.Controllers
{
    public class HomeController : Controller
    {
        private const string ApiKey = "07729bef670a78e80c07b39ba1f3ec7a";

        public ActionResult Index()
        {
            ViewBag.Message = "Home";

            TMDbClient client = new TMDbClient(ApiKey);
            IEnumerable<MovieResult> movieNowPlaying = client.GetMovieList(MovieListType.NowPlaying).Results.Take(5);
            IEnumerable<MovieResult> movieTopRated = client.GetMovieList(MovieListType.TopRated).Results.Take(5);
            IEnumerable<MovieResult> moviePopular = client.GetMovieList(MovieListType.Popular).Results.Take(5);
            IEnumerable<MovieResult> movieUpcoming = client.GetMovieList(MovieListType.Upcoming).Results.Take(5);

            var viewModel = new ListMovies
            {
                MovieNowPlaying = movieNowPlaying,
                MovieTopRated = movieTopRated,
                MoviePopular = moviePopular,
                MovieUpcoming = movieUpcoming
            };
            
            return View(viewModel);
        }

        public ActionResult SeeAll(string pType, int ? page)
        {
            ViewBag.Message = "See all";
            string movieTypeName = "";
            MovieListType movieType = new MovieListType();

            switch (pType)
            {
                case "seeAllPopular":
                    movieType = MovieListType.Popular;
                    movieTypeName = "Popular";
                    break;
                case "seeAllTopRated":
                    movieType = MovieListType.TopRated;
                    movieTypeName = "Top Rated";
                    break;
                case "seeAllNowPlaying":
                    movieType = MovieListType.NowPlaying;
                    movieTypeName = "Now Playing";
                    break;
                case "seeAllUpcoming":
                    movieType = MovieListType.Upcoming;
                    movieTypeName = "Upcoming";
                    break;
            }

            TMDbClient client = new TMDbClient(ApiKey);
            int pageNumber = (page ?? 1);
            IEnumerable<MovieResult> currentAllMovies = client.GetMovieList(movieType, pageNumber).Results;
            
            AllMovies allMovies = new AllMovies();
            allMovies.currentAllMovies = currentAllMovies;
            allMovies.currentMovieType = pType.ToString();
            allMovies.currentMovieTypeName = movieTypeName;
            allMovies.totalPages = client.GetMovieList(movieType).TotalPages;

            return View("SeeAll", allMovies);
        }

        // GET: Home/Search/
        public ActionResult Search(string textToSearch, int? page)
        {
            if (textToSearch == "")
            {
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Search result";
            TMDbClient client = new TMDbClient(ApiKey);

            int pageNumber = (page ?? 1);

            List<SearchMovie> searchMovies = client.SearchMovie(textToSearch, pageNumber).Results;

            MovieSearchResult moviesSearchResult = new MovieSearchResult();
            moviesSearchResult.textToSearch = textToSearch;
            moviesSearchResult.movieSearchResult = searchMovies;
            moviesSearchResult.totalPages = client.SearchMovie(textToSearch).TotalPages;

            return View("SearchResult", moviesSearchResult);
        }

        // GET: Home/Trailer
        public ActionResult Trailer(int movieID)
        {
            ViewBag.Message = "Trailer";
            TMDbClient client = new TMDbClient(ApiKey);
            List<Video> searchMovies = client.GetMovieVideos(movieID).Results;
            return View("Trailer", searchMovies);
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            throw new ArgumentException("Not found.", "description");
            // or return default(T);
        }

        // GET: Home/AdvancedSearch
        public ActionResult AdvancedSearch()
        {
            ViewBag.Message = "Advanced Search";

            int currentYear = 0;

            AdvancedSearch searchCriterias = new AdvancedSearch();

            // Create DataSource for Year
            var dsYear = new[]{new {yearID=0, yearString="None"}}.ToList();
            currentYear = DateTime.Today.Year;
            for (int i = currentYear; i >= 1900; i--)
            {
                dsYear.Add(new { yearID = i, yearString = i.ToString() });
            }
            ViewBag.year = new SelectList(dsYear, "yearID", "yearString");

            // Create DataSource for Sort by
            var dsSortBy = Enum.GetValues(typeof(DiscoverMovieSortBy)).Cast<DiscoverMovieSortBy>().Select(v => new SelectListItem
                        {
                            Text = v.ToString(),
                            Value = ((int)v).ToString()
                        }).ToList();

            ViewBag.SortBy = new SelectList(dsSortBy, "Value", "Text");

            // Create DataSource for Genre
            TMDbClient client = new TMDbClient(ApiKey);
            List<Genre> listGenres = client.GetMovieGenres();
            ViewBag.Genres = new SelectList(listGenres, "Id", "Name");

            return View(searchCriterias);
        }      

        // GET: Home/AdvancedSearchResult/
        public ActionResult AdvancedSearchResult(AdvancedSearch searchCriterias, int? page, int[] GenreIDs)
        {
            if (GenreIDs == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Message = "Result advanced Search";
            
            TMDbClient client = new TMDbClient(ApiKey);

            int pageNumber = (page ?? 1);

            AdvancedSearch discoveryViewModel = new AdvancedSearch();
            SearchContainer<SearchMovie> discoveryMovies;

            if (GenreIDs.Length > 0 && searchCriterias.year > 0)
            {
                discoveryMovies = client.DiscoverMovies()
                                    .WherePrimaryReleaseIsInYear(searchCriterias.year)
                                    .IncludeWithAllOfGenre(GenreIDs)
                                    .OrderBy(searchCriterias.sortBy)
                                    .Query(pageNumber);
            }
            else if (GenreIDs.Length > 0 && searchCriterias.year == 0)
            {
                discoveryMovies = client.DiscoverMovies()
                                    .IncludeWithAllOfGenre(GenreIDs)
                                    .OrderBy(searchCriterias.sortBy)
                                    .Query(pageNumber);
            }
            else if (GenreIDs.Length == 0 && searchCriterias.year > 0)
            {
                discoveryMovies = client.DiscoverMovies()
                                    .WherePrimaryReleaseIsInYear(searchCriterias.year)
                                    .OrderBy(searchCriterias.sortBy)
                                    .Query(pageNumber);
            }
            else
            {
                discoveryMovies = client.DiscoverMovies()
                                    .OrderBy(searchCriterias.sortBy)
                                    .Query(pageNumber);
            }
            
            discoveryViewModel.movieSearchResult = discoveryMovies.Results;
            discoveryViewModel.totalPages = discoveryMovies.TotalPages;

            // Create DataSource for Year
            int currentYear = 0;
            var dsYear = new[] { new { yearID = 0, yearString = "None" } }.ToList();
            currentYear = DateTime.Today.Year;
            for (int i = currentYear; i >= 1900; i--)
            {
                dsYear.Add(new { yearID = i, yearString = i.ToString() });
            }
            ViewBag.year = new SelectList(dsYear, "yearID", "yearString", searchCriterias.year);
            discoveryViewModel.yearSave = searchCriterias.year;

            // Create DataSource for Sort by
            var dsSortBy = Enum.GetValues(typeof(DiscoverMovieSortBy)).Cast<DiscoverMovieSortBy>().Select(v => new SelectListItem
            {
                //Text = GetEnumDescription(v),
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();

            //string tmp = GetEnumDescription(searchCriterias.sortBy);
            //var tmpSortBy = GetValueFromDescription<DiscoverMovieSortBy>(tmp);
            ViewBag.SortBy = new SelectList(dsSortBy, "Value", "Text", searchCriterias.sortBySave);
            discoveryViewModel.sortBySave = searchCriterias.sortBy.ToString();

            // Create DataSource for Genre
            List<Genre> listGenres = client.GetMovieGenres();
            ViewBag.Genres = new SelectList(listGenres, "Id", "Name", GenreIDs);
            discoveryViewModel.genresSave = GenreIDs;

            return View("AdvancedSearchResult", discoveryViewModel);
        }
        
        public ActionResult People()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "About";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact";

            return View();
        }
    }
}
