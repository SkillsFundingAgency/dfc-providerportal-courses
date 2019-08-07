using Dfc.ProviderPortal.Courses.Helpers;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Courses.Settings;
using FluentAssertions;
using RichardSzalay.MockHttp;
using System.Linq;
using System.Net.Http;
using Xunit;

namespace DFC.ProviderPortal.Courses.Tests.CoursesTests.Unit
{
    public class ProviderServiceWrapperUnitTests
    {
        /// <summary>
        /// Method under test
        /// </summary>
        public class GetLiveProvidersForAzureSearch
        {
            [Fact]  // COUR-1491
            public void When_CourseDirectoryName_NotNull_Then_ProviderName_ShouldBe_CourseDirectoryName()
            {
                //
                //  Arrange
                //

                IProviderServiceSettings settings = new ProviderServiceSettings()
                {
                    ApiKey = "",
                    ApiUrl = "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/",
                };
             
                var mockHttpMessageHandler = new MockHttpMessageHandler();
                mockHttpMessageHandler.When(HttpMethod.Post, "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/GetLiveProvidersForAzureSearch")
                                        .Respond("application/json",
                                        "[{\"id\":\"00000000-0000-0000-0000-000000000000\",\"UnitedKingdomProviderReferenceNumber\":123456,\"ProviderName\":\"DANDELION AND BURDOCK COLLEGE\",\"Status\":1,\"ProviderStatus\":\"Active\",\"CourseDirectoryName\":\"D&B\",\"TradingName\":null,\"ProviderAlias\":\"D&B - Alias\"}]"
                                        ); 
                var httpclient = new HttpClient(mockHttpMessageHandler);
                var service = new ProviderServiceWrapper(settings, httpclient);

                //
                //  Act
                //

                var searchResults = service.GetLiveProvidersForAzureSearch();

                //
                //  Assert
                //

                searchResults.Should().NotBeNull();
                searchResults.Should().HaveCount(1);
                searchResults.First().ProviderName.Should().Be("D&B", "CourseDirectoryName is set to 'D&B'");
            }

            [Fact]  // COUR-1491
            public void When_CourseDirectoryName_IsNull_Then_ProviderName_ShouldBe_ProviderName()
            {
                //
                //  Arrange
                //

                IProviderServiceSettings settings = new ProviderServiceSettings()
                {
                    ApiKey = "",
                    ApiUrl = "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/",
                };

                var mockHttpMessageHandler = new MockHttpMessageHandler();
                mockHttpMessageHandler.When(HttpMethod.Post, "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/GetLiveProvidersForAzureSearch")
                                        .Respond("application/json",
                                        "[{\"id\":\"00000000-0000-0000-0000-000000000000\",\"UnitedKingdomProviderReferenceNumber\":123456,\"ProviderName\":\"DANDELION AND BURDOCK COLLEGE\",\"Status\":1,\"ProviderStatus\":\"Active\",\"CourseDirectoryName\":null,\"TradingName\":null,\"ProviderAlias\":\"D&B - Alias\"}]"
                                        );
                var httpclient = new HttpClient(mockHttpMessageHandler);
                var service = new ProviderServiceWrapper(settings, httpclient);

                //
                //  Act
                //

                var searchResults = service.GetLiveProvidersForAzureSearch();

                //
                //  Assert
                //

                searchResults.Should().NotBeNull();
                searchResults.Should().HaveCount(1);
                searchResults.First().ProviderName.Should().Be("DANDELION AND BURDOCK COLLEGE", "CourseDirectoryName is null so use existing ProviderName value");
            }
        }
    }
}
