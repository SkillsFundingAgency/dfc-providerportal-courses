using System.Threading.Tasks;

namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IQualificationServiceWrapper
    {
        Task<dynamic> GetQualificationById(string LARSRef);
    }
}
