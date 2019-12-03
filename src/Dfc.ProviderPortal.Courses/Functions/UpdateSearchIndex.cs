using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Interfaces;
using Dfc.ProviderPortal.Packages.AzureFunctions.DependencyInjection;
using Microsoft.Azure.WebJobs;

namespace Dfc.ProviderPortal.Courses.Functions
{
    public static class UpdateSearchIndex
    {
        [FunctionName("UpdateSearchIndex")]
        [NoAutomaticTrigger]
        public static Task Run(
            string input,  // Work around https://github.com/Azure/azure-functions-vs-build-sdk/issues/168
            [Inject] ISearchServiceWrapper searchServiceWrapper)
        {
            return searchServiceWrapper.UpdateCourseIndex(recreate: false);
        }
    }
}
