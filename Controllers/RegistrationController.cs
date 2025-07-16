using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Registration_MVC.Models;

namespace Registration_MVC.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IConfiguration _config;
        public RegistrationController(IConfiguration config) { _config = config; }

        public IActionResult Index() => View();
        public IActionResult List() => View();
        public IActionResult Profile() => View();

        [HttpPost]
        public IActionResult Save(UserModel data)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest("Invalid data.");

                string imagePath = "";
                if (data.ProfilePic != null)
                {
                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    string fileName = Guid.NewGuid() + Path.GetExtension(data.ProfilePic.FileName);
                    string filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        data.ProfilePic.CopyTo(stream);
                    }
                    imagePath = "/uploads/" + fileName;
                }

                int newUserId = 0;
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_InsertOrUpdateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", DBNull.Value); // Insert
                    cmd.Parameters.AddWithValue("@FirstName", data.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", data.LastName);
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(data.DOB));
                    cmd.Parameters.AddWithValue("@Email", data.Email);
                    cmd.Parameters.AddWithValue("@ImagePath", imagePath ?? "");
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            newUserId = Convert.ToInt32(reader[0]);
                        }
                    }
                }
                // Save login info (email and password, hash password)
                string hashedPassword = HashPassword(data.Password);
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_InsertOrUpdateUserLogin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", newUserId); // Pass the new UserID
                    cmd.Parameters.AddWithValue("@Email", data.Email);
                    cmd.Parameters.AddWithValue("@Password", hashedPassword);
                    cmd.ExecuteNonQuery();
                }
                return Ok(new { UserID = newUserId });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult SaveUserDetails(UserDetailsModel model)
        {
            try
            {
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_InsertOrUpdateUserDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", model.UserID);
                    cmd.Parameters.AddWithValue("@Aadhar_No", model.Aadhar_No ?? "");
                    cmd.Parameters.AddWithValue("@Pan_No", model.Pan_No ?? "");
                    cmd.Parameters.AddWithValue("@Gender", model.Gender ?? "");
                    cmd.Parameters.AddWithValue("@Highest_Qualification", model.Highest_Qualification ?? "");
                    cmd.Parameters.AddWithValue("@Company_Name", model.Company_Name ?? "");
                    cmd.Parameters.AddWithValue("@Dept", model.Dept ?? "");
                    cmd.Parameters.AddWithValue("@Post", model.Post ?? "");
                    cmd.Parameters.AddWithValue("@Mode", model.Mode ?? "");
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true, message = "User details saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error saving details: " + ex.Message });
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<UserModel> list = new();
            using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new("sp_GetAllUsers", con);
                cmd.CommandType = CommandType.StoredProcedure;
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new UserModel
                    {
                        UserID = Convert.ToInt32(dr["UserID"]),
                        FirstName = dr["FirstName"].ToString() ?? "",
                        LastName = dr["LastName"].ToString() ?? "",
                        DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd") : "",
                        Email = dr["Email"].ToString() ?? "",
                        ImagePath = dr["ImagePath"]?.ToString() ?? ""
                    });
                }
            }
            return Json(list);
        }

        [HttpPost]
        public IActionResult Delete(int UserID)
        {
            try
            {
                // Get the image path for the user
                string? imagePath = null;
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_GetUserById", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    using SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        imagePath = dr["ImagePath"]?.ToString();
                    }
                }
                // Delete the image file if it exists and is not empty
                if (!string.IsNullOrEmpty(imagePath))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                // Delete the user from the database
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_DeleteUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.ExecuteNonQuery();
                }
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error deleting user: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult Update(int UserID, string FirstName, string LastName, string DOB, string ImagePath, IFormFile? ProfilePic)
        {
            try
            {
                Console.WriteLine($"Update called with UserID: {UserID}");
                Console.WriteLine($"FirstName: {FirstName}");
                Console.WriteLine($"LastName: {LastName}");
                Console.WriteLine($"DOB: {DOB}");
                Console.WriteLine($"ImagePath: {ImagePath}");
                Console.WriteLine($"ProfilePic: {(ProfilePic == null ? "Null" : "Provided")}");

                string newImagePath = ImagePath;

                // Handle new image upload if provided
                if (ProfilePic != null)
                {
                    // Delete old image if it exists and is not default
                    if (!string.IsNullOrEmpty(ImagePath) && !ImagePath.EndsWith("default.png"))
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", ImagePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }

                    string uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);

                    string fileName = Guid.NewGuid() + Path.GetExtension(ProfilePic.FileName);
                    string filePath = Path.Combine(uploads, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        ProfilePic.CopyTo(stream);
                    }
                    newImagePath = "/uploads/" + fileName;
                    Console.WriteLine($"New image saved to: {newImagePath}");
                }

                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_UpdateUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", UserID);
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@DOB", DateTime.Parse(DOB));
                    cmd.Parameters.AddWithValue("@ImagePath", newImagePath);
                    
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"Update operation completed. Rows affected: {rowsAffected}");
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return BadRequest($"Error updating user: {ex.Message}");
            }
        }

        [HttpGet]
        public IActionResult SearchCandidates(string term)
        {
            List<object> results = new();
            using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new("sp_SearchCandidates", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@term", "%" + term + "%");
                using SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    results.Add(new
                    {
                        userID = dr["UserID"],
                        firstName = dr["FirstName"].ToString(),
                        lastName = dr["LastName"].ToString(),
                        email = dr["Email"].ToString(),
                        label = dr["FirstName"] + " " + dr["LastName"], // Only name and surname
                        imagePath = dr["ImagePath"]?.ToString() ?? ""
                    });
                }
            }
            return Json(results);
        }

        [HttpGet]
        public IActionResult GetUserById(int id)
        {
            object user = null;
            Console.WriteLine($"GetUserById called with id: {id}");

            using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                Console.WriteLine("Database connection opened");

                SqlCommand cmd = new("sp_GetUserById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", id);
                Console.WriteLine($"Executing stored procedure with UserID: {id}");

                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    var firstName = dr["FirstName"]?.ToString();
                    var lastName = dr["LastName"]?.ToString();
                    var email = dr["Email"]?.ToString();
                    var dob = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd") : "";
                    var imagePath = dr["ImagePath"]?.ToString();

                    // Additional details
                    var aadharNo = dr["Aadhar_No"]?.ToString();
                    var panNo = dr["Pan_No"]?.ToString();
                    var gender = dr["Gender"]?.ToString();
                    var highestQualification = dr["Highest_Qualification"]?.ToString();
                    var companyName = dr["Company_Name"]?.ToString();
                    var dept = dr["Dept"]?.ToString();
                    var post = dr["Post"]?.ToString();
                    var mode = dr["Mode"]?.ToString();

                    user = new
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Email = email,
                        DOB = dob,
                        ImagePath = imagePath,
                        Aadhar_No = aadharNo,
                        Pan_No = panNo,
                        Gender = gender,
                        Highest_Qualification = highestQualification,
                        Company_Name = companyName,
                        Dept = dept,
                        Post = post,
                        Mode = mode
                    };
                }
                else
                {
                    Console.WriteLine("No user found with the given ID");
                    return Json(new { success = false, message = "User not found" });
                }
            }

            Console.WriteLine("Returning user data");
            return Json(new { success = true, user });
        }

        [HttpGet]
        public IActionResult GetProfilePartial(int id)
        {
            UserModel registration = null;
            UserDetailsModel details = null;
            using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
            {
                con.Open();
                SqlCommand cmd = new("sp_GetUserById", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@UserID", id);
                using SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    int dbUserId = dr["UserID"] != DBNull.Value ? Convert.ToInt32(dr["UserID"]) : id;
                    registration = new UserModel
                    {
                        UserID = dbUserId,
                        FirstName = dr["FirstName"]?.ToString() ?? string.Empty,
                        LastName = dr["LastName"]?.ToString() ?? string.Empty,
                        DOB = dr["DOB"] != DBNull.Value ? Convert.ToDateTime(dr["DOB"]).ToString("yyyy-MM-dd") : string.Empty,
                        Email = dr["Email"]?.ToString() ?? string.Empty,
                        ImagePath = dr["ImagePath"]?.ToString() ?? string.Empty
                    };
                    details = new UserDetailsModel
                    {
                        UserID = dbUserId,
                        Aadhar_No = dr["Aadhar_No"]?.ToString(),
                        Pan_No = dr["Pan_No"]?.ToString(),
                        Gender = dr["Gender"]?.ToString(),
                        Highest_Qualification = dr["Highest_Qualification"]?.ToString(),
                        Company_Name = dr["Company_Name"]?.ToString(),
                        Dept = dr["Dept"]?.ToString(),
                        Post = dr["Post"]?.ToString(),
                        Mode = dr["Mode"]?.ToString()
                    };
                }
            }
            if (registration == null)
                return Content($"No user found with ID {id}");
            var model = new Registration_MVC.Models.ProfileFullDetails { Registration = registration, Details = details };
            return PartialView("Partials/_ProfileDetails", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateProfileFullDetails(Models.ProfileFullDetails model)
        {
            try
            {
                Console.WriteLine("UpdateProfileFullDetails: UserID=" + model.Registration.UserID);
                if (model.Registration.UserID == 0)
                {
                    Console.WriteLine("ERROR: Attempted update with UserID=0");
                    return BadRequest("Invalid UserID for update. Data not saved.");
                }

                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    SqlCommand cmd = new("sp_InsertOrUpdateUserDetails", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", model.Details.UserID);
                    cmd.Parameters.AddWithValue("@Aadhar_No", model.Details.Aadhar_No ?? "");
                    cmd.Parameters.AddWithValue("@Pan_No", model.Details.Pan_No ?? "");
                    cmd.Parameters.AddWithValue("@Gender", model.Details.Gender ?? "");
                    cmd.Parameters.AddWithValue("@Highest_Qualification", model.Details.Highest_Qualification ?? "");
                    cmd.Parameters.AddWithValue("@Company_Name", model.Details.Company_Name ?? "");
                    cmd.Parameters.AddWithValue("@Dept", model.Details.Dept ?? "");
                    cmd.Parameters.AddWithValue("@Post", model.Details.Post ?? "");
                    cmd.Parameters.AddWithValue("@Mode", model.Details.Mode ?? "");
                    cmd.ExecuteNonQuery();
                }
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        public IActionResult TestConnection()
        {
            try
            {
                using (SqlConnection con = new(_config.GetConnectionString("DefaultConnection")))
                {
                    con.Open();
                    Console.WriteLine("Test connection successful");
                    return Json(new { success = true, message = "Database connection successful" });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Test connection failed: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        private static string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
