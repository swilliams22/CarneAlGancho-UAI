using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class UsuarioDAL
    {
        public static Usuario GetByEmail(string usuario)
        {
            string mCommandText = $"SELECT * FROM Usuario WHERE Usuario = '{usuario}'";
            DAO dao = new DAO();

            DataSet dataSet = dao.ExecuteDataSet(mCommandText);

            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
            {
                Usuario pUsuario = new Usuario();
                ValorizarEntidad(pUsuario, dataSet.Tables[0].Rows[0]);
                return pUsuario;
            }
            else
            {
                return null;
            }
        }

        public static int UpdateRetries(Usuario usuario)
        {
            string mCommandText = $"UPDATE Usuario SET Retries = @Retries WHERE Id = @Id";
            var ps = new List<SqlParameter>
            {
                new SqlParameter("@Retries", usuario.Retries),
                new SqlParameter("@Id", usuario.Id)
            };
            return new DAO().ExecuteNonQuery(mCommandText, ps);
        }

        public static void BlockUser(Usuario usuario)
        {
            string mCommandText = $"UPDATE Usuario SET Active = @Active WHERE Id = @Id";
            var ps = new List<SqlParameter>
            {
                new SqlParameter("@Active", usuario.Active),
                new SqlParameter("@Id", usuario.Id)
            };
            new DAO().ExecuteNonQuery(mCommandText, ps);
        }
        public static int UnblockUser(string usuario)
        {
            string mCommandText = $"UPDATE Usuario SET Active = @Active, Retries = @Retries WHERE Usuario = @Usuario";
            var ps = new List<SqlParameter>
            {
                new SqlParameter("@Active",   SqlDbType.Bit)      { Value = true },
                new SqlParameter("@Retries",  SqlDbType.Int)      { Value = 0    },
                new SqlParameter("@Usuario",    SqlDbType.NVarChar, 256) { Value = usuario }
            };
            return new DAO().ExecuteNonQuery(mCommandText, ps);
        }

        public static bool CheckUsersDVH()
        {
            string spName = "SP_CalculateAndVerifyUsersDVV";

            // Llama al método genérico de la DAO.
            int spReturnValue = new DAO().ExecuteStoredProcedure(spName);

            return spReturnValue != 0;
        }

        public static void CalculateDVV()
        {
            string spName = "SP_RecordDVH";

            // Llama al método genérico de la DAO.
            int spReturnValue = new DAO().ExecuteStoredProcedure(spName);
        }


        internal static void ValorizarEntidad(Usuario pUsuario, DataRow pDataRow)
        {
            pUsuario.Id = int.Parse(pDataRow["Id"].ToString());
            pUsuario.Email = pDataRow["Usuario"].ToString();
            pUsuario.PasswordHash = pDataRow["Password"].ToString();
            pUsuario.Retries = int.Parse(pDataRow["Retries"].ToString());
            pUsuario.Active = bool.Parse(pDataRow["Active"].ToString());
            pUsuario.Salt = pDataRow["Salt"].ToString();
        }
    }
}
