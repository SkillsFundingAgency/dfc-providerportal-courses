using System.Threading.Tasks;
using Dfc.ProviderPortal.Courses.Models;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IFeChoiceServiceWrapper
    {
        Task<FeChoice> GetByUKPRNAsync(int ukprn);
    }
}