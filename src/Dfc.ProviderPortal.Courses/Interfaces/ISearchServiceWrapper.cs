﻿
using System;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Dfc.ProviderPortal.Courses.Models;


namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface ISearchServiceWrapper
    {
        IEnumerable<Document> UploadBatch(IReadOnlyList<Document> documents, out int succeeded);
    }
}
