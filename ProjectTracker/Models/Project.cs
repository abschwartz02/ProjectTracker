
namespace ProjectTracker.Models
{
    internal class Project
    {

        public string name { get; set; }
        public string description { get; set; }
        public Boolean status { get; set; }
        public string dueDate { get; set; }
        public int taskCount { get; set; }
        public Dictionary<int, ProjectTask> tasks { get; set; }

        public int maxTaskLength { get; set; }
        public int maxActiveTaskLength { get; set; }
        public int maxInactiveTaskLength { get; set;  }
        public Project(string name, string description, Boolean status, string dueDate, int taskCount)
        {
            
            
            this.name = name;
            this.description = description;
            this.status = status;
            this.dueDate = dueDate;
            this.tasks = new Dictionary<int, ProjectTask>();
            this.taskCount = taskCount;

            maxTaskLength = 0;
            maxActiveTaskLength = 0;
            maxInactiveTaskLength = 0;
        }

        public void addTask(ProjectTask task)
        {
            tasks.Add(task.id, task);
            checkLength(task.name, task.status);
        }

        void checkLength(string name, bool status)
        {
            if (name.Length > maxTaskLength)
            {
                maxTaskLength = name.Length;
            }

            if (status && name.Length > maxActiveTaskLength)
            {
                maxActiveTaskLength = name.Length;
            }

            if (!status && name.Length > maxInactiveTaskLength)
            {
                maxInactiveTaskLength = name.Length;
            }
        }


    }
}
