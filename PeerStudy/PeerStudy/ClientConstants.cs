using System.Collections.Generic;

namespace PeerStudy
{
    public static class ClientConstants
    {
        public const string Token = "token";

        public const int ImageWidth = 300;
        public const int ImageHeight = 500;

        // caching constants
        public static string ActiveCoursesCacheKey = "ActiveCourses";
        public static string StudentStudyGroupsKey = "StudentStudyGroups";

        public static IReadOnlyCollection<string> CacheConstants = new List<string> { ActiveCoursesCacheKey, StudentStudyGroupsKey };

    }
}
