using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SimpleInjector;
using System;
using System.Data.Entity;
using WaterTreatment.Web.Attributes;
using WaterTreatment.Web.Entities;
using WaterTreatment.Web.Properties;
using WaterTreatment.Web.Services;
using WaterTreatment.Web.Services.Interface;

namespace WaterTreatment.Web
{

    public class CompositionRoot
    {

        public static Container Build()
        {
            Container DIContainer = new Container();

            DIContainer.RegisterMvcControllers();
            DIContainer.RegisterMvcIntegratedFilterProvider();

            DIContainer.RegisterPerWebRequest<WTContext>(() => {                
                var context = new WTContext();
                context._userService = new UserService(context);
                return context;
            });

            // Tell it not to dispose at end of request because the previous registration will do it.
            DIContainer.RegisterPerWebRequest<DbContext>(DIContainer.GetInstance<WTContext>, false); 

            DIContainer.RegisterPerWebRequest<IUserStore<User, int>, UserStore<User, Role, int, UserLogin, UserRole, UserClaim>>();
            DIContainer.RegisterPerWebRequest<IRoleStore<Role, int>, RoleStore<Role, int, UserRole>>();
            DIContainer.RegisterPerWebRequest<UserManager<User, int>>();
            DIContainer.RegisterPerWebRequest<IFileStorage, FileStorage>();
            DIContainer.RegisterPerWebRequest<IAWSS3, AWSS3>();
            DIContainer.RegisterPerWebRequest<IEmailService, EmailService>();
            DIContainer.RegisterPerWebRequest<IUserService, UserService>();
            DIContainer.RegisterPerWebRequest<IReportService, ReportService>();
            DIContainer.RegisterPerWebRequest<IPDFService, PDFService>();

            RegisterAttributes(DIContainer);
            RegisterLockoutDefaults(DIContainer);

            var userManager = DIContainer.GetInstance<UserManager<User, int>>();

            //This is necessary because of limitations in the IdentityDBContext
            using (var context = new WTContext())
            {
//#if !Production
//                userManager.AddToRole(context.Ref.Users.NIKA.Id, context.Ref.Roles.SystemAdministrator.Name);
//                userManager.AddToRole(context.Ref.Users.NIKATest.Id, context.Ref.Roles.SystemAdministrator.Name);
//                userManager.AddToRole(context.Ref.Users.DR.Id, context.Ref.Roles.DataRecorder.Name);
//                userManager.AddToRole(context.Ref.Users.ERV.Id, context.Ref.Roles.ExecutiveReportViewer.Name);
//                userManager.AddToRole(context.Ref.Users.RV.Id, context.Ref.Roles.ReportViewer.Name);
//                userManager.AddToRole(context.Ref.Users.SA.Id, context.Ref.Roles.SiteAdministrator.Name);
//                userManager.AddToRole(context.Ref.Users.AuditEngineer.Id, context.Ref.Roles.AuditRecorder.Name);
//                userManager.AddToRole(context.Ref.Users.AuditDataEngineer.Id, context.Ref.Roles.DataRecorder.Name);
//                userManager.AddToRole(context.Ref.Users.AuditDataEngineer.Id, context.Ref.Roles.AuditRecorder.Name);
//#endif

            }

            DIContainer.Verify();

            return DIContainer;
        }

        private static void RegisterAttributes(Container DIContainer)
        {

            DIContainer.RegisterInitializer<SectionAuthorize>(x => {
                x.Context = DIContainer.GetInstance<WTContext>();
            });
        }

        private static void RegisterLockoutDefaults(Container DIContainer) {
            DIContainer.RegisterInitializer<UserManager<User, int>>(m => {
                var settings = Properties.Settings.Default;
                m.UserLockoutEnabledByDefault = true;
                m.MaxFailedAccessAttemptsBeforeLockout = settings.MaxFailedAccessAttemptsBeforeLockout;
                m.DefaultAccountLockoutTimeSpan = TimeSpan.FromMinutes(settings.DefaultAccountLockoutTimeSpan);
            });
        }

    }

}