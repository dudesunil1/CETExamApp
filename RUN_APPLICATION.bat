@echo off
echo ================================================
echo    CET Exam Application - Quick Start
echo ================================================
echo.

echo Checking .NET installation...
dotnet --version
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: .NET 8.0 SDK not found!
    echo Please install from: https://dotnet.microsoft.com/download
    pause
    exit /b 1
)
echo.

echo Checking database configuration...
echo Connection String: Server=.;Database=CETExamAppDb;User Id=sa;Password=sa
echo.

echo Creating database migration...
dotnet ef migrations add CompleteIntegration --context ApplicationDbContext
echo.

echo Updating database...
dotnet ef database update
if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Database update failed!
    echo Please check:
    echo   1. SQL Server is running
    echo   2. Connection string is correct in appsettings.json
    echo   3. You have permission to create databases
    echo.
    pause
    exit /b 1
)
echo.

echo ================================================
echo    Database created successfully!
echo ================================================
echo.
echo The following has been set up:
echo   - Database: CETExamAppDb
echo   - Tables: 19 tables created
echo   - Sample Data: Classes, Subjects, Groups, Topics seeded
echo   - Admin User: admin@cetexam.com / Admin@123
echo   - Roles: Admin, Student
echo.

echo ================================================
echo    Starting Application...
echo ================================================
echo.
echo Application will open at: https://localhost:5001
echo.
echo Login Credentials:
echo   Email: admin@cetexam.com
echo   Password: Admin@123
echo.
echo Press Ctrl+C to stop the application
echo.

dotnet run

