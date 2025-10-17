# CETExamApp Database Migration Guide

## Overview
This guide helps you update your CETExamApp database structure to remove Subject and Group dependencies from Tests.

## Files Included
- `database_cleanup.sql` - SQL script to clean up database structure
- `run_database_cleanup.bat` - Windows batch file (Option 1)
- `run_database_cleanup.ps1` - PowerShell script (Option 2)

## Prerequisites
- SQL Server (LocalDB, Express, or Full)
- SQL Server Management Studio (SSMS) OR sqlcmd command-line tool
- .NET 8.0 Runtime

## Method 1: Using SQL Server Management Studio (Recommended)
1. Open SQL Server Management Studio
2. Connect to your database server
3. Navigate to your CETExamAppDb database
4. Right-click → New Query
5. Copy and paste the contents of `database_cleanup.sql`
6. Execute the script (F5)
7. Verify success message appears

## Method 2: Using Command Line
### Windows Batch File:
1. Double-click `run_database_cleanup.bat`
2. Follow the on-screen instructions

### PowerShell Script:
1. Right-click `run_database_cleanup.ps1` → "Run with PowerShell"
2. Follow the on-screen instructions

### Manual Command Line:
```cmd
sqlcmd -S "(localdb)\mssqllocaldb" -d "CETExamAppDb" -i "database_cleanup.sql"
```

## Method 3: Using Application Connection String
If you know your application's connection string, modify the scripts:
1. Open `run_database_cleanup.bat` or `run_database_cleanup.ps1`
2. Update the SERVER_NAME and DATABASE_NAME variables
3. Run the script

## After Database Cleanup
1. Navigate to your CETExamApp folder
2. Open Command Prompt or PowerShell
3. Run: `dotnet ef database update`
4. Verify the application starts without errors

## Troubleshooting

### Common Issues:
1. **"Login failed"** - Check server name and credentials
2. **"Database does not exist"** - Verify database name
3. **"Permission denied"** - Ensure user has DDL permissions
4. **"Index dependency error"** - The updated script handles this automatically

### Connection String Examples:
- LocalDB: `(localdb)\mssqllocaldb`
- SQL Express: `.\SQLEXPRESS` or `localhost\SQLEXPRESS`
- Named Instance: `SERVERNAME\INSTANCENAME`
- Default Instance: `SERVERNAME`

## Support
If you encounter issues, please provide:
1. Error messages
2. Your connection string (without passwords)
3. SQL Server version
4. Operating system details

## Important Notes
- **BACKUP YOUR DATABASE** before running this script
- This script will delete all existing test data
- The script is safe to run multiple times
- Test the application thoroughly after migration
