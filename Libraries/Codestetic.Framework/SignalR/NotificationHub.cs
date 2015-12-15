using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Specter.Web.Core.Domain.Security;
using Specter.Web.Core.Domain.GPS;
using Specter.Web.Services.GPS;
using Specter.Web.Core.Data;

namespace Specter.Web.Framework.SignalR
{
    //[HubName("notificationHub")]
    //public class NotificationHub : Hub
    //{
    //    #region Fields
    //    private static readonly ConcurrentDictionary<long, DevicePosition> DevicePositions = new ConcurrentDictionary<long, DevicePosition>();
    //    Int16 totalNewMessages = 0;
    //    Int16 totalNewCircles = 0;
    //    Int16 totalNewJobs = 0;
    //    Int16 totalNewNotification = 0;
    //    private IRepository<DevicePosition> _devicePositionRepository;
    //    #endregion Fields

    //    #region Constructor
    //    public NotificationHub(IRepository<DevicePosition> devicePositionRepository)
    //    {
    //        _devicePositionRepository = devicePositionRepository;
    //    }
    //    #endregion Constructor

    //    #region Methods
    //    /// <summary>
    //    /// Provides the handler for SignalR OnConnected event
    //    /// supports async threading
    //    /// </summary>
    //    /// <returns></returns>
    //    public override Task OnConnected()
    //    {
    //        //string profileId = "111"; //Context.QueryString["id"];
    //        var id = 1;
    //        string connectionId = Context.ConnectionId;
    //        var user = DevicePositions.GetOrAdd(id, _ => new DevicePosition()
    //        {
    //            ConnectionIds = new HashSet<string>()
    //        });
    //        lock (user.ConnectionIds)
    //        {
    //            user.ConnectionIds.Add(connectionId);
    //            Groups.Add(connectionId, user.ProfileId);
    //        }
    //        return base.OnConnected();
    //    }

    //    /// <summary>
    //    /// Provides the handler for SignalR OnDisconnected event
    //    /// supports async threading
    //    /// </summary>
    //    /// <returns></returns>
    //    public override Task OnDisconnected()
    //    {
    //        long id = Context.QueryString["id"];
    //        string connectionId = Context.ConnectionId;
    //        DevicePosition devicePosition;
    //        DevicePositions.TryGetValue(id, out devicePosition);
    //        if (devicePosition != null)
    //        {
    //            lock (devicePosition.ConnectionIds)
    //            {
    //                devicePosition.ConnectionIds.RemoveWhere(cid => cid.Equals(connectionId));
    //                Groups.Remove(connectionId, devicePosition.ProfileId);
    //                if (!devicePosition.ConnectionIds.Any())
    //                {
    //                    DevicePosition removedDevicePosition;
    //                    DevicePositions.TryRemove(id, out removedDevicePosition);
    //                }
    //            }
    //        }
    //        return base.OnDisconnected();
    //    }

    //    /// <summary>
    //    /// Provides the handler for SignalR OnReconnected event
    //    /// supports async threading
    //    /// </summary>
    //    /// <returns></returns>
    //    public override Task OnReconnected()
    //    {
    //        return base.OnReconnected();
    //    }

    //    /// <summary>
    //    /// Provides the facility to send individual user notification message
    //    /// </summary>
    //    /// <param name="profileId">
    //    /// Set to the ProfileId of user who will receive the notification
    //    /// </param>
    //    /// <param name="message">
    //    /// set to the notification message
    //    /// </param>
    //    public void Send(string profileId, string message)
    //    {
    //        //Clients.User(profileId).send(message);
    //    }

    //    /// <summary>
    //    /// Provides the facility to send group notification message
    //    /// </summary>
    //    /// <param name="username">
    //    /// set to the user groupd name who will receive the message
    //    /// </param>
    //    /// <param name="message">
    //    /// set to the notification message
    //    /// </param>
    //    public void SendUserMessage(String username, String message)
    //    {
    //        Clients.Group(username).sendUserMessage(message);
    //    }

    //    /// <summary>
    //    /// Provides the ability to get User from the dictionary for passed in profileId
    //    /// </summary>
    //    /// <param name="profileId">
    //    /// set to the profileId of user that need to be fetched from the dictionary
    //    /// </param>
    //    /// <returns>
    //    /// return User object if found otherwise returns null
    //    /// </returns>
    //    private DevicePosition GetUser(long id)
    //    {
    //        DevicePosition devicePosition;
    //        DevicePositions.TryGetValue(id, out devicePosition);
    //        return devicePosition;
    //    }

    //    /// <summary>
    //    /// Provide theability to get currently connected user
    //    /// </summary>
    //    /// <returns>
    //    /// profileId of user based on current connectionId
    //    /// </returns>
    //    public IEnumerable<string> GetConnectedUser()
    //    {
    //        return DevicePositions.Where(x =>
    //        {
    //            lock (x.Value.ConnectionIds)
    //            {
    //                return !x.Value.ConnectionIds.Contains(Context.ConnectionId, StringComparer.InvariantCultureIgnoreCase);
    //            }
    //        }).Select(x => x.Key);
    //    }
    //    #endregion

    //    [HubMethodName("sendNotifications")]
    //    public string SendNotifications()
    //    {
    //        using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString))
    //        {
    //            string query = "SELECT  NewMessageCount, NewCircleRequestCount, NewNotificationsCount, NewJobNotificationsCount FROM [dbo].[Modeling_NewMessageNotificationCount] WHERE UserProfileId=" + "62021";
    //            connection.Open();
    //            using (SqlCommand command = new SqlCommand(query, connection))
    //            {
    //                try
    //                {
    //                    command.Notification = null;
    //                    DataTable dt = new DataTable();
    //                    SqlDependency dependency = new SqlDependency(command);
    //                    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);
    //                    if (connection.State == ConnectionState.Closed)
    //                        connection.Open();
    //                    var reader = command.ExecuteReader();
    //                    dt.Load(reader);
    //                    if (dt.Rows.Count > 0)
    //                    {
    //                        totalNewMessages = Int16.Parse(dt.Rows[0]["NewMessageCount"].ToString());
    //                        totalNewCircles = Int16.Parse(dt.Rows[0]["NewCircleRequestCount"].ToString());
    //                        totalNewJobs = Int16.Parse(dt.Rows[0]["NewJobNotificationsCount"].ToString());
    //                        totalNewNotification = Int16.Parse(dt.Rows[0]["NewNotificationsCount"].ToString());
    //                    }
    //                    connection.Close();
    //                }
    //                catch (Exception ex)
    //                {
    //                    throw;
    //                }
    //            }
    //        }
    //        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<NotificationHub>();
    //        return context.Clients.All.RecieveNotification(totalNewMessages, totalNewCircles, totalNewJobs, totalNewNotification);
    //    }

    //    private void dependency_OnChange(object sender, SqlNotificationEventArgs e)
    //    {
    //        if (e.Type == SqlNotificationType.Change)
    //        {
    //            NotificationHub nHub = new NotificationHub(_devicePositionRepository);
    //            nHub.SendNotifications();
    //        }
    //    }
    //}
}
