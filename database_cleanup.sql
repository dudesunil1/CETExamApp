-- Database Cleanup Script for CETExamApp
-- This script will remove existing tests and clean up the database structure

-- Step 1: Delete all existing test-related data to avoid foreign key constraints
DELETE FROM StudentAnswers;
DELETE FROM TestResults;
DELETE FROM TestAttempts;
DELETE FROM TestAllocations;
DELETE FROM TestQuestions;
DELETE FROM Tests;

-- Step 2: Drop foreign key constraints (with error handling)
-- Check if constraints exist before dropping them
DECLARE @sql NVARCHAR(MAX) = '';

-- Drop FK_Tests_Subjects_SubjectId if it exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tests_Subjects_SubjectId')
BEGIN
    SET @sql = 'ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Subjects_SubjectId';
    EXEC sp_executesql @sql;
    PRINT 'Dropped FK_Tests_Subjects_SubjectId';
END
ELSE
BEGIN
    PRINT 'FK_Tests_Subjects_SubjectId does not exist, skipping...';
END

-- Drop FK_Tests_Groups_GroupId if it exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Tests_Groups_GroupId')
BEGIN
    SET @sql = 'ALTER TABLE Tests DROP CONSTRAINT FK_Tests_Groups_GroupId';
    EXEC sp_executesql @sql;
    PRINT 'Dropped FK_Tests_Groups_GroupId';
END
ELSE
BEGIN
    PRINT 'FK_Tests_Groups_GroupId does not exist, skipping...';
END

-- Step 2.5: Drop indexes that depend on the columns we want to remove
-- Drop IX_Tests_GroupId if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Tests') AND name = 'IX_Tests_GroupId')
BEGIN
    DROP INDEX IX_Tests_GroupId ON Tests;
    PRINT 'Dropped IX_Tests_GroupId';
END
ELSE
BEGIN
    PRINT 'IX_Tests_GroupId does not exist, skipping...';
END

-- Drop IX_Tests_SubjectId if it exists
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('Tests') AND name = 'IX_Tests_SubjectId')
BEGIN
    DROP INDEX IX_Tests_SubjectId ON Tests;
    PRINT 'Dropped IX_Tests_SubjectId';
END
ELSE
BEGIN
    PRINT 'IX_Tests_SubjectId does not exist, skipping...';
END

-- Step 3: Drop the columns
-- Drop GroupId column if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Tests') AND name = 'GroupId')
BEGIN
    ALTER TABLE Tests DROP COLUMN GroupId;
    PRINT 'Dropped GroupId column';
END
ELSE
BEGIN
    PRINT 'GroupId column does not exist, skipping...';
END

-- Drop SubjectId column if it exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Tests') AND name = 'SubjectId')
BEGIN
    ALTER TABLE Tests DROP COLUMN SubjectId;
    PRINT 'Dropped SubjectId column';
END
ELSE
BEGIN
    PRINT 'SubjectId column does not exist, skipping...';
END

-- Step 4: Verify the changes
SELECT 'Database cleanup completed successfully!' as Status;
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'Tests' 
ORDER BY ORDINAL_POSITION;
