﻿namespace Dfc.ProviderPortal.Courses.Interfaces
{
    public interface IQualificationServiceSettings
    {
        string SearchService { get; }
        string QueryKey { get; }
        string AdminKey { get; }
        string Index { get; }
    }
}
