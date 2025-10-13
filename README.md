# CET Exam Application

A comprehensive ASP.NET Core 8.0 MVC web application for managing online examinations with multitenancy support, role-based authentication, and modern Bootstrap 5 UI.

## Features

- **ASP.NET Core 8.0 MVC** - Modern, cross-platform web framework
- **Entity Framework Core** - Code-first ORM for data access
- **SQL Server Database** - Robust relational database
- **ASP.NET Core Identity** - Secure authentication and authorization
- **Role-Based Access Control** - Admin and Student roles with different dashboards
- **Multitenancy Support** - Configurable exam center name, logo, and branding
- **Bootstrap 5 UI** - Responsive, modern user interface
- **Bootstrap Icons** - Beautiful icon library integration

## Project Structure

```
CETExamApp/
├── Controllers/           # MVC Controllers
│   ├── HomeController.cs
│   ├── AccountController.cs
│   ├── AdminController.cs
│   └── StudentController.cs
├── Data/                  # Database context and initializer
│   ├── ApplicationDbContext.cs
│   └── DbInitializer.cs
├── Models/               # Data models and view models
│   ├── ApplicationUser.cs
│   ├── TenantSettings.cs
│   ├── ErrorViewModel.cs
│   └── ViewModels/
├── Services/             # Application services
│   ├── ITenantService.cs
│   └── TenantService.cs
├── Views/                # Razor views
│   ├── Account/
│   ├── Admin/
│   ├── Home/
│   ├── Student/
│   └── Shared/
├── wwwroot/              # Static files
│   ├── css/
│   ├── js/
│   └── images/
├── appsettings.json      # Configuration
└── Program.cs            # Application entry point
```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or Full version)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) (optional)

## Getting Started

### 1. Clone or Download the Repository

```bash
cd E:\ASP.Net Core\CETExamApp
```

### 2. Configure Database Connection

Update the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

**Connection String Options:**

- **SQL Server LocalDB** (Default):
  ```
  Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true
  ```

- **SQL Server Express**:
  ```
  Server=.\\SQLEXPRESS;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true
  ```

- **SQL Server with credentials**:
  ```
  Server=YOUR_SERVER;Database=CETExamAppDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true
  ```

### 3. Configure Tenant Settings

Customize your exam center branding in `appsettings.json`:

```json
{
  "TenantSettings": {
    "ExamCenterName": "Central Exam Testing Center",
    "LogoPath": "/images/logo.png",
    "PrimaryColor": "#007bff",
    "SecondaryColor": "#6c757d"
  }
}
```

**Customization Options:**
- `ExamCenterName`: Your organization/center name
- `LogoPath`: Path to your logo image (place in `wwwroot/images/`)
- `PrimaryColor`: Primary brand color (hex code)
- `SecondaryColor`: Secondary brand color (hex code)

### 4. Install Dependencies

```bash
dotnet restore
```

### 5. Create Database and Apply Migrations

The application automatically creates the database and applies migrations on first run. Alternatively, you can manually create migrations:

```bash
# Install EF Core tools (if not already installed)
dotnet tool install --global dotnet-ef

# Create initial migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

Or press **F5** in Visual Studio.

The application will be available at:
- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`

## Default User Accounts

The application seeds two default users:

### Admin Account
- **Email**: `admin@cetexam.com`
- **Password**: `Admin@123`
- **Role**: Admin
- **Access**: Admin Dashboard, User Management

### Student Account
- **Email**: `student@cetexam.com`
- **Password**: `Student@123`
- **Role**: Student
- **Access**: Student Dashboard, Profile

## User Roles and Permissions

### Admin Role
- Access to Admin Dashboard
- View all users and their roles
- Manage system settings
- View statistics

### Student Role
- Access to Student Dashboard
- View personal profile
- Take exams (future feature)
- View results (future feature)

## Application Features

### Authentication & Authorization
- Secure login/logout
- User registration with email
- Password requirements (minimum 6 characters, uppercase, lowercase, digit)
- Role-based access control
- Access denied page

### Multitenancy
- Configurable exam center name
- Custom logo support
- Brand color customization
- Dynamic UI theming

### Responsive Design
- Bootstrap 5 framework
- Mobile-friendly interface
- Modern card-based layouts
- Bootstrap Icons integration

### Dashboards

#### Admin Dashboard
- User statistics overview
- User management table
- Role distribution
- Active user count

#### Student Dashboard
- Personalized welcome message
- Profile information
- Exam schedule (placeholder)
- Results tracking (placeholder)

## Database Schema

The application uses ASP.NET Core Identity tables plus custom fields:

### ApplicationUser (extends IdentityUser)
- FirstName
- LastName
- CreatedDate
- IsActive

### Identity Tables
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetRoleClaims
- AspNetUserLogins
- AspNetUserTokens

## Configuration Options

### Password Requirements

Modify in `Program.cs`:

```csharp
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
```

### Cookie Settings

Configure authentication cookies in `Program.cs`:

```csharp
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
```

## Troubleshooting

### Database Connection Issues

1. **LocalDB not available**:
   - Install SQL Server LocalDB from [Microsoft SQL Server Downloads](https://www.microsoft.com/sql-server/sql-server-downloads)
   - Or use SQL Server Express

2. **Connection string error**:
   - Verify server name: `(localdb)\\mssqllocaldb` or `.\\SQLEXPRESS`
   - Check if SQL Server is running
   - Test connection with SSMS

### Migration Errors

```bash
# Delete existing database and migrations
dotnet ef database drop
dotnet ef migrations remove

# Create fresh migration
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Already in Use

Modify `Properties/launchSettings.json` to change ports:

```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

## Development

### Adding New Features

1. **Create Model** in `Models/`
2. **Update DbContext** in `Data/ApplicationDbContext.cs`
3. **Create Migration**: `dotnet ef migrations add YourMigrationName`
4. **Update Database**: `dotnet ef database update`
5. **Create Controller** in `Controllers/`
6. **Create Views** in `Views/`

### Adding New Roles

Update `Data/DbInitializer.cs`:

```csharp
string[] roleNames = { "Admin", "Student", "Teacher", "YourNewRole" };
```

## Technology Stack

- **Framework**: ASP.NET Core 8.0 MVC
- **Database**: SQL Server
- **ORM**: Entity Framework Core 8.0
- **Authentication**: ASP.NET Core Identity
- **UI Framework**: Bootstrap 5.3.2
- **Icons**: Bootstrap Icons 1.11.1
- **Validation**: jQuery Validation

## Security Features

- Password hashing and salting
- Anti-forgery tokens
- HTTPS enforcement
- Secure cookie authentication
- Role-based authorization
- SQL injection protection (EF Core parameterized queries)

## Admin Features (New!)

The application now includes comprehensive admin functionality:

### 1. Student Registration
- Register and manage student accounts
- Assign students to groups
- Edit/delete student records

### 2. Master Data Management
- **Subjects**: Manage academic subjects
- **Classes**: Manage class/grade levels
- **Groups**: Manage student groups/sections
- **Topics**: Manage topics within subjects

### 3. Question Bank
- Create questions (Multiple Choice, True/False, Short Answer, Essay)
- Set difficulty levels (Easy, Medium, Hard)
- Organize by topics
- Add explanations and marks

### 4. Test Creation
- Create tests with configurable settings
- Add questions from question bank
- Set duration, marks, and passing criteria
- Manage test status (Draft, Published, etc.)

### 5. Test Allocation
- Allocate tests to students
- Schedule test start/end times
- Reschedule tests (individual or bulk)
- Track completion status

### 6. Results & Reports
- View test results with detailed breakdowns
- Generate test-wise reports with statistics
- Generate student-wise performance reports
- View individual student answers

### 7. Exam Center Configuration
- Configure center name and logo
- Set brand colors
- Add contact information (address, phone, email)
- Customize UI theming

**See [ADMIN_FEATURES.md](ADMIN_FEATURES.md) for complete documentation.**

## Future Enhancements

- [ ] Online exam taking interface for students
- [ ] Real-time exam monitoring
- [ ] Email notifications for test allocations
- [ ] Bulk student import via CSV
- [ ] PDF report generation
- [ ] Advanced analytics with charts
- [ ] Question templates
- [ ] Multi-language support

## License

This project is created for educational purposes.

## Support

For issues and questions:
- Check the [Troubleshooting](#troubleshooting) section
- Review [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core/)
- Check [Entity Framework Core Documentation](https://docs.microsoft.com/ef/core/)

## Author

Created as a demonstration of ASP.NET Core MVC with Identity, EF Core, and multitenancy support.

