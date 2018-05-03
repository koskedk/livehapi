using System;
using System.IO;
using System.Linq;
using LiveHAPI.Core.Interfaces.Handler;
using LiveHAPI.Core.Interfaces.Repository;
using LiveHAPI.Core.Interfaces.Services;
using LiveHAPI.Core.Model.Encounters;
using LiveHAPI.Core.Model.Lookup;
using LiveHAPI.Core.Model.Network;
using LiveHAPI.Core.Model.People;
using LiveHAPI.Core.Model.QModel;
using LiveHAPI.Core.Model.Studio;
using LiveHAPI.Core.Model.Subscriber;
using LiveHAPI.Core.Service;
using LiveHAPI.Infrastructure;
using LiveHAPI.Infrastructure.Repository;
using LiveHAPI.IQCare.Core.Handlers;
using LiveHAPI.IQCare.Core.Interfaces.Repository;
using LiveHAPI.IQCare.Core.Model;
using LiveHAPI.IQCare.Infrastructure;
using LiveHAPI.IQCare.Infrastructure.Repository;
using LiveHAPI.Shared.Interfaces;
using LiveHAPI.Shared.ValueObject;
using LiveHAPI.Shared.ValueObject.Meta;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Z.Dapper.Plus;
using Action = LiveHAPI.Core.Model.QModel.Action;
using Encounter = LiveHAPI.Core.Model.Encounters.Encounter;
using User = LiveHAPI.IQCare.Core.Model.User;

namespace LiveHAPI
{
    public class Startup
    {

        public static IConfiguration Configuration;

        public Startup(IHostingEnvironment env)
        {
            //TODO: Use Environment Variables

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appSettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter()))
                .AddJsonOptions(o =>
                    o.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            var connectionString = Startup.Configuration["connectionStrings:hAPIConnection"];
            services.AddDbContext<LiveHAPIContext>(o => o.UseSqlServer(connectionString));

            var emrconnectionString = Startup.Configuration["connectionStrings:EMRConnection"];
            services.AddDbContext<EMRContext>(o => o.UseSqlServer(emrconnectionString));

            services.AddScoped<IMasterFacilityRepository, MasterFacilityRepository>();
            services.AddScoped<IObsRepository, ObsRepository>();

            services.AddScoped<IObsTraceResultRepository, ObsTraceResultRepository>();
            services.AddScoped<IObsTestResultRepository, ObsTestResultRepository>();
            services.AddScoped<IObsFinalTestResultRepository, ObsFinalTestResultRepository>();
            services.AddScoped<IObsLinkageRepository, ObsLinkageRepository>();
            services.AddScoped<IObsMemberScreeningRepository, ObsMemberScreeningRepository>();
            services.AddScoped<IObsFamilyTraceResultRepository, ObsFamilyTraceResultRepository>();
            services.AddScoped<IObsPartnerScreeningRepository, ObsPartnerScreeningRepository>();
            services.AddScoped<IObsPartnerTraceResultRepository, ObsPartnerTraceResultRepository>();

            services.AddScoped<IPersonNameRepository, PersonNameRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPracticeActivationRepository, PracticeActivationRepository>();
            services.AddScoped<IPracticeRepository, PracticeRepository>();
            services.AddScoped<IProviderRepository, ProviderRepository>();
            services.AddScoped<ISubCountyRepository, SubCountyRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<ICountyRepository, CountyRepository>();
            services.AddScoped<IEncounterRepository, EncounterRepository>();
            services.AddScoped<ILookupRepository, LookupRepository>();
            services.AddScoped<ISubscriberSystemRepository, SubscriberSystemRepository>();
            services.AddScoped<ISubscriberConfigRepository, SubscriberConfigRepository>();
            services.AddScoped<IUserSummaryRepository, UserSummaryRepository>();
            services.AddScoped<IClientSummaryRepository, ClientSummaryRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IPSmartStoreRepository, PSmartStoreRepository>();

            services.AddScoped<IMetaService, MetaService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<IActivationService, ActivationService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IEncounterService, EncounterService>();
            services.AddScoped<IFormsService, FormsService>();
            services.AddScoped<IPSmartStoreService, PSmartStoreService>();

            services.AddScoped<IClientSavedHandler, ClientSavedHandler>();
            services.AddScoped<IEncounterSavedHandler, EncounterSavedHandler>();

            services.AddScoped<IConfigRepository, ConfigRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientEncounterRepository, PatientEncounterRepository>();
            services.AddScoped<IPStoreRepository, PStoreRepository>();
            services.AddScoped<IPatientFamilyRepository, PatientFamilyRepository>();

            services.AddScoped<ISetupService, SetupService>();
            services.AddScoped<ISetupFacilty, SetupFacilty>();
            services.AddScoped<ISummaryService, SummaryService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LiveHAPIContext dbcontext,
            EMRContext emrContext, ISetupFacilty setupFacilty)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            app.UseMvcWithDefaultRoute();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            Log.Debug($"database initializing...");

            var bulkConfigName = Startup.Configuration["bulkConfig:name"];
            var bulkConfigCode = Startup.Configuration["bulkConfig:code"];

            try
            {
                DapperPlusManager.AddLicense(bulkConfigName, bulkConfigCode);
                if (!Z.Dapper.Plus.DapperPlusManager.ValidateLicense(out var licenseErrorMessage))
                {
                    throw new Exception(licenseErrorMessage);
                }
            }
            catch (Exception e)
            {
                Log.Debug($"{e}");
                throw;
            }


      bool imHapi = true;
            string herror = "";
            try
            {
                dbcontext.EnsureSeeded();
            }
            catch (Exception e)
            {
                herror = "Seeding";
                imHapi = false;
                Log.Error(new string('<', 30));
                Log.Error($"{e}");
                Log.Error(new string('>', 30));
            }

            Log.Debug($"database initializing... [Views]");
            try
            {
                dbcontext.CreateViews();
            }
            catch (Exception e)
            {
                herror = "Views";
                imHapi = false;
                Log.Error($"{e}");
            }

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<County, CountyInfo>();
                cfg.CreateMap<SubCounty, SubCountyInfo>();

                cfg.CreateMap<Category, CategoryInfo>();
                cfg.CreateMap<Item, ItemInfo>();
                cfg.CreateMap<CategoryItem, CategoryItemInfo>();

                cfg.CreateMap<PracticeType, PracticeTypeInfo>();
                cfg.CreateMap<IdentifierType, IdentifierTypeInfo>();
                cfg.CreateMap<RelationshipType, RelationshipTypeInfo>();
                cfg.CreateMap<KeyPop, KeyPopInfo>();
                cfg.CreateMap<MaritalStatus, MaritalStatusInfo>();
                cfg.CreateMap<ProviderType, ProviderTypeInfo>();
                cfg.CreateMap<Action, ActionInfo>();
                cfg.CreateMap<Condition, ConditionInfo>();
                cfg.CreateMap<ValidatorType, ValidatorTypeInfo>();
                cfg.CreateMap<CategoryItem, CategoryItemInfo>();
                cfg.CreateMap<ConceptType, ConceptTypeInfo>();
                cfg.CreateMap<Validator, ValidatorInfo>();
                cfg.CreateMap<EncounterType, EncounterTypeInfo>();

                cfg.CreateMap<SubscriberCohort, CohortInfo>();

                cfg.CreateMap<Encounter, EncounterInfo>();
                cfg.CreateMap<Obs, ObsInfo>();
                cfg.CreateMap<ObsTestResult, ObsTestResultInfo>();
                cfg.CreateMap<ObsFinalTestResult, ObsFinalTestResultInfo>();
                cfg.CreateMap<ObsTraceResult, ObsTraceResultInfo>();
                cfg.CreateMap<ObsLinkage, ObsLinkageInfo>();
                cfg.CreateMap<ObsMemberScreening, ObsMemberScreeningInfo>();
                cfg.CreateMap<ObsPartnerScreening, ObsPartnerScreeningInfo>();
                cfg.CreateMap<ObsFamilyTraceResult, ObsFamilyTraceResultInfo>();
                cfg.CreateMap<ObsPartnerTraceResult, ObsPartnerTraceResultInfo>();

                cfg.CreateMap<ClientSummaryInfo, ClientSummary>();

                cfg.CreateMap<Location, Practice>()
                    .ForMember(x => x.Code, o => o.MapFrom(s => s.PosID))
                    .ForMember(x => x.IsDefault, o => o.MapFrom(s => s.Preferred.HasValue && s.Preferred == 1))
                    .ForMember(x => x.Name, o => o.MapFrom(s => s.FacilityName));

                cfg.CreateMap<User, Core.Model.People.User>()
                    .ForMember(x => x.Source, o => o.MapFrom(s => s.UserFirstName))
                    .ForMember(x => x.SourceSys, o => o.MapFrom(s => s.UserLastName))
                    .ForMember(x => x.SourceRef, o => o.MapFrom(s => s.UserId));
                int userId;
                cfg.CreateMap<Core.Model.People.User, UserDTO>()
                    .ForMember(x => x.Password, o => o.MapFrom(s => s.DecryptedPassword))
                    .ForMember(x => x.UserId, o => o.MapFrom(s => int.TryParse(s.SourceRef, out userId) ? userId : 0));

                cfg.CreateMap<Person, PersonDTO>()
                    .ForMember(x => x.FirstName,
                        o => o.MapFrom(s => null != s.Names.FirstOrDefault() ? s.Names.FirstOrDefault().FirstName : ""))
                    .ForMember(x => x.MiddleName,
                        o => o.MapFrom(s =>
                            null != s.Names.FirstOrDefault() ? s.Names.FirstOrDefault().MiddleName : ""))
                    .ForMember(x => x.LastName,
                        o => o.MapFrom(s => null != s.Names.FirstOrDefault() ? s.Names.FirstOrDefault().LastName : ""));
                cfg.CreateMap<Provider, ProviderDTO>();

            });

            Log.Debug(@"
                            ╔═╗┌─┐┬ ┬┌─┐  ╔╦╗┌─┐┌┐ ┬ ┬  ┌─┐
                            ╠═╣├┤ └┬┘├─┤  ║║║│ │├┴┐│ │  ├┤ 
                            ╩ ╩└   ┴ ┴ ┴  ╩ ╩└─┘└─┘┴ ┴─┘└─┘
                      ");
            Log.Debug("");
            Log.Debug(@"
                                  _        _    ____ ___ 
                                 | |__    / \  |  _ \_ _|
                                 | '_ \  / _ \ | |_) | | 
                                 | | | |/ ___ \|  __/| | 
                                 |_| |_/_/   \_\_|  |___|
                    ");

            if (imHapi)
            {
                Log.Debug($"im hAPI !!! ");
            }
            else
            {
                Log.Error($"im NOT hAPI    >*|*< ");
                Log.Error($"cause: {herror}");
            }

        }
    }
}
