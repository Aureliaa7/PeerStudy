﻿using PeerStudy.Core.Models.Resources;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Core.Interfaces.DomainServices
{
    public interface ICourseResourceService
    {
        Task<List<CourseResourceDetailsModel>> UploadResourcesAsync(List<UploadCourseResourceModel> resources);
    }
}