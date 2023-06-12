using PeerStudy.Core;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Models.Emails;
using System;

namespace PeerStudy.Infrastructure.Services
{
    public class EmailTemplateBodyService : IEmailTemplateBodyService
    {
        public string ReplaceTokens(EmailBaseModel emailModel, string emailBody)
        {
            emailBody = ReplaceRecipientToken(emailModel, emailBody);

            switch (emailModel)
            {
                case CourseEnrollmentRequestEmailModel enrollmentModel:
                    emailBody = ReplaceCourseEnrollmentRequestEmailTokens(enrollmentModel, emailBody);
                    break;

                case CreatedStudyGroupsEmailModel createdStudyGroupsEmailModel:
                    emailBody = ReplaceCreatedStudyGroupsEmailTokens(createdStudyGroupsEmailModel, emailBody);
                    break;

                case ApprovedCourseEnrollmentRequestEmailModel approvedEnrollmentRequestEmailModel:
                    emailBody = ReplaceCourseEnrollmentRequestStatusEmailTokens(approvedEnrollmentRequestEmailModel, emailBody);
                    break;

                case RejectedCourseEnrollmentRequestEmailModel rejectedEnrollmentRequestEmailModel:
                    emailBody = ReplaceCourseEnrollmentRequestStatusEmailTokens(rejectedEnrollmentRequestEmailModel, emailBody);
                    break;

                case NewAssignmentEmailModel newAssignmentEmailModel:
                    emailBody = ReplaceNewAssignmentEmailTokens(newAssignmentEmailModel, emailBody);
                    break;

                case GradedAssignmentEmailModel gradedAssignmentEmailModel:
                    emailBody = ReplaceGradedAssignmentEmailTokens(gradedAssignmentEmailModel, emailBody);
                    break;

                case NewCourseResourceEmailModel courseResourceEmailModel:
                    emailBody = ReplaceNewCourseResourceEmailTokens(courseResourceEmailModel, emailBody);
                    break;

                case EarnedBadgeEmailModel earnedBadgeEmailModel:
                    emailBody = ReplaceEarnedBadgeEmailTokens(earnedBadgeEmailModel, emailBody);
                    break;

                case NewQuestionAnswerEmailModel questionAnswerEmailModel:
                    emailBody = ReplaceNewQuestionAnswerEmailTokens(questionAnswerEmailModel, emailBody);
                    break;

                case AssignWorkItemEmailModel workItemEmailModel:
                    emailBody = ReplaceWorkItemEmailTokens(workItemEmailModel, emailBody);
                    break;

                case UnassignWorkItemEmailModel workItemEmailModel:
                    emailBody = ReplaceWorkItemEmailTokens(workItemEmailModel, emailBody);
                    break;

                default:
                    throw new ArgumentException("Invalid token value type.");
            }

            return emailBody;
        }

        private static string ReplaceRecipientToken(EmailBaseModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.RecipientName, emailModel.RecipientName);

            return emailBody;
        }

        private static string ReplaceCourseEnrollmentRequestEmailTokens(CourseEnrollmentRequestEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.StudentName, emailModel.StudentName);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);

            return emailBody;
        }

        private static string ReplaceCreatedStudyGroupsEmailTokens(CreatedStudyGroupsEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.TeacherName, emailModel.TeacherName);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);

            string studyGroupMembers = emailModel.StudyGroupMembers != null ? string.Join(", ", emailModel.StudyGroupMembers) : string.Empty;
            emailBody = emailBody.Replace(EmailTokens.StudyGroupMembers, studyGroupMembers);

            return emailBody;
        }

        private static string ReplaceCourseEnrollmentRequestStatusEmailTokens(CourseEnrollmentRequestStatusEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.TeacherName, emailModel.TeacherName);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);

            return emailBody;
        }

        private static string ReplaceNewAssignmentEmailTokens(NewAssignmentEmailModel emailModel, string emailBody)
        {
            emailBody = ReplaceAssignmentRelatedTokens(emailModel, emailBody);
            emailBody = emailBody.Replace(EmailTokens.AssignmentDeadline, emailModel.Deadline.ToString());

            return emailBody;
        }

        private static string ReplaceGradedAssignmentEmailTokens(GradedAssignmentEmailModel emailModel, string emailBody)
        {
            emailBody = ReplaceAssignmentRelatedTokens(emailModel, emailBody);
            emailBody = emailBody.Replace(EmailTokens.EarnedPoints, emailModel.EarnedPoints.ToString());

            return emailBody;
        }

        private static string ReplaceAssignmentRelatedTokens(AssignmentBaseModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.TeacherName, emailModel.TeacherName);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);
            emailBody = emailBody.Replace(EmailTokens.CourseUnitTitle, emailModel.CourseUnitTitle);
            emailBody = emailBody.Replace(EmailTokens.AssignmentDeadline, emailModel.AssignmentTitle);

            return emailBody;
        }

        private static string ReplaceNewCourseResourceEmailTokens(NewCourseResourceEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.TeacherName, emailModel.TeacherName);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);
            emailBody = emailBody.Replace(EmailTokens.ResourceName, emailModel.ResourceName);

            return emailBody;
        }

        private static string ReplaceEarnedBadgeEmailTokens(EarnedBadgeEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.BadgeTitle, emailModel.BadgeTitle);
            emailBody = emailBody.Replace(EmailTokens.BadgeDescription, emailModel.BadgeDescription);
            emailBody = emailBody.Replace(EmailTokens.EarnedPoints, emailModel.NoEarnedPoints.ToString());

            return emailBody;
        } 

        private static string ReplaceNewQuestionAnswerEmailTokens(NewQuestionAnswerEmailModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.QuestionTitle, emailModel.QuestionTitle);
            emailBody = emailBody.Replace(EmailTokens.AnswerAuthorName, emailModel.AnswerAuthorName);

            return emailBody;
        }

        private static string ReplaceWorkItemEmailTokens(WorkItemEmailBaseModel emailModel, string emailBody)
        {
            emailBody = emailBody.Replace(EmailTokens.WorkItemTitle, emailModel.WorkItemTitle);
            emailBody = emailBody.Replace(EmailTokens.StudyGroupTitle, emailModel.StudyGroupTitle);
            emailBody = emailBody.Replace(EmailTokens.CourseTitle, emailModel.CourseTitle);
            emailBody = emailBody.Replace(EmailTokens.WorkItemChangedBy, emailModel.ChangedBy);

            return emailBody;
        }
    }
}
