# Quick Start Guide

## Fastest Way to Run the Application

### Option 1: Using PowerShell Script (Windows)

```powershell
.\setup-and-run.ps1
```

This script will:
1. Check if .NET SDK is installed
2. Restore NuGet packages
3. Install EF Core tools (if needed)
4. Build the application
5. Run the application

### Option 2: Manual Steps

```bash
# 1. Restore packages
dotnet restore

# 2. Build the application
dotnet build

# 3. Run the application
dotnet run
```

### Option 3: Using Visual Studio 2022

1. Open `CETExamApp.csproj` in Visual Studio 2022
2. Press **F5** or click the **Run** button
3. The application will build and launch automatically

## Access the Application

Once running, open your browser and navigate to:
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

## Login Credentials

### Admin Account
- **Email**: admin@cetexam.com
- **Password**: Admin@123

### Student Account
- **Email**: student@cetexam.com
- **Password**: Student@123

## First Time Setup

The application will automatically:
1. Create the SQL Server database
2. Apply all migrations
3. Seed default admin and student users
4. Seed Admin and Student roles

No manual database setup required!

## Customization

To customize the exam center name and branding, edit `appsettings.json`:

```json
{
  "TenantSettings": {
    "ExamCenterName": "Your Exam Center Name",
    "LogoPath": "/images/your-logo.png",
    "PrimaryColor": "#007bff",
    "SecondaryColor": "#6c757d"
  }
}
```

## Troubleshooting

### Database Connection Error

If you see a database connection error, update the connection string in `appsettings.json`:

**For SQL Server Express:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**For SQL Server with credentials:**
```json
"DefaultConnection": "Server=YOUR_SERVER;Database=CETExamAppDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true"
```

### Port Already in Use

If port 5001 is already in use, the application will automatically use a different port. Check the console output for the actual URL.

## Next Steps

1. **Login** with admin or student account
2. **Explore** the different dashboards
3. **Register** new users from the registration page
4. **Customize** tenant settings in appsettings.json
5. **Add** your logo to `wwwroot/images/logo.png`

## Need Help?

Check the full [README.md](README.md) for detailed documentation.

