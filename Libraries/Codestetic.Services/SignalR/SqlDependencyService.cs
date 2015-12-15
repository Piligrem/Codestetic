using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codestetic.Web.Services.GPS;
using Codestetic.Web.Core.Domain.GPS;
using System.Diagnostics;
using Codestetic.Web.Core.Data;
using System.Data.SqlClient;
using System.Data;
using Codestetic.Web.Core.Domain.Users;
using Codestetic.Web.Core.Logging;
using System.Threading;
using Codestetic.Web.Core;
//using Specter.Web.Data.Notification;
using Codestetic.Web.Services.Users;
using Codestetic.Web.Services.Devices;

namespace Codestetic.Web.Services.SignalR
{
    public class SqlDependencyService<TEntity> where TEntity : BaseEntity
    {
        #region Constants
        private readonly string deviceIndicatorQuery = "Select [Id],[DeviceId],[TrackId],[DateOnUtc],[Satellites],[Battery],[GSM] From [dbo].[DeviceIndicator]";
        private readonly string devicePositionQuery = "Select [Id],[TimestampOnUtc],[DeviceId],[DateOnUtc],[Valid],[Position],[Altitude],[Angle],[Speed] From [dbo].[DevicePosition]";
        #endregion Constants

        #region Fields
        private readonly IDbContext context;
        private readonly IDeviceIndicatorService deviceIndicatorService;
        private readonly IDevicePositionService devicePositionService;
        private readonly IRepository<SignalR_> signalRRepository;
        private readonly ILogger logger;
        //private ImmediateNotificationRegister<SignalR_> notification = null;
        #endregion Fields

        #region Constructors
        public SqlDependencyService()
        {
            var entity = EngineContext.Current.Resolve<IRepository<TEntity>>();
            //var query = entity.TableName;
            context = EngineContext.Current.Resolve<IDbContext>();
            deviceIndicatorService = EngineContext.Current.Resolve<IDeviceIndicatorService>();
            devicePositionService = EngineContext.Current.Resolve<IDevicePositionService>();
            signalRRepository = EngineContext.Current.Resolve<IRepository<SignalR_>>();

            logger = EngineContext.Current.Resolve<ILogger>();

            var iQuery = signalRRepository.Table;
            //notification = new ImmediateNotificationRegister<SignalR_>(context, iQuery);
            //notification.OnChanged += NotificationOnChanged;
        }
        #endregion Constructors

        #region Properties
        #endregion Properties

        #region Methods
        public IEnumerable<DeviceIndicator> GetDeviceIndicatorByUser(User user)
        {
            try
            {
                Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "start", "SignalRInfoRepository.GetData");

                using (var connection = new SqlConnection(DataSettings.Current.DataConnectionStringNotify))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (SqlCommand command = new SqlCommand(deviceIndicatorQuery, connection))
                    {
                        // Make sure the command object does not already have
                        // a notification object associated with it.
                        command.Notification = null;

                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += OnSqlDependency_DevicePosition;

                        Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "end", "SignalRInfoRepository.GetData");

                        using (var reader = command.ExecuteReader())
                            reader.Read();
                        connection.Close();

                        return deviceIndicatorService.GetAllDeviceIndicatorByUser(user);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("SqlDependencyService.GetDeviceIndicatorByUser", ex);
                throw;
            }
        }
        public IEnumerable<DevicePosition> GetDevicePositionByUser(User user)
        {
            try
            {
                Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "start", "SignalRInfoRepository.GetData");

                using (var connection = new SqlConnection(DataSettings.Current.DataConnectionStringNotify))
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (SqlCommand command = new SqlCommand(devicePositionQuery, connection))
                    {
                        // Make sure the command object does not already have
                        // a notification object associated with it.
                        command.Notification = null;

                        SqlDependency dependency = new SqlDependency(command);
                        dependency.OnChange += OnSqlDependency;

                        Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "end", "SignalRInfoRepository.GetData");

                        using (var reader = command.ExecuteReader())
                            reader.Read();

                        return devicePositionService.GetAllDeviceLastPositionByUser(user);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("SqlDependencyService.GetDevicePositionByUser", ex);
                throw;
            }
        }

        public IEnumerable<SignalR_> GetData()
        {
            Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "start", "SignalRInfoRepository.GetData");

            using (var connection = new SqlConnection(DataSettings.Current.DataConnectionStringNotify))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(@"SELECT [Id],[Message],[DeviceId],[DateOnUtc] FROM [dbo].[SignalR_]", connection))
                //using (SqlCommand command = new SqlCommand(@"SELECT S.Id, S.Message, S.DeviceId, S.DateOnUtc FROM SignalR_ S INNER JOIN CHANGETABLE(CHANGES SignalR_, 1) C ON C.Id = S.Id", connection))
                {
                    // Make sure the command object does not already have
                    // a notification object associated with it.
                    command.Notification = null;

                    SqlDependency dependency = new SqlDependency(command, null, 30000);
                    dependency.OnChange += OnSqlDependency;

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();


                    Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), "end", "SignalRInfoRepository.GetData");

                    using (var reader = command.ExecuteReader())
                        reader.Read();
                    connection.Close();

                    var signalRRepository = EngineContext.Current.Resolve<IRepository<SignalR_>>();
                    return signalRRepository.Table.ToList();
                }
            }
        }

        //private Task GetData(SqlCommand command, DataSet dataToWatch, string tableName)
        //{
            // Start the retrieval of data on another thread to let the UI thread free
            //ThreadPool.QueueUserWorkItem(o =>
            //{
            //    running.WaitOne();

            //    // Empty the dataset so that there is only
            //    // one batch of data displayed.
            //    dataToWatch.Clear();

            //    // Make sure the command object does not already have
            //    // a notification object associated with it.
            //    command.Notification = null;

            //    // Create and bind the SqlDependency object
            //    // to the command object.
            //    SqlDependency dependency = new SqlDependency(command);

            //    dependency.OnChange += new OnChangeEventHandler(dependency_OnChange);

            //    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
            //    {
            //        adapter.Fill(dataToWatch, tableName);

            //        running.Set();

            //        // Update the UI
            //        dgv.Invoke(() =>
            //        {
            //            dgv.DataSource = dataToWatch;
            //            dgv.DataMember = tableName;
            //            dgv.FirstDisplayedScrollingRowIndex = dgv.Rows.Count - 1;
            //        });
            //    }
            //});
        //}

        private void OnSqlDependency(object sender, SqlNotificationEventArgs e)
        {
            ((SqlDependency)sender).OnChange -= OnSqlDependency;
            Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), e.Type, "SignalRInfoRepository.dependency_OnChange");
            NotificationHub.Show();
        }

        private void NotificationOnChanged(object sender, EventArgs e)
        {
            //NotificationHub.Show();

            ////GetDataSignalR();
        }

        public IEnumerable<SignalR_> GetDataSignalR()
        {
            return signalRRepository.Table.ToList();
        }

        private void OnSqlDependency_DevicePosition(object sender, SqlNotificationEventArgs e)
        {
            ((SqlDependency)sender).OnChange -= OnSqlDependency_DevicePosition;
            Debug.WriteLine("#{0} - Event: {1}, Method: {2}", DateTime.Now.ToString("hh:mm:ss.fff"), e.Type, "SignalRInfoRepository.dependency_OnChange");
            //NotificationHub.Show();
        }
        #endregion Methods
    }
}
