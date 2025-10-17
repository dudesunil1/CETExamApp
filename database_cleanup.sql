-- Database Cleanup Script for CETExamApp
-- This script will remove existing tests and clean up the database structure

-- Step 1: Delete all existing test-related data to avoid foreign key constraints
DELETE FROM StudentAnswers;
DELETE FROM TestResults;
DELETE FROM TestAttempts;
DELETE FROM TestAllocations;
DELETE FROM TestQuestions;
DELETE FROM Tests;

-- Step 2: Drop foreign key constraints
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tests_Subjects_SubjectId')
    ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Subjects_SubjectId;

IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tests_Groups_GroupId')
    ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Groups_GroupId;

-- Step 2.5: Drop indexes that depend on the columns we want to remove
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Tests') AND name = 'IX_Tests_GroupId')
    DROP INDEX IX_Tests_GroupId ON Tests;

IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Tests') AND name = 'IX_Tests_SubjectId')
    DROP INDEX IX_Tests_SubjectId ON Tests;

-- Step 3: Drop the columns
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Tests') AND name = 'SubjectId')
    ALTER TABLE Tests DROP COLUMN SubjectId;

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Tests') AND name = 'GroupId')
    ALTER TABLE Tests DROP COLUMN GroupId;

-- Step 4: Verify the changes
SELECT 'Database cleanup completed successfully!' as Status;
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Tests' 
ORDER BY ORDINAL_POSITION;
