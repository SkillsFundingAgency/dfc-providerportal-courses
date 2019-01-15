using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace Dfc.ProviderPortal.Courses.Services
{
    public class CoursesService : ICourseService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;

        public CoursesService(
            ICosmosDbHelper cosmosDbHelper,
            IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(settings, nameof(settings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
        }

        public async Task<ICourse> AddCourse(ICourse course)
        {
            Throw.IfNull(course, nameof(course));

            Course persisted;

            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDatabaseIfNotExistsAsync(client);
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client, _settings.CoursesCollectionId);
                var doc = await _cosmosDbHelper.CreateDocumentAsync(client, _settings.CoursesCollectionId, course);
                persisted = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            return persisted;
        }

        public async Task<ICourse> GetCourseById(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"Cannot be an empty {nameof(Guid)}", nameof(id));

            Course persisted = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                var doc = _cosmosDbHelper.GetDocumentById(client, _settings.CoursesCollectionId, id);
                persisted = _cosmosDbHelper.DocumentTo<Course>(doc);
            }

            return persisted;
        }



        public async Task<ICourse> Update(ICourse course)
        {
            Throw.IfNull(course, nameof(course));
         
            Course updated = null;

            using (var client = _cosmosDbHelper.GetClient())
            {
                var updatedDocument = await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.CoursesCollectionId, course);

                updated = _cosmosDbHelper.DocumentTo<Course>(updatedDocument);
            }

            return updated;

        }

        public async Task<IEnumerable<ICourse>> GetCoursesByUKPRN(int UKPRN)
        {
            Throw.IfNull<int>(UKPRN, nameof(UKPRN));
            Throw.IfLessThan(0, UKPRN, nameof(UKPRN));

            IEnumerable<Course> persisted = null;
            using (var client = _cosmosDbHelper.GetClient())
            {
                var docs = _cosmosDbHelper.GetDocumentsByUKPRN(client, _settings.CoursesCollectionId, UKPRN);
                persisted = docs; // _cosmosDbHelper.DocumentsTo<Course>(docs);
            }

            return persisted;
        }
    }
}
