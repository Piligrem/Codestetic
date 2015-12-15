using System.Collections.Generic;
using Codestetic.Web.Core.Domain.Topics;

namespace Codestetic.Web.Services.Topics
{
    /// <summary>
    /// Topic service interface
    /// </summary>
    public partial interface ITopicService
    {
        /// <summary>
        /// Deletes a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void DeleteTopic(Topic topic);

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="topicId">The topic identifier</param>
        /// <returns>Topic</returns>
        Topic GetTopicById(long topicId);

        /// <summary>
        /// Gets a topic
        /// </summary>
        /// <param name="systemName">The topic system name</param>
		/// <param name="storeId">Store identifier</param>
        /// <returns>Topic</returns>
		Topic GetTopicBySystemName(string systemName);

        /// <summary>
        /// Gets all topics
        /// </summary>
		/// <param name="storeId">Store identifier; pass 0 to load all records</param>
        /// <returns>Topics</returns>
		IList<Topic> GetAllTopics();

        /// <summary>
        /// Inserts a topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void InsertTopic(Topic topic);

        /// <summary>
        /// Updates the topic
        /// </summary>
        /// <param name="topic">Topic</param>
        void UpdateTopic(Topic topic);
    }
}
