using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSPC.Common;

namespace NSPC.Data
{
    public class BaseTableDefault
    {
        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;

        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? CreatedOnDate { get; set; } = DateTime.Now;
        public virtual Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Idm_Tenant Idm_Tenant { get; set; }
    }
    public class BaseTable<T> where T : BaseTable<T>
    {
        #region Init Create/Update

        public T InitCreate()
        {
            return InitCreate(AppConstants.HO_APP, UserConstants.AdministratorUserId);
        }

        public T InitCreate(Guid? application, Guid? userId)
        {
            return InitCreate(application ?? AppConstants.HO_APP, userId ?? UserConstants.AdministratorUserId);
        }

        public T InitCreate(Guid? application, Guid userId)
        {
            return InitCreate(application ?? AppConstants.HO_APP, userId);
        }

        public T InitCreate(Guid application, Guid? userId)
        {
            return InitCreate(application, userId ?? UserConstants.AdministratorUserId);
        }

        public T InitCreate(Guid application, Guid userId)
        {
            ApplicationId = application;
            CreatedByUserId = userId;
            LastModifiedByUserId = userId;
            CreatedOnDate = DateTime.Now;
            LastModifiedOnDate = DateTime.Now;
            return (T)this;
        }

        public T InitUpdate()
        {
            return InitUpdate(AppConstants.HO_APP, UserConstants.AdministratorUserId);
        }

        public T InitUpdate(Guid? application, Guid? userId)
        {
            return InitUpdate(application ?? AppConstants.HO_APP, userId ?? UserConstants.AdministratorUserId);
        }

        public T InitUpdate(Guid? application, Guid userId)
        {
            return InitUpdate(application ?? AppConstants.HO_APP, userId);
        }

        public T InitUpdate(Guid application, Guid? userId)
        {
            return InitUpdate(application, userId ?? UserConstants.AdministratorUserId);
        }

        public T InitUpdate(Guid application, Guid userId)
        {
            ApplicationId = application;
            LastModifiedByUserId = userId;
            LastModifiedOnDate = DateTime.Now;
            return (T)this;
        }

        #endregion Init Create/Update

        public Guid CreatedByUserId { get; set; } = UserConstants.AdministratorUserId;
        public Guid LastModifiedByUserId { get; set; } = UserConstants.AdministratorUserId;
        public DateTime LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public virtual Guid ApplicationId { get; set; } = AppConstants.HO_APP;
        public virtual Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Idm_Tenant Idm_Tenant { get; set; }
    }

    public class BaseTableService<T> where T : BaseTableService<T>
    {
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public virtual Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Idm_Tenant Idm_Tenant { get; set; }
    }
    public class BaseTableService
    {
        public Guid CreatedByUserId { get; set; }
        public Guid? LastModifiedByUserId { get; set; }
        public DateTime? LastModifiedOnDate { get; set; } = DateTime.Now;
        public DateTime CreatedOnDate { get; set; } = DateTime.Now;
        public string CreatedByUserName { get; set; }
        public string LastModifiedByUserName { get; set; }
        public virtual Guid? TenantId { get; set; }
        [ForeignKey("TenantId")]
        public virtual Idm_Tenant Idm_Tenant { get; set; }
    }
}
