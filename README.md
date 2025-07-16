# Registration MVC

A simple ASP.NET Core MVC project for user registration, profile management, and candidate search.

---

## Features

- User registration with profile picture upload
- User details management (Aadhar, PAN, etc.)
- List, update, and delete users
- Search candidates by name/email
- View user profile (partial and full)
- SQL Server database integration

---

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server (local or remote)
- Visual Studio or VS Code

---

## Setup

1. **Clone the repository**
2. **Restore NuGet packages**
3. **Configure your database connection string**  
   Edit `appsettings.json` and `appsettings.Development.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Your SQL Server connection string here"
   }
   ```
4. **Create the required stored procedures in your SQL Server database (see below).**

---

## Stored Procedures (SP) Used

### 1. `sp_InsertOrUpdateUser`
- **Purpose:** Insert a new user or update an existing user's basic info.
- **Parameters:**
  - `@UserID` (nullable, for insert use `NULL`)
  - `@FirstName`
  - `@LastName`
  - `@DOB`
  - `@Email`
  - `@ImagePath`
- **Returns:** The new or updated `UserID` (as a result set).

---

### 2. `sp_InsertOrUpdateUserLogin`
- **Purpose:** Insert or update the user's login credentials.
- **Parameters:**
  - `@UserID`
  - `@Email`
  - `@Password` (hashed)

---

### 3. `sp_InsertOrUpdateUserDetails`
- **Purpose:** Insert or update additional user details.
- **Parameters:**
  - `@UserID`
  - `@Aadhar_No`
  - `@Pan_No`
  - `@Gender`
  - `@Highest_Qualification`
  - `@Company_Name`
  - `@Dept`
  - `@Post`
  - `@Mode`

---

### 4. `sp_GetAllUsers`
- **Purpose:** Retrieve a list of all users with their basic info.
- **Returns:** List of users with fields:
  - `UserID`
  - `FirstName`
  - `LastName`
  - `DOB`
  - `Email`
  - `ImagePath`

---

### 5. `sp_DeleteUser`
- **Purpose:** Delete a user by their `UserID`.
- **Parameters:**
  - `@UserID`

---

### 6. `sp_UpdateUser`
- **Purpose:** Update a user's basic info (first name, last name, DOB, image path).
- **Parameters:**
  - `@UserID`
  - `@FirstName`
  - `@LastName`
  - `@DOB`
  - `@ImagePath`

---

### 7. `sp_SearchCandidates`
- **Purpose:** Search for users matching a search term (name or email).
- **Parameters:**
  - `@term` (string, e.g., `%search%`)
- **Returns:** List of users matching the term.

---

### 8. `sp_GetUserById`
- **Purpose:** Retrieve all information for a user by their `UserID`.
- **Parameters:**
  - `@UserID`
- **Returns:** All user fields, including basic info and details.

---

## Running the Project

1. Build and run the project in Visual Studio or using:
   ```
   dotnet run
   ```
2. Open your browser and navigate to the displayed URL (usually `https://localhost:5001/`).

---

## File Uploads

- Uploaded profile pictures are saved in `wwwroot/uploads/`.
- This folder is ignored in `.gitignore` and should be created automatically.

---

## Security

- Passwords are hashed using SHA256 before storing in the database.
