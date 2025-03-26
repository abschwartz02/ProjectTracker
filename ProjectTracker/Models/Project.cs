
namespace ProjectTracker.Models
{
    internal class Project
    {

        public int projectId { get; set; } 
        public string name { get; set; }
        public string description { get; set; }
        public Boolean status { get; set; }
        public string dueDate { get; set; }

        public Dictionary<int, ProjectTask> tasks { get; set; }

        public Project(int projectId, string name, string description, Boolean status, string dueDate)
        {
            
            this.projectId = projectId;
            this.name = name;
            this.description = description;
            this.status = status;
            this.dueDate = dueDate;
            this.tasks = new Dictionary<int, ProjectTask>();
        }
        
        
    }
}
