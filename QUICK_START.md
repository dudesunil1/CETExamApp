# ⚡ QUICK START GUIDE

## Get Your CET Exam App Running in 5 Minutes

---

## 🚀 5-STEP QUICK START

### **Step 1: Install Prerequisites** (2 minutes)

**Required:**
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/sql-server/sql-server-downloads) (LocalDB, Express, or Full)

**Verify Installation:**
```bash
dotnet --version
# Should show: 8.0.x or higher
```

---

### **Step 2: Configure Database** (30 seconds)

Your `appsettings.json` is already configured:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

✅ **Already Done!** Using local SQL Server with `sa`/`sa` credentials.

---

### **Step 3: Create Database** (1 minute)

```bash
# Navigate to project directory
cd "E:\ASP.Net Core\CETExamApp"

# Create migration (if not exists)
dotnet ef migrations add InitialCreate

# Create database and tables
dotnet ef database update
```

**What This Does:**
- Creates `CETExamAppDb` database
- Creates 19 tables
- Seeds sample data (Classes, Subjects, Groups, Topics)
- Creates Admin user (admin@cetexam.com / Admin@123)
- Creates Admin and Student roles

---

### **Step 4: Run Application** (30 seconds)

```bash
# Start the application
dotnet run
```

**Expected Output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5001
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
```

---

### **Step 5: Login and Test** (1 minute)

**Open Browser:**
```
https://localhost:5001
```

**Login as Admin:**
```
Email: admin@cetexam.com
Password: Admin@123
```

**You're In! 🎉**

---

## 📋 WHAT YOU GET

### **Pre-Configured Features:**

**Master Data (Pre-Seeded):**
- ✅ Classes: 10th, 11th, 12th
- ✅ Subjects: Physics, Chemistry, Mathematics, Biology
- ✅ Groups: PCMB, PCB, PCM
- ✅ Topics: 11 topics across subjects

**Admin Dashboard Sections:**
1. Student Registration
2. Master Data (Subjects, Classes, Groups, Topics)
3. Question Bank
4. Test Creation
5. Test Allocation
6. Results & Reports
7. Exam Center Configuration

---

## 🎯 QUICK TESTING WORKFLOW

### **Test 1: View Pre-Seeded Data** (2 minutes)

```
1. Login as admin@cetexam.com
2. Click "Master Data" > "Subjects"
   → See 4 subjects: Physics, Chemistry, Mathematics, Biology
3. Click "Classes"
   → See 3 classes: 10th, 11th, 12th
4. Click "Groups"
   → See 3 groups: PCMB, PCB, PCM
5. Click "Topics"
   → See 11 pre-configured topics
```

---

### **Test 2: Create a Student** (2 minutes)

```
1. Click "Student Registration"
2. Click "Create New"
3. Fill in:
   - First Name: John
   - Last Name: Doe
   - Email: john@test.com
   - Password: Student@123
   - Confirm Password: Student@123
   - Class: 11th
   - Group: PCM
   - Mobile No: 9876543210
   - Parents Mobile No: 9876543211
   - Check "Is Active"
4. Click "Create"
5. Student created with Student role!
```

---

### **Test 3: Create a Question** (3 minutes)

```
1. Click "Question Bank"
2. Click "Create New"
3. Fill in:
   - Question Type: MCQ
   - Subject: Physics
   - Topic: (select any)
   - Class: 11th
   - Question Text: "What is Newton's First Law?"
   - Option A: "Law of Inertia"
   - Option B: "F=ma"
   - Option C: "Action-Reaction"
   - Option D: "Law of Gravity"
   - Correct Answer: A
   - Marks: 1
   - Explanation: "Newton's First Law states..."
4. Click "Create"
5. Question created!
```

---

### **Test 4: Create a Test** (4 minutes)

```
1. Click "Test Creation"
2. Click "Create New"
3. Fill in:
   - Title: "Physics Test 1"
   - Description: "First test for 11th PCM"
   - Subject: Physics
   - Group: PCM
   - Class: 11th
   - Start Date/Time: Today's date, current time
   - End Date/Time: Tomorrow's date
   - Duration: 30 minutes
   - Total Marks: 10
   - Passing Marks: 4
   - Check "Shuffle Questions"
   - Check "Show Results Immediately"
4. Click "Create"
5. Test created!
6. Click "Add Questions" next to the test
7. Select questions and assign marks
8. Click "Add Questions"
```

---

### **Test 5: Allocate Test to Student** (2 minutes)

```
1. Click "Test Allocation"
2. Click "Allocate Test"
3. Select:
   - Test: "Physics Test 1"
   - Student: John Doe
   - Scheduled Start: Today, current time
   - Scheduled End: Tomorrow
4. Click "Allocate"
5. Test allocated!
```

---

### **Test 6: Student Takes Test** (5 minutes)

```
1. Logout (top right, click admin name > Logout)
2. Login as Student:
   Email: john@test.com
   Password: Student@123
3. Student Dashboard loads
4. See "Physics Test 1" in Upcoming Tests
5. Click "Start Test"
6. Confirm "Are you ready?"
7. Test interface loads with:
   - Timer counting down
   - Questions in navigation (all Red)
   - First question displayed
8. Answer questions:
   - Click question → Turns Blue
   - Select answer → Turns Green
   - Click "Mark for Review" → Turns Yellow
9. Navigate using question numbers
10. Click "Submit Test"
11. Results page shows:
    - Score
    - Percentage
    - Pass/Fail
    - Correct answers (if enabled)
    - Explanations
```

---

### **Test 7: View Results (Admin)** (2 minutes)

```
1. Logout from student
2. Login as admin@cetexam.com
3. Click "Results & Reports"
4. See John Doe's result
5. Click "View Details"
6. See complete answer key
7. Try generating reports:
   - Student-wise Result
   - Topic-wise Performance
   - Test-wise Summary
8. Export to PDF/Excel
```

---

### **Test 8: Customize Branding** (2 minutes)

```
1. Click "Exam Center Config"
2. Update:
   - Exam Center Name: "My Custom Center"
   - Primary Color: #e74c3c (red)
   - Secondary Color: #3498db (blue)
   - Upload a logo (optional)
3. Click "Save"
4. Navigate to any page
5. See navbar now has red/blue gradient!
6. Logo appears in navbar!
```

---

## 🐛 COMMON ISSUES & QUICK FIXES

### **Issue 1: Migration Error**

```bash
Error: "Cannot connect to SQL Server"

Fix:
1. Ensure SQL Server is running
2. Check connection string in appsettings.json
3. Test with SQL Server Management Studio:
   Server: .
   Auth: SQL Server Authentication
   Login: sa
   Password: sa
```

---

### **Issue 2: Can't Login as Admin**

```bash
Error: "Invalid email or password"

Fix:
1. Drop and recreate database:
   dotnet ef database drop
   dotnet ef database update
   
2. This will re-run seeding
3. Try login again with admin@cetexam.com / Admin@123
```

---

### **Issue 3: Port Already in Use**

```bash
Error: "Address already in use"

Fix:
1. Change port in Properties/launchSettings.json
2. Or stop other applications using port 5001
3. Or run on different port:
   dotnet run --urls "https://localhost:7001"
```

---

## 📚 NEXT STEPS

### **After Quick Start:**

1. **Read Full Documentation:**
   - COMPLETE_INTEGRATION_GUIDE.md
   - ADMIN_FEATURES.md
   - STUDENT_DASHBOARD_GUIDE.md
   - SECURITY_GUIDE.md

2. **Customize:**
   - Update Exam Center name and logo
   - Change theme colors
   - Add more subjects/classes/topics

3. **Create Real Data:**
   - Add real students
   - Create question bank
   - Design tests
   - Allocate to students

4. **Deploy to Production:**
   - Read DEPLOYMENT_INSTRUCTIONS.md
   - Update appsettings.Production.json
   - Configure production database
   - Install SSL certificate

---

## ✅ CHECKLIST

**Before Starting:**
- [ ] .NET 8.0 SDK installed
- [ ] SQL Server installed and running
- [ ] Project files downloaded

**Quick Start:**
- [ ] Database connection configured
- [ ] Migration run successfully
- [ ] Database created with 19 tables
- [ ] Application running (https://localhost:5001)
- [ ] Admin login successful

**Testing:**
- [ ] Viewed pre-seeded data
- [ ] Created a student
- [ ] Created a question
- [ ] Created a test
- [ ] Allocated test to student
- [ ] Student took test successfully
- [ ] Viewed results as admin
- [ ] Customized branding

**Production Ready:**
- [ ] All features working
- [ ] Security verified
- [ ] Performance acceptable
- [ ] Ready to deploy

---

## 🎉 YOU'RE READY!

**Your CET Exam Application is running and fully functional!**

**Time to Complete Quick Start:** ~5-10 minutes  
**Features Available:** 100+  
**Ready for:** Production Use  

**Default Credentials:**
```
Admin:
Email: admin@cetexam.com
Password: Admin@123

First Student (create yourself):
Email: (your choice)
Password: (minimum 8 chars, uppercase, lowercase, digit, special char)
```

---

**Need Help?**
- Check: COMPLETE_INTEGRATION_GUIDE.md
- Check: Troubleshooting sections
- Review: Individual feature guides

**Enjoy your complete exam management system!** 🚀✨

