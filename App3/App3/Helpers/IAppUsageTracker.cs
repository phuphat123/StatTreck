using System;
using System.Collections.Generic;
using System.Text;

namespace App3.Helpers
{
    public interface IAppUsageTracker
    {
        Dictionary<string, double> GetAppUsageTime();
        bool HasUsageAccessGranted();
        void RequestUsageAccess();
    }
}
