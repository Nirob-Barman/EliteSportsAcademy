# Elite Sports Academy

Elite Sports Academy is a web-based application developed using ASP.NET Core MVC, designed to streamline the management of sports classes for instructors and students. The platform allows instructors to create and manage classes, while students can explore available classes, enroll, and manage their profiles.

## Features

### General

* User Authentication (Login, Registration, Roles)
* Role-based Authorization (Instructors, Students, Admins)
* Secure password management with ASP.NET Identity
* Image file uploads for classes and user profiles
* Data persistence using Entity Framework Core with SQL Server

### Instructor Module

* **Dashboard:** Overview of instructor activities
* **Class Management:** Add, edit, and manage classes
* **Profile Management:** Update profile details and profile picture

### Student Module

* **Dashboard:** Overview of selected and enrolled classes
* **Class Enrollment:** View, select, and pay for classes
* **Profile Management:** Update profile details and profile picture

## Project Structure

The project follows the ASP.NET Core MVC pattern with a clean architecture, separating concerns into Models, Views, and Controllers:

* **Models:** Define the data structure for Users, Instructors, Students, and Classes.
* **Views:** Razor pages for user interactions.
* **Controllers:** Manage the business logic and request handling for each module.

## Prerequisites

* .NET 6 SDK or later
* SQL Server (LocalDB or SQL Server Express recommended)
* Visual Studio 2022 or Visual Studio Code

## Getting Started

1. **Clone the Repository**

```bash
git clone https://github.com/nirob-barman/EliteSportsAcademy.git
cd EliteSportsAcademy
```

2. **Set Up Database**
   Update the connection string in `appsettings.json`:

```json
{
    "Logging": {
        "LogLevel": {
            "Default": "Information",
            "Microsoft.AspNetCore": "Warning"
        }
    },
    "ConnectionStrings": {
        "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=EliteSportsAcademy;Trusted_Connection=True;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
    },
    "AllowedHosts": "*"
}
```

3. **Apply Migrations**

```bash
cd EliteSportsAcademy
dotnet ef database update
```

4. **Run the Application**

```bash
dotnet run
```

5. **Access the Application**
   Visit `http://localhost:5000` in your browser.

<!--## Seed Default Roles and Users

To seed default roles and a sample admin user, use the following command:

```bash
dotnet run seed
```
-->
## License

This project is licensed under the MIT License. See the LICENSE file for more details.

## Contributions

Contributions are welcome! Please open an issue or submit a pull request.

## Contact

For any queries, reach out to the project owner at [nirob.barman@gmail.com](mailto:nirob.barman@gmail.com).
