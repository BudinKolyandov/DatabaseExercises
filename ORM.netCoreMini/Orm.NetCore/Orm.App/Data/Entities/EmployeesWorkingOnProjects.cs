namespace Orm.App.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class EmployeesWorkingOnProjects
    {        
        [Key]
        [ForeignKey(nameof(Project))]
        public int ProjectId { get; set; }

        [Key]
        [ForeignKey(nameof(Employee))]
        public int EmploeeId { get; set; }

        public Employee Employee { get; set; }

        public Project Project { get; set; }

    }
}
