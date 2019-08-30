namespace Orm.App.Data.Entities
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Project
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public int RemainingTime { get; set; }

        public ICollection<EmployeesWorkingOnProjects> EmployeesWorkingOnProjects { get; }

    }
}
