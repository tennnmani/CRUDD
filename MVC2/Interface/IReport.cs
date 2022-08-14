using MVC2.ViewModels;

namespace MVC2.Interface
{
    public interface IReport
    {
        List<MasterVM> getJoinedDateCount();
        List<MasterVM> studentSubjectCount();
        List<AgeVM> gradeAge();
        List<MasterVM> gradeAgeSum();
        List<MasterVM> gradeAgeMin();
        List<MasterVM> gradeAgeMax();
        List<MasterVM> gradeAgeAvrage();
    }
}
