using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Linq;

namespace Dfc.ProviderPortal.Courses.Helpers
{
    public static class DocumentQueryExtensions
    {
        // source: https://stackoverflow.com/questions/39338131/documentclient-createdocumentquery-async/49322332#49322332
        public static async Task<List<T>> ToListAsync<T>(this IDocumentQuery<T> queryable)
        {
            var list = new List<T>();
            while (queryable.HasMoreResults)
            {
                //Note that ExecuteNextAsync can return many records in each call
                var response = await queryable.ExecuteNextAsync<T>();
                list.AddRange(response);
            }

            return list;
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> query)
        {
            return await query.AsDocumentQuery().ToListAsync();
        }
    }
}