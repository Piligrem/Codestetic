using System;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Domain.Messages;
using Codestetic.Web.Core.Domain.Forums;

namespace Codestetic.Web.Services.Messages
{
    public partial interface IWorkflowMessageService
    {
        #region User workflow
        /// <summary>
        /// Sends 'New user' notification message to a store owner
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendUserRegisteredNotificationMessage(User user, long languageId);

        /// <summary>
        /// Sends a welcome message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendUserWelcomeMessage(User user, long languageId);

        /// <summary>
        /// Sends an email validation message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendUserEmailValidationMessage(User user, long languageId);

        /// <summary>
        /// Sends password recovery message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendUserPasswordRecoveryMessage(User user, long languageId);
        #endregion User workflow

        #region Newsletter workflow
        /// <summary>
        /// Sends a newsletter subscription activation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendNewsLetterSubscriptionActivationMessage(NewsLetterSubscription subscription, long languageId);

        /// <summary>
        /// Sends a newsletter subscription deactivation message
        /// </summary>
        /// <param name="subscription">Newsletter subscription</param>
        /// <param name="languageId">Language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendNewsLetterSubscriptionDeactivationMessage(NewsLetterSubscription subscription, long languageId);
        #endregion Newsletter workflow

        #region Forum Notifications
        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendNewForumTopicMessage(User user, ForumTopic forumTopic, Forum forum, long languageId);

        /// <summary>
        /// Sends a forum subscription message to a user
        /// </summary>
        /// <param name="user">User instance</param>
        /// <param name="forumPost">Forum post</param>
        /// <param name="forumTopic">Forum Topic</param>
        /// <param name="forum">Forum</param>
        /// <param name="friendlyForumTopicPageIndex">Friendly (starts with 1) forum topic page to use for URL generation</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendNewForumPostMessage(User user,
            ForumPost forumPost, ForumTopic forumTopic,
            Forum forum, int friendlyForumTopicPageIndex,
            long languageId);

        /// <summary>
        /// Sends a private message notification
        /// </summary>
        /// <param name="privateMessage">Private message</param>
        /// <param name="languageId">Message language identifier</param>
        /// <returns>Queued email identifier</returns>
        long SendPrivateMessageNotification(PrivateMessage privateMessage, long languageId);
        #endregion Forum Notifications


        #region Misc
        /// <summary>
        /// Sends a generic message
        /// </summary>
        /// <param name="messageTemplateName">The name of the message template</param>
        /// <param name="cfg">Configurator action for the message</param>
        /// <returns>Queued email identifier</returns>
        long SendGenericMessage(string messageTemplateName, Action<GenericMessageContext> cfg);
        #endregion Misc
    }
}
