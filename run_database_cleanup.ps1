# Database Cleanup Script for CETExamApp
# PowerShell version with error handling

param(
    [string]$ServerName = "(localdb)\mssqllocaldb",
    [string]$DatabaseName = "CETExamAppDb",
    [string]$SqlScriptPath = "database_cleanup.sql"
)

Write-Host "Database Cleanup Script for CETExamApp" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# Check if SQL script exists
if (-not (Test-Path $SqlScriptPath)) {
    Write-Host "Error: SQL script '$SqlScriptPath' not found!" -ForegroundColor Red
    Write-Host "Please ensure the database_cleanup.sql file is in the same directory." -ForegroundColor Red
    exit 1
}

Write-Host "Connecting to database: $DatabaseName on $ServerName" -ForegroundColor Yellow
Write-Host ""

try {
    # Run the SQL script
    $result = sqlcmd -S $ServerName -d $DatabaseName -i $SqlScriptPath
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host ""
        Write-Host "Database cleanup completed successfully!" -ForegroundColor Green
        Write-Host "You can now run: dotnet ef database update" -ForegroundColor Cyan
    } else {
        Write-Host ""
        Write-Host "Error occurred during database cleanup." -ForegroundColor Red
        Write-Host "Exit code: $LASTEXITCODE" -ForegroundColor Red
    }
} catch {
    Write-Host ""
    Write-Host "Exception occurred: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Read-Host "Press Enter to continue"
