﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Models;
using Dfc.ProviderPortal.Courses.Settings;
using Dfc.ProviderPortal.Packages;
using Microsoft.Extensions.Options;

namespace Dfc.ProviderPortal.Courses.Services
{
    public class CourseMigrationReportService : ICourseMigrationReportService
    {
        private readonly ICosmosDbHelper _cosmosDbHelper;
        private readonly ICosmosDbCollectionSettings _settings;

        public CourseMigrationReportService(ICosmosDbHelper cosmosDbHelper, IOptions<CosmosDbCollectionSettings> settings)
        {
            Throw.IfNull(cosmosDbHelper, nameof(cosmosDbHelper));
            Throw.IfNull(settings, nameof(settings));

            _cosmosDbHelper = cosmosDbHelper;
            _settings = settings.Value;
        }

        public async Task Initialise()
        {
            using (var client = _cosmosDbHelper.GetClient())
            {
                await _cosmosDbHelper.CreateDocumentCollectionIfNotExistsAsync(client,
                   _settings.CoursesMigrationReportCollectionId);
            }
        }

        public async Task AddMigrationReport(CourseMigrationReport courseReport)
        {
            Throw.IfNull(courseReport, nameof(courseReport));
            try
            {
                using (var client = _cosmosDbHelper.GetClient())
                {
                    var result = await _cosmosDbHelper.GetDocumentsByUKPRN<CourseMigrationReport>(client, _settings.CoursesMigrationReportCollectionId,
                        courseReport.ProviderUKPRN);

                    if (result.Any())
                    {
                        courseReport.Id = result.FirstOrDefault().Id;

                        await _cosmosDbHelper.UpdateDocumentAsync(client, _settings.CoursesMigrationReportCollectionId,
                            courseReport);
                    }
                    else
                    {
                        var doc = await _cosmosDbHelper.CreateDocumentAsync(client, _settings.CoursesMigrationReportCollectionId, courseReport);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task<CourseMigrationReport> GetMigrationReport(int UKPRN)
        {
            try
            {
                using (var client = _cosmosDbHelper.GetClient())
                {
                    var result = await _cosmosDbHelper.GetDocumentsByUKPRN<CourseMigrationReport>(client,
                        _settings.CoursesMigrationReportCollectionId, UKPRN);
                    return result.FirstOrDefault();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
