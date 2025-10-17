@echo off
echo Database Cleanup Script for CETExamApp
echo =====================================
echo.

REM Set your database connection details here
set SERVER_NAME=(localdb)\mssqllocaldb
set DATABASE_NAME=CETExamAppDb

echo Connecting to database: %DATABASE_NAME% on %SERVER_NAME%
echo.

REM Run the SQL script
sqlcmd -S "%SERVER_NAME%" -d "%DATABASE_NAME%" -i "database_cleanup.sql"

if %ERRORLEVEL% EQU 0 (
    echo.
    echo Database cleanup completed successfully!
    echo You can now run: dotnet ef database update
) else (
    echo.
    echo Error occurred during database cleanup.
    echo Please check the error messages above.
)

echo.
pause
