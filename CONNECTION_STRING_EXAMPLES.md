# 🔌 DATABASE CONNECTION STRING EXAMPLES

## SQL Server Connection String Configurations

---

## ✅ YOUR CURRENT CONFIGURATION

**As Requested:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Settings:**
- **Server Name:** `.` (local SQL Server instance)
- **Database:** `CETExamAppDb`
- **Username:** `sa`
- **Password:** `sa`
- **Authentication:** SQL Server Authentication

---

## 📋 CONNECTION STRING FORMATS

### 1. SQL Server Authentication (Your Current)

**Format:**
```
Server=SERVER_NAME;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true
```

**Examples:**

**Local Instance (Named `.`):**
```json
"DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Local Instance (Named `localhost`):**
```json
"DefaultConnection": "Server=localhost;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Local Instance (Named `127.0.0.1`):**
```json
"DefaultConnection": "Server=127.0.0.1;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Named Instance:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Remote Server:**
```json
"DefaultConnection": "Server=192.168.1.100;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

**Remote Server with Port:**
```json
"DefaultConnection": "Server=192.168.1.100,1433;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

---

### 2. Windows Authentication

**Format:**
```
Server=SERVER_NAME;Database=DATABASE_NAME;Trusted_Connection=True;MultipleActiveResultSets=true
```

**Examples:**

**LocalDB (Development):**
```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
```

**Local SQL Server Express:**
```json
"DefaultConnection": "Server=.\\SQLEXPRESS;Database=CETExamAppDb;Integrated Security=True;MultipleActiveResultSets=true"
```

**Local SQL Server:**
```json
"DefaultConnection": "Server=localhost;Database=CETExamAppDb;Integrated Security=SSPI;MultipleActiveResultSets=true"
```

---

### 3. Azure SQL Database

**Format:**
```
Server=tcp:SERVER_NAME.database.windows.net,1433;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

**Example:**
```json
"DefaultConnection": "Server=tcp:cetexamserver.database.windows.net,1433;Database=CETExamAppDb;User Id=adminuser@cetexamserver;Password=YourStrongPassword!;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;MultipleActiveResultSets=true"
```

---

### 4. Production SQL Server (Encrypted)

**Format:**
```
Server=SERVER_NAME;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=true
```

**Example:**
```json
"DefaultConnection": "Server=prodserver.company.com;Database=CETExamAppDb;User Id=cetexam_app;Password=StrongProd@Pass123!;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=true"
```

---

## 🔧 CONNECTION STRING PARAMETERS EXPLAINED

### Required Parameters

| Parameter | Description | Example |
|-----------|-------------|---------|
| **Server** | SQL Server instance name | `Server=.` or `Server=localhost` |
| **Database** | Database name | `Database=CETExamAppDb` |

### Authentication (Choose One)

**SQL Server Authentication:**
| Parameter | Description | Example |
|-----------|-------------|---------|
| **User Id** | SQL Server username | `User Id=sa` |
| **Password** | SQL Server password | `Password=sa` |

**Windows Authentication:**
| Parameter | Description | Example |
|-----------|-------------|---------|
| **Trusted_Connection** | Use Windows auth | `Trusted_Connection=True` |
| **Integrated Security** | Same as Trusted_Connection | `Integrated Security=SSPI` |

### Optional Parameters

| Parameter | Description | Default | Your Setting |
|-----------|-------------|---------|--------------|
| **MultipleActiveResultSets** | Allow multiple result sets | False | True ✅ |
| **TrustServerCertificate** | Trust SSL certificate | False | True (dev) |
| **Encrypt** | Encrypt connection | False | False (dev) |
| **Connection Timeout** | Connection timeout (seconds) | 15 | Not set |
| **Max Pool Size** | Maximum connections | 100 | Not set |
| **Min Pool Size** | Minimum connections | 0 | Not set |

---

## 🔐 SECURITY RECOMMENDATIONS

### Development Environment ✅

**Your Current (ACCEPTABLE for development):**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Why it's OK for development:**
- ✅ Using `sa` is acceptable locally
- ✅ Simple password is fine for local DB
- ✅ `TrustServerCertificate=True` skips SSL validation (local only)
- ✅ No encryption needed for localhost

---

### Production Environment ⚠️

**CHANGE BEFORE PRODUCTION:**

1. **Create Dedicated User (Not `sa`):**
```sql
-- Create application-specific user
CREATE LOGIN CETExamUser WITH PASSWORD = 'StrongPassword@2024!';

USE CETExamAppDb;
CREATE USER CETExamUser FOR LOGIN CETExamUser;

-- Grant only necessary permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO CETExamUser;

-- Test login
-- Login with: CETExamUser / StrongPassword@2024!
```

2. **Use Strong Password:**
```
❌ Bad: sa, password, 123456, admin
✅ Good: MyStr0ng!P@ssw0rd2024, Pr0d_DB_P@ss!2024
```

3. **Enable Encryption:**
```json
"DefaultConnection": "Server=prodserver;Database=CETExamAppDb;User Id=CETExamUser;Password=StrongPassword@2024!;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=true"
```

4. **Use Environment Variables:**
```bash
# Set in production server environment
set ConnectionStrings__DefaultConnection="Server=...;Database=...;User Id=...;Password=..."

# Or in Azure App Service Application Settings
ConnectionStrings__DefaultConnection = "Server=...;Database=...;User Id=...;Password=..."
```

---

## 📝 CONFIGURATION FILES

### appsettings.json (Development) ✅

**Your Current File:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true",
    "AlternativeConnection": "Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

---

### appsettings.Production.json (Production) ⚠️

**Created for You:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_PRODUCTION_SERVER;Database=CETExamAppDb;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=False;Encrypt=True;MultipleActiveResultSets=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "AllowedHosts": "yourdomain.com,www.yourdomain.com"
}
```

**Update Before Deploying:**
- Replace `YOUR_PRODUCTION_SERVER` with actual server
- Replace `YOUR_USERNAME` with dedicated app user
- Replace `YOUR_PASSWORD` with strong password
- Update `AllowedHosts` with actual domain

---

## 🧪 TESTING CONNECTION STRING

### Method 1: Run Migration

```bash
# Test connection by running migration
dotnet ef migrations add TestConnection
dotnet ef database update

# If successful, connection string is correct
# If fails, check:
# - SQL Server is running
# - Credentials are correct
# - Server name is correct
# - Firewall allows connection
```

### Method 2: SQL Server Management Studio (SSMS)

```
1. Open SQL Server Management Studio
2. Connect with your credentials:
   Server Name: .
   Authentication: SQL Server Authentication
   Login: sa
   Password: sa

3. If connection succeeds, your connection string is correct
4. Create database manually if needed:
   CREATE DATABASE CETExamAppDb;
```

### Method 3: Run Application

```bash
# Run application
dotnet run

# Check logs for database connection errors
# If no errors, connection is successful
```

---

## 🔍 COMMON CONNECTION ISSUES

### Issue 1: "Cannot connect to server"

**Possible Causes:**
- SQL Server not running
- Wrong server name
- Firewall blocking connection

**Solutions:**
```bash
# Check if SQL Server is running
# Services → SQL Server (MSSQLSERVER) → Status should be "Running"

# Or using command line
sc query MSSQLSERVER

# Start SQL Server if stopped
net start MSSQLSERVER
```

---

### Issue 2: "Login failed for user 'sa'"

**Possible Causes:**
- Wrong password
- SQL Server authentication disabled
- Account locked

**Solutions:**
```sql
-- Enable SQL Server Authentication
-- 1. Open SSMS, connect with Windows Authentication
-- 2. Right-click server → Properties → Security
-- 3. Select "SQL Server and Windows Authentication mode"
-- 4. Restart SQL Server service

-- Reset sa password (if needed)
ALTER LOGIN sa WITH PASSWORD = 'sa';
ALTER LOGIN sa ENABLE;
```

---

### Issue 3: "Database does not exist"

**Solution:**
```bash
# Let EF Core create database
dotnet ef database update

# Or create manually in SSMS
CREATE DATABASE CETExamAppDb;
```

---

### Issue 4: "Certificate chain not trusted"

**Solution:**
```json
// For development, use TrustServerCertificate=True
"DefaultConnection": "Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true"

// For production, install proper SSL certificate
```

---

## 📊 CONNECTION STRING QUICK REFERENCE

| Scenario | Connection String |
|----------|-------------------|
| **Local (Your Current)** | `Server=.;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| **LocalDB** | `Server=(localdb)\\mssqllocaldb;Database=CETExamAppDb;Trusted_Connection=True;MultipleActiveResultSets=true` |
| **SQL Express** | `Server=.\\SQLEXPRESS;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| **Windows Auth** | `Server=.;Database=CETExamAppDb;Integrated Security=True;MultipleActiveResultSets=true` |
| **Remote Server** | `Server=192.168.1.100;Database=CETExamAppDb;User Id=sa;Password=sa;TrustServerCertificate=True;MultipleActiveResultSets=true` |
| **Azure SQL** | `Server=tcp:yourserver.database.windows.net,1433;Database=CETExamAppDb;User Id=user@yourserver;Password=pass;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;` |
| **Production** | `Server=prodserver;Database=CETExamAppDb;User Id=app_user;Password=strong_pass;Encrypt=True;TrustServerCertificate=False;MultipleActiveResultSets=true` |

---

## ✅ YOUR SETUP IS CONFIGURED ✅

**Current Configuration:**
- ✅ Server: `.` (local instance)
- ✅ Database: `CETExamAppDb`
- ✅ Username: `sa`
- ✅ Password: `sa`
- ✅ Ready to use for development

**Next Steps:**
1. Ensure SQL Server is running
2. Run migration: `dotnet ef database update`
3. Run application: `dotnet run`
4. Everything should work!

**For Production:**
- Update `appsettings.Production.json`
- Create dedicated database user
- Use strong password
- Enable encryption

---

**Version:** 3.0.0  
**Status:** Configured & Ready ✅

