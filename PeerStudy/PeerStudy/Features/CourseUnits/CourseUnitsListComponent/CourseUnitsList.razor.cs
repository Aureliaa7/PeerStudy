﻿using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.CourseUnits;
using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.CourseUnits.CourseUnitsListComponent
{
    public partial class CourseUnitsList
    {
        [Parameter]
        public List<CourseUnitDetailsModel> CourseUnits { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public Guid CurrentUserId { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDelete { get; set; }

        [Parameter]
        public EventCallback<Guid> OnRename { get; set; }

        [Parameter]
        public EventCallback<Guid> OnUploadFiles { get; set; }

        [Parameter]
        public EventCallback<DeleteCourseUnitResourceModel> OnDeleteResource { get; set; }

        private async Task Delete(Guid courseUnitId)
        {
            await OnDelete.InvokeAsync(courseUnitId);
        }

        private async Task Rename(Guid courseUnitId)
        {
            await OnRename.InvokeAsync(courseUnitId);
        }

        private async Task UploadFiles(Guid courseUnitId)
        {
            await OnUploadFiles.InvokeAsync(courseUnitId);
        }

        private async Task DeleteResource(DeleteCourseUnitResourceModel courseUnitIdResourceId)
        {
            await OnDeleteResource.InvokeAsync(courseUnitIdResourceId);
        }
    }
}