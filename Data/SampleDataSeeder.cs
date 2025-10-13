using CETExamApp.Models;

namespace CETExamApp.Data
{
    public static class SampleDataSeeder
    {
        public static async Task SeedSampleData(ApplicationDbContext context)
        {
            // Check if data already exists
            if (context.Subjects.Any())
                return;

            // Seed Classes (10th, 11th, 12th)
            var class10 = new Class { Name = "10th Standard", Code = "10TH", Description = "Class 10", IsActive = true };
            var class11 = new Class { Name = "11th Standard", Code = "11TH", Description = "Class 11", IsActive = true };
            var class12 = new Class { Name = "12th Standard", Code = "12TH", Description = "Class 12", IsActive = true };

            context.Classes.AddRange(class10, class11, class12);
            await context.SaveChangesAsync();

            // Seed Subjects (P, C, M, B)
            var physics = new Subject { Name = "Physics", Code = "P", Description = "Physics subject", IsActive = true };
            var chemistry = new Subject { Name = "Chemistry", Code = "C", Description = "Chemistry subject", IsActive = true };
            var mathematics = new Subject { Name = "Mathematics", Code = "M", Description = "Mathematics subject", IsActive = true };
            var biology = new Subject { Name = "Biology", Code = "B", Description = "Biology subject", IsActive = true };

            context.Subjects.AddRange(physics, chemistry, mathematics, biology);
            await context.SaveChangesAsync();

            // Seed Groups (PCMB, PCB, PCM)
            var pcmb = new Group 
            { 
                Name = "PCMB", 
                Code = "PCMB", 
                Description = "Physics, Chemistry, Mathematics, Biology", 
                ClassId = class11.Id,
                IsActive = true 
            };
            var pcb = new Group 
            { 
                Name = "PCB", 
                Code = "PCB", 
                Description = "Physics, Chemistry, Biology (Medical Stream)", 
                ClassId = class11.Id,
                IsActive = true 
            };
            var pcm = new Group 
            { 
                Name = "PCM", 
                Code = "PCM", 
                Description = "Physics, Chemistry, Mathematics (Engineering Stream)", 
                ClassId = class11.Id,
                IsActive = true 
            };

            context.Groups.AddRange(pcmb, pcb, pcm);
            await context.SaveChangesAsync();

            // Seed Sample Topics (Class-wise and Subject-wise)
            var topics = new List<Topic>
            {
                // Physics Topics - 11th
                new Topic { Name = "Units and Measurements", Code = "P-11-01", SubjectId = physics.Id, ClassId = class11.Id, Description = "Basic units and measurements", IsActive = true },
                new Topic { Name = "Motion in a Straight Line", Code = "P-11-02", SubjectId = physics.Id, ClassId = class11.Id, Description = "Kinematics in one dimension", IsActive = true },
                new Topic { Name = "Laws of Motion", Code = "P-11-03", SubjectId = physics.Id, ClassId = class11.Id, Description = "Newton's laws", IsActive = true },
                
                // Physics Topics - 12th
                new Topic { Name = "Electric Charges and Fields", Code = "P-12-01", SubjectId = physics.Id, ClassId = class12.Id, Description = "Electrostatics", IsActive = true },
                new Topic { Name = "Current Electricity", Code = "P-12-02", SubjectId = physics.Id, ClassId = class12.Id, Description = "Electric current and circuits", IsActive = true },

                // Chemistry Topics - 11th
                new Topic { Name = "Some Basic Concepts of Chemistry", Code = "C-11-01", SubjectId = chemistry.Id, ClassId = class11.Id, Description = "Introduction to chemistry", IsActive = true },
                new Topic { Name = "Structure of Atom", Code = "C-11-02", SubjectId = chemistry.Id, ClassId = class11.Id, Description = "Atomic structure", IsActive = true },
                new Topic { Name = "Chemical Bonding", Code = "C-11-03", SubjectId = chemistry.Id, ClassId = class11.Id, Description = "Types of chemical bonds", IsActive = true },

                // Chemistry Topics - 12th
                new Topic { Name = "Solid State", Code = "C-12-01", SubjectId = chemistry.Id, ClassId = class12.Id, Description = "Properties of solids", IsActive = true },
                new Topic { Name = "Solutions", Code = "C-12-02", SubjectId = chemistry.Id, ClassId = class12.Id, Description = "Types of solutions", IsActive = true },

                // Mathematics Topics - 11th
                new Topic { Name = "Sets", Code = "M-11-01", SubjectId = mathematics.Id, ClassId = class11.Id, Description = "Set theory", IsActive = true },
                new Topic { Name = "Relations and Functions", Code = "M-11-02", SubjectId = mathematics.Id, ClassId = class11.Id, Description = "Relations and functions", IsActive = true },
                new Topic { Name = "Trigonometry", Code = "M-11-03", SubjectId = mathematics.Id, ClassId = class11.Id, Description = "Trigonometric functions", IsActive = true },

                // Mathematics Topics - 12th
                new Topic { Name = "Relations and Functions", Code = "M-12-01", SubjectId = mathematics.Id, ClassId = class12.Id, Description = "Advanced functions", IsActive = true },
                new Topic { Name = "Calculus", Code = "M-12-02", SubjectId = mathematics.Id, ClassId = class12.Id, Description = "Differentiation and Integration", IsActive = true },

                // Biology Topics - 11th
                new Topic { Name = "The Living World", Code = "B-11-01", SubjectId = biology.Id, ClassId = class11.Id, Description = "Introduction to biology", IsActive = true },
                new Topic { Name = "Biological Classification", Code = "B-11-02", SubjectId = biology.Id, ClassId = class11.Id, Description = "Taxonomy", IsActive = true },
                new Topic { Name = "Cell: The Unit of Life", Code = "B-11-03", SubjectId = biology.Id, ClassId = class11.Id, Description = "Cell structure", IsActive = true },

                // Biology Topics - 12th
                new Topic { Name = "Reproduction in Organisms", Code = "B-12-01", SubjectId = biology.Id, ClassId = class12.Id, Description = "Types of reproduction", IsActive = true },
                new Topic { Name = "Genetics and Evolution", Code = "B-12-02", SubjectId = biology.Id, ClassId = class12.Id, Description = "Mendelian genetics", IsActive = true }
            };

            context.Topics.AddRange(topics);
            await context.SaveChangesAsync();
        }
    }
}

