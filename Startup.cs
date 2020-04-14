using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutoMapper;
using esquire_backend.security;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Esquire.Data;
using Esquire.Models;
using Esquire.Resolvers;
using Esquire.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Esquire
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var hostname = Environment.GetEnvironmentVariable("SQLSERVER_HOST") ?? "localhost";

            var password = Environment.GetEnvironmentVariable("SQLSERVER_SA_PASSWORD") ?? "provide a password here if needed";
            var database = Environment.GetEnvironmentVariable("SQLSERVER_SA_DATABASE") ?? "master";
            var userid = Environment.GetEnvironmentVariable("SQLSERVER_SA_USERID") ?? "sa";
            var connString = $"Data Source={hostname};Database={database};User ID={userid};Password={password};";
            services.AddDbContext<ProjectContext>(options => options.UseSqlServer(connString));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         RequireExpirationTime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = JwtTokenOptions.Issuer,
                         ValidAudience = JwtTokenOptions.Audience,
                         IssuerSigningKey = JwtTokenOptions.Key,
                         ClockSkew = TimeSpan.Zero
                     };
                     options.Events = new JwtBearerEvents
                     {
                         OnAuthenticationFailed = context =>
                         {
                             return Task.CompletedTask;
                         },
                         OnTokenValidated = context =>
                         {
                             return Task.CompletedTask;
                         }
                     };
                 });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                                  .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                                  .RequireAuthenticatedUser().Build());
            });
            services.AddMvc(o => o.EnableEndpointRouting = false)
                .AddNewtonsoftJson(o => o.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IProjectExportService, ProjectExportService>();
            services.AddSingleton<ILockingService, LockingService>();
            services.AddSingleton<IFileProvider>(
                new PhysicalFileProvider(
                    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));


#if DEBUG
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PHLIP Backend API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\". You just need to put the token in the textbox below.",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "esquire-backend-doc.xml"));
            });
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseAuthentication();

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<SchemeQuestionCreationDto, SchemeQuestion>();
                cfg.CreateMap<SchemeQuestionUpdateDto, SchemeQuestion>()
                    .ForMember(q => q.PossibleAnswers, opt => opt.Ignore());
                cfg.CreateMap<SchemeQuestion, SchemeQuestionWithChildQuestionsDto>();
                cfg.CreateMap<SchemeAnswerCreationDto, SchemeAnswer>();
                cfg.CreateMap<Project, ProjectReturnDto>()
                    .ForMember(p => p.LastEditedBy, opts => opts
                        .MapFrom(src =>
                                src.LastEditedBy.GetFullName()))
                    .ForMember(p => p.CreatedBy,
                        opts => opts.MapFrom(src =>
                            src.CreatedBy.GetFullName()))
                    .ForMember(p => p.CreatedById, opts => opts.MapFrom(src => src.CreatedBy.Id))
                    .ForMember(p => p.CreatedByEmail, opts => opts.MapFrom(src => src.CreatedBy.Email));

                cfg.CreateMap<ProjectUpdateDto, Project>()
                    .ForMember(p => p.LastEditedBy, opts => opts.Ignore())
                    .ForMember(p => p.DateLastEdited, opts => opts.ResolveUsing(d => DateTime.UtcNow));

                cfg.CreateMap<ProjectCreationDto, Project>()
                    .ForMember(p => p.LastEditedBy, opts => opts.Ignore())
                    .ForMember(p => p.CreatedBy, opts => opts.Ignore())
                    .ForMember(p => p.DateCreated, opts => opts.ResolveUsing(d => DateTime.UtcNow))
                    .ForMember(p => p.DateLastEdited, opts => opts.ResolveUsing(d => DateTime.UtcNow));

                cfg.CreateMap<JurisdictionCreationDto, Jurisdiction>();
                cfg.CreateMap<Jurisdiction, JurisdictionReturnDto>();

                cfg.CreateMap<ProjectJurisdiction, ProjectJurisdictionReturnDto>()
                    .ForMember(p => p.Id, opts => opts.MapFrom(src => src.Id))
                    .ForMember(p => p.JurisdictionId, opts => opts.MapFrom(src => src.Jurisdiction.Id))
                    .ForMember(p => p.Name, opts => opts.MapFrom(src => src.Jurisdiction.Name));

                cfg.CreateMap<ProjectJurisdictionCreationDto, ProjectJurisdiction>();
                cfg.CreateMap<ProjectJurisdictionUpdateDto, ProjectJurisdiction>();
                cfg.CreateMap<ProjectJurisdictionFromPresetCreationDto, ProjectJurisdiction>();

                cfg.CreateMap<CodedQuestionCreationDto, CodedQuestion>()
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<CodedQuestionUpdateDto, CodedQuestion>()
                    .ForMember(ucq => ucq.Id, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<CodedQuestionCreationDto, CodedCategoryQuestion>()
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Category, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<CodedQuestionUpdateDto, CodedCategoryQuestion>()
                    .ForMember(ucq => ucq.Id, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Category, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<ValidatedQuestionCreationDto, ValidatedQuestion>()
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<ValidatedQuestionCreationDto, ValidatedCategoryQuestion>()
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Category, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<ValidatedQuestionUpdateDto, ValidatedQuestion>()
                    .ForMember(ucq => ucq.Id, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());

                cfg.CreateMap<ValidatedQuestionUpdateDto, ValidatedCategoryQuestion>()
                    .ForMember(ucq => ucq.Id, opts => opts.Ignore())
                    .ForMember(ucq => ucq.CodedAnswers, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Category, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore())
                    .ForMember(ucq => ucq.Flag, opts => opts.Ignore());



                cfg.CreateMap<CodedQuestion, CodedQuestionReturnDto>();
                cfg.CreateMap<CodedCategoryQuestion, CodedCategoryQuestionReturnDto>()
                    .ForMember(ucq => ucq.CategoryId, opts => opts.MapFrom(src =>
                        src.Category.Id));
                cfg.CreateMap<string, List<Annotation>>().ConvertUsing<AnnotationTypeConverter>();
                cfg.CreateMap<List<Annotation>, string>().ConvertUsing<AnnotationTypeToStrConverter>();
                cfg.CreateMap<CodedAnswer, CodedAnswerReturnDto>();
                cfg.CreateMap<CodedAnswerCreationDto, CodedAnswer>();
                cfg.CreateMap<Project, BookmarkedProjectReturnDto>()
                    .ForMember(dest => dest.ProjectId, opts => opts.MapFrom(src => src.Id));

                cfg.CreateMap<ValidatedQuestion, ValidatedQuestionReturnDto>();
                cfg.CreateMap<ValidatedCategoryQuestion, ValidatedCategoryQuestionReturnDto>()
                    .ForMember(ucq => ucq.CategoryId, opts => opts.MapFrom(src =>
                        src.Category.Id));

                cfg.CreateMap<User, UserReturnDto>()
                    .ForMember(u => u.UserId, opts => opts.MapFrom(src => src.Id));

                cfg.CreateMap<User, UserWithAvatarReturnDto>()
                    .ForMember(u => u.UserId, opts => opts.MapFrom(src => src.Id));

                cfg.CreateMap<User, UserGetDto>();
                cfg.CreateMap<User, UserWithTokenGetDto>()
                    .ForMember(src => src.Token, opts => opts.Ignore());

                cfg.CreateMap<User, ProjectUser>()
                    .ForMember(u => u.UserId, opts => opts.MapFrom(src => src.Id));

                cfg.CreateMap<UserPatchDto, User>();
                cfg.CreateMap<UserPutDto, User>();
                cfg.CreateMap<UserPostDto, User>();

                cfg.CreateMap<Protocol, ProtocolReturnDto>();
                cfg.CreateMap<CodedQuestionFlag, CodedQuestionFlagReturnDto>();
                cfg.CreateMap<SchemeQuestionFlag, SchemeQuestionFlagReturnDto>();
                cfg.CreateMap<SchemeQuestion, SchemeQuestionReturnDto>();
                // new mappings
                cfg.CreateMap<CodedQuestionReturnDto, ValidatedQuestionCreationDto>()
                    .ForMember(ucq => ucq.CategoryId, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());

                cfg.CreateMap<CodedQuestionReturnDto, ValidatedQuestionUpdateDto>()
                    .ForMember(ucq => ucq.CategoryId, opts => opts.Ignore())
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());

                cfg.CreateMap<CodedCategoryQuestionReturnDto, ValidatedQuestionUpdateDto>()
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());

                cfg.CreateMap<CodedCategoryQuestionReturnDto, ValidatedQuestionCreationDto>()
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());

                cfg.CreateMap<CodedCategoryQuestion, ValidatedQuestionCreationDto>()
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());
                cfg.CreateMap<CodedCategoryQuestion, ValidatedQuestionUpdateDto>()
                    .ForMember(ucq => ucq.ValidatedBy, opts => opts.Ignore());
            });

            app.UseMvc();

            // Enable middle ware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
#if DEBUG
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "PHLIP API V1");
                c.DisplayRequestDuration();
            });
#endif
        }
    }
}
