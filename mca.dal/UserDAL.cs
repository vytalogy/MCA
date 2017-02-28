using mca.model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Dapper;


namespace mca.dal
{
    public class UserDAL : BaseDAL
    {
        public List<mca.model.User> GetAll(Boolean? Active = null)
        {
            List<mca.model.User> _User = new List<User> { };
            try
            {               
                _User = this._db.Query<User>("UserGetAll", commandType: CommandType.StoredProcedure).ToList();
                return _User;
            }
            catch(Exception exe) { }

            return _User;
        }
                
        public mca.model.User Get(string userName, String password,out string erroMsg)
        {
            erroMsg = string.Empty;
            try
            {
                string query = @"UserGet";
                User _data = this._db.Query<User>(query, new
                {
                    userName = userName,
                    password = password
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                if (_data != null)
                {
                    _data.NoOfLogin = _data.NoOfLogin + 1;
                    query = "UPDATE [User] SET [LastLogin]=@LastLogin ,[NoOfLogin]=@NoOfLogin WHERE [id]=@ID";
                    this._db.Query(query, new
                    {
                        LastLogin = DateTime.Now,
                        NoOfLogin = _data.NoOfLogin,
                        ID = _data.id
                    }, commandType: CommandType.Text);
                }

                return _data;
            }
            catch (Exception exe)
            {
                erroMsg = exe.Message;
            }
            return null; 
        }

        public List<SelectList> GetRoles()
        {
            try
            {
                string query = @"select Value = id, Text = RoleName From Roles";
                return this._db.Query<SelectList>(query).ToList();                             
            }
            catch (Exception exe)
            {
            }
            return null;
        }
       
        public mca.model.User GetByUserId(int? UserId)
        {
            try
            {
                mca.model.User _user;
                string query = @"UserGetByID";
                _user = this._db.Query<mca.model.User>(query, new
                {
                    id = UserId,
                }, commandType: CommandType.StoredProcedure).FirstOrDefault();

                return _user;
            }
            catch (SqlException ex)
            {
            }

            return null;
        }
        public bool Create(mca.model.User _User,out string errorMsg)
        {
            errorMsg = string.Empty;                     
            try
            {
                string query = @"Insert into [User] (FirstName,LastName,UserName,Password,NoOfLogin,CreatedBy,CreatedOn,Active)
                               Values (@FirstName,@LastName,@UserName,@Password,@NoOfLogin,@CreatedBy,@CreatedOn,@Active) SELECT SCOPE_IDENTITY()";

                int userId = this._db.ExecuteScalar<int>(query, new
                {
                    FirstName = _User.FirstName,
                    LastName = _User.LastName,
                    UserName = _User.UserName,
                    Password = _User.Password,
                    NoOfLogin =0,
                    CreatedBy = _User.CreatedBy,
                    CreatedOn = DateTime.Now,
                    Active = true,
                }, commandType: CommandType.Text);

                query = @"Insert into [UserRoles] (UserId,RoleId) Values (@UserId,@RoleId) SELECT SCOPE_IDENTITY()";
                int value = this._db.ExecuteScalar<int>(query, new
                {
                    UserId = userId,
                    RoleId = _User.RoleID,
                   
                }, commandType: CommandType.Text);

            }
            catch (SqlException ex)
            {
                errorMsg = string.Empty;
                return false;
            }
           
            return true;
        }
        
        public bool Update(mca.model.User _User,out string errorMsg)
        {
            errorMsg = string.Empty;
            try
            {
                string query = @"Update [User] 
                                Set FirstName = @FirstName,
                                LastName = @LastName,
                                UserName = @UserName                               
                                where id = @userId";

                this._db.ExecuteScalar<int>(query, new
                {
                    FirstName = _User.FirstName,
                    LastName = _User.LastName,
                    UserName = _User.UserName,                 
                    UserId = _User.id,
                }, commandType: CommandType.Text);

                query = @"delete from [UserRoles] where UserId=@UserId";
                this._db.Query(query,new { UserId = _User.id, });

                query = @"Insert into [UserRoles] (UserId,RoleId) Values (@UserId,@RoleId) SELECT SCOPE_IDENTITY()";
                int value = this._db.ExecuteScalar<int>(query, new
                {
                    UserId = _User.id,
                    RoleId = _User.RoleID,

                }, commandType: CommandType.Text);

            }
            catch (SqlException ex)
            {
                errorMsg = string.Empty;
                return false;
            }

            return true;
        }

        public bool Delete(int? UserId)
        {           
            try
            {
                string query = @"delete from [UserRoles] where UserId=@UserId";
                this._db.Query(query, new { UserId = UserId, });

                query = @"delete from [User] where id = @userId";
                this._db.Query(query, new
                {                   
                    UserId = UserId,
                }, commandType: CommandType.Text);                
            }
            catch (SqlException ex)
            {               
                return false;
            }

            return true;
        }
    }
}
