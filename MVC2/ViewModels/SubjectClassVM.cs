using MVC2.Models;

namespace MVC2.ViewModels
{
    public class SubjectClassVM
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } 
        public ICollection<Grade> Grade { get; set; }
    }
}
