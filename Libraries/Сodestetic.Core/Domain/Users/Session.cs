using Specter.Web.Core.Data;
using Specter.Web.Core.Domain.GPS;
using System;
using System.Collections.Generic;

namespace Specter.Web.Core.Domain.Users
{
    public partial class Session : BaseEntity
    {
        #region Fields
        //private IRepository<Device> deviceRepository;
        //private IRepository<User> userRepository;
        #endregion Fields

        #region Constructors
        public Session() { }
        #endregion Constructors

        #region Properties
        public long UserId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? ExpireOnUtc { get; set; }
        public string IPAddress { get; set; }
        public long? DeviceId { get; set; }
        #endregion Properties

        #region Entity
        //public virtual Device Device
        //{
        //    get 
        //    {
        //        if (DeviceId == null) return new Device();
        //        return deviceRepository.GetById(DeviceId); 
        //    }
        //}
        //public virtual User Device
        //{
        //    get
        //    {
        //        if (UserId == null) return new User();
        //        return userRepository.GetById(UserId);
        //    }
        //}
        #endregion Entity
    }
}
