using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Extensions.Logging;
using repositories.Models;
using services.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace services.Services
{
    public class WordDocumentService : IWordDocumentService
    {
        private readonly ILogger<WordDocumentService> _logger;

        public WordDocumentService(ILogger<WordDocumentService> logger)
        {
            _logger = logger;
        }

        public async Task<MemoryStream> GenerateLessonPlanDocumentAsync(LessonPlan lessonPlan)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("Generating Word document for lesson plan: {Title}", lessonPlan.Title);

                    var memoryStream = new MemoryStream();
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        // Add main document part
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        var body = mainPart.Document.AppendChild(new Body());

                        // Add title
                        AddTitle(body, lessonPlan.Title);

                        // Add lesson plan metadata
                        AddHeading(body, "Thông tin chung", 2);
                        AddParagraph(body, $"Cấp học: {lessonPlan.Level?.LevelName ?? "N/A"}");
                        AddParagraph(body, $"Lớp: {lessonPlan.Grade}");
                        AddParagraph(body, $"Chủ đề: {lessonPlan.Topic}");
                        AddParagraph(body, $"Thời lượng: {lessonPlan.Duration} phút");
                        AddParagraph(body, $"Giáo viên: {lessonPlan.Teacher?.Username ?? "N/A"}");
                        AddParagraph(body, $"Ngày tạo: {lessonPlan.CreatedAt:dd/MM/yyyy HH:mm}");

                        // Add learning objectives
                        if (!string.IsNullOrEmpty(lessonPlan.LearningObjectives))
                        {
                            AddHeading(body, "Mục tiêu học tập", 2);
                            var objectives = lessonPlan.LearningObjectives.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var objective in objectives)
                            {
                                AddBulletPoint(body, objective.Trim());
                            }
                        }

                        // Add math formulas if any
                        if (!string.IsNullOrEmpty(lessonPlan.MathFormulas))
                        {
                            AddHeading(body, "Công thức toán học", 2);
                            AddParagraph(body, lessonPlan.MathFormulas);
                        }

                        // Add content/assessment
                        if (!string.IsNullOrEmpty(lessonPlan.Content))
                        {
                            AddHeading(body, "Đánh giá", 2);
                            AddParagraph(body, lessonPlan.Content);
                        }

                        // Save the document
                        mainPart.Document.Save();
                    }

                    memoryStream.Position = 0;
                    _logger.LogInformation("Successfully generated Word document for lesson plan: {Title}", lessonPlan.Title);
                    return memoryStream;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating Word document for lesson plan");
                    throw new Exception($"Failed to generate Word document: {ex.Message}", ex);
                }
            });
        }

        public async Task<MemoryStream> GenerateLessonPlanWithLessonsDocumentAsync(LessonPlan lessonPlan)
        {
            return await Task.Run(() =>
            {
                try
                {
                    _logger.LogInformation("Generating detailed Word document for lesson plan: {Title}", lessonPlan.Title);

                    var memoryStream = new MemoryStream();
                    using (var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document, true))
                    {
                        // Add main document part
                        var mainPart = wordDocument.AddMainDocumentPart();
                        mainPart.Document = new Document();
                        var body = mainPart.Document.AppendChild(new Body());

                        // Add title
                        AddTitle(body, lessonPlan.Title);

                        // Add lesson plan metadata
                        AddHeading(body, "Thông tin chung", 2);
                        AddParagraph(body, $"Cấp học: {lessonPlan.Level?.LevelName ?? "N/A"}");
                        AddParagraph(body, $"Lớp: {lessonPlan.Grade}");
                        AddParagraph(body, $"Chủ đề: {lessonPlan.Topic}");
                        AddParagraph(body, $"Thời lượng: {lessonPlan.Duration} phút");
                        AddParagraph(body, $"Giáo viên: {lessonPlan.Teacher?.Username ?? "N/A"}");
                        AddParagraph(body, $"Ngày tạo: {lessonPlan.CreatedAt:dd/MM/yyyy HH:mm}");

                        AddEmptyLine(body);

                        // Add learning objectives
                        if (!string.IsNullOrEmpty(lessonPlan.LearningObjectives))
                        {
                            AddHeading(body, "Mục tiêu học tập", 2);
                            var objectives = lessonPlan.LearningObjectives.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var objective in objectives)
                            {
                                AddBulletPoint(body, objective.Trim());
                            }
                            AddEmptyLine(body);
                        }

                        // Add math formulas if any
                        if (!string.IsNullOrEmpty(lessonPlan.MathFormulas))
                        {
                            AddHeading(body, "Công thức toán học", 2);
                            AddParagraph(body, lessonPlan.MathFormulas);
                            AddEmptyLine(body);
                        }

                        // Add lessons
                        if (lessonPlan.Lessons != null && lessonPlan.Lessons.Any())
                        {
                            AddHeading(body, "Các bài học", 2);

                            var orderedLessons = lessonPlan.Lessons.OrderBy(l => l.Order).ToList();
                            foreach (var lesson in orderedLessons)
                            {
                                // Lesson title
                                AddHeading(body, $"Bài {lesson.Order}: {lesson.Title}", 3);

                                // Lesson objective
                                if (!string.IsNullOrEmpty(lesson.Objective))
                                {
                                    AddParagraphWithLabel(body, "Mục tiêu:", lesson.Objective);
                                }

                                // Lesson content
                                if (!string.IsNullOrEmpty(lesson.Content))
                                {
                                    AddParagraphWithLabel(body, "Nội dung:", lesson.Content);
                                }

                                // Lesson example
                                if (!string.IsNullOrEmpty(lesson.Example))
                                {
                                    AddParagraphWithLabel(body, "Ví dụ:", lesson.Example);
                                }

                                // Resource URL
                                if (!string.IsNullOrEmpty(lesson.ResourceUrl))
                                {
                                    AddParagraphWithLabel(body, "Tài nguyên:", lesson.ResourceUrl);
                                }

                                // Lesson details (activities)
                                if (lesson.LessonDetails != null && lesson.LessonDetails.Any())
                                {
                                    AddParagraphBold(body, "Hoạt động:");
                                    var orderedDetails = lesson.LessonDetails.OrderBy(d => d.Order).ToList();
                                    foreach (var detail in orderedDetails)
                                    {
                                        AddBulletPoint(body, detail.Content);
                                    }
                                }

                                AddEmptyLine(body);
                            }
                        }

                        // Add content/assessment
                        if (!string.IsNullOrEmpty(lessonPlan.Content))
                        {
                            AddHeading(body, "Đánh giá", 2);
                            AddParagraph(body, lessonPlan.Content);
                        }

                        // Save the document
                        mainPart.Document.Save();
                    }

                    memoryStream.Position = 0;
                    _logger.LogInformation("Successfully generated detailed Word document for lesson plan: {Title}", lessonPlan.Title);
                    return memoryStream;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating detailed Word document for lesson plan");
                    throw new Exception($"Failed to generate Word document: {ex.Message}", ex);
                }
            });
        }

        #region Helper Methods

        private void AddTitle(Body body, string text)
        {
            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            var runProperties = run.AppendChild(new RunProperties());
            runProperties.AppendChild(new Bold());
            runProperties.AppendChild(new FontSize { Val = "32" }); // 16pt = 32 half-points
            run.AppendChild(new Text(text));

            // Add paragraph properties for center alignment
            var paragraphProperties = paragraph.InsertAt(new ParagraphProperties(), 0);
            paragraphProperties.AppendChild(new Justification { Val = JustificationValues.Center });
            paragraphProperties.AppendChild(new SpacingBetweenLines { After = "400" }); // Add space after
        }

        private void AddHeading(Body body, string text, int level)
        {
            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            var runProperties = run.AppendChild(new RunProperties());
            runProperties.AppendChild(new Bold());

            // Set font size based on heading level
            var fontSize = level switch
            {
                1 => "32", // 16pt
                2 => "28", // 14pt
                3 => "24", // 12pt
                _ => "22"  // 11pt
            };
            runProperties.AppendChild(new FontSize { Val = fontSize });

            run.AppendChild(new Text(text));

            // Add spacing
            var paragraphProperties = paragraph.InsertAt(new ParagraphProperties(), 0);
            paragraphProperties.AppendChild(new SpacingBetweenLines { Before = "240", After = "120" });
        }

        private void AddParagraph(Body body, string text)
        {
            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
        }

        private void AddParagraphBold(Body body, string text)
        {
            var paragraph = body.AppendChild(new Paragraph());
            var run = paragraph.AppendChild(new Run());
            var runProperties = run.AppendChild(new RunProperties());
            runProperties.AppendChild(new Bold());
            run.AppendChild(new Text(text));
        }

        private void AddParagraphWithLabel(Body body, string label, string text)
        {
            var paragraph = body.AppendChild(new Paragraph());

            // Add bold label
            var labelRun = paragraph.AppendChild(new Run());
            var labelRunProperties = labelRun.AppendChild(new RunProperties());
            labelRunProperties.AppendChild(new Bold());
            labelRun.AppendChild(new Text(label + " "));

            // Add normal text
            var textRun = paragraph.AppendChild(new Run());
            textRun.AppendChild(new Text(text));
        }

        private void AddBulletPoint(Body body, string text)
        {
            var paragraph = body.AppendChild(new Paragraph());

            // Add paragraph properties for bullet
            var paragraphProperties = paragraph.InsertAt(new ParagraphProperties(), 0);
            var numberingProperties = paragraphProperties.AppendChild(new NumberingProperties());
            numberingProperties.AppendChild(new NumberingLevelReference { Val = 0 });
            numberingProperties.AppendChild(new NumberingId { Val = 1 });

            // Add indentation
            paragraphProperties.AppendChild(new Indentation { Left = "720", Hanging = "360" });

            // Add text
            var run = paragraph.AppendChild(new Run());
            run.AppendChild(new Text(text));
        }

        private void AddEmptyLine(Body body)
        {
            body.AppendChild(new Paragraph());
        }

        #endregion
    }
}
