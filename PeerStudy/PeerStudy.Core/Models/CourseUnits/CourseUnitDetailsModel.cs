using PeerStudy.Core.Models.Resources;
using System;
using System.Collections.Generic;

namespace PeerStudy.Core.Models.CourseUnits
{
    public class CourseUnitDetailsModel : CourseUnitModel
    {
        public Guid Id { get; set; }

        public bool IsActive { get; set; }

        public List<CourseResourceDetailsModel> Resources { get; set; }
    }
}
