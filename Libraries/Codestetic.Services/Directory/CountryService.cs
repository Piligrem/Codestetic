using System;
using System.Collections.Generic;
using System.Linq;
using Codestetic.Web.Core.Caching;
using Codestetic.Web.Core.Data;
using Codestetic.Web.Core.Domain.Directory;
using Codestetic.Web.Core.Events;

namespace Codestetic.Web.Services.Directory
{
    /// <summary>
    /// Country service
    /// </summary>
    public partial class CountryService : ICountryService
    {
        #region Constants
        private const string COUNTRIES_ALL_KEY = "specter.country.all-{0}";
        private const string COUNTRIES_PATTERN_KEY = "specter.country.";
        #endregion
        
        #region Fields
        
        private readonly IRepository<Country> _countryRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="cacheManager">Cache manager</param>
        /// <param name="countryRepository">Country repository</param>
        /// <param name="eventPublisher">Event published</param>
        public CountryService(ICacheManager cacheManager,
            IRepository<Country> countryRepository,
            IEventPublisher eventPublisher)
        {
            _cacheManager = cacheManager;
            _countryRepository = countryRepository;
            _eventPublisher = eventPublisher;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Deletes a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void DeleteCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Delete(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityDeleted(country);
        }

        /// <summary>
        /// Gets all countries
        /// </summary>
        /// <param name="showHidden">A value indicating whether to show hidden records</param>
        /// <returns>Country collection</returns>
        public virtual IList<Country> GetAllCountries(bool showHidden = false)
        {
            string key = string.Format(COUNTRIES_ALL_KEY, showHidden);
            return _cacheManager.Get(key, () =>
            {
                var query = from c in _countryRepository.Table
                            orderby c.DisplayOrder, c.Name
                            where showHidden || c.Published
                            select c;
                var countries = query.ToList();
                return countries;
            });
        }

        /// <summary>
        /// Gets a country 
        /// </summary>
        /// <param name="countryId">Country identifier</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryById(long countryId)
        {
            if (countryId == 0)
                return null;

            return _countryRepository.GetById(countryId);
        }

        /// <summary>
        /// Gets a country by two letter ISO code
        /// </summary>
        /// <param name="twoLetterIsoCode">Country two letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByTwoLetterIsoCode(string twoLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.TwoLetterIsoCode == twoLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();

            return country;
        }

        /// <summary>
        /// Gets a country by three letter ISO code
        /// </summary>
        /// <param name="threeLetterIsoCode">Country three letter ISO code</param>
        /// <returns>Country</returns>
        public virtual Country GetCountryByThreeLetterIsoCode(string threeLetterIsoCode)
        {
            var query = from c in _countryRepository.Table
                        where c.ThreeLetterIsoCode == threeLetterIsoCode
                        select c;
            var country = query.FirstOrDefault();
            return country;
        }

        /// <summary>
        /// Inserts a country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void InsertCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Insert(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityInserted(country);
        }

        /// <summary>
        /// Updates the country
        /// </summary>
        /// <param name="country">Country</param>
        public virtual void UpdateCountry(Country country)
        {
            if (country == null)
                throw new ArgumentNullException("country");

            _countryRepository.Update(country);

            _cacheManager.RemoveByPattern(COUNTRIES_PATTERN_KEY);

            //event notification
            _eventPublisher.EntityUpdated(country);
        }
        #endregion
    }
}