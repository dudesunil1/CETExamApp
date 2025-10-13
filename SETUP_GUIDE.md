# Complete Setup Guide - CET Exam Application

This guide will help you set up the CET Exam Application with all the new admin features.

## Prerequisites

- .NET 8.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 or VS Code

## Step 1: Initial Setup

### Option A: Using PowerShell Script (Recommended)

```powershell
.\setup-and-run.ps1
```

### Option B: Manual Setup

```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Create and apply migrations
dotnet ef migrations add InitialCreate
dotnet ef database update

# Run the application
dotnet run
```

## Step 2: Database Migration

The application will automatically create the database on first run. However, if you want to create migrations manually:

```bash
# Add migration
dotnet ef migrations add AddAdminFeatures

# Update database
dotnet ef database update
```

## Step 3: Access the Application

1. Navigate to `https://localhost:5001` or `http://localhost:5000`
2. Login with admin credentials:
   - Email: `admin@cetexam.com`
   - Password: `Admin@123`

## Step 4: Configure Exam Center

1. Go to **Admin Dashboard** → **Exam Center Config**
2. Set your center name
3. Upload your logo (optional)
4. Set brand colors
5. Add contact information
6. Click **Update Configuration**

## Step 5: Set Up Master Data

### Create Subjects

1. Go to **Admin Dashboard** → **Master Data** → **Subjects**
2. Click **Add New Subject**
3. Enter subject details:
   - Name: "Mathematics"
   - Code: "MATH101"
   - Description: "Advanced Mathematics"
4. Click **Create**
5. Repeat for other subjects (Physics, Chemistry, English, etc.)

### Create Classes

1. Go to **Admin Dashboard** → **Master Data** → **Classes**
2. Click **Add New Class**
3. Enter class details:
   - Name: "Grade 10"
   - Code: "G10"
4. Click **Create**
5. Repeat for other classes

### Create Groups

1. Go to **Admin Dashboard** → **Master Data** → **Groups**
2. Click **Add New Group**
3. Enter group details:
   - Name: "Section A"
   - Code: "SEC-A"
   - Class: Select "Grade 10"
4. Click **Create**
5. Repeat for other groups

### Create Topics

1. Go to **Admin Dashboard** → **Master Data** → **Topics**
2. Click **Add New Topic**
3. Enter topic details:
   - Name: "Algebra"
   - Code: "ALG"
   - Subject: Select "Mathematics"
4. Click **Create**
5. Repeat for other topics

## Step 6: Register Students

1. Go to **Admin Dashboard** → **Student Registration**
2. Click **Register New Student**
3. Fill in student details:
   - First Name: "John"
   - Last Name: "Doe"
   - Email: "john.doe@example.com"
   - Group: Select "Section A"
   - Password: Create a strong password
4. Click **Register Student**
5. Repeat for other students

## Step 7: Build Question Bank

1. Go to **Admin Dashboard** → **Question Bank**
2. Click **Add New Question**
3. Fill in question details:
   - Topic: Select "Algebra"
   - Question Text: "What is 2 + 2?"
   - Question Type: Multiple Choice
   - Difficulty: Easy
   - Options A-D: 1, 2, 3, 4
   - Correct Answer: D
   - Marks: 1
4. Click **Create Question**
5. Repeat to build a comprehensive question bank

## Step 8: Create Tests

1. Go to **Admin Dashboard** → **Test Creation**
2. Click **Create New Test**
3. Fill in test details:
   - Title: "Algebra Quiz 1"
   - Description: "Basic algebra questions"
   - Subject: Select "Mathematics"
   - Class: Select "Grade 10"
   - Duration: 60 minutes
   - Total Marks: 100
   - Passing Marks: 40
4. Click **Create**
5. On the next page, add questions:
   - Select questions from the list
   - Set marks for each question
   - Click **Add to Test**
6. Repeat until test is complete

## Step 9: Allocate Tests to Students

1. Go to **Admin Dashboard** → **Test Allocation**
2. Click **Allocate Test**
3. Select test and students:
   - Test: Select "Algebra Quiz 1"
   - Students: Select multiple students (hold Ctrl/Cmd)
   - Scheduled Start: Set start date/time
   - Scheduled End: Set end date/time
4. Click **Allocate Test**

## Step 10: Monitor and Manage

### View Allocations

- Go to **Test Allocations** → **Index**
- Filter by test if needed
- See completion status

### Reschedule Tests

**Individual Reschedule:**
1. Click **Reschedule** button for a student
2. Set new start and end times
3. Click **Update**

**Bulk Reschedule:**
1. Go to **Test Allocations**
2. Filter by test
3. Click **Reschedule All**
4. Set new dates for all students
5. Click **Update**

### View Results

1. Go to **Admin Dashboard** → **Results & Reports**
2. Filter by test or student
3. Click **View Details** for detailed result
4. Click **Test Report** for test statistics
5. Click **Student Report** for student performance

## Complete Setup Checklist

- [ ] Application installed and running
- [ ] Database created and migrated
- [ ] Admin login successful
- [ ] Exam center configured
- [ ] Subjects created
- [ ] Classes created
- [ ] Groups created
- [ ] Topics created
- [ ] Students registered
- [ ] Questions added to question bank
- [ ] Tests created
- [ ] Questions added to tests
- [ ] Tests allocated to students
- [ ] Results monitoring set up

## Sample Data Setup Script

For testing purposes, you can quickly set up sample data:

### Sample Subjects
- Mathematics (MATH)
- Physics (PHYS)
- Chemistry (CHEM)
- English (ENG)

### Sample Classes
- Grade 9 (G9)
- Grade 10 (G10)
- Grade 11 (G11)

### Sample Groups
- Section A (G10-A)
- Section B (G10-B)

### Sample Topics
- Algebra (under Mathematics)
- Geometry (under Mathematics)
- Mechanics (under Physics)
- Thermodynamics (under Physics)

### Sample Students
- john.doe@example.com / Student@123
- jane.smith@example.com / Student@123
- bob.wilson@example.com / Student@123

## Database Connection Strings

### LocalDB (Default)
```json
"Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server Express
```json
"Server=.\\SQLEXPRESS;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

### SQL Server with Authentication
```json
"Server=YOUR_SERVER;Database=CETExamAppDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;MultipleActiveResultSets=true"
```

## Troubleshooting Setup

### Migration Issues

If you encounter migration errors:

```bash
# Remove existing migrations
dotnet ef migrations remove

# Drop database
dotnet ef database drop

# Create fresh migration
dotnet ef migrations add InitialCreate

# Update database
dotnet ef database update
```

### Connection String Issues

1. Verify SQL Server is running
2. Check server name in connection string
3. Test connection with SQL Server Management Studio (SSMS)
4. Ensure database name is correct

### Permission Issues

If you get permission errors:

1. Run Visual Studio as Administrator
2. Or grant permissions to your user in SQL Server
3. Or use SQL authentication instead of Windows authentication

### Port Already in Use

Edit `Properties/launchSettings.json`:

```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

## Next Steps

After setup is complete:

1. Review [ADMIN_FEATURES.md](ADMIN_FEATURES.md) for detailed feature documentation
2. Check [README.md](README.md) for general application information
3. Start using the application!

## Production Deployment

For production deployment:

1. Update connection string in `appsettings.Production.json`
2. Set `ASPNETCORE_ENVIRONMENT=Production`
3. Use proper SSL certificates
4. Enable logging and monitoring
5. Set up regular database backups
6. Configure email service for notifications
7. Review security settings

## Support

If you encounter issues during setup:

1. Check this guide for common solutions
2. Review error messages carefully
3. Check database connection and permissions
4. Verify .NET SDK version
5. Check application logs

---

**Setup Version**: 1.0.0
**Last Updated**: October 2025

