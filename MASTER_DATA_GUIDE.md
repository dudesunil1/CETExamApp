# Master Data CRUD Guide

Complete guide for all Master Data modules in the CET Exam Application.

## Overview

All master data modules have full CRUD (Create, Read, Update, Delete) functionality with Bootstrap 5 UI.

---

## 1. Class Master (10th, 11th, 12th)

**Route:** `/Classes`

### Features
- ✅ List all classes with filtering
- ✅ Create new class (10th, 11th, 12th standards)
- ✅ Edit existing classes
- ✅ Delete classes (with warning if groups assigned)
- ✅ Active/Inactive status management
- ✅ Shows count of groups assigned to each class

### Fields
- **Name** (Required): Class name (e.g., "10th Standard", "11th Standard")
- **Code** (Optional): Short code (e.g., "10TH", "11TH", "12TH")
- **Description** (Optional): Additional details
- **IsActive**: Enable/disable class

### Sample Data
The system automatically creates:
- 10th Standard (Code: 10TH)
- 11th Standard (Code: 11TH)
- 12th Standard (Code: 12TH)

### Usage
1. Navigate to **Admin Dashboard** → **Master Data** → **Classes**
2. Click "Add New Class" to create
3. Fill in class details
4. Click "Create Class"

### Relationships
- **One-to-Many with Groups**: A class can have multiple groups (PCMB, PCB, PCM)
- **One-to-Many with Topics**: Topics are organized by class

---

## 2. Subject Master (P, C, M, B)

**Route:** `/Subjects`

### Features
- ✅ List all subjects
- ✅ Create new subject (Physics, Chemistry, Mathematics, Biology)
- ✅ Edit existing subjects
- ✅ Delete subjects (with dependency check)
- ✅ Active/Inactive status management
- ✅ Code-based organization

### Fields
- **Name** (Required): Subject name (e.g., "Physics", "Chemistry")
- **Code** (Optional): Short code (e.g., "P", "C", "M", "B")
- **Description** (Optional): Subject details
- **IsActive**: Enable/disable subject

### Sample Data
The system automatically creates:
- Physics (Code: P)
- Chemistry (Code: C)
- Mathematics (Code: M)
- Biology (Code: B)

### Usage
1. Navigate to **Admin Dashboard** → **Master Data** → **Subjects**
2. Click "Add New Subject" to create
3. Fill in subject details
4. Click "Create"

### Relationships
- **One-to-Many with Topics**: Each subject can have multiple topics
- **One-to-Many with Tests**: Tests are created for specific subjects

---

## 3. Student Group (PCMB, PCB, PCM)

**Route:** `/Groups`

### Features
- ✅ List all student groups with visual indicators
- ✅ Create new group (PCMB, PCB, PCM combinations)
- ✅ Edit existing groups
- ✅ Delete groups (with student count warning)
- ✅ Active/Inactive status management
- ✅ Class assignment
- ✅ Shows count of students in each group

### Fields
- **Name** (Required): Group name (e.g., "PCMB", "PCB", "PCM")
- **Code** (Optional): Short code
- **ClassId** (Optional): Assign to specific class (10th/11th/12th)
- **Description** (Optional): Group details (e.g., "Physics, Chemistry, Mathematics, Biology")
- **IsActive**: Enable/disable group

### Sample Data
The system automatically creates (assigned to 11th Standard):
- **PCMB** - Physics, Chemistry, Mathematics, Biology (All subjects)
- **PCB** - Physics, Chemistry, Biology (Medical Stream)
- **PCM** - Physics, Chemistry, Mathematics (Engineering Stream)

### Visual Indicators
- **PCMB**: Red badge (All Subjects)
- **PCB**: Blue badge (Medical Stream)
- **PCM**: Yellow badge (Engineering Stream)

### Usage
1. Navigate to **Admin Dashboard** → **Master Data** → **Groups**
2. Click "Add New Group" to create
3. Fill in group details:
   - Name: PCMB/PCB/PCM
   - Select class (11th/12th)
   - Add description
4. Click "Create Group"

### Relationships
- **Many-to-One with Class**: Group belongs to a class
- **One-to-Many with Students**: Multiple students can be in a group

### Group Descriptions

#### PCMB Group
- **Full Name**: Physics, Chemistry, Mathematics, Biology
- **Stream**: All subjects
- **Career Paths**: Medicine, Engineering, Research
- **Subjects**: All four core subjects

#### PCB Group (Medical Stream)
- **Full Name**: Physics, Chemistry, Biology
- **Stream**: Medical/Biological Sciences
- **Career Paths**: MBBS, BDS, Pharmacy, Nursing, Biotechnology
- **Subjects**: Physics, Chemistry, Biology (no Mathematics)

#### PCM Group (Engineering Stream)
- **Full Name**: Physics, Chemistry, Mathematics
- **Stream**: Engineering/Technical
- **Career Paths**: Engineering (B.Tech/B.E.), Architecture, Computer Science
- **Subjects**: Physics, Chemistry, Mathematics (no Biology)

---

## 4. Topic Master (Subject-wise & Class-wise)

**Route:** `/Topics`

### Features
- ✅ List all topics with subject and class information
- ✅ Create new topic (must select both subject and class)
- ✅ Edit existing topics
- ✅ Delete topics (with dependency check)
- ✅ Active/Inactive status management
- ✅ Dual organization (Subject + Class)

### Fields
- **Name** (Required): Topic name (e.g., "Laws of Motion")
- **Code** (Optional): Topic code (e.g., "P-11-03")
- **SubjectId** (Required): Which subject (P/C/M/B)
- **ClassId** (Required): Which class (10th/11th/12th)
- **Description** (Optional): Topic details
- **IsActive**: Enable/disable topic

### Sample Data
The system automatically creates 20+ topics including:

**Physics (11th):**
- Units and Measurements (P-11-01)
- Motion in a Straight Line (P-11-02)
- Laws of Motion (P-11-03)

**Physics (12th):**
- Electric Charges and Fields (P-12-01)
- Current Electricity (P-12-02)

**Chemistry (11th):**
- Some Basic Concepts of Chemistry (C-11-01)
- Structure of Atom (C-11-02)
- Chemical Bonding (C-11-03)

**Mathematics (11th):**
- Sets (M-11-01)
- Relations and Functions (M-11-02)
- Trigonometry (M-11-03)

**Biology (11th):**
- The Living World (B-11-01)
- Biological Classification (B-11-02)
- Cell: The Unit of Life (B-11-03)

### Naming Convention
**Format:** `SubjectCode-ClassNumber-TopicNumber`

Examples:
- `P-11-01` = Physics, 11th Standard, Topic 1
- `C-12-05` = Chemistry, 12th Standard, Topic 5
- `M-11-03` = Mathematics, 11th Standard, Topic 3

### Usage
1. Navigate to **Admin Dashboard** → **Master Data** → **Topics**
2. Click "Add New Topic" to create
3. Fill in topic details:
   - Topic name
   - Select subject (P/C/M/B)
   - Select class (10th/11th/12th)
   - Add description
4. Click "Create"

### Relationships
- **Many-to-One with Subject**: Topic belongs to a subject
- **Many-to-One with Class**: Topic belongs to a class
- **One-to-Many with Questions**: Each topic can have multiple questions

---

## UI Features (All Modules)

### Common Features Across All Master Data

#### 1. Index/List View
- **Search/Filter**: Quick filtering options
- **Sorting**: Click column headers to sort
- **Status Badges**: Visual indicators for active/inactive
- **Action Buttons**: Edit and Delete buttons
- **Count Information**: Shows related entities count
- **Responsive Table**: Works on mobile and desktop

#### 2. Create View
- **Floating Labels**: Modern Bootstrap 5 form design
- **Validation**: Client and server-side validation
- **Help Text**: Guidance for each field
- **Default Values**: Pre-selected active status
- **Cancel Button**: Return to list without saving

#### 3. Edit View
- **Pre-filled Form**: All current values loaded
- **Validation**: Same as create
- **Warning Color**: Yellow/warning theme
- **Update Button**: Clear action button

#### 4. Delete View
- **Confirmation**: Warning before deletion
- **Relationship Info**: Shows dependent entities
- **Warning Messages**: Alerts for existing relationships
- **Cancel Option**: Safe exit without deleting

### Bootstrap 5 Components Used
- ✅ Cards with shadows
- ✅ Floating labels for forms
- ✅ Color-coded badges
- ✅ Alert messages (success, warning, danger, info)
- ✅ Responsive tables
- ✅ Button groups
- ✅ Form validation
- ✅ Icons (Bootstrap Icons)

---

## Navigation

### From Admin Dashboard

1. **Class Master**: Admin Dashboard → Master Data → Classes
2. **Subject Master**: Admin Dashboard → Master Data → Subjects
3. **Group Master**: Admin Dashboard → Master Data → Groups
4. **Topic Master**: Admin Dashboard → Master Data → Topics

### Direct URLs

- Classes: `https://localhost:5001/Classes`
- Subjects: `https://localhost:5001/Subjects`
- Groups: `https://localhost:5001/Groups`
- Topics: `https://localhost:5001/Topics`

---

## Workflow

### Initial Setup Workflow

1. **Create Classes** (if not auto-seeded)
   - 10th Standard
   - 11th Standard
   - 12th Standard

2. **Create Subjects** (if not auto-seeded)
   - Physics (P)
   - Chemistry (C)
   - Mathematics (M)
   - Biology (B)

3. **Create Groups** (if not auto-seeded)
   - PCMB (assign to 11th)
   - PCB (assign to 11th)
   - PCM (assign to 11th)

4. **Create Topics**
   - For each subject
   - For each class
   - Use proper naming convention

### Adding Custom Data

#### Add a New Class
```
Name: "9th Standard"
Code: "9TH"
Description: "Class 9"
Active: ✓
```

#### Add a New Subject
```
Name: "English"
Code: "ENG"
Description: "English Language"
Active: ✓
```

#### Add a New Group
```
Name: "PCME"
Code: "PCME"
Class: 11th Standard
Description: "Physics, Chemistry, Mathematics, English"
Active: ✓
```

#### Add a New Topic
```
Name: "Thermodynamics"
Code: "P-12-03"
Subject: Physics
Class: 12th Standard
Description: "Heat and temperature"
Active: ✓
```

---

## Data Validation

### Class Master
- Name: Required, max 100 characters
- Code: Optional, max 50 characters
- Description: Optional, max 500 characters

### Subject Master
- Name: Required, max 100 characters
- Code: Optional, max 50 characters
- Description: Optional, max 500 characters

### Group Master
- Name: Required, max 100 characters
- Code: Optional, max 50 characters
- Description: Optional, max 500 characters
- ClassId: Optional (can be null)

### Topic Master
- Name: Required, max 200 characters
- Code: Optional, max 50 characters
- SubjectId: Required
- ClassId: Required
- Description: Optional, max 500 characters

---

## Tips & Best Practices

### 1. Class Master
- ✅ Use standard naming: "10th Standard", "11th Standard"
- ✅ Keep codes short and consistent: "10TH", "11TH"
- ✅ Don't delete classes that have groups assigned

### 2. Subject Master
- ✅ Use single letter codes for PCMB: P, C, M, B
- ✅ Keep subject names consistent across system
- ✅ Check for topics before deleting subjects

### 3. Group Master
- ✅ Always assign groups to classes (11th/12th)
- ✅ Use descriptive names: PCMB, PCB, PCM
- ✅ Check student count before deleting
- ✅ PCMB for comprehensive students
- ✅ PCB for medical stream
- ✅ PCM for engineering stream

### 4. Topic Master
- ✅ Use naming convention: SubjectCode-ClassNo-TopicNo
- ✅ Always select both subject and class
- ✅ Organize topics sequentially
- ✅ Keep topic names clear and concise
- ✅ Check for questions before deleting

---

## Troubleshooting

### Can't Delete Class
**Reason**: Class has groups assigned  
**Solution**: Remove or reassign groups first, then delete

### Can't Delete Subject
**Reason**: Subject has topics or tests  
**Solution**: Remove dependent topics and tests first

### Can't Delete Group
**Reason**: Group has students assigned  
**Solution**: Remove student assignments first, or reassign students

### Can't Delete Topic
**Reason**: Topic has questions  
**Solution**: Remove or reassign questions first

### Validation Errors
**Issue**: Form won't submit  
**Solution**: Check all required fields are filled and within character limits

---

## Sample Data Reference

### Pre-seeded Classes
| Name | Code | Description |
|------|------|-------------|
| 10th Standard | 10TH | Class 10 |
| 11th Standard | 11TH | Class 11 |
| 12th Standard | 12TH | Class 12 |

### Pre-seeded Subjects
| Name | Code | Description |
|------|------|-------------|
| Physics | P | Physics subject |
| Chemistry | C | Chemistry subject |
| Mathematics | M | Mathematics subject |
| Biology | B | Biology subject |

### Pre-seeded Groups
| Name | Code | Class | Description |
|------|------|-------|-------------|
| PCMB | PCMB | 11th | Physics, Chemistry, Mathematics, Biology |
| PCB | PCB | 11th | Physics, Chemistry, Biology |
| PCM | PCM | 11th | Physics, Chemistry, Mathematics |

### Pre-seeded Topics (Sample)
See [INDIAN_EDUCATION_SETUP.md](INDIAN_EDUCATION_SETUP.md) for complete list.

---

## Summary

✅ **4 Master Data Modules** fully implemented  
✅ **Full CRUD** operations on all modules  
✅ **Bootstrap 5 UI** with modern design  
✅ **Automatic seeding** of sample data  
✅ **Relationship tracking** between entities  
✅ **Validation** on all forms  
✅ **Responsive design** for all devices  
✅ **Indian education system** aligned  

---

**Version**: 2.1.0  
**Last Updated**: October 2025  
**Related Docs**: [INDIAN_EDUCATION_SETUP.md](INDIAN_EDUCATION_SETUP.md), [ADMIN_FEATURES.md](ADMIN_FEATURES.md)

