using System;
using System.Runtime.Serialization;

namespace NSPC.Data
{
    public class BaseModel
    {
        [IgnoreDataMember] public Guid ApplicationId { get; set; }

        //[IgnoreDataMember]
        //public BaseApplicationModel Application { get => ApplicationCollection.Instance.GetModel(ApplicationId); }
        [IgnoreDataMember] public Guid CreatedByUserId { get; set; }

        //[IgnoreDataMember]
        // public BaseUserModel CreatedByUser { get => UserCollection.Instance.GetModel(CreatedByUserId); }
        public DateTime CreatedOnDate { get; set; }

        [IgnoreDataMember] public Guid LastModifiedByUserId { get; set; }

        //[IgnoreDataMember]
        //public BaseUserModel LastModifiedByUser { get => UserCollection.Instance.GetModel(LastModifiedByUserId); }
        public DateTime LastModifiedOnDate { get; set; }
    }

    public class DeleteResponse<T>
    {
        public string Message { get; set; }
        public T Model { get; set; }
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
    }
}