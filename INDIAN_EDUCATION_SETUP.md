# Indian Education System Setup Guide

This guide explains the specific setup for the Indian education system structure (10th/11th/12th with PCMB combinations).

## Overview

The application is now configured for the Indian education system with:
- **Classes**: 10th, 11th, 12th Standards
- **Subjects**: P (Physics), C (Chemistry), M (Mathematics), B (Biology)
- **Student Groups**: PCMB, PCB, PCM combinations
- **Topics**: Subject-wise AND Class-wise organization

## Automatic Data Seeding

The application automatically seeds sample data on first run with:

### Classes
- **10th Standard** (Code: 10TH)
- **11th Standard** (Code: 11TH)
- **12th Standard** (Code: 12TH)

### Subjects
- **Physics** (Code: P)
- **Chemistry** (Code: C)
- **Mathematics** (Code: M)
- **Biology** (Code: B)

### Student Groups
- **PCMB** - Physics, Chemistry, Mathematics, Biology (All subjects)
- **PCB** - Physics, Chemistry, Biology (Medical Stream)
- **PCM** - Physics, Chemistry, Mathematics (Engineering Stream)

### Sample Topics

The system creates sample topics for each subject and class combination:

#### Physics Topics
**11th Standard:**
- Units and Measurements (P-11-01)
- Motion in a Straight Line (P-11-02)
- Laws of Motion (P-11-03)

**12th Standard:**
- Electric Charges and Fields (P-12-01)
- Current Electricity (P-12-02)

#### Chemistry Topics
**11th Standard:**
- Some Basic Concepts of Chemistry (C-11-01)
- Structure of Atom (C-11-02)
- Chemical Bonding (C-11-03)

**12th Standard:**
- Solid State (C-12-01)
- Solutions (C-12-02)

#### Mathematics Topics
**11th Standard:**
- Sets (M-11-01)
- Relations and Functions (M-11-02)
- Trigonometry (M-11-03)

**12th Standard:**
- Relations and Functions (M-12-01)
- Calculus (M-12-02)

#### Biology Topics
**11th Standard:**
- The Living World (B-11-01)
- Biological Classification (B-11-02)
- Cell: The Unit of Life (B-11-03)

**12th Standard:**
- Reproduction in Organisms (B-12-01)
- Genetics and Evolution (B-12-02)

## Topic Organization

Topics are now organized by **BOTH Subject AND Class**:
- Each topic belongs to a specific subject (P/C/M/B)
- Each topic is also assigned to a specific class (10th/11th/12th)
- This allows proper curriculum organization per class level

## Student Group Structure

### PCMB Group
- **Target**: Students taking all four subjects
- **Typical Career Paths**: Medicine, Engineering, Research
- **Subjects**: Physics, Chemistry, Mathematics, Biology

### PCB Group (Medical Stream)
- **Target**: Students pursuing medical/biological sciences
- **Typical Career Paths**: MBBS, BDS, Pharmacy, Nursing, Biotechnology
- **Subjects**: Physics, Chemistry, Biology

### PCM Group (Engineering Stream)
- **Target**: Students pursuing engineering/technical fields
- **Typical Career Paths**: Engineering (B.Tech/B.E.), Architecture, Computer Science
- **Subjects**: Physics, Chemistry, Mathematics

## Setup Workflow

### 1. Initial Setup (Automatic)
When you run the application for the first time:
1. Database is created
2. Identity tables are set up
3. Admin and Student roles are created
4. Default admin user is created
5. **Sample data is automatically seeded**

### 2. Verify Seeded Data
After first run, login as admin and verify:
- Go to **Master Data** → **Classes**: You should see 10th, 11th, 12th
- Go to **Master Data** → **Subjects**: You should see Physics, Chemistry, Mathematics, Biology
- Go to **Master Data** → **Groups**: You should see PCMB, PCB, PCM
- Go to **Master Data** → **Topics**: You should see sample topics for all subjects

### 3. Customize as Needed
You can add more:
- **Classes**: Add more standards if needed
- **Subjects**: Add additional subjects (English, Hindi, etc.)
- **Groups**: Create custom combinations
- **Topics**: Add more topics for each subject-class combination

## Adding New Topics

When creating a topic, you must select:
1. **Subject**: Which subject does this topic belong to? (P/C/M/B)
2. **Class**: Which class/standard is this topic for? (10th/11th/12th)

Example:
- Topic: "Thermodynamics"
- Subject: Physics
- Class: 12th Standard
- Code: P-12-03

## Registering Students

When registering students:
1. Create student account with email and password
2. Assign to appropriate group (PCMB/PCB/PCM)
3. The group determines which subjects the student studies

Example:
- Student: John Doe
- Email: john.doe@example.com
- Group: PCM
- This student will take: Physics, Chemistry, Mathematics

## Creating Tests

When creating tests for Indian education system:
1. Select the subject (P/C/M/B)
2. Select the class (10th/11th/12th)
3. Add questions from topics of that subject and class
4. Allocate to students in appropriate groups

Example:
- Test: "Physics - Laws of Motion Test"
- Subject: Physics
- Class: 11th Standard
- Topics: Laws of Motion (P-11-03)
- Allocate to: Students in PCMB, PCB, or PCM groups

## Question Bank Organization

Organize questions by:
1. **Topic**: Specific topic (e.g., "Laws of Motion")
2. **Subject**: Subject category (Physics/Chemistry/Mathematics/Biology)
3. **Class**: Class level (10th/11th/12th)

This allows you to:
- Filter questions by class and subject
- Build class-specific tests
- Maintain separate question banks for different levels

## Best Practices

### 1. Topic Naming Convention
Use format: `SubjectCode-ClassNumber-TopicNumber`
- Example: P-11-01 (Physics, 11th Standard, Topic 1)

### 2. Group Assignment
- Assign 11th and 12th standard students to groups (PCMB/PCB/PCM)
- 10th standard students can be in general groups

### 3. Test Creation
- Create class-specific tests
- Ensure questions match the class level
- Consider the student group's subject combination

### 4. Student Registration
- Register students with appropriate class groups
- PCMB for students taking all four subjects
- PCB for medical stream
- PCM for engineering stream

## Sample Student Setup

### Medical Stream Student
- Name: Priya Sharma
- Email: priya.sharma@example.com
- Group: PCB (11th)
- Tests: Physics (11th), Chemistry (11th), Biology (11th)

### Engineering Stream Student
- Name: Rahul Kumar
- Email: rahul.kumar@example.com
- Group: PCM (11th)
- Tests: Physics (11th), Chemistry (11th), Mathematics (11th)

### All Subjects Student
- Name: Ananya Reddy
- Email: ananya.reddy@example.com
- Group: PCMB (11th)
- Tests: Physics (11th), Chemistry (11th), Mathematics (11th), Biology (11th)

## Database Schema Changes

### Topic Model (Updated)
```csharp
public class Topic
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Code { get; set; }
    public int SubjectId { get; set; }      // Subject reference (P/C/M/B)
    public int ClassId { get; set; }        // NEW: Class reference (10th/11th/12th)
    public virtual Subject? Subject { get; set; }
    public virtual Class? Class { get; set; }  // NEW: Navigation property
}
```

## Migration Required

After updating the Topic model, you need to create a new migration:

```bash
# Create migration for Topic ClassId
dotnet ef migrations add AddClassIdToTopic

# Update database
dotnet ef database update
```

## Quick Start Commands

```bash
# 1. Restore packages
dotnet restore

# 2. Create migration (if needed)
dotnet ef migrations add InitialCreateWithIndianEducation

# 3. Update database
dotnet ef database update

# 4. Run application
dotnet run
```

## Verification Checklist

After setup, verify:
- [ ] 3 classes created (10th, 11th, 12th)
- [ ] 4 subjects created (P, C, M, B)
- [ ] 3 groups created (PCMB, PCB, PCM)
- [ ] Sample topics created for each subject-class combination
- [ ] Topics show both Subject and Class in listing
- [ ] Can create new topics with both subject and class selection
- [ ] Can register students and assign to groups
- [ ] Can create class-specific tests

## Support

For issues specific to Indian education setup:
1. Check if sample data was seeded correctly
2. Verify classes have correct names (10th/11th/12th)
3. Verify subjects have correct codes (P/C/M/B)
4. Verify groups are assigned to correct classes
5. Check topics have both SubjectId and ClassId

---

**Setup Version**: 2.1.0  
**Education System**: Indian Education System (10th/11th/12th with PCMB)  
**Last Updated**: October 2025

