# CET Exam App - Setup and Run Script
# This PowerShell script sets up and runs the application

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "CET Exam App - Setup Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET 8.0 SDK is installed
Write-Host "Checking .NET SDK..." -ForegroundColor Yellow
$dotnetVersion = dotnet --version
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ .NET SDK found: $dotnetVersion" -ForegroundColor Green
} else {
    Write-Host "✗ .NET SDK not found. Please install .NET 8.0 SDK" -ForegroundColor Red
    Write-Host "Download from: https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Yellow
    exit 1
}

Write-Host ""

# Restore NuGet packages
Write-Host "Restoring NuGet packages..." -ForegroundColor Yellow
dotnet restore
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Packages restored successfully" -ForegroundColor Green
} else {
    Write-Host "✗ Failed to restore packages" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Check if EF Core tools are installed
Write-Host "Checking EF Core tools..." -ForegroundColor Yellow
$efToolsInstalled = dotnet ef --version 2>&1
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ EF Core tools found" -ForegroundColor Green
} else {
    Write-Host "Installing EF Core tools..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "✓ EF Core tools installed" -ForegroundColor Green
}

Write-Host ""

# Build the application
Write-Host "Building application..." -ForegroundColor Yellow
dotnet build
if ($LASTEXITCODE -eq 0) {
    Write-Host "✓ Build successful" -ForegroundColor Green
} else {
    Write-Host "✗ Build failed" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Setup Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Default Accounts:" -ForegroundColor Yellow
Write-Host "  Admin   : admin@cetexam.com / Admin@123" -ForegroundColor White
Write-Host "  Student : student@cetexam.com / Student@123" -ForegroundColor White
Write-Host ""
Write-Host "The application will create the database automatically on first run." -ForegroundColor Cyan
Write-Host ""
Write-Host "Starting application..." -ForegroundColor Yellow
Write-Host ""

# Run the application
dotnet run

