using mca.model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace mca.dal
{
    public class UserDAL : BaseDAL
    {
        public UserDAL(String conn) : base(conn)
        {

        }

        public UserDAL() { }

        public List<mca.model.User> GetAll(Boolean? Active = null)
        {
            List<mca.model.User> _User = null;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserGetAll", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterActive = new SqlParameter("@Active", SqlDbType.Bit);
                _SqlParameterActive.Value = Active;
                cmd.Parameters.Add(_SqlParameterActive);

                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                _SqlDataAdapter.Fill(ds);
                _User = mca.utility.Utilities.ConvertToList<mca.model.User>(ds.Tables[0]);
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
             //   DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return _User == null ? new List<mca.model.User>() : _User;
        }
        public List<mca.model.User> GetAll(Roles _Role, Boolean? Active = null)
        {
            List<mca.model.User> _User = null;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserGetAllByRoles", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterRole = new SqlParameter("@Role", SqlDbType.VarChar);
                _SqlParameterRole.Value = _Role;
                cmd.Parameters.Add(_SqlParameterRole);


                SqlParameter _SqlParameterActive = new SqlParameter("@Active", SqlDbType.Bit);
                if (Active == null)
                    _SqlParameterActive.Value = System.DBNull.Value;
                else
                    _SqlParameterActive.Value = Active;
                cmd.Parameters.Add(_SqlParameterActive);

                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                _SqlDataAdapter.Fill(ds);
                _User = mca.utility.Utilities.ConvertToList<mca.model.User>(ds.Tables[0]);
                conn.Close();
            }
            catch (SqlException ex)
            {
                //mca.utility.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return _User == null ? new List<mca.model.User> () : _User;
        }
        public mca.model.User Get(String UID, String PWD)
        {
            mca.model.User _User = null;
            SqlConnection conn = null;            
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserGet", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterUserName = new SqlParameter("@UserName", SqlDbType.VarChar, 500);
                _SqlParameterUserName.Value = UID;
                cmd.Parameters.Add(_SqlParameterUserName);

                SqlParameter _SqlParameterPassword = new SqlParameter("@Password", SqlDbType.VarChar, 500);
                _SqlParameterPassword.Value = PWD;
                cmd.Parameters.Add(_SqlParameterPassword);               

                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                _SqlDataAdapter.Fill(ds);
                _User = mca.utility.Utilities.ConvertToEntity<mca.model.User>(ds.Tables[0]);
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return _User;
        }
        public mca.model.User Get(String UID)
        {
            mca.model.User _User = null;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserGetByUID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterUserName = new SqlParameter("@UserName", SqlDbType.VarChar);
                _SqlParameterUserName.Value = UID;
                cmd.Parameters.Add(_SqlParameterUserName);

                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                _SqlDataAdapter.Fill(ds);
                _User = mca.utility.Utilities.ConvertToEntity<mca.model.User>(ds.Tables[0]);
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
             //   DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return _User;
        }
        public mca.model.User Get(int UserId)
        {
            mca.model.User _User = null;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserGetByID", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterID = new SqlParameter("@id",SqlDbType.Int);
                _SqlParameterID.Value = UserId;
                cmd.Parameters.Add(_SqlParameterID);

                DataSet ds = new DataSet();
                SqlDataAdapter _SqlDataAdapter = new SqlDataAdapter(cmd);
                conn.Open();
                _SqlDataAdapter.Fill(ds);
                _User = mca.utility.Utilities.ConvertToEntity<mca.model.User>(ds.Tables[0]);
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return _User;
        }
        public int Add(mca.model.User _User)
        {
            int returnVal = -1;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserAdd", conn);
                cmd.CommandType =CommandType.StoredProcedure;

                SqlParameter _SqlParameterUserName = new SqlParameter("@UserName", SqlDbType.VarChar);
                _SqlParameterUserName.Value = _User.UserName;
                cmd.Parameters.Add(_SqlParameterUserName);

                //SqlParameter _SqlParameterEmail = new SqlParameter("@Email",SqlDbType.VarChar);
                //_SqlParameterEmail.Value = _User.Email;
                //cmd.Parameters.Add(_SqlParameterEmail);

                SqlParameter _SqlParameterPassword = new SqlParameter("@Password", SqlDbType.VarChar);
                _SqlParameterPassword.Value = _User.Password;
                cmd.Parameters.Add(_SqlParameterPassword);                

                //SqlParameter _SqlParameterCreatedBy = new SqlParameter("@CreatedBy",SqlDbType.Int);
                //_SqlParameterCreatedBy.Value = _User.CreatedBy;
                //cmd.Parameters.Add(_SqlParameterCreatedBy);

                SqlParameter _SqlParameterActive = new SqlParameter("@Active", SqlDbType.Bit);
                _SqlParameterActive.Value = _User.Active;
                cmd.Parameters.Add(_SqlParameterActive);

                conn.Open();
                returnVal = Convert.ToInt16(cmd.ExecuteScalar());
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return returnVal;
        }
        public int ResetPassword(int id, String newPassword)
        {
            int returnVal = -1;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("ResetPassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterID = new SqlParameter("@id", SqlDbType.Int);
                _SqlParameterID.Value = id;
                cmd.Parameters.Add(_SqlParameterID);

                SqlParameter _SqlParameterOldPassword = new SqlParameter("@OldPassword", SqlDbType.VarChar);
                _SqlParameterOldPassword.Value = DBNull.Value;
                cmd.Parameters.Add(_SqlParameterOldPassword);

                SqlParameter _SqlParameternewPassword = new SqlParameter("@newPassword", SqlDbType.VarChar);
                _SqlParameternewPassword.Value = newPassword;
                cmd.Parameters.Add(_SqlParameternewPassword);


                conn.Open();
                returnVal = Convert.ToInt16(cmd.ExecuteScalar());
                conn.Close();
            }
            catch (SqlException ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return returnVal;
        }
        public int ResetPassword(int id, String oldPassword, String newPassword)
        {
            int returnVal = -1;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("ResetPassword", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterID = new SqlParameter("@id", SqlDbType.Int);
                _SqlParameterID.Value = id;
                cmd.Parameters.Add(_SqlParameterID);

                SqlParameter _SqlParameterOldPassword = new SqlParameter("@OldPassword", SqlDbType.VarChar);
                _SqlParameterOldPassword.Value = oldPassword;
                cmd.Parameters.Add(_SqlParameterOldPassword);

                SqlParameter _SqlParameternewPassword = new SqlParameter("@newPassword", SqlDbType.VarChar);
                _SqlParameternewPassword.Value = newPassword;
                cmd.Parameters.Add(_SqlParameternewPassword);


                conn.Open();
                returnVal = Convert.ToInt16(cmd.ExecuteScalar());
                conn.Close();
            }
            catch (SqlException ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
               // DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return returnVal;
        }
        public int Update(mca.model.User _User)
        {
            int returnVal = -1;
            SqlConnection conn = null;
            try
            {
                conn = new System.Data.SqlClient.SqlConnection(this.conn);
                SqlCommand cmd = new SqlCommand("UserUpdate", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter _SqlParameterID = new SqlParameter("@id", SqlDbType.Int);
                _SqlParameterID.Value = _User.id;
                cmd.Parameters.Add(_SqlParameterID);

                //SqlParameter _SqlParameterUserName = new SqlParameter("@UserName", System.Data.SqlDbType.VarChar);
                //_SqlParameterUserName.Value = _User.UserName;
                //cmd.Parameters.Add(_SqlParameterUserName);

                //SqlParameter _SqlParameterEmail = new SqlParameter("@Email", System.Data.SqlDbType.VarChar);
                //_SqlParameterEmail.Value = _User.Email;
                //cmd.Parameters.Add(_SqlParameterEmail);

                SqlParameter _SqlParameterPassword = new SqlParameter("@Password", SqlDbType.VarChar);
                _SqlParameterPassword.Value = _User.Password;
                cmd.Parameters.Add(_SqlParameterPassword);

                //SqlParameter _SqlParameterRoles = new SqlParameter("@Roles", System.Data.SqlDbType.VarChar);
                //_SqlParameterRoles.Value = _User.Roles;
                //cmd.Parameters.Add(_SqlParameterRoles);

                //SqlParameter _SqlParameterSecretQuestion = new SqlParameter("@SecretQuestion", SqlDbType.VarChar);
                //_SqlParameterSecretQuestion.Value = _User.SecretQuestion;
                //cmd.Parameters.Add(_SqlParameterSecretQuestion);

                //SqlParameter _SqlParameterSecretAnswer = new SqlParameter("@SecretAnswer", SqlDbType.VarChar);
                //_SqlParameterSecretAnswer.Value = _User.SecretAnswer;
                //cmd.Parameters.Add(_SqlParameterSecretAnswer);

                SqlParameter _SqlParameterActive = new SqlParameter("@Active", SqlDbType.Bit);
                _SqlParameterActive.Value = _User.Active;
                cmd.Parameters.Add(_SqlParameterActive);

                conn.Open();
                returnVal = Convert.ToInt16(cmd.ExecuteScalar());
                conn.Close();
            }
            catch (SqlException ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            catch (Exception ex)
            {
                //DAL.Utilities.SaveException(ex);
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                conn = null;
            }
            return returnVal;
        }     
    }
}
