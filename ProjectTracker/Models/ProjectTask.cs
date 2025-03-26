namespace ProjectTracker.Models
{
    internal class ProjectTask
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string name { get; set; }
        public Boolean status { get; set; }
        public string description { get; set; }
        public string dueDate { get; set; }
        public ProjectTask(int id, int projectId, string name, Boolean status, string description, string dueDate)
        {
            this.id = id;
            this.projectId = projectId;
            this.name = name;
            this.status = status;
            this.description = description;
            this.dueDate = dueDate;
        }
    }
}
