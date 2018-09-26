using System;
using System.Data;
using System.Data.SqlClient;

namespace C_Box
{
    public class SQLTransaction
    {
        SqlCommand sqlCommand;
        SqlConnection sqlConnection;
        string connectionString;

        public string UserID
        { get; set; }

        public string Password
        { get; set; }

        public string Server
        { get; set; }

        public string DataBase
        { get; set; }

        public SQLTransaction()
        {
            UserID = "";
            Password = "";
            Server = "";
            DataBase = "";
        }

        public void GetNextAvailableMAC(out int id, out string mac)
        {
            SqlDataReader reader;
            id = 0;
            mac = "";
            connectionString = $"Server={Server}; Database={DataBase}; User Id={UserID}; Password={Password}";
            try
            {
                using (sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlCommand = new SqlCommand("spGetMACAdresses", sqlConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.Add("@intMACSToSelect", SqlDbType.Int).Value = 1;
                    sqlCommand.Parameters.Add("@strStationName", SqlDbType.VarChar).Value = "IT_TEST001";
                    sqlCommand.Parameters[0].Direction = ParameterDirection.Input;
                    sqlCommand.Parameters[1].Direction = ParameterDirection.Input;
                    sqlConnection.Open();
                    reader = sqlCommand.ExecuteReader();
                    while(reader.Read())
                    {
                        id = int.Parse(reader["ID"].ToString());
                        mac = reader["MAC"].ToString();
                    }
                    reader.Close();
                    sqlConnection.Close();
                }
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }

        public bool SetMACIsUsed(int ID, bool isUsed, string station = "", string serialNumber = "")
        {
            int result = 0;
            connectionString = $"Server={Server}; Database={DataBase}; User Id={UserID}; Password={Password}";
            try
            {
                using (sqlConnection = new SqlConnection())
                {
                    sqlConnection.ConnectionString = connectionString;
                    sqlConnection.Open();
                    if (station != "" && serialNumber != "")
                        sqlCommand = new SqlCommand($"UPDATE MAC_ADDRESSES SET IS_USED={Convert.ToInt16(isUsed)}, TIME_STAMP=GETDATE(), STATION=\'{station}\', SNR=\'{serialNumber}\' WHERE ID={ID};", sqlConnection);
                    else
                        sqlCommand = new SqlCommand($"UPDATE MAC_ADDRESSES SET IS_USED={Convert.ToInt16(isUsed)}, STATION=NULL WHERE ID={ID.ToString()};", sqlConnection);
                    sqlCommand.CommandType = CommandType.Text;
                    result = sqlCommand.ExecuteNonQuery();
                    sqlConnection.Close();
                    sqlCommand.Dispose();
                    if (result > 0)
                        return true;
                    return false;
                }
            }
            catch (InvalidOperationException e)
            {
                throw e;
            }
            catch (SqlException e)
            {
                throw e;
            }
        }
    }
}
