using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using MedicalStoreSystem.Models;

namespace MedicalStoreSystem.DAL
{
    public class UserDAL
    {
        // تسجيل الدخول
        public User Login(string username, string password)
        {
            string query = @"SELECT UserID, Username, Password, FullName, Role, Phone, Email, IsActive, LastLogin, CreatedDate 
                           FROM Users 
                           WHERE Username = @Username AND Password = @Password AND IsActive = 1";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", username),
                new SqlParameter("@Password", password)
            };

            DataTable dt = DatabaseConnection.ExecuteDataTable(query, parameters);

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                User user = new User
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    FullName = row["FullName"].ToString(),
                    Role = row["Role"].ToString(),
                    Phone = row["Phone"].ToString(),
                    Email = row["Email"].ToString(),
                    IsActive = Convert.ToBoolean(row["IsActive"]),
                    LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null,
                    CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                };

                // تحديث آخر دخول
                UpdateLastLogin(user.UserID);

                return user;
            }

            return null;
        }

        // تحديث آخر دخول
        private void UpdateLastLogin(int userID)
        {
            string query = @"UPDATE Users SET LastLogin = @LastLogin WHERE UserID = @UserID";

            SqlParameter[] parameters = {
                new SqlParameter("@UserID", userID),
                new SqlParameter("@LastLogin", DateTime.Now)
            };

            DatabaseConnection.ExecuteNonQuery(query, parameters);
        }

        // جلب جميع المستخدمين
        public List<User> GetAllUsers()
        {
            List<User> users = new List<User>();

            string query = @"SELECT UserID, Username, Password, FullName, Role, Phone, Email, IsActive, LastLogin, CreatedDate 
                           FROM Users 
                           ORDER BY FullName";

            DataTable dt = DatabaseConnection.ExecuteDataTable(query);

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    users.Add(new User
                    {
                        UserID = Convert.ToInt32(row["UserID"]),
                        Username = row["Username"].ToString(),
                        Password = row["Password"].ToString(),
                        FullName = row["FullName"].ToString(),
                        Role = row["Role"].ToString(),
                        Phone = row["Phone"].ToString(),
                        Email = row["Email"].ToString(),
                        IsActive = Convert.ToBoolean(row["IsActive"]),
                        LastLogin = row["LastLogin"] != DBNull.Value ? Convert.ToDateTime(row["LastLogin"]) : (DateTime?)null,
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    });
                }
            }

            return users;
        }

        // إضافة مستخدم جديد
        public bool InsertUser(User user)
        {
            // التحقق من عدم تكرار اسم المستخدم
            if (IsUsernameExists(user.Username))
                return false;

            string query = @"INSERT INTO Users (Username, Password, FullName, Role, Phone, Email, IsActive, CreatedDate) 
                           VALUES (@Username, @Password, @FullName, @Role, @Phone, @Email, @IsActive, @CreatedDate)";

            SqlParameter[] parameters = {
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@FullName", user.FullName),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@Phone", user.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Email", user.Email ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", user.IsActive),
                new SqlParameter("@CreatedDate", user.CreatedDate)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // تعديل مستخدم
        public bool UpdateUser(User user)
        {
            string query = @"UPDATE Users 
                           SET Username = @Username,
                               Password = @Password,
                               FullName = @FullName,
                               Role = @Role,
                               Phone = @Phone,
                               Email = @Email,
                               IsActive = @IsActive
                           WHERE UserID = @UserID";

            SqlParameter[] parameters = {
                new SqlParameter("@UserID", user.UserID),
                new SqlParameter("@Username", user.Username),
                new SqlParameter("@Password", user.Password),
                new SqlParameter("@FullName", user.FullName),
                new SqlParameter("@Role", user.Role),
                new SqlParameter("@Phone", user.Phone ?? (object)DBNull.Value),
                new SqlParameter("@Email", user.Email ?? (object)DBNull.Value),
                new SqlParameter("@IsActive", user.IsActive)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // حذف مستخدم (تعطيل)
        public bool DeleteUser(int userID)
        {
            string query = @"UPDATE Users SET IsActive = 0 WHERE UserID = @UserID";

            SqlParameter[] parameters = {
                new SqlParameter("@UserID", userID)
            };

            int result = DatabaseConnection.ExecuteNonQuery(query, parameters);
            return result > 0;
        }

        // التحقق من وجود اسم المستخدم
        public bool IsUsernameExists(string username, int? excludeUserID = null)
        {
            string query = @"SELECT COUNT(*) FROM Users WHERE Username = @Username";

            if (excludeUserID.HasValue)
                query += " AND UserID != @UserID";

            SqlParameter[] parameters = excludeUserID.HasValue ?
                new SqlParameter[] {
                    new SqlParameter("@Username", username),
                    new SqlParameter("@UserID", excludeUserID.Value)
                } :
                new SqlParameter[] {
                    new SqlParameter("@Username", username)
                };

            object result = DatabaseConnection.ExecuteScalar(query, parameters);
            return result != null && Convert.ToInt32(result) > 0;
        }

        // جلب DataTable للعرض
        public DataTable GetUsersDataTable()
        {
            string query = @"SELECT 
                               UserID AS 'الرقم',
                               Username AS 'اسم المستخدم',
                               FullName AS 'الاسم الكامل',
                               Role AS 'الصلاحية',
                               Phone AS 'التليفون',
                               CASE WHEN IsActive = 1 THEN 'نشط' ELSE 'غير نشط' END AS 'الحالة',
                               LastLogin AS 'آخر دخول'
                           FROM Users 
                           ORDER BY FullName";

            return DatabaseConnection.ExecuteDataTable(query);
        }
    }
}
