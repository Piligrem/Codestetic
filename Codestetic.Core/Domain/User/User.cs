using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Codestetic.Core.Domain.Common;
using Codestetic.Core.Domain.Documents;
//using Codestetic.Core.Domain.Forums;

namespace Codestetic.Core.Domain.User
{
    /// <summary>
    /// Represents a employee
    /// </summary>
    [DataContract]
    public partial class User : BaseEntity
    {
        #region Fields
        private ICollection<Document> _document;
        private ICollection<Address> _addresses;
        // private ICollection<ForumTopic> _forumTopics;
        // private ICollection<ForumPost> _forumPosts;
        #endregion Fields

        #region Constructors
        /// <summary>
		/// Ctor
		/// </summary>
        public User()
        {
            this.UserGuid = Guid.NewGuid();
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets or sets the employee Guid
        /// </summary>
        [DataMember]
        public Guid UserGuid { get; set; }

		/// <summary>
		/// Gets or sets the username
		/// </summary>
        [DataMember]
        public string Username { get; set; }

		/// <summary>
		/// Gets or sets the email
		/// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the password salt
        /// </summary>
        public string PasswordSalt { get; set; }

		/// <summary>
		/// Gets or sets the password
		/// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the last IP address
        /// </summary>
        public string LastIpAddress { get; set; }

        /// <summary>
        /// Gets or sets the date and time of entity creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last login
        /// </summary>
        public DateTime? LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the date and time of last activity
        /// </summary>
        public DateTime LastActivityDateUtc { get; set; }
        #endregion Properties

        #region Navigation properties


        /// <summary>
        /// Gets or sets orders
        /// </summary>
        public virtual ICollection<Document> Documents
        {
            get { return _document ?? (_document = new List<Document>()); }
            protected set { _document = value; }            
        }

        /// <summary>
        /// Gets or sets customer addresses
        /// </summary>
        public virtual ICollection<Address> Addresses
        {
            get { return _addresses ?? (_addresses = new List<Address>()); }
            protected set { _addresses = value; }            
        }
        
        #endregion       
    }
}
