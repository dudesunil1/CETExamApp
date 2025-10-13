using CETExamApp.Data;
using CETExamApp.Models;
using CETExamApp.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CETExamApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class ResultsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<IActionResult> Index(int? testId, string? studentId)
        {
            var resultsQuery = _context.TestResults
                .Include(tr => tr.Test)
                .ThenInclude(t => t!.Subject)
                .Include(tr => tr.Student)
                .AsQueryable();

            if (testId.HasValue)
                resultsQuery = resultsQuery.Where(tr => tr.TestId == testId.Value);

            if (!string.IsNullOrEmpty(studentId))
                resultsQuery = resultsQuery.Where(tr => tr.StudentId == studentId);

            var results = await resultsQuery
                .OrderByDescending(tr => tr.SubmittedAt)
                .ToListAsync();

            ViewData["TestId"] = new SelectList(await _context.Tests.Include(t => t.Subject).ToListAsync(), "Id", "Title", testId);
            
            var students = await _context.Users.Where(u => u.GroupId != null).ToListAsync();
            ViewData["StudentId"] = new SelectList(students, "Id", "Email", studentId);

            return View(results);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var result = await _context.TestResults
                .Include(tr => tr.Test)
                .ThenInclude(t => t!.Subject)
                .Include(tr => tr.Student)
                .Include(tr => tr.StudentAnswers)
                .ThenInclude(sa => sa.Question)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (result == null) return NotFound();

            return View(result);
        }

        public async Task<IActionResult> TestReport(int? id)
        {
            if (id == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.TestResults)
                .ThenInclude(tr => tr.Student)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null) return NotFound();

            var stats = new
            {
                TotalAllocated = await _context.TestAllocations.CountAsync(ta => ta.TestId == id),
                TotalCompleted = test.TestResults?.Count ?? 0,
                AverageScore = test.TestResults?.Any() == true ? test.TestResults.Average(tr => tr.Percentage) : 0,
                PassCount = test.TestResults?.Count(tr => tr.IsPassed) ?? 0,
                FailCount = test.TestResults?.Count(tr => !tr.IsPassed) ?? 0,
                HighestScore = test.TestResults?.Any() == true ? test.TestResults.Max(tr => tr.ObtainedMarks) : 0,
                LowestScore = test.TestResults?.Any() == true ? test.TestResults.Min(tr => tr.ObtainedMarks) : 0
            };

            ViewBag.Stats = stats;
            return View(test);
        }

        public async Task<IActionResult> StudentReport(string? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Users.FindAsync(id);
            if (student == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.Test)
                .ThenInclude(t => t!.Subject)
                .Where(tr => tr.StudentId == id)
                .OrderByDescending(tr => tr.SubmittedAt)
                .ToListAsync();

            var stats = new
            {
                TotalTests = results.Count,
                TotalPassed = results.Count(r => r.IsPassed),
                TotalFailed = results.Count(r => !r.IsPassed),
                AveragePercentage = results.Any() ? results.Average(r => r.Percentage) : 0,
                HighestPercentage = results.Any() ? results.Max(r => r.Percentage) : 0,
                LowestPercentage = results.Any() ? results.Min(r => r.Percentage) : 0
            };

            ViewBag.Student = student;
            ViewBag.Stats = stats;
            return View(results);
        }

        // Student-wise Test Result (Detailed)
        public async Task<IActionResult> StudentWiseResult(string? studentId)
        {
            if (studentId == null) return NotFound();

            var student = await _context.Users
                .Include(u => u.Class)
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == studentId);
            
            if (student == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.Test)
                    .ThenInclude(t => t!.Subject)
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q!.Topic)
                .Where(tr => tr.StudentId == studentId)
                .OrderByDescending(tr => tr.SubmittedAt)
                .ToListAsync();

            var viewModel = new StudentResultViewModel
            {
                Student = student,
                TestResults = results,
                TotalTestsTaken = results.Count,
                TestsPassed = results.Count(r => r.IsPassed),
                TestsFailed = results.Count(r => !r.IsPassed),
                AveragePercentage = results.Any() ? results.Average(r => r.Percentage) : 0,
                TotalMarksObtained = results.Sum(r => r.ObtainedMarks),
                TotalMaxMarks = results.Sum(r => r.TotalMarks)
            };

            // Calculate topic-wise performance
            var topicPerformance = new Dictionary<string, TopicPerformance>();
            foreach (var result in results)
            {
                foreach (var answer in result.StudentAnswers ?? new List<StudentAnswer>())
                {
                    var topicName = answer.Question?.Topic?.Name ?? "Unknown";
                    if (!topicPerformance.ContainsKey(topicName))
                    {
                        topicPerformance[topicName] = new TopicPerformance { TopicName = topicName };
                    }

                    topicPerformance[topicName].QuestionsAttempted++;
                    topicPerformance[topicName].TotalMarks += answer.Question?.Marks ?? 0;
                    topicPerformance[topicName].MarksObtained += answer.MarksObtained;
                    
                    if (answer.IsCorrect)
                        topicPerformance[topicName].CorrectAnswers++;
                    else
                        topicPerformance[topicName].WrongAnswers++;
                }
            }

            foreach (var topic in topicPerformance.Values)
            {
                topic.Percentage = topic.TotalMarks > 0 
                    ? (decimal)topic.MarksObtained / topic.TotalMarks * 100 
                    : 0;
            }

            viewModel.TopicWisePerformance = topicPerformance;

            // Calculate subject-wise performance
            var subjectPerformance = new Dictionary<string, SubjectPerformance>();
            foreach (var result in results)
            {
                var subjectName = result.Test?.Subject?.Name ?? "Unknown";
                if (!subjectPerformance.ContainsKey(subjectName))
                {
                    subjectPerformance[subjectName] = new SubjectPerformance { SubjectName = subjectName };
                }

                subjectPerformance[subjectName].TestsCount++;
                subjectPerformance[subjectName].TotalMarksObtained += result.ObtainedMarks;
                subjectPerformance[subjectName].TotalMaxMarks += result.TotalMarks;
            }

            foreach (var subject in subjectPerformance.Values)
            {
                subject.AveragePercentage = subject.TotalMaxMarks > 0
                    ? (decimal)subject.TotalMarksObtained / subject.TotalMaxMarks * 100
                    : 0;
            }

            viewModel.SubjectWisePerformance = subjectPerformance;

            return View(viewModel);
        }

        // Topic-wise Performance Analysis
        public async Task<IActionResult> TopicWisePerformance(int? testId)
        {
            if (testId == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.TestQuestions)
                    .ThenInclude(tq => tq.Question)
                        .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q!.Topic)
                .Where(tr => tr.TestId == testId)
                .ToListAsync();

            var topicAnalysis = new Dictionary<string, TopicPerformance>();

            foreach (var result in results)
            {
                foreach (var answer in result.StudentAnswers ?? new List<StudentAnswer>())
                {
                    var topicName = answer.Question?.Topic?.Name ?? "Unknown";
                    if (!topicAnalysis.ContainsKey(topicName))
                    {
                        topicAnalysis[topicName] = new TopicPerformance { TopicName = topicName };
                    }

                    topicAnalysis[topicName].QuestionsAttempted++;
                    topicAnalysis[topicName].TotalMarks += answer.Question?.Marks ?? 0;
                    topicAnalysis[topicName].MarksObtained += answer.MarksObtained;
                    
                    if (answer.IsCorrect)
                        topicAnalysis[topicName].CorrectAnswers++;
                    else
                        topicAnalysis[topicName].WrongAnswers++;
                }
            }

            foreach (var topic in topicAnalysis.Values)
            {
                topic.Percentage = topic.TotalMarks > 0
                    ? (decimal)topic.MarksObtained / topic.TotalMarks * 100
                    : 0;
            }

            ViewBag.Test = test;
            ViewBag.TotalStudents = results.Count;
            return View(topicAnalysis);
        }

        // Test-wise Summary Report (Enhanced)
        public async Task<IActionResult> TestWiseSummary(int? testId)
        {
            if (testId == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .Include(t => t.Class)
                .Include(t => t.Group)
                .Include(t => t.TestQuestions)
                    .ThenInclude(tq => tq.Question)
                        .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) return NotFound();

            var allocations = await _context.TestAllocations.CountAsync(ta => ta.TestId == testId);
            
            var results = await _context.TestResults
                .Include(tr => tr.Student)
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q!.Topic)
                .Where(tr => tr.TestId == testId)
                .ToListAsync();

            var viewModel = new TestWiseSummaryViewModel
            {
                Test = test,
                TotalAllocated = allocations,
                TotalCompleted = results.Count,
                TotalPending = allocations - results.Count,
                PassCount = results.Count(r => r.IsPassed),
                FailCount = results.Count(r => !r.IsPassed),
                AverageScore = results.Any() ? (decimal)results.Average(r => r.ObtainedMarks) : 0,
                AveragePercentage = results.Any() ? results.Average(r => r.Percentage) : 0,
                HighestScore = results.Any() ? results.Max(r => r.ObtainedMarks) : 0,
                LowestScore = results.Any() ? results.Min(r => r.ObtainedMarks) : 0,
                Results = results
            };

            // Topic-wise analysis for the test
            var topicAnalysis = new Dictionary<string, TopicPerformance>();
            foreach (var result in results)
            {
                foreach (var answer in result.StudentAnswers ?? new List<StudentAnswer>())
                {
                    var topicName = answer.Question?.Topic?.Name ?? "Unknown";
                    if (!topicAnalysis.ContainsKey(topicName))
                    {
                        topicAnalysis[topicName] = new TopicPerformance { TopicName = topicName };
                    }

                    topicAnalysis[topicName].QuestionsAttempted++;
                    topicAnalysis[topicName].TotalMarks += answer.Question?.Marks ?? 0;
                    topicAnalysis[topicName].MarksObtained += answer.MarksObtained;
                    
                    if (answer.IsCorrect)
                        topicAnalysis[topicName].CorrectAnswers++;
                    else
                        topicAnalysis[topicName].WrongAnswers++;
                }
            }

            foreach (var topic in topicAnalysis.Values)
            {
                topic.Percentage = topic.TotalMarks > 0
                    ? (decimal)topic.MarksObtained / topic.TotalMarks * 100
                    : 0;
            }

            viewModel.TopicWiseAnalysis = topicAnalysis;

            return View(viewModel);
        }

        // Detailed Answer Key for Student
        public async Task<IActionResult> DetailedAnswerKey(int? resultId)
        {
            if (resultId == null) return NotFound();

            var testResult = await _context.TestResults
                .Include(tr => tr.Test)
                    .ThenInclude(t => t!.Subject)
                .Include(tr => tr.Student)
                    .ThenInclude(s => s!.Class)
                .Include(tr => tr.Student)
                    .ThenInclude(s => s!.Group)
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                        .ThenInclude(q => q!.Topic)
                .FirstOrDefaultAsync(tr => tr.Id == resultId);

            if (testResult == null) return NotFound();

            var testQuestions = await _context.TestQuestions
                .Include(tq => tq.Question)
                .Where(tq => tq.TestId == testResult.TestId)
                .OrderBy(tq => tq.QuestionOrder)
                .ToListAsync();

            var answerDetails = new List<StudentAnswerDetail>();
            int questionNumber = 1;

            foreach (var tq in testQuestions)
            {
                var studentAnswer = testResult.StudentAnswers?
                    .FirstOrDefault(sa => sa.QuestionId == tq.QuestionId);

                answerDetails.Add(new StudentAnswerDetail
                {
                    QuestionNumber = questionNumber++,
                    Question = tq.Question,
                    Answer = studentAnswer,
                    IsCorrect = studentAnswer?.IsCorrect ?? false,
                    MarksObtained = studentAnswer?.MarksObtained ?? 0,
                    MaxMarks = tq.Marks
                });
            }

            var viewModel = new DetailedAnswerKeyViewModel
            {
                TestResult = testResult,
                Test = testResult.Test,
                Student = testResult.Student,
                Answers = answerDetails
            };

            return View(viewModel);
        }

        // Result Card for All Tests
        public async Task<IActionResult> ResultCard(string? studentId)
        {
            if (studentId == null) return NotFound();

            var student = await _context.Users
                .Include(u => u.Class)
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == studentId);
            
            if (student == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.Test)
                    .ThenInclude(t => t!.Subject)
                .Where(tr => tr.StudentId == studentId)
                .OrderBy(tr => tr.SubmittedAt)
                .ToListAsync();

            var viewModel = new StudentResultViewModel
            {
                Student = student,
                TestResults = results,
                TotalTestsTaken = results.Count,
                TestsPassed = results.Count(r => r.IsPassed),
                TestsFailed = results.Count(r => !r.IsPassed),
                AveragePercentage = results.Any() ? results.Average(r => r.Percentage) : 0,
                TotalMarksObtained = results.Sum(r => r.ObtainedMarks),
                TotalMaxMarks = results.Sum(r => r.TotalMarks)
            };

            // Subject-wise performance
            var subjectPerformance = new Dictionary<string, SubjectPerformance>();
            foreach (var result in results)
            {
                var subjectName = result.Test?.Subject?.Name ?? "Unknown";
                if (!subjectPerformance.ContainsKey(subjectName))
                {
                    subjectPerformance[subjectName] = new SubjectPerformance { SubjectName = subjectName };
                }

                subjectPerformance[subjectName].TestsCount++;
                subjectPerformance[subjectName].TotalMarksObtained += result.ObtainedMarks;
                subjectPerformance[subjectName].TotalMaxMarks += result.TotalMarks;
            }

            foreach (var subject in subjectPerformance.Values)
            {
                subject.AveragePercentage = subject.TotalMaxMarks > 0
                    ? (decimal)subject.TotalMarksObtained / subject.TotalMaxMarks * 100
                    : 0;
            }

            viewModel.SubjectWisePerformance = subjectPerformance;

            return View(viewModel);
        }

        // Export Student Result to PDF
        public async Task<IActionResult> ExportStudentResultPDF(string? studentId)
        {
            if (studentId == null) return NotFound();

            var student = await _context.Users
                .Include(u => u.Class)
                .Include(u => u.Group)
                .FirstOrDefaultAsync(u => u.Id == studentId);
            
            if (student == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.Test)
                    .ThenInclude(t => t!.Subject)
                .Where(tr => tr.StudentId == studentId)
                .OrderBy(tr => tr.SubmittedAt)
                .ToListAsync();

            var pdfBytes = GenerateStudentResultPDF(student, results);
            
            return File(pdfBytes, "application/pdf", $"ResultCard_{student.FirstName}_{student.LastName}.pdf");
        }

        // Export Test Results to Excel
        public async Task<IActionResult> ExportTestResultsExcel(int? testId)
        {
            if (testId == null) return NotFound();

            var test = await _context.Tests
                .Include(t => t.Subject)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null) return NotFound();

            var results = await _context.TestResults
                .Include(tr => tr.Student)
                .Where(tr => tr.TestId == testId)
                .OrderByDescending(tr => tr.Percentage)
                .ToListAsync();

            var excelBytes = GenerateTestResultsExcel(test, results);
            
            return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"TestResults_{test.Title}.xlsx");
        }

        // Export Answer Key to PDF
        public async Task<IActionResult> ExportAnswerKeyPDF(int? resultId)
        {
            if (resultId == null) return NotFound();

            var testResult = await _context.TestResults
                .Include(tr => tr.Test)
                .Include(tr => tr.Student)
                .Include(tr => tr.StudentAnswers)
                    .ThenInclude(sa => sa.Question)
                .FirstOrDefaultAsync(tr => tr.Id == resultId);

            if (testResult == null) return NotFound();

            var testQuestions = await _context.TestQuestions
                .Include(tq => tq.Question)
                    .ThenInclude(q => q!.Topic)
                .Where(tq => tq.TestId == testResult.TestId)
                .OrderBy(tq => tq.QuestionOrder)
                .ToListAsync();

            var pdfBytes = GenerateAnswerKeyPDF(testResult, testQuestions);
            
            return File(pdfBytes, "application/pdf", 
                $"AnswerKey_{testResult.Student?.FirstName}_{testResult.Test?.Title}.pdf");
        }

        // Helper: Generate Student Result PDF
        private byte[] GenerateStudentResultPDF(ApplicationUser student, List<TestResult> results)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header()
                        .Text("STUDENT RESULT CARD")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            // Student Info
                            x.Item().Text($"Name: {student.FirstName} {student.LastName}").FontSize(14).SemiBold();
                            x.Item().Text($"Class: {student.Class?.Name ?? "N/A"}");
                            x.Item().Text($"Group: {student.Group?.Name ?? "N/A"}");
                            x.Item().Text($"Email: {student.Email}");
                            x.Item().PaddingTop(0.5f, Unit.Centimetre).LineHorizontal(1);

                            // Summary
                            x.Item().PaddingTop(0.5f, Unit.Centimetre).Text("OVERALL PERFORMANCE").FontSize(14).SemiBold();
                            x.Item().Text($"Total Tests Taken: {results.Count}");
                            x.Item().Text($"Tests Passed: {results.Count(r => r.IsPassed)}");
                            x.Item().Text($"Tests Failed: {results.Count(r => !r.IsPassed)}");
                            x.Item().Text($"Average Percentage: {(results.Any() ? results.Average(r => r.Percentage).ToString("0.00") : "0.00")}%");
                            
                            x.Item().PaddingTop(0.5f, Unit.Centimetre).LineHorizontal(1);

                            // Test Results Table
                            x.Item().PaddingTop(0.5f, Unit.Centimetre).Text("TEST RESULTS").FontSize(14).SemiBold();
                            
                            x.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Element(CellStyle).Text("Test Name");
                                    header.Cell().Element(CellStyle).Text("Subject");
                                    header.Cell().Element(CellStyle).Text("Obtained");
                                    header.Cell().Element(CellStyle).Text("Total");
                                    header.Cell().Element(CellStyle).Text("%");
                                    header.Cell().Element(CellStyle).Text("Result");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.Background(Colors.Grey.Lighten2).Padding(5);
                                    }
                                });

                                foreach (var result in results)
                                {
                                    table.Cell().Element(CellStyle).Text(result.Test?.Title ?? "");
                                    table.Cell().Element(CellStyle).Text(result.Test?.Subject?.Name ?? "");
                                    table.Cell().Element(CellStyle).Text(result.ObtainedMarks.ToString());
                                    table.Cell().Element(CellStyle).Text(result.TotalMarks.ToString());
                                    table.Cell().Element(CellStyle).Text(result.Percentage.ToString("0.00"));
                                    table.Cell().Element(CellStyle).Text(result.IsPassed ? "PASS" : "FAIL");

                                    static IContainer CellStyle(IContainer container)
                                    {
                                        return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Padding(5);
                                    }
                                }
                            });
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Generated on: ");
                            x.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                        });
                });
            });

            return document.GeneratePdf();
        }

        // Helper: Generate Answer Key PDF
        private byte[] GenerateAnswerKeyPDF(TestResult testResult, List<TestQuestion> testQuestions)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header()
                        .Column(column =>
                        {
                            column.Item().Text("DETAILED ANSWER KEY").SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);
                            column.Item().Text($"Test: {testResult.Test?.Title}").FontSize(12);
                            column.Item().Text($"Student: {testResult.Student?.FirstName} {testResult.Student?.LastName}");
                        });

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            int qNum = 1;
                            foreach (var tq in testQuestions)
                            {
                                var studentAnswer = testResult.StudentAnswers?
                                    .FirstOrDefault(sa => sa.QuestionId == tq.QuestionId);

                                x.Item().PaddingTop(0.3f, Unit.Centimetre).Text($"Q{qNum++}. {tq.Question?.QuestionText}");
                                x.Item().Text($"Correct Answer: {tq.Question?.CorrectAnswer}").FontColor(Colors.Green.Medium);
                                x.Item().Text($"Student Answer: {studentAnswer?.AnswerText ?? "Not Answered"}")
                                    .FontColor(studentAnswer?.IsCorrect == true ? Colors.Green.Medium : Colors.Red.Medium);
                                x.Item().Text($"Marks: {studentAnswer?.MarksObtained ?? 0} / {tq.Marks}");
                                x.Item().LineHorizontal(0.5f);
                            }

                            x.Item().PaddingTop(1, Unit.Centimetre).Text($"Total Score: {testResult.ObtainedMarks} / {testResult.TotalMarks}").FontSize(14).SemiBold();
                            x.Item().Text($"Percentage: {testResult.Percentage:0.00}%").FontSize(14).SemiBold();
                            x.Item().Text($"Result: {(testResult.IsPassed ? "PASS" : "FAIL")}")
                                .FontSize(14).SemiBold()
                                .FontColor(testResult.IsPassed ? Colors.Green.Medium : Colors.Red.Medium);
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            });

            return document.GeneratePdf();
        }

        // Helper: Generate Excel for Test Results
        private byte[] GenerateTestResultsExcel(Test test, List<TestResult> results)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Test Results");

            // Header
            worksheet.Cell(1, 1).Value = "Test Results Report";
            worksheet.Cell(1, 1).Style.Font.Bold = true;
            worksheet.Cell(1, 1).Style.Font.FontSize = 16;
            
            worksheet.Cell(2, 1).Value = $"Test: {test.Title}";
            worksheet.Cell(3, 1).Value = $"Subject: {test.Subject?.Name}";
            worksheet.Cell(4, 1).Value = $"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}";

            // Column headers
            int currentRow = 6;
            worksheet.Cell(currentRow, 1).Value = "Student Name";
            worksheet.Cell(currentRow, 2).Value = "Email";
            worksheet.Cell(currentRow, 3).Value = "Group";
            worksheet.Cell(currentRow, 4).Value = "Submitted At";
            worksheet.Cell(currentRow, 5).Value = "Obtained Marks";
            worksheet.Cell(currentRow, 6).Value = "Total Marks";
            worksheet.Cell(currentRow, 7).Value = "Percentage";
            worksheet.Cell(currentRow, 8).Value = "Result";
            worksheet.Cell(currentRow, 9).Value = "Time Taken (min)";

            var headerRange = worksheet.Range(currentRow, 1, currentRow, 9);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

            // Data rows
            currentRow++;
            foreach (var result in results)
            {
                worksheet.Cell(currentRow, 1).Value = $"{result.Student?.FirstName} {result.Student?.LastName}";
                worksheet.Cell(currentRow, 2).Value = result.Student?.Email ?? "";
                worksheet.Cell(currentRow, 3).Value = result.Student?.Group?.Name ?? "N/A";
                worksheet.Cell(currentRow, 4).Value = result.SubmittedAt?.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "";
                worksheet.Cell(currentRow, 5).Value = result.ObtainedMarks;
                worksheet.Cell(currentRow, 6).Value = result.TotalMarks;
                worksheet.Cell(currentRow, 7).Value = result.Percentage;
                worksheet.Cell(currentRow, 8).Value = result.IsPassed ? "PASS" : "FAIL";
                worksheet.Cell(currentRow, 9).Value = result.TimeTakenMinutes;

                // Color code results
                if (result.IsPassed)
                    worksheet.Cell(currentRow, 8).Style.Font.FontColor = XLColor.Green;
                else
                    worksheet.Cell(currentRow, 8).Style.Font.FontColor = XLColor.Red;

                currentRow++;
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}

