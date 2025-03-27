namespace ProjectTracker.Models
{
    internal class ProjectTask
    {
        public int id { get; set; }
        public string projectName { get; set; }
        public string name { get; set; }
        public Boolean status { get; set; }
        public string description { get; set; }
        public string dueDate { get; set; }

       
        public ProjectTask(int id, string projectName, string name, Boolean status, string description, string dueDate)
        {
            this.id = id;
            this.projectName = projectName;
            this.name = name;
            this.status = status;
            this.description = description;
            this.dueDate = dueDate;

             
        }


    }
}
