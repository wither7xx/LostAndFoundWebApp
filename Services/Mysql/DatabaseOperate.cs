using LostAndFoundWebApp.Models;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace LostAndFoundWebApp.Services.Mysql
{
    public class DatabaseOperate
    {
        // 数据库连接字符串
        private static string connectionString = "server=160.202.233.53;userid=root;pwd=123456;port=3306;database=system;sslmode=none;allowPublicKeyRetrieval=true;";

        // 测试连接是否有效
        public static bool TestConnection()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"连接失败: {ex.Message}");
                    return false;
                }
            }
        }

        // 创建失物信息
        public static int CreateItem(Item item)
        {
            const string sql = @"INSERT INTO Items 
                            (Name, Location, Campus, Time, Description, ContactInfo, Status, Category, UserID)
                            VALUES (@Name, @Location, @Campus, @Time, @Desc, @Contact, @Status, @Category, @UserID);
                            SELECT LAST_INSERT_ID();";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                // 参数化查询防止SQL注入
                cmd.Parameters.AddWithValue("@Name", item.Name);
                cmd.Parameters.AddWithValue("@Location", item.Location);
                cmd.Parameters.AddWithValue("@Campus", item.Campus);
                cmd.Parameters.AddWithValue("@Time", item.Time);
                cmd.Parameters.AddWithValue("@Desc", item.Description);
                cmd.Parameters.AddWithValue("@Contact", item.ContactInfo);
                cmd.Parameters.AddWithValue("@Status", item.Status);
                cmd.Parameters.AddWithValue("@Category", item.Category);
                cmd.Parameters.AddWithValue("@UserID", item.UserId);

                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()); // 返回新插入的ID
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"创建失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 获取物品列表
        public static List<Item> GetAllItems()
        {
            var items = new List<Item>();
            const string sql = "SELECT * FROM Items";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new Item
                            {
                                ItemId = reader["ItemID"] != DBNull.Value ? Convert.ToInt32(reader["ItemID"]) : 0,
                                Name = reader["Name"]?.ToString() ?? string.Empty,
                                Location = reader["Location"]?.ToString() ?? string.Empty,
                                Campus = reader["Campus"]?.ToString() ?? string.Empty,
                                Time = reader["Time"] != DBNull.Value ? Convert.ToDateTime(reader["Time"]) : DateTime.MinValue,
                                Description = reader["Description"]?.ToString() ?? string.Empty,
                                ContactInfo = reader["ContactInfo"]?.ToString() ?? string.Empty,
                                Status = reader["Status"]?.ToString() ?? ItemMetadata.Status.DefaultStatus,
                                Category = reader["Category"]?.ToString() ?? ItemMetadata.Category.DefaultCategory,
                                UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : -1,
                                IsValid = reader["IsValid"] != DBNull.Value ? Convert.ToBoolean(reader["IsValid"]) : false
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"获取物品列表失败: {ex.Message}");
                }
            }

            return items;
        }

        // 获取单个失物信息
        public static Item? GetItem(int ItemId)
        {
            const string sql = "SELECT * FROM Items WHERE ItemID = @ItemID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", ItemId);

                try
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Item
                            {
                                ItemId = reader["ItemID"] != DBNull.Value ? Convert.ToInt32(reader["ItemID"]) : 0,
                                Name = reader["Name"] != DBNull.Value ? (reader["Name"].ToString() ?? string.Empty) : string.Empty,
                                Location = reader["Location"] != DBNull.Value ? (reader["Location"].ToString() ?? string.Empty) : string.Empty,
                                Campus = reader["Campus"] != DBNull.Value ? (reader["Campus"].ToString() ?? string.Empty) : string.Empty,
                                Time = reader["Time"] != DBNull.Value ? Convert.ToDateTime(reader["Time"]) : DateTime.MinValue,
                                Description = reader["Description"] != DBNull.Value ? (reader["Description"].ToString() ?? string.Empty) : string.Empty,
                                ContactInfo = reader["ContactInfo"] != DBNull.Value ? (reader["ContactInfo"].ToString() ?? string.Empty) : string.Empty,
                                Status = reader["Status"] != DBNull.Value ? (reader["Status"].ToString() ?? ItemMetadata.Status.DefaultStatus) : ItemMetadata.Status.DefaultStatus,
                                Category = reader["Category"] != DBNull.Value ? (reader["Category"].ToString() ?? ItemMetadata.Category.DefaultCategory) : ItemMetadata.Category.DefaultCategory,
                                UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : -1,
                                IsValid = reader["IsValid"] != DBNull.Value ? Convert.ToBoolean(reader["IsValid"]) : false
                            };

                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"查询失败: {ex.Message}");
                    return null;
                }
            }
        }

        // 更新失物状态
        public static bool UpdateItem(int ItemId, string Status)
        {
            const string sql = @"UPDATE Items SET 
                            Status = @Status
                            WHERE ItemID = @ItemID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", ItemId);
                cmd.Parameters.AddWithValue("@Status", Status);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"更新失败: {ex.Message}");
                    return false;
                }
            }
        }

        // 删除失物数据
        public static bool DeleteItem(int ItemId)
        {
            const string sql = "DELETE FROM Items WHERE ItemID = @ItemID";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            using (MySqlCommand cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemID", ItemId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"删除失败: {ex.Message}");
                    return false;
                }
            }
        }
        // 创建图片记录
        public static int CreateImage(Image image)
        {
            const string sql = @"INSERT INTO Images 
                                (ImagePath, ItemId)
                                VALUES (@Path, @ItemId);
                                SELECT LAST_INSERT_ID();";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Path", image.ImagePath);
                cmd.Parameters.AddWithValue("@ItemId", image.ItemId);

                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()); // 返回新ImageId
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"图片创建失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 获取单个图片记录
        public static Image? GetImage(int imageId)
        {
            const string sql = "SELECT * FROM Images WHERE ImageId = @ImageId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ImageId", imageId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Image
                            {
                                ImageId = reader["ImageId"] != DBNull.Value ? Convert.ToInt32(reader["ImageId"]) : 0,
                                ImagePath = reader["ImagePath"] != DBNull.Value ? (reader["ImagePath"].ToString() ?? string.Empty) : string.Empty,
                                ItemId = reader["ItemId"] != DBNull.Value ? Convert.ToInt32(reader["ItemId"]) : 0
                            };
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"图片查询失败: {ex.Message}");
                    return null;
                }
            }
        }

        // 更新图片路径
        public static int UpdateImagePath(int imageId, string newPath)
        {
            const string sql = "UPDATE Images SET ImagePath = @NewPath WHERE ImageId = @ImageId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@NewPath", newPath);
                cmd.Parameters.AddWithValue("@ImageId", imageId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"图片更新失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 删除图片记录
        public static int DeleteImage(int imageId)
        {
            const string sql = "DELETE FROM Images WHERE ImageId = @ImageId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ImageId", imageId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"图片删除失败: {ex.Message}");
                    return -1;
                }
            }
        }
        // 获取指定物品的所有图片
        public static List<Image> GetImagesByItem(int ItemId)
        {
            var images = new List<Image>();
            const string sql = "SELECT * FROM Images WHERE ItemId = @ItemId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", ItemId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            images.Add(new Image
                            {
                                ImageId = Convert.ToInt32(reader["ImageId"]),
                                ImagePath = reader["ImagePath"].ToString() ?? string.Empty,
                                ItemId = Convert.ToInt32(reader["ItemId"])
                            });
                        }
                        return images;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"按物品查询图片失败: {ex.Message}");
                    return new List<Image>();
                }
            }
        }

        //创建认领记录
        public static int CreateClaim(Claim claim)
        {
            const string sql = @"INSERT INTO Claims 
                                (ClaimDescription, ProofDocPath, Status, CreateTime, ItemId, UserId)
                                VALUES (@Desc, @DocPath, @Status, @CreateTime, @ItemId, @UserId);
                                SELECT LAST_INSERT_ID();";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Desc", claim.ClaimDescription);
                cmd.Parameters.AddWithValue("@DocPath", claim.ProofDocPath);
                cmd.Parameters.AddWithValue("@Status", claim.Status);
                cmd.Parameters.AddWithValue("@CreateTime", claim.CreateTime);
                cmd.Parameters.AddWithValue("@ItemId", claim.ItemId);
                cmd.Parameters.AddWithValue("@UserId", claim.UserId);

                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()); // 返回新ClaimId
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"认领记录创建失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 删除认领记录
        public static int DeleteClaim(int claimId)
        {
            const string sql = "DELETE FROM Claims WHERE ClaimId = @ClaimId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ClaimId", claimId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"认领记录删除失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 更新认领状态
        public static int UpdateClaimStatus(int claimId, string newStatus)
        {
            const string sql = "UPDATE Claims SET Status = @Status WHERE ClaimId = @ClaimId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@ClaimId", claimId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"认领状态更新失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 获取单个认领记录
        public static Claim? GetClaim(int claimId)
        {
            const string sql = "SELECT * FROM Claims WHERE ClaimId = @ClaimId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ClaimId", claimId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Claim
                            {
                                ClaimId = reader["ClaimId"] != DBNull.Value ? Convert.ToInt32(reader["ClaimId"]) : 0,
                                ClaimDescription = reader["ClaimDescription"] != DBNull.Value ? (reader["ClaimDescription"].ToString() ?? string.Empty) : string.Empty,
                                ProofDocPath = reader["ProofDocPath"] != DBNull.Value ? (reader["ProofDocPath"].ToString() ?? string.Empty) : string.Empty,
                                Status = reader["Status"] != DBNull.Value ? (reader["Status"].ToString() ?? ClaimMetadata.Status.DefaultStatus) : ClaimMetadata.Status.DefaultStatus,
                                CreateTime = reader["CreateTime"] != DBNull.Value ? Convert.ToDateTime(reader["CreateTime"]) : DateTime.MinValue,
                                ItemId = reader["ItemId"] != DBNull.Value ? Convert.ToInt32(reader["ItemId"]) : 0,
                                UserId = reader["UserId"] != DBNull.Value ? Convert.ToInt32(reader["UserId"]) : 0
                            };
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"认领记录查询失败: {ex.Message}");
                    return null;
                }
            }
        }
        // 获取某物品的所有认领记录
        public static List<Claim> GetClaimsByItem(int itemId)
        {
            var claims = new List<Claim>();
            const string sql = "SELECT * FROM Claims WHERE ItemId = @ItemId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@ItemId", itemId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            claims.Add(new Claim
                            {
                                ClaimId = Convert.ToInt32(reader["ClaimId"]),
                                ClaimDescription = reader["ClaimDescription"].ToString() ?? string.Empty,
                                ProofDocPath = reader["ProofDocPath"].ToString() ?? string.Empty,
                                Status = reader["Status"].ToString() ?? ClaimMetadata.Status.DefaultStatus,
                                CreateTime = Convert.ToDateTime(reader["CreateTime"]),
                                ItemId = Convert.ToInt32(reader["ItemId"]),
                                UserId = Convert.ToInt32(reader["UserId"])
                            });
                        }
                        return claims;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"按物品查询认领记录失败: {ex.Message}");
                    return new List<Claim>();
                }
            }
        }
        //创建新用户
        public static int CreateUser(User user)
        {
            const string sql = @"INSERT INTO Users 
                                (Email, Password, Name, Role, IsValid)
                                VALUES (@Email, @Pwd, @Name, @Role, @Valid);
                                SELECT LAST_INSERT_ID();";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Pwd", user.Password);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Valid", user.IsValid ?? (object)DBNull.Value);

                try
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar()); // 返回新UserId
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"用户创建失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 删除用户
        public static int DeleteUser(int userId)
        {
            const string sql = "DELETE FROM Users WHERE UserId = @UserId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"用户删除失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 更新用户信息
        public static int UpdateUser(User user)
        {
            const string sql = @"UPDATE Users SET 
                               Email = @Email,
                               Password = @Pwd,
                               Name = @Name,
                               Role = @Role,
                               IsValid = @Valid
                               WHERE UserId = @UserId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Pwd", user.Password);
                cmd.Parameters.AddWithValue("@Name", user.Name);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@Valid", user.IsValid ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@UserId", user.UserId);

                try
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery(); // 返回受影响行数
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"用户更新失败: {ex.Message}");
                    return -1;
                }
            }
        }

        // 根据ID获取用户
        public static User? GetUserById(int userId)
        {
            const string sql = "SELECT * FROM Users WHERE UserId = @UserId";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Email = reader["Email"].ToString() ?? string.Empty,
                                Password = reader["Password"].ToString() ?? string.Empty,
                                Name = reader["Name"].ToString() ?? string.Empty,
                                Role = reader["Role"].ToString() ?? UserMetadata.Role.DefaultRole,
                                IsValid = reader["IsValid"] != DBNull.Value ? Convert.ToBoolean(reader["IsValid"]) : null
                            };
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"用户查询失败: {ex.Message}");
                    return null;
                }
            }
        }

        // 根据邮箱获取用户
        public static User? GetUserByEmail(string email)
        {
            const string sql = "SELECT * FROM Users WHERE Email = @Email";

            using (var conn = new MySqlConnection(connectionString))
            using (var cmd = new MySqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);

                try
                {
                    conn.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserId = Convert.ToInt32(reader["UserId"]),
                                Email = reader["Email"].ToString() ?? string.Empty,
                                Password = reader["Password"].ToString() ?? string.Empty,
                                Name = reader["Name"].ToString() ?? string.Empty,
                                Role = reader["Role"].ToString() ?? UserMetadata.Role.DefaultRole,
                                IsValid = reader["IsValid"] != DBNull.Value ? Convert.ToBoolean(reader["IsValid"]) : null
                            };
                        }
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"邮箱查询用户失败: {ex.Message}");
                    return null;
                }
            }
        }

    }

}
