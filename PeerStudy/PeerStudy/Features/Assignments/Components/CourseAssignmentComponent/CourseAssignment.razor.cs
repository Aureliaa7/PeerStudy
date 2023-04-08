using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components;
using PeerStudy.Core.Models.Assignments;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PeerStudy.Features.Assignments.Components.CourseAssignmentComponent
{
    public partial class CourseAssignment
    {
        [Parameter]
        public ExtendedAssignmentDetailsModel Assignment { get; set; }

        [Parameter]
        public bool IsReadOnly { get; set; }

        [Parameter]
        public EventCallback<SaveGradeModel> OnSaveGrade { get; set; }

        [Parameter]
        public EventCallback<Guid> OnDeleteAssignment { get; set; }

        [Parameter]
        public EventCallback<(Guid assignmentId, Guid studentId)> OnViewWork { get; set; }

        private bool expanded;
        private bool showConfirmationPopup;
        private string deleteAssignmentConfirmationMessage;
        private const string confirmationPopupTitle = "Delete assignment";
        private GradeAssignmentModel selectedRow;

        private async Task GradeAssignment(SavedRowItem<GradeAssignmentModel, Dictionary<string, object>> data)
        {
            await OnSaveGrade.InvokeAsync(new SaveGradeModel
            {
                AssignmentId = Assignment.Id,
                StudentId = data.Item.StudentId,
                Points = data.Item.Points
            });
        }

        private void DeleteAssignment()
        {
            showConfirmationPopup = true;
            deleteAssignmentConfirmationMessage = $"Are you sure you want to delete {Assignment.Title}?";
        }

        private void CancelDeleteAssignmentHandler()
        {
            showConfirmationPopup = false;
        }

        private async Task DeleteAssignmentHandler()
        {
            showConfirmationPopup = false;
            await OnDeleteAssignment.InvokeAsync(Assignment.Id);
        }

        private async Task ViewSubmittedWork()
        {
            await OnViewWork.InvokeAsync((Assignment.Id, Assignment.StudentGroupId));
        }
    }
}
