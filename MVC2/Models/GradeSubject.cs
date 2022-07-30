﻿namespace MVC2.Models
{
    public class GradeSubject
    {
        public int GradeId { get; set; }
        public Grade Grade { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
