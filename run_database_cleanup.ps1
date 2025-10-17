# Database Cleanup Script using Entity Framework
# This script will clean up the database structure

Write-Host "Starting Database Cleanup..." -ForegroundColor Green

# Load the project and context
$projectPath = Get-Location
Write-Host "Project Path: $projectPath" -ForegroundColor Yellow

try {
    # Build the project first
    Write-Host "Building project..." -ForegroundColor Yellow
    dotnet build --no-restore
    
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed. Please fix compilation errors first." -ForegroundColor Red
        exit 1
    }
    
    Write-Host "Build successful!" -ForegroundColor Green
    
    # Run the database cleanup using EF Core
    Write-Host "Running database cleanup..." -ForegroundColor Yellow
    
    # Create a simple cleanup script
    $cleanupScript = @"
using Microsoft.EntityFrameworkCore;
using CETExamApp.Data;
using CETExamApp.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();
using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

Console.WriteLine("Starting database cleanup...");

try {
    // Delete all test-related data
    Console.WriteLine("Deleting test-related data...");
    context.Database.ExecuteSqlRaw("DELETE FROM StudentAnswers");
    context.Database.ExecuteSqlRaw("DELETE FROM TestResults");
    context.Database.ExecuteSqlRaw("DELETE FROM TestAttempts");
    context.Database.ExecuteSqlRaw("DELETE FROM TestAllocations");
    context.Database.ExecuteSqlRaw("DELETE FROM TestQuestions");
    context.Database.ExecuteSqlRaw("DELETE FROM Tests");
    
    // Drop foreign key constraints if they exist
    Console.WriteLine("Dropping foreign key constraints...");
    try {
        context.Database.ExecuteSqlRaw("ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Subjects_SubjectId");
        Console.WriteLine("Dropped FK_Tests_Subjects_SubjectId");
    } catch (Exception ex) {
        Console.WriteLine($"FK_Tests_Subjects_SubjectId: {ex.Message}");
    }
    
    try {
        context.Database.ExecuteSqlRaw("ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Groups_GroupId");
        Console.WriteLine("Dropped FK_Tests_Groups_GroupId");
    } catch (Exception ex) {
        Console.WriteLine($"FK_Tests_Groups_GroupId: {ex.Message}");
    }
    
    // Drop indexes if they exist
    Console.WriteLine("Dropping indexes...");
    try {
        context.Database.ExecuteSqlRaw("DROP INDEX IX_Tests_GroupId ON Tests");
        Console.WriteLine("Dropped IX_Tests_GroupId");
    } catch (Exception ex) {
        Console.WriteLine($"IX_Tests_GroupId: {ex.Message}");
    }
    
    try {
        context.Database.ExecuteSqlRaw("DROP INDEX IX_Tests_SubjectId ON Tests");
        Console.WriteLine("Dropped IX_Tests_SubjectId");
    } catch (Exception ex) {
        Console.WriteLine($"IX_Tests_SubjectId: {ex.Message}");
    }
    
    // Drop columns if they exist
    Console.WriteLine("Dropping columns...");
    try {
        context.Database.ExecuteSqlRaw("ALTER TABLE Tests DROP COLUMN GroupId");
        Console.WriteLine("Dropped GroupId column");
    } catch (Exception ex) {
        Console.WriteLine($"GroupId column: {ex.Message}");
    }
    
    try {
        context.Database.ExecuteSqlRaw("ALTER TABLE Tests DROP COLUMN SubjectId");
        Console.WriteLine("Dropped SubjectId column");
    } catch (Exception ex) {
        Console.WriteLine($"SubjectId column: {ex.Message}");
    }
    
    Console.WriteLine("Database cleanup completed successfully!");
    
} catch (Exception ex) {
    Console.WriteLine($"Error during cleanup: {ex.Message}");
    throw;
}
"@

    # Write the cleanup script to a temporary file
    $tempScript = "temp_cleanup.cs"
    $cleanupScript | Out-File -FilePath $tempScript -Encoding UTF8
    
    # Run the cleanup script
    Write-Host "Executing cleanup script..." -ForegroundColor Yellow
    dotnet run --project . --configuration Release -- $tempScript
    
    # Clean up temporary file
    Remove-Item $tempScript -ErrorAction SilentlyContinue
    
    Write-Host "Database cleanup completed!" -ForegroundColor Green
    
} catch {
    Write-Host "Error during cleanup: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "Cleanup process finished." -ForegroundColor Green