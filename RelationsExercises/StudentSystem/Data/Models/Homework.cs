namespace StudentSystem.Data.Models
{
    using System;

    public class Homework
    {
        public Homework()
        {
            this.SubmitionTime = DateTime.Now;
        }

        public int HomeworkId { get; set; }

        public string Content { get; set; }

        public ContentType ContentType { get; set; }

        public DateTime SubmitionTime { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; }

        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
