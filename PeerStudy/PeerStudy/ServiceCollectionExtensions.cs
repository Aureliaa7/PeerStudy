﻿using Blazored.LocalStorage;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise.RichTextEdit;
using MatBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PeerStudy.Core.DomainServices;
using PeerStudy.Core.Interfaces.DomainServices;
using PeerStudy.Core.Interfaces.Services;
using PeerStudy.Core.Interfaces.UnitOfWork;
using PeerStudy.Core.Models.Emails;
using PeerStudy.Infrastructure.AppDbContext;
using PeerStudy.Infrastructure.Interfaces;
using PeerStudy.Infrastructure.Services;
using PeerStudy.Infrastructure.UnitOfWork;
using PeerStudy.Services;
using PeerStudy.Services.Interfaces;

namespace PeerStudy
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterDbContext(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
              options.UseSqlServer(
                  connectionString)
            );
        }

        public static void RegisterServices(this IServiceCollection services)
        {
            RegisterDomainServices(services);
            RegisterClientServices(services);
            RegisterServicesFromInfrastructure(services);
        }

        public static void RegisterBlazorComponentLibraries(this IServiceCollection services)
        {
            services.AddMatBlazor();
            services.AddBlazorise(options =>
            {
                options.Immediate = true;
            })
            .AddBootstrapProviders()
            .AddFontAwesomeIcons();

            services.AddBlazoriseRichTextEdit();
        }

        private static void RegisterDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ICourseEnrollmentService, CourseEnrollmentService>();
            services.AddScoped<ICourseResourceService, CourseResourceService>();
            services.AddScoped<IStudyGroupService, StudyGroupService>();
            services.AddScoped<IAssignmentService, AssignmentService>();
            services.AddScoped<IAssignmentFileService, AssignmentFileService>();
            services.AddScoped<IStudyGroupResourceService, StudyGroupResourceService>();
            services.AddScoped<IWorkItemService, WorkItemService>();
            services.AddScoped<ICourseUnitService, CourseUnitService>();
            services.AddScoped<IStudentPointsService, StudentPointsService>();
            services.AddScoped<IQuestionService, QuestionService>();
            services.AddScoped<IQuestionPaginationService, QuestionPaginationService>();
            services.AddScoped<IAnswerService, AnswerService>();
            services.AddScoped<IStudentBadgeService, StudentBadgeService>();
            services.AddScoped<IRewardingService, RewardingService>();
            services.AddScoped<IAchievementService, AchievementService>();
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IBadgeService, BadgeService>();
        }

        private static void RegisterClientServices(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();

            services.AddScoped<AuthenticationStateProvider, PeerStudyAuthStateProvider>();
            services.AddAuthorizationCore();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<INavigationMenuService, NavigationMenuService>();
            services.AddScoped<IPeerStudyToastService, PeerStudyToastService>();

            services.AddScoped<IChartDataService, ChartDataService>();
        }

        private static void RegisterServicesFromInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<IDatabaseSeedingService, DatabaseSeedingService>();
            services.AddScoped<IGoogleDriveFileService, GoogleDriveFileService>();
            services.AddScoped<IGoogleDrivePermissionService, GoogleDrivePermissionService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IEmailTemplateBodyService, EmailTemplateBodyService>();
            services.AddScoped<IEmailService, EmailService>();
        }

        public static void RegisterEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration
                .GetSection(nameof(EmailConfiguration))
                .Get<EmailConfiguration>();

            services.AddSingleton(emailConfig);
        }
    }
}
