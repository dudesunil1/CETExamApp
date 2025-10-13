# 🚀 DEPLOYMENT INSTRUCTIONS

## CET Exam Application v3.0 - Final Deployment

---

## ⚠️ IMPORTANT: Database Migration Required

This update adds the **Student Dashboard** and **Test Taking** features, which require new database tables and columns.

---

## 🔧 Step 1: Install Required Tools (If Not Already Installed)

```bash
# Install EF Core Tools (if not already installed)
dotnet tool install --global dotnet-ef

# OR Update if already installed
dotnet tool update --global dotnet-ef
```

---

## 📊 Step 2: Create and Apply Migration

### Option A: Using Terminal/Command Prompt

```bash
# Navigate to project directory
cd "E:\ASP.Net Core\CETExamApp"

# Create the migration
dotnet ef migrations add AddStudentDashboardAndTestTaking

# Apply migration to database
dotnet ef database update
```

### Option B: Using Package Manager Console (Visual Studio)

```powershell
# In Visual Studio: Tools > NuGet Package Manager > Package Manager Console

Add-Migration AddStudentDashboardAndTestTaking
Update-Database
```

---

## ✅ Step 3: Verify Migration Success

After running the migration, verify these changes were applied:

### New Table Created:
- **TestAttempts** (8 columns)
  - Id, TestAllocationId, StudentId, TestId
  - StartedAt, SubmittedAt, Status
  - CurrentQuestionIndex, LastActivityAt
  - ShuffledQuestionOrder

### StudentAnswers Table - New Columns:
- **Status** (INT) - QuestionStatus enum (0-3)
- **IsMarkedForReview** (BIT) - Review flag
- **AnsweredAt** (DATETIME2) - Timestamp

### Verify in SQL Server Management Studio or Azure Data Studio:
```sql
-- Check TestAttempts table exists
SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'TestAttempts'

-- Check StudentAnswers new columns
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'StudentAnswers'
AND COLUMN_NAME IN ('Status', 'IsMarkedForReview', 'AnsweredAt')
```

---

## 🏃 Step 4: Run the Application

```bash
# Run the application
dotnet run

# OR in Visual Studio
# Press F5 or click "Run"
```

---

## 🎓 Step 5: Test the Student Dashboard

### 5.1 Create a Test Student (If Not Exists)

**Login as Admin:**
- Email: admin@cetexam.com
- Password: Admin@123

**Create Student:**
1. Navigate to: Admin > Student Registration
2. Create a new student with:
   - Name: Test Student
   - Class: 11th
   - Group: PCM
   - Email: student@test.com
   - Password: Student@123
   - Upload photo (optional)
3. Save

### 5.2 Create a Test Exam

**As Admin:**
1. Go to: Admin > Test Management > Create New Test
2. Fill in details:
   - Name: Sample Physics Test
   - Group: PCM
   - Subject: Physics
   - Duration: 30 minutes
   - Add some questions
   - Enable "Shuffle Questions"
   - Enable "Show Results Immediately"
3. Save

### 5.3 Allocate Test to Student

**As Admin:**
1. Go to: Admin > Test Allocations
2. Click "Allocate Test"
3. Select:
   - Test: Sample Physics Test
   - Students: Test Student
   - Schedule: Current date/time (or near future)
4. Save

### 5.4 Take Test as Student

**Logout and Login as Student:**
- Email: student@test.com
- Password: Student@123

**Test the Dashboard:**
1. ✅ See student dashboard with profile
2. ✅ View statistics cards
3. ✅ See "Upcoming Tests" table
4. ✅ Test shows in upcoming (if scheduled)

**Start Test:**
1. ✅ Click "Start Test" button
2. ✅ Confirm dialog appears
3. ✅ Redirects to test interface

**Take Test:**
1. ✅ Timer appears in header
2. ✅ Questions navigation panel visible
3. ✅ All buttons show Red (Unvisited)
4. ✅ Click Question 1 → Turns Blue (Visited)
5. ✅ Select an answer → Turns Green (Answered)
6. ✅ Click "Mark for Review" → Turns Yellow
7. ✅ Navigate to other questions
8. ✅ Summary updates in real-time
9. ✅ Timer counts down
10. ✅ Math equations render (if any)

**Submit Test:**
1. ✅ Click "Submit Test"
2. ✅ Confirm submission
3. ✅ Redirects to review page
4. ✅ See results and score

**Review Test:**
1. ✅ Overall result shown (Pass/Fail)
2. ✅ Score and percentage displayed
3. ✅ If "Show Results Immediately" ON:
   - ✅ Correct answers shown
   - ✅ Explanations visible
   - ✅ Images rendered
   - ✅ LaTeX equations rendered

---

## 🎯 Step 6: Test All Student Features

### Dashboard Tests

- [ ] Login as student
- [ ] Profile displays with photo
- [ ] Statistics show correct counts
- [ ] Upcoming tests list
- [ ] Completed tests list
- [ ] In-progress tests alert (if applicable)

### Test Timing Tests

- [ ] Create test with future scheduled time
- [ ] Student sees "Scheduled" status
- [ ] "Start Test" button is disabled
- [ ] Wait for scheduled time (or update DB to past time)
- [ ] Status changes to "Available"
- [ ] Button becomes enabled

### Test Taking Tests

- [ ] Questions display correctly
- [ ] All 4 status colors work (Red/Blue/Green/Yellow)
- [ ] Answer auto-saves
- [ ] Mark for review works
- [ ] Clear answer works
- [ ] Previous/Next buttons work
- [ ] Question navigation buttons work
- [ ] Timer counts down
- [ ] Summary updates
- [ ] Images display (if any)
- [ ] LaTeX renders (if any)

### Submission Tests

- [ ] Manual submit works
- [ ] Auto-submit works (set duration to 1 min and wait)
- [ ] Results calculate correctly
- [ ] Pass/Fail determined correctly

### Review Tests

- [ ] Review page loads
- [ ] Score shown correctly
- [ ] If allowed: Correct answers shown
- [ ] If allowed: Explanations shown
- [ ] Back to dashboard works

---

## 🔐 Step 7: Test Admin Features

### Result Management Tests

**As Admin:**
- [ ] Login as admin
- [ ] Go to: Admin > Results
- [ ] View all test results
- [ ] Generate Student-wise Result report
- [ ] Generate Topic-wise Performance report
- [ ] Generate Test-wise Summary report
- [ ] View Detailed Answer Key
- [ ] Generate Result Card
- [ ] Export to PDF (Student Result)
- [ ] Export to Excel (Test Results)
- [ ] Export Answer Key PDF

---

## 📱 Step 8: Browser Testing

Test in multiple browsers:
- [ ] Chrome
- [ ] Firefox
- [ ] Edge
- [ ] Safari (if available)

Check for:
- [ ] Layout renders correctly
- [ ] JavaScript works
- [ ] Timer functions
- [ ] AJAX saves work
- [ ] No console errors

---

## 🛠️ Troubleshooting

### Issue: Migration Fails

**Error:** "A network-related or instance-specific error..."

**Solution:**
1. Verify SQL Server is running
2. Check connection string in `appsettings.json`
3. Ensure LocalDB installed (SQL Server Express LocalDB)

```bash
# Check if LocalDB is installed
sqllocaldb info

# If not, install SQL Server Express with LocalDB
```

---

### Issue: Tables Not Created

**Error:** Migration runs but tables missing

**Solution:**
1. Check if migration file was created in `Migrations` folder
2. Verify `database update` completed successfully
3. Check database connection:

```sql
-- In SQL Server Management Studio / Azure Data Studio
SELECT name FROM sys.databases WHERE name = 'CETExamDB';
SELECT * FROM sys.tables WHERE name = 'TestAttempts';
```

---

### Issue: Student Can't See Tests

**Possible Causes:**
1. Test not allocated to student
2. Test scheduled for future time
3. Test already completed

**Solution:**
1. As Admin: Check Test Allocations
2. Verify student is in the allocation
3. Check scheduled times
4. Create new test if needed

---

### Issue: Timer Not Working

**Possible Causes:**
1. JavaScript disabled
2. Browser compatibility
3. Console errors

**Solution:**
1. Enable JavaScript in browser
2. Check browser console (F12)
3. Try different browser
4. Clear cache and reload

---

### Issue: Answers Not Saving

**Possible Causes:**
1. Network error
2. Server error
3. Session expired

**Solution:**
1. Check network tab in browser dev tools
2. Check server logs for errors
3. Re-login if session expired
4. Verify `SaveAnswer` endpoint works

---

### Issue: LaTeX Not Rendering

**Possible Causes:**
1. MathJax not loading
2. Network blocked CDN
3. Syntax errors in LaTeX

**Solution:**
1. Check if MathJax CDN accessible
2. View browser console for errors
3. Test with simple LaTeX: `\( x^2 \)`
4. Verify LaTeX syntax is correct

---

## 📊 Database Backup (Recommended)

**Before Migration:**
```sql
-- Backup database before migration
BACKUP DATABASE CETExamDB 
TO DISK = 'C:\Backups\CETExamDB_PreMigration.bak'
WITH FORMAT;
```

**After Migration:**
```sql
-- Backup after successful migration
BACKUP DATABASE CETExamDB 
TO DISK = 'C:\Backups\CETExamDB_PostMigration.bak'
WITH FORMAT;
```

---

## 🚀 Production Deployment Checklist

### Pre-Deployment

- [ ] All features tested locally
- [ ] All tests passing
- [ ] Database backed up
- [ ] Migration scripts ready
- [ ] Connection strings configured
- [ ] HTTPS configured
- [ ] File upload limits set
- [ ] Error logging enabled

### Deployment Steps

1. **Prepare Server:**
   ```bash
   # Install .NET 8.0 Runtime
   # Install SQL Server
   # Configure IIS/Nginx
   ```

2. **Deploy Application:**
   ```bash
   # Publish application
   dotnet publish -c Release -o ./publish
   
   # Copy to server
   # Configure web server
   ```

3. **Run Migration on Production:**
   ```bash
   # On production server
   dotnet ef database update --connection "ProductionConnectionString"
   ```

4. **Configure Settings:**
   - Update `appsettings.Production.json`
   - Set connection strings
   - Set file paths
   - Configure email (if needed)

5. **Verify Deployment:**
   - [ ] Application loads
   - [ ] Database connected
   - [ ] Admin can login
   - [ ] Student can login
   - [ ] All features work
   - [ ] File uploads work
   - [ ] Reports generate
   - [ ] Exports work

### Post-Deployment

- [ ] Monitor logs for errors
- [ ] Test all critical features
- [ ] Verify performance
- [ ] Check security settings
- [ ] Train administrators
- [ ] Onboard students

---

## 📞 Support & Documentation

**Complete Documentation:**
1. **README.md** - Main overview
2. **QUICKSTART.md** - Quick start guide
3. **SETUP_GUIDE.md** - Detailed setup
4. **STUDENT_DASHBOARD_GUIDE.md** - Student features
5. **ADMIN_FEATURES.md** - Admin features
6. **TEST_MANAGEMENT_GUIDE.md** - Test management
7. **RESULT_MANAGEMENT_GUIDE.md** - Results & reports
8. **FINAL_IMPLEMENTATION_STATUS.md** - Complete status

**For Issues:**
- Check documentation first
- Review troubleshooting section
- Check browser console
- Check server logs
- Verify database migration

---

## ✅ Post-Migration Verification SQL Queries

```sql
-- 1. Verify TestAttempts table structure
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'TestAttempts'
ORDER BY ORDINAL_POSITION;

-- Expected columns:
-- Id, TestAllocationId, StudentId, TestId, StartedAt, SubmittedAt,
-- Status, CurrentQuestionIndex, LastActivityAt, ShuffledQuestionOrder

-- 2. Verify StudentAnswers enhancements
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'StudentAnswers'
AND COLUMN_NAME IN ('Status', 'IsMarkedForReview', 'AnsweredAt');

-- Expected 3 rows

-- 3. Check all relationships
SELECT 
    fk.name AS ForeignKey,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys AS fk
INNER JOIN sys.tables AS tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables AS tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns AS cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns AS cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name = 'TestAttempts';

-- Expected 3 foreign keys from TestAttempts

-- 4. Test data integrity
SELECT 
    (SELECT COUNT(*) FROM TestAttempts) AS TestAttempts,
    (SELECT COUNT(*) FROM StudentAnswers) AS StudentAnswers,
    (SELECT COUNT(*) FROM TestAllocations) AS TestAllocations,
    (SELECT COUNT(*) FROM Tests) AS Tests,
    (SELECT COUNT(*) FROM Questions) AS Questions,
    (SELECT COUNT(*) FROM AspNetUsers WHERE Id IN (SELECT Id FROM AspNetUserRoles WHERE RoleId = (SELECT Id FROM AspNetRoles WHERE Name = 'Student'))) AS Students;
```

---

## 🎉 Final Checklist

**Before Using:**
- [ ] EF Tools installed
- [ ] Migration created
- [ ] Database updated
- [ ] Tables verified
- [ ] Application runs
- [ ] No errors in logs

**First Time Setup:**
- [ ] Admin account works (admin@cetexam.com)
- [ ] Sample student created
- [ ] Sample test created
- [ ] Test allocated
- [ ] Student can take test
- [ ] Results generate
- [ ] Reports work
- [ ] Exports work

**Production Ready:**
- [ ] All features tested
- [ ] Security verified
- [ ] Performance acceptable
- [ ] Backups configured
- [ ] Monitoring setup
- [ ] Documentation reviewed

---

## 🚀 Quick Start Command Sequence

```bash
# Complete setup in 5 commands:

# 1. Install EF Tools (if needed)
dotnet tool install --global dotnet-ef

# 2. Navigate to project
cd "E:\ASP.Net Core\CETExamApp"

# 3. Create migration
dotnet ef migrations add AddStudentDashboardAndTestTaking

# 4. Apply migration
dotnet ef database update

# 5. Run application
dotnet run
```

**Then:**
- Login as admin@cetexam.com / Admin@123
- Create a student
- Create a test
- Allocate test
- Login as student
- Take test!

---

**Status:** Ready for Migration & Deployment ✅  
**Version:** 3.0.0  
**Last Updated:** October 2025

