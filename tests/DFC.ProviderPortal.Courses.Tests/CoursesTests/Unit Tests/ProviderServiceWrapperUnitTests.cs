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
                    ApiKey = "3b5ef63d1ba84346a05b25e880119399",
                    ApiUrl = "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/",
                };
             
                var mockHttpMessageHandler = new MockHttpMessageHandler();
                mockHttpMessageHandler.When(HttpMethod.Post, "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/GetLiveProvidersForAzureSearch")
                                        .Respond("application/json", 
                                        "[{\"id\":\"b8b08904-662d-47af-a4e6-61822bb34e7e\",\"UnitedKingdomProviderReferenceNumber\":10000055,\"ProviderName\":\"ABINGDON AND WITNEY COLLEGE\",\"Status\":1,\"ProviderStatus\":\"Active\",\"CourseDirectoryName\":\"ABBO AND WIT\",\"TradingName\":null,\"ProviderAlias\":\"Mark Paddock College - Alias\"}]"
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
                searchResults.First().ProviderName.Should().Be("ABBO AND WIT", "CourseDirectoryName is set to 'ABBO AND WIT'");
            }

            [Fact]  // COUR-1491
            public void When_CourseDirectoryName_IsNull_Then_ProviderName_ShouldBe_ProviderName()
            {
                //
                //  Arrange
                //

                IProviderServiceSettings settings = new ProviderServiceSettings()
                {
                    ApiKey = "3b5ef63d1ba84346a05b25e880119399",
                    ApiUrl = "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/",
                };

                var mockHttpMessageHandler = new MockHttpMessageHandler();
                mockHttpMessageHandler.When(HttpMethod.Post, "https://dev.api.nationalcareersservice.org.uk/coursedirectory/ukrlp/api/GetLiveProvidersForAzureSearch")
                                        .Respond("application/json",
                                        "[{\"id\":\"b8b08904-662d-47af-a4e6-61822bb34e7e\",\"UnitedKingdomProviderReferenceNumber\":10000055,\"ProviderName\":\"ABINGDON AND WITNEY COLLEGE\",\"Status\":1,\"ProviderStatus\":\"Active\",\"CourseDirectoryName\":null,\"TradingName\":null,\"ProviderAlias\":\"Mark Paddock College - Alias\"}]"
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
                searchResults.First().ProviderName.Should().Be("ABINGDON AND WITNEY COLLEGE", "CourseDirectoryName is null so use existing ProviderName value");
            }
        }
    }
}
