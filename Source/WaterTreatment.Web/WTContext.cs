using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Mapping;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using WaterTreatment.Web.Entities;
using System.Threading.Tasks;
using System.Threading;
using WaterTreatment.Web.Services;

namespace WaterTreatment.Web
{
    public partial class WTContext : IdentityDbContext<User, Role, int, UserLogin, UserRole, UserClaim>
    {

        //Must be injected in, see CompositionRoot.cs
        public IUserService _userService { get; set; }

        public IDbSet<RoleAction> RoleActions { get; set; }
        public IDbSet<LandingAction> LandingActions { get; set; }

        public IDbSet<MainSection> MainSections { get; set; }
        public IDbSet<SubSection> SubSections { get; set; }

        public IDbSet<SystemType> SystemTypes { get; set; }
        public IDbSet<Parameter> Parameters { get; set; }
        public IDbSet<ParameterType> ParameterTypes { get; set; }
        public IDbSet<ParameterBound> Bounds { get; set; }
        public IDbSet<Vendor> Vendors { get; set; }
        public IDbSet<Location> Locations { get; set; }
        public IDbSet<Site> Sites { get; set; }
        public IDbSet<Building> Buildings { get; set; }
        public IDbSet<BuildingSystem> BuildingSystems { get; set; }
        //public IDbSet<BuildingReport> BuildingReports { get; set; }
        public IDbSet<Report> Reports { get; set; }
        public IDbSet<Attachment> Attachments { get; set; }
        public IDbSet<SystemMeasurement> SystemMeasurements { get; set; }
        public IDbSet<Measurement> Measurements { get; set; }

        //public IDbSet<UserSiteAccess> SiteAccess { get; set; }

        public IDbSet<Setting> Settings { get; set; }
        public IDbSet<ExtensionWhitelist> ExtensionWhitelist { get; set; }
        public IDbSet<ReportSubscription> ReportSubscriptions { get; set; }

        public DbSet<AdhocParameter> AdhocParameters { get; set; }

        public DbSet<AdhocMeasurement> AdhocMeasurements { get; set; }

        //So I have addrange support, because for whatever genius reason only the concrete implement has but no the interface.
        public DbSet<AuditEntity> AuditEntities { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer<WTContext>(null);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<Role>().ToTable("Role");
            modelBuilder.Entity<UserRole>().ToTable("UserRole");
            modelBuilder.Entity<UserLogin>().ToTable("UserLogin");
            modelBuilder.Entity<UserClaim>().ToTable("UserClaim");

            modelBuilder.Entity<Site>().
            HasMany(c => c.SiteAccess).
            WithMany(p => p.userSiteAccess).
            Map(
                m =>
                {
                    m.MapLeftKey("Site_Id");  //Site_Id   User_Id
                    m.MapRightKey("User_Id");
                    m.ToTable("UserSiteAccess");
                });

            modelBuilder.Entity<User>().
            HasMany(c => c.BuildingSystems).
            WithMany(p => p.Users).
            Map(
                m =>
                {
                    m.MapLeftKey("User_Id");  //Site_Id   User_Id
                    m.MapRightKey("BuildingSystem_Id");
                    m.ToTable("UserBuildingSystem");
                });

        }

        private void HandleReportAuditing()
        {

            //Seeding
            if (_userService == null || !_userService.HasContext)
                return;

            var currentUser = _userService.CurrentUser;
            if (currentUser == null)
                return;

            HandleAuditRecords<Report>("Report", currentUser);
            // HandleAuditRecords<BuildingReport>("BuildingReport", currentUser);
            HandleAuditRecords<SystemMeasurement>("SystemMeasurement", currentUser);
            HandleAuditRecords<Measurement>("Measurement", currentUser);
        }

        private void HandleAuditRecords<TEntity>(string TableName, User CurrentUser) where TEntity : class, IEntity
        {
            var reportRecords = ChangeTracker.Entries<TEntity>().Where(x => x.State == EntityState.Added
                || x.State == EntityState.Modified
                || x.State == EntityState.Deleted).ToList();

            //Needs select many because 1 record can generate multiple log entries
            AuditEntities.AddRange(reportRecords.SelectMany(x => GetAuditRecordsForChange(x, TableName, CurrentUser)));
        }

        //This still needs to be generic because of the table name
        private IEnumerable<AuditEntity> GetAuditRecordsForChange<TEntity>(DbEntityEntry<TEntity> Record, string TableName, User ModifiedBy) where TEntity : class, IEntity
        {

            var auditLog = new List<AuditEntity>();

            var instanceGuid = Guid.NewGuid();

            //One single audit record can produce multiple changes at once

            DbPropertyValues Values = Record.State == EntityState.Added ? Record.CurrentValues : Record.OriginalValues;

            IEnumerable<String> properties = Values.PropertyNames.ToDictionary(pn => pn, pn => Values[pn]).Keys;

            Dictionary<String, Object> Original = Record.State != EntityState.Added ? Record.OriginalValues.PropertyNames.ToDictionary(pn => pn, pn => Record.OriginalValues[pn]) : null;
            Dictionary<String, Object> Current = Record.State != EntityState.Deleted ? Record.CurrentValues.PropertyNames.ToDictionary(pn => pn, pn => Record.CurrentValues[pn]) : null;

            foreach (var field in properties)
            {

                var AE = new AuditEntity();

                AE.ModifiedOn = DateTime.UtcNow;
                AE.EventType = Record.State;
                AE.EventInstance = instanceGuid;
                AE.TableName = TableName;//Not generic
                AE.EntityId = Record.Entity.Id;
                AE.ColumnName = field;

                if (Record.State != EntityState.Added)
                    AE.OriginalValue = Convert.ToString(Original[field]);
                if (Record.State != EntityState.Deleted)
                    AE.CurrentValue = Convert.ToString(Current[field]);

                AE.ModifiedBy = ModifiedBy;

                auditLog.Add(AE);

            }

            return auditLog;
        }

        public override int SaveChanges()
        {
            HandleReportAuditing();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            HandleReportAuditing();
            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            HandleReportAuditing();
            return base.SaveChangesAsync(cancellationToken);
        }

    }

}