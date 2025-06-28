using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DAL
{
    public class DAO
    {
        SqlConnection mCon = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString);
        SqlCommand mCommand;

        public DataSet ExecuteDataSet(string pCommandText)
        {
            try
            {
                SqlDataAdapter mDa = new SqlDataAdapter(pCommandText, mCon);
                DataSet dataSet = new DataSet();
                mDa.Fill(dataSet);
                return dataSet;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mCon.State != ConnectionState.Closed)
                {
                    mCon.Close();
                }
            }
        }

        public int ExecuteNonQuery(string pCommandText, List<SqlParameter> sqlParameters)
        {
            SeteoParametros(pCommandText, sqlParameters);
            try
            {
                mCon.Open();
                return mCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mCon.State != ConnectionState.Closed)
                {
                    mCon.Close();
                }
            }
        }

        public int ExecuteStoredProcedure(string spName)
        {
            int returnValue = -1; // Valor predeterminado en caso de que la SP no devuelva nada explícitamente

            try
            {
                mCommand = new SqlCommand(spName, mCon);
                mCommand.CommandType = CommandType.StoredProcedure;


                // Añadir un parámetro de retorno para capturar el RETURN de la SP
                SqlParameter returnParam = new SqlParameter();
                returnParam.Direction = ParameterDirection.ReturnValue;
                mCommand.Parameters.Add(returnParam);

                mCon.Open();
                mCommand.ExecuteNonQuery();
                // Recuperar el valor de retorno
                if (returnParam.Value != DBNull.Value && returnParam.Value != null)
                {
                    returnValue = Convert.ToInt32(returnParam.Value);
                }

                return returnValue;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (mCon.State != ConnectionState.Closed)
                {
                    mCon.Close();
                }
            }
        }

        // Metodo que setea los parametros de la query
        public void SeteoParametros(string pCommandText, List<SqlParameter> sqlParameters)
        {
            mCommand = new SqlCommand(pCommandText, mCon);
            if (sqlParameters != null && sqlParameters.Count > 0)
            {
                foreach (SqlParameter param in sqlParameters)
                {
                    mCommand.Parameters.AddWithValue(param.ParameterName, param.Value);
                }
            }
        }
    }
}