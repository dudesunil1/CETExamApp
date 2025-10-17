@echo off
echo CETExamApp Database Migration Tool
echo ==================================
echo.

REM Configuration - Update these values for your environment
set APP_NAME=CETExamApp
set DB_SERVER=(localdb)\mssqllocaldb
set DB_NAME=CETExamAppDb
set APP_PATH=%~dp0

echo Application: %APP_NAME%
echo Database Server: %DB_SERVER%
echo Database Name: %DB_NAME%
echo Application Path: %APP_PATH%
echo.

REM Check if database_cleanup.sql exists
if not exist "%APP_PATH%database_cleanup.sql" (
    echo ERROR: database_cleanup.sql not found in application directory!
    echo Please ensure the SQL script is in the same folder as this batch file.
    pause
    exit /b 1
)

echo Step 1: Running database cleanup script...
echo ==========================================
sqlcmd -S "%DB_SERVER%" -d "%DB_NAME%" -i "%APP_PATH%database_cleanup.sql"

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Database cleanup failed!
    echo Please check the error messages above and try again.
    pause
    exit /b 1
)

echo.
echo Step 2: Applying Entity Framework migrations...
echo ==============================================
cd /d "%APP_PATH%"
dotnet ef database update

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo ERROR: Migration update failed!
    echo Please check the error messages above and try again.
    pause
    exit /b 1
)

echo.
echo ==========================================
echo Database migration completed successfully!
echo ==========================================
echo.
echo You can now start your application.
echo.
pause
