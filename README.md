# Registration MVC

A modern, full-featured ASP.NET Core MVC application for user registration, profile management, and candidate search, featuring a beautiful UI and robust image handling.

---

## ✨ Features
- **User Registration & Login**: Register new users with profile images and secure password storage.
- **Profile Management**: View, update, and manage user profiles with additional details and image uploads.
- **Candidate Search**: Fast, dynamic search with autocomplete and instant profile preview.
- **Image Handling**: Upload, update, and automatically delete profile images from the server when users are updated or removed.
- **Modern UI**: Clean, card-based design with responsive layouts and smooth user experience.
- **Separation of Concerns**: Follows MVC best practices for maintainability and scalability.

---

## 🗂️ Project Structure
```
Registration MVC/
├── Controllers/           # MVC controllers (business logic)
├── Models/                # Data models
├── Views/                 # Razor views (UI)
│   └── Registration/      # Registration and profile pages
│       └── Partials/      # Partial views for modular UI
├── wwwroot/uploads/       # Uploaded profile images
├── sql/                   # SQL scripts for database setup
├── appsettings.json       # Configuration
├── Program.cs             # App entry point
└── ...
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 6+ SDK](https://dotnet.microsoft.com/download)
- SQL Server (Express or higher)

### Installation
1. **Clone the repository**
   ```bash
   git clone <your-repo-url>
   cd Registration\ MVC
   ```
2. **Set up the database**
   - Create a new SQL Server database.
   - Run all scripts in the `sql/` folder to create tables and procedures.
3. **Configure the application**
   - Update your connection string in `appsettings.json` to point to your database.
4. **Run the application**
   ```bash
   dotnet run
   ```
   The app will be available at `https://localhost:5001` (or as shown in your terminal).

---

## 🖼️ Image Management
- Profile images are stored in `wwwroot/uploads`.
- When a user is deleted or their image is updated, the old image file is automatically removed from the server.
- Default images are preserved and never deleted.

---

## 💡 Customization & Contribution
- The UI is built with Bootstrap and custom CSS for easy theming.
- All business logic is in the `Controllers` folder; UI logic in `Views`.
- Contributions are welcome! Fork the repo, create a feature branch, and submit a pull request.

---

## 📄 License
This project is open-source and available under the [MIT License](LICENSE).

---

## 🙋‍♂️ Need Help?
If you have questions or need support, feel free to open an issue or contact the maintainer. 