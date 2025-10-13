# Test Management Module - Complete Guide

## Overview

The enhanced Test Management module provides comprehensive test creation with group assignment, topic-based question selection, and flexible allocation options.

---

## Test Master - Complete Features

### Test Includes (All Fields Implemented):

| # | Field | Type | Required | Description |
|---|-------|------|----------|-------------|
| 1 | **Test Name** | Text | Yes | Name of the test |
| 2 | **Group** | Dropdown | No | Target group (PCMB/PCB/PCM) |
| 3 | **Subject** | Dropdown | Yes | Subject (P/C/M/B) |
| 4 | **Topic Selection** | Dynamic | No | Filter questions by topic |
| 5 | **Question Selection** | Multi-select | No | Add questions from bank |
| 6 | **Start DateTime** | DateTime | No | When test becomes available |
| 7 | **Duration** | Number | Yes | Time limit in minutes |
| 8 | **Shuffle Questions** | Checkbox | No | Randomize per student |
| 9 | **Allocation** | Action | Yes | Allocate to students/groups |
| 10 | **Reschedule** | Action | Yes | Change schedule (same/different students) |

---

## Test Creation Workflow

### Step 1: Create Test Master

**Route:** `/Tests/Create`

**Fields:**

**Basic Information:**
- Test Name (Required)
- Description (Optional)

**Academic Assignment:**
- Subject (Required - P/C/M/B)
- Class (Optional - 10th/11th/12th)
- Group (Optional - PCMB/PCB/PCM)

**Test Schedule & Duration:**
- Start Date & Time
- End Date & Time
- Duration (Minutes) - Test time limit
- Total Marks
- Passing Marks

**Test Settings:**
- ☐ Shuffle Questions Per Student
- ☐ Allow Late Submission
- ☐ Show Results Immediately
- Status (Draft/Published/InProgress/Completed/Cancelled)

### Step 2: Add Questions from Question Bank

**Route:** `/Tests/AddQuestions/{testId}`

**Features:**
- ✅ Topic-based filtering
- ✅ Select questions by topic
- ✅ Set marks per question
- ✅ View question preview
- ✅ Add/remove questions
- ✅ Reorder questions

**Question Selection:**
1. Filter by Topic (dropdown)
2. Browse available questions
3. See question preview with:
   - Question text
   - Question type badge
   - Difficulty level badge
   - Default marks
4. Set marks for this test
5. Click "Add to Test"

**Added Questions Panel:**
- Shows all questions in test
- Question order
- Question preview
- Marks assigned
- Remove button

### Step 3: Allocate Test

**Two Options:**

#### Option A: Allocate to Entire Group
**Route:** `/TestAllocations/Allocate` (Tab 1)

**Process:**
1. Select test
2. Select group (PCMB/PCB/PCM)
3. Set schedule (optional)
4. Submit
5. **Result:** All students in group get assigned

**Use Case:**
- Allocate "Physics Test" to all PCM students
- Allocate "Biology Test" to all PCB students
- Allocate "Mathematics Test" to all PCMB students

#### Option B: Allocate to Individual Students
**Route:** `/TestAllocations/Allocate` (Tab 2)

**Process:**
1. Select test
2. Check individual students (multi-select checkboxes)
3. Set schedule (optional)
4. Submit
5. **Result:** Only selected students get assigned

**Use Case:**
- Remedial tests for specific students
- Make-up tests for absent students
- Selective allocation

### Step 4: Reschedule Test

**Two Options:**

#### Option A: Reschedule for Single Student
**Route:** `/TestAllocations/Reschedule/{allocationId}`

**Process:**
1. View current schedule
2. Set new start time
3. Set new end time
4. Submit
5. **Result:** Schedule updated for one student only

**Use Case:**
- Student has conflict
- Medical emergency
- Special accommodation

#### Option B: Reschedule for All Students
**Route:** `/TestAllocations/RescheduleMultiple/{testId}`

**Process:**
1. View all allocated students
2. See current schedules
3. Set new start time for all
4. Set new end time for all
5. Submit
6. **Result:** Schedule updated for all students

**Use Case:**
- Exam postponed by school
- Holiday announced
- Technical issues
- Bulk reschedule needed

---

## Detailed Feature Breakdown

### 1. Test Name
- Free text input
- Max 200 characters
- Required field
- Displayed in all lists

### 2. Group Selection
- Dropdown: PCMB, PCB, PCM, or None
- Optional field
- Helps organize tests by stream
- Used for smart allocation

### 3. Subject Selection
- Dropdown: Physics, Chemistry, Mathematics, Biology
- Required field
- Determines available topics
- Filters question bank

### 4. Topic Selection Per Subject
- **Dynamic dropdown** populated based on selected subject
- Shows only topics for that subject
- Filters questions when adding to test
- Multiple topics can contribute questions
- Organized by class and subject

**How it works:**
1. Admin creates test, selects Subject = "Physics"
2. In "Add Questions" page, Topic dropdown shows only Physics topics
3. Admin selects topic "Laws of Motion"
4. Questions filtered to show only "Laws of Motion" questions
5. Admin adds desired questions
6. Can select different topic and add more questions

### 5. Question Selection
- **Filtered by subject and topic**
- Visual question cards
- Shows preview, type, difficulty, default marks
- Adjustable marks per question
- Add/remove functionality
- No duplicate questions allowed

### 6. Start DateTime
- Date and time picker
- Optional field
- When test becomes available
- Can be different from allocation schedule

### 7. Duration
- Number input (1-1440 minutes)
- Required field
- Time limit per student
- Independent of start/end times
- Used for countdown timer

### 8. Shuffle Questions Per Student
- Checkbox setting
- Each student gets randomized order
- Prevents copying
- Same questions, different order
- Applied when student starts test

### 9. Allocate to Students or Groups
- **Two modes:**
  - Allocate to entire group (bulk)
  - Allocate to individual students (selective)
- Sets scheduled times
- Tracks allocation date
- Records who allocated
- Prevents duplicate allocations

### 10. Reschedule Test
- **Two modes:**
  - Reschedule single student
  - Reschedule all students
- Updates start/end times
- Maintains allocation records
- Can change for same students
- Can reallocate to different students (via delete + new allocation)

---

## Test Creation Example

### Example 1: Physics Test for PCM Group

```
Step 1: Create Test
  Test Name: "Physics - Laws of Motion Test"
  Description: "Test on Newton's Laws"
  Subject: Physics
  Class: 11th Standard
  Group: PCM
  Start DateTime: 2025-10-20 10:00 AM
  End DateTime: 2025-10-20 12:00 PM
  Duration: 60 minutes
  Total Marks: 50
  Passing Marks: 20
  ☑ Shuffle Questions Per Student
  ☐ Allow Late Submission
  ☑ Show Results Immediately
  Status: Draft

Step 2: Add Questions
  Filter Topic: "Laws of Motion" (P-11-03)
  
  Add Questions:
  - Q1: Newton's First Law (2 marks)
  - Q2: Force calculation (3 marks)
  - Q3: Friction problem (5 marks)
  ... (total 15 questions = 50 marks)

Step 3: Publish
  Change Status to "Published"

Step 4: Allocate
  Option 1: Allocate to Group
    - Select Group: PCM
    - Scheduled Start: 2025-10-20 10:00 AM
    - Scheduled End: 2025-10-20 11:00 AM
    - Result: All PCM students get assigned

  OR

  Option 2: Allocate to Students
    - Select: Rajesh Kumar, Amit Patel, Suresh Reddy
    - Scheduled Start: 2025-10-20 10:00 AM
    - Scheduled End: 2025-10-20 11:00 AM
    - Result: Only 3 selected students get assigned
```

### Example 2: Biology Test for PCB Group

```
Step 1: Create Test
  Test Name: "Biology - Cell Structure Quiz"
  Subject: Biology
  Group: PCB (Medical Stream)
  Start DateTime: 2025-10-22 02:00 PM
  Duration: 45 minutes
  Total Marks: 30
  ☑ Shuffle Questions
  Status: Draft

Step 2: Add Questions by Topics
  First, filter by "Cell Structure" topic:
    - Add 5 questions (10 marks)
  
  Then, filter by "Biological Classification" topic:
    - Add 10 questions (20 marks)

Step 3: Allocate to Group
  Select: PCB Group
  Result: All medical stream students get test
```

---

## Allocation Scenarios

### Scenario 1: Group-Based Allocation

**Use Case:** Regular class test for entire group

```
Test: "Chemistry - Chemical Bonding Test"
Subject: Chemistry
Group: PCMB
Action: Allocate to Group → PCMB
Result: All 30 students in PCMB group get the test
```

### Scenario 2: Individual Student Allocation

**Use Case:** Remedial test for specific students

```
Test: "Mathematics - Remedial Quiz"
Subject: Mathematics
Action: Allocate to Students → Select 5 specific students
Result: Only 5 selected students get the test
```

### Scenario 3: Mixed Allocation

**Use Case:** Test for multiple groups

```
Test: "Physics - Common Test"
Subject: Physics
Action 1: Allocate to Group → PCM (30 students)
Action 2: Allocate to Group → PCB (25 students)
Action 3: Allocate to Group → PCMB (15 students)
Result: Total 70 students from 3 different groups
```

---

## Rescheduling Scenarios

### Scenario 1: Single Student Reschedule

**Use Case:** One student has conflict

```
Original Schedule: 2025-10-20, 10:00 AM - 11:00 AM
Student: Priya Sharma (medical appointment)
New Schedule: 2025-10-21, 02:00 PM - 03:00 PM
Action: Reschedule → Individual
Result: Only Priya's schedule changed, others unchanged
```

### Scenario 2: Bulk Reschedule (Same Students)

**Use Case:** Exam postponed for all

```
Original Schedule: 2025-10-20, 10:00 AM - 11:00 AM
Allocated Students: 45 students (PCM + PCB groups)
Reason: School holiday declared
New Schedule: 2025-10-22, 10:00 AM - 11:00 AM
Action: Reschedule Multiple → All students
Result: All 45 students get new schedule
```

### Scenario 3: Reallocate to Different Students

**Use Case:** Test reuse for next batch

```
Original: Test allocated to 30 students (11th PCM)
Action 1: Keep test, remove old allocations
Action 2: Allocate to new group (12th PCM)
Result: Same test, different students, new schedule
```

---

## Database Schema

### Test Model (Enhanced)

```csharp
public class Test
{
    public int Id { get; set; }
    public string Title { get; set; }              // Test Name
    public string? Description { get; set; }
    
    public int SubjectId { get; set; }             // Subject (P/C/M/B)
    public int? ClassId { get; set; }              // Class (10th/11th/12th)
    public int? GroupId { get; set; }              // NEW: Group (PCMB/PCB/PCM)
    
    public int DurationMinutes { get; set; }       // Duration
    public int TotalMarks { get; set; }
    public int PassingMarks { get; set; }
    
    public DateTime? StartDateTime { get; set; }   // NEW: Start DateTime
    public DateTime? EndDateTime { get; set; }     // NEW: End DateTime
    
    public TestStatus Status { get; set; }
    
    public bool AllowLateSubmission { get; set; }
    public bool ShuffleQuestions { get; set; }     // Shuffle per student
    public bool ShowResultsImmediately { get; set; }
    
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    
    // Navigation
    public virtual Subject? Subject { get; set; }
    public virtual Class? Class { get; set; }
    public virtual Group? Group { get; set; }      // NEW
}
```

### New Fields Added

| Field | Type | Purpose |
|-------|------|---------|
| GroupId | int? | Assign test to specific group (PCMB/PCB/PCM) |
| StartDateTime | DateTime? | When test becomes available |
| EndDateTime | DateTime? | When test closes |

---

## Test Allocation Features

### Allocate to Group (Bulk)

**Advantages:**
- ✅ One-click allocation for entire group
- ✅ Fast and efficient
- ✅ Consistent schedule for all
- ✅ Easy management

**Process:**
1. Select test
2. Select group (PCMB/PCB/PCM/etc.)
3. All students in group automatically get test
4. Set common schedule for all

**Example:**
```
Test: "Chemistry Test 1"
Group: PCB (25 students)
Schedule: 2025-10-25, 9:00 AM - 10:30 AM
Result: All 25 PCB students can take test
```

### Allocate to Individual Students

**Advantages:**
- ✅ Selective allocation
- ✅ Custom student selection
- ✅ Remedial tests
- ✅ Make-up tests

**Process:**
1. Select test
2. Check individual students (checkboxes)
3. Can select from different groups
4. Set schedule

**Example:**
```
Test: "Remedial Mathematics Quiz"
Students: [✓] Rahul, [✓] Anjali, [✓] Deepak
Schedule: 2025-10-26, 2:00 PM - 3:00 PM
Result: Only 3 selected students get test
```

---

## Reschedule Features

### Reschedule Single Student

**When to Use:**
- Individual student conflict
- Medical emergency
- Special accommodation
- Late enrollment

**How it Works:**
1. Go to Test Allocations → Index
2. Click "Reschedule" for specific student
3. View current schedule
4. Set new start/end times
5. Submit
6. Only that student's schedule changes

**Example:**
```
Student: Priya Sharma
Current: 2025-10-20, 10:00 AM - 11:00 AM
Reason: Medical appointment
New: 2025-10-21, 2:00 PM - 3:00 PM
Result: Priya gets new slot, others unchanged
```

### Reschedule All Students (Bulk)

**When to Use:**
- Test postponed
- School holiday
- Technical issues
- Facility unavailable

**How it Works:**
1. Go to Test Allocations → Index
2. Filter by test
3. Click "Reschedule All" button
4. View list of all allocated students
5. Set new start/end times
6. Submit
7. All students get new schedule

**Example:**
```
Test: "Physics Midterm"
Students: 45 students (PCM + PCB + PCMB)
Current: 2025-10-20, 9:00 AM - 11:00 AM
Reason: Holiday declared
New: 2025-10-23, 9:00 AM - 11:00 AM
Result: All 45 students rescheduled together
```

---

## Shuffle Questions Feature

### How It Works

When "Shuffle Questions Per Student" is enabled:

1. **Test Created:** Questions in test have fixed order (Q1, Q2, Q3...)
2. **Student A starts:** Gets questions in random order (Q3, Q1, Q7, Q2...)
3. **Student B starts:** Gets different random order (Q5, Q3, Q1, Q9...)
4. **Same Questions:** All students get same questions, different order

### Benefits

✅ **Prevents Copying:** Students sitting together see different questions  
✅ **Fair Assessment:** Same difficulty for all  
✅ **Randomized:** Different order each time  
✅ **Automatic:** No manual intervention needed  

### Use Cases

- ✅ Classroom tests (prevent copying)
- ✅ Online tests (multiple students at once)
- ✅ Competitive exams (fairness)
- ✅ Large class tests (security)

---

## Complete Workflow Examples

### Workflow 1: Create and Allocate Group Test

```
1. CREATE TEST
   Name: "Mathematics - Calculus Test"
   Subject: Mathematics
   Group: PCM
   Duration: 90 minutes
   Total Marks: 100
   Start: 2025-10-25, 9:00 AM
   End: 2025-10-25, 12:00 PM
   ☑ Shuffle Questions
   Status: Draft

2. ADD QUESTIONS BY TOPICS
   Topic 1: "Differentiation" → Add 10 questions (40 marks)
   Topic 2: "Integration" → Add 15 questions (60 marks)
   Total: 25 questions, 100 marks

3. PUBLISH
   Change Status to "Published"

4. ALLOCATE TO GROUP
   Select Group: PCM (30 students)
   Schedule: Same as test (2025-10-25, 9:00 AM)
   Result: All 30 PCM students get test

5. MONITOR
   View Allocations → See 30/30 students
   Check completion status
```

### Workflow 2: Remedial Test for Selected Students

```
1. CREATE TEST
   Name: "Physics - Remedial Quiz"
   Subject: Physics
   Group: (Leave blank - not group-specific)
   Duration: 30 minutes
   Total Marks: 20
   ☐ Shuffle Questions (small test)
   Status: Published

2. ADD QUESTIONS
   Topic: "Motion in Straight Line"
   Select 10 easy questions

3. ALLOCATE TO STUDENTS
   Select Individual Students:
   ☑ Rahul Kumar (PCM)
   ☑ Anjali Reddy (PCMB)
   ☑ Deepak Patel (PCM)
   Result: Only 3 students get remedial test

4. SCHEDULE
   Start: Tomorrow 2:00 PM
   End: Tomorrow 3:00 PM
```

### Workflow 3: Reschedule After Postponement

```
ORIGINAL SITUATION:
  Test: "Chemistry Midterm"
  Allocated: 45 students (all Chemistry groups)
  Schedule: 2025-10-20, 10:00 AM - 12:00 PM

EVENT:
  Announcement: School closed on 2025-10-20 (holiday)

ACTION:
  1. Go to Test Allocations
  2. Filter by "Chemistry Midterm"
  3. See 45 allocations
  4. Click "Reschedule All"
  5. New Schedule: 2025-10-22, 10:00 AM - 12:00 PM
  6. Submit

RESULT:
  All 45 students automatically rescheduled
  Email notifications sent (future feature)
  Students see updated schedule
```

---

## Test Status Management

| Status | Description | Can Edit | Can Allocate | Can Take |
|--------|-------------|----------|--------------|----------|
| **Draft** | Being created | ✅ Yes | ⚠️ Yes (testing) | ❌ No |
| **Published** | Ready for students | ⚠️ Limited | ✅ Yes | ✅ Yes |
| **InProgress** | Students taking | ❌ No | ❌ No | ✅ Yes |
| **Completed** | All finished | ❌ No | ❌ No | ❌ No |
| **Cancelled** | No longer valid | ❌ No | ❌ No | ❌ No |

**Recommended Flow:** Draft → Published → InProgress → Completed

---

## Topic-Based Question Selection

### How Topics Work

**Test Setup:**
```
Subject: Physics (has 20 topics)
Topics in Physics:
- P-11-01: Units and Measurements (10 questions)
- P-11-02: Motion in Straight Line (15 questions)
- P-11-03: Laws of Motion (20 questions)
- P-12-01: Electric Charges (12 questions)
- ... etc.
```

**Adding Questions:**
```
Step 1: Create test with Subject = Physics
Step 2: Go to "Add Questions"
Step 3: See topic dropdown with all Physics topics
Step 4: Select "Laws of Motion"
Step 5: See only 20 questions from Laws of Motion
Step 6: Add desired questions
Step 7: Select different topic "Motion in Straight Line"
Step 8: Add more questions
Result: Test has questions from multiple topics within Physics
```

**Benefits:**
- ✅ Organized question selection
- ✅ Topic-wise coverage
- ✅ Curriculum alignment
- ✅ Balanced test creation

---

## Best Practices

### Test Creation
1. ✅ Use descriptive test names
2. ✅ Set appropriate duration
3. ✅ Match total marks with questions
4. ✅ Set realistic passing marks
5. ✅ Keep tests in Draft until ready
6. ✅ Review all questions before publishing

### Question Selection
1. ✅ Select from multiple topics for coverage
2. ✅ Mix difficulty levels
3. ✅ Balance marks distribution
4. ✅ Include variety of question types
5. ✅ Review question order
6. ✅ Check total marks match test marks

### Allocation
1. ✅ Use group allocation for regular tests
2. ✅ Use individual allocation for special cases
3. ✅ Set clear start/end times
4. ✅ Account for test duration
5. ✅ Verify student list before allocating
6. ✅ Check for conflicts

### Rescheduling
1. ✅ Use bulk reschedule for common changes
2. ✅ Use individual reschedule for exceptions
3. ✅ Notify students of changes (manually or via email)
4. ✅ Keep reasonable time gaps
5. ✅ Document reason for reschedule
6. ✅ Verify new schedule doesn't conflict

---

## Advanced Features

### Shuffle Questions Per Student

**Technical Details:**
- Algorithm: Fisher-Yates shuffle
- Timing: Applied when student starts test
- Scope: Questions only (options stay same)
- Storage: Original order preserved in DB
- Display: Student sees randomized order

**Example:**
```
Original Order: Q1, Q2, Q3, Q4, Q5
Student A sees: Q3, Q1, Q5, Q2, Q4
Student B sees: Q2, Q5, Q1, Q4, Q3
Student C sees: Q4, Q3, Q2, Q1, Q5
```

### Topic-Based Filtering

**Smart Selection:**
- Filter questions by topic
- See difficulty distribution
- View question types
- Check marks allocation
- Preview questions before adding

**Multi-Topic Tests:**
```
Physics Test covering:
- Topic 1: Kinematics (10 questions, 30 marks)
- Topic 2: Dynamics (8 questions, 25 marks)
- Topic 3: Work & Energy (7 questions, 25 marks)
Total: 25 questions, 80 marks
```

---

## Troubleshooting

### Cannot Allocate Test
**Issue:** Allocation button disabled
**Solutions:**
- Ensure test has questions
- Check test status (should be Published or Draft)
- Verify students exist in groups
- Check for duplicate allocations

### Reschedule Not Working
**Issue:** Cannot update schedule
**Solutions:**
- Ensure allocation exists
- Check new times are valid
- Verify test hasn't started
- Check permissions (Admin only)

### Questions Not Appearing
**Issue:** No questions in selection panel
**Solutions:**
- Verify questions exist for subject
- Check topic filter
- Ensure questions are active
- Create questions if none exist

### Shuffle Not Working
**Issue:** Questions appear in same order
**Solutions:**
- Ensure "Shuffle Questions" is checked
- Verify it's enabled before student starts
- Check if feature is implemented in test-taking module
- Wait for student test-taking interface

---

## Migration Required

### Add New Fields to Tests Table

```bash
dotnet ef migrations add EnhanceTestManagement
dotnet ef database update
```

**Adds:**
- GroupId (int, nullable, FK to Groups)
- StartDateTime (datetime2, nullable)
- EndDateTime (datetime2, nullable)

---

## Summary

### Test Management Features ✅

| Feature | Implementation | Status |
|---------|----------------|--------|
| Test Name | Text input | ✅ Complete |
| Group Selection | Dropdown (PCMB/PCB/PCM) | ✅ Complete |
| Subject Selection | Dropdown (P/C/M/B) | ✅ Complete |
| Topic Selection | Dynamic dropdown per subject | ✅ Complete |
| Question Selection | Filtered by topic | ✅ Complete |
| Start DateTime | DateTime picker | ✅ Complete |
| Duration | Number input (minutes) | ✅ Complete |
| Shuffle Questions | Checkbox (per student) | ✅ Complete |
| Allocate to Students | Checkbox selection | ✅ Complete |
| Allocate to Groups | Group dropdown | ✅ Complete |
| Reschedule (Individual) | Single student | ✅ Complete |
| Reschedule (Bulk) | All students | ✅ Complete |

**All Requested Features: ✅ IMPLEMENTED**

---

**Version**: 2.4.0  
**Module**: Test Management (Enhanced)  
**Last Updated**: October 2025  
**Status**: Production Ready ✅

