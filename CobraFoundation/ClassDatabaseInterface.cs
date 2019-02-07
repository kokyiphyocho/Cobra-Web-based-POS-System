using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Configuration;
using System.Threading;
using System.Data.SqlClient;
using System.Web;

namespace CobraFoundation
{
    //******************************************************************************
    // [DatabaseInterface Class]
    //
    // - All functions related to accessing Database are collect here.
    //******************************************************************************
    public class DatabaseInterface
    {
        public static DatabaseInterface clDatabaseInterface;

        public static DatabaseInterface GetInstance()
        {
            if (clDatabaseInterface == null) clDatabaseInterface = new DatabaseInterface();
            return (clDatabaseInterface);
        }

        // SINGLETON CONSTRUCTOR.
        private DatabaseInterface() { }

        // Run the Query and Return the Result Database.
        public DataTable RunQuery(String paQuery, QueryClass.ConnectionMode paConnectionMode)
        {
            DataTable lcDataTable;
            SqlCommand lcCommand;
            SqlDataAdapter lcDataAdapter;
            SqlConnection lcConnection;

            if (paConnectionMode != QueryClass.ConnectionMode.None)
            {
                lcConnection = new SqlConnection((paConnectionMode == QueryClass.ConnectionMode.EService) && (!String.IsNullOrEmpty(ConfigClass.GetInstance().EServiceRemoteConnectionStr)) ? ConfigClass.GetInstance().EServiceRemoteConnectionStr : ConfigClass.GetInstance().DefaultConnectionStr);                                
                
                lcCommand = new SqlCommand(paQuery, lcConnection);

                lcCommand.CommandTimeout = ConfigClass.GetInstance().CommandTimeOut;
                lcDataAdapter = new SqlDataAdapter(lcCommand);
                lcDataTable = new DataTable();            
                lcDataAdapter.Fill(lcDataTable);
                return (lcDataTable);
            }
            else return (null);
        }

        // Run the Query and Return the Result Database.
        public DataSet RunQueryEx(String paQuery, QueryClass.ConnectionMode paConnectionMode)
        {
            DataSet lcDataSet;
            SqlCommand lcCommand;
            SqlDataAdapter lcDataAdapter;
            SqlConnection lcConnection;

            if (paConnectionMode != QueryClass.ConnectionMode.None)
            {
                lcConnection = new SqlConnection((paConnectionMode == QueryClass.ConnectionMode.EService) && (!String.IsNullOrEmpty(ConfigClass.GetInstance().EServiceRemoteConnectionStr)) ? ConfigClass.GetInstance().EServiceRemoteConnectionStr : ConfigClass.GetInstance().DefaultConnectionStr);                                

                lcCommand = new SqlCommand(paQuery, lcConnection);

                lcCommand.CommandTimeout = ConfigClass.GetInstance().CommandTimeOut;
                lcDataAdapter = new SqlDataAdapter(lcCommand);
                lcDataSet = new DataSet();
                lcDataAdapter.Fill(lcDataSet);
                return (lcDataSet);
            }
            else return (null);
        }

        // Execute the Query Return the Single Value result.
        public object ExecuteScalar(String paQuery, QueryClass.ConnectionMode paConnectionMode)
        {
            object lcResult;
            SqlCommand lcCommand;
            SqlConnection lcConnection;

            if (paConnectionMode != QueryClass.ConnectionMode.None)
            {
                lcConnection = new SqlConnection((paConnectionMode == QueryClass.ConnectionMode.EService) && (!String.IsNullOrEmpty(ConfigClass.GetInstance().EServiceRemoteConnectionStr)) ? ConfigClass.GetInstance().EServiceRemoteConnectionStr : ConfigClass.GetInstance().DefaultConnectionStr);                                

                lcCommand = new SqlCommand(paQuery, lcConnection);                

                try
                {
                    lcConnection.Open();
                    lcResult = lcCommand.ExecuteScalar();
                    lcConnection.Close();

                    if ((lcResult == null) || (lcResult.GetType().ToString() == "System.DBNull")) lcResult = null;
                }
                catch (Exception paException)
                {
                    lcConnection.Close();
                    throw new Exception("ExecuteScalar() : Query Execution Error", paException);
                }

                return (lcResult);
            }
            else return (null);
        }

        // Execute the Query and Return the Number of Effected Rows.
        public int ExecuteNonQuery(String paQuery, QueryClass.ConnectionMode paConnectionMode)
        {
            int             lcResult;
            SqlCommand      lcCommand;
            SqlConnection   lcConnection;
            SqlParameter    lcEffectedRowCount;
         
            if (paConnectionMode != QueryClass.ConnectionMode.None)
            {
                lcConnection = new SqlConnection((paConnectionMode == QueryClass.ConnectionMode.EService) && (!String.IsNullOrEmpty(ConfigClass.GetInstance().EServiceRemoteConnectionStr)) ? ConfigClass.GetInstance().EServiceRemoteConnectionStr : ConfigClass.GetInstance().DefaultConnectionStr);                                

                lcCommand = new SqlCommand(paQuery, lcConnection);
                lcEffectedRowCount = new SqlParameter("@CSP_EffectedRowCount", SqlDbType.Int) { Direction = ParameterDirection.Output };
                lcCommand.Parameters.Add(lcEffectedRowCount);
                
                try
                {
                    lcConnection.Open();
                    lcResult = lcCommand.ExecuteNonQuery();
                    lcConnection.Close();
                }
                catch (Exception paException)
                {
                    lcConnection.Close();
                    throw new Exception("ExecuteNonQuery() : Query Execution Error", paException);
                }

                if (lcEffectedRowCount.Value.GetType() != typeof(DBNull)) return (General.ParseInt(lcEffectedRowCount.Value.ToString(), -1));
                else return (lcResult);
            }
            else return (-1);
        }

        // Execute the Query and Fetch the Value from the Feedback Variable.
        public object ExecuteFeedBackQuery(String paQuery, String paFeedBackVar, QueryClass.ConnectionMode paConnectionMode)
        {
            object lcResult;
            SqlCommand lcCommand;
            SqlConnection lcConnection;

            lcResult = null;

            if (paConnectionMode != QueryClass.ConnectionMode.None)
            {
                lcConnection = new SqlConnection((paConnectionMode == QueryClass.ConnectionMode.EService) && (!String.IsNullOrEmpty(ConfigClass.GetInstance().EServiceRemoteConnectionStr)) ? ConfigClass.GetInstance().EServiceRemoteConnectionStr : ConfigClass.GetInstance().DefaultConnectionStr);                                
                lcCommand = new SqlCommand(paQuery, lcConnection);

                try
                {
                    lcConnection.Open();
                    lcCommand.ExecuteNonQuery();

                    lcCommand.CommandText = "SELECT " + paFeedBackVar;
                    lcResult = lcCommand.ExecuteScalar();

                    lcConnection.Close();
                }
                catch (Exception paException)
                {
                    lcConnection.Close();
                    throw new Exception("ExecuteFeedBackQuery() : Query Execution Error", paException);
                }
            }

            return (lcResult);
        } // DatabaseInterface Class
    }
}
