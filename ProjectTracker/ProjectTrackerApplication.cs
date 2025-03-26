using System.Net;
using ProjectTracker.Models;
using System.Text.Json;
using System.Text;
using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;


class ProjectTrackerApplication
{
    
    Dictionary<int, Project> projects;
    int currentProject;
    
    static void Main()
    {
        ProjectTrackerApplication app = new ProjectTrackerApplication();

        
        app.UserInterface();
        
    }

    public ProjectTrackerApplication()
    {
        loadData();
        currentProject = -1;
       
    }
    void UserInterface()
    {
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
        Console.CancelKeyPress += (sender, e) =>
        {
            e.Cancel = true;
            // do nothing
        };
        string[] projectTaskTracker = new string[]
        {
           " ****   ****    *****     ***** *****  ***** *****    ***** ****         *        ***** *     * *****  ****    ** ",
            "*    * *    *  *     *      *   *     *        *        *  *    *       * *      *      *    *  *     *    *   **" ,
            "*    * *    *  *     *      *   *     *        *        *  *    *      *   *     *      *   *   *     *    *   **",
            "*****  *****   *     *      *   ***   *        *        *  *****      *******    *      ****    ***   *****    **",
            "*      *   *   *     * *    *   *     *        *        *  *   *     *       *   *      *   *   *     *   *    **      ",
            "*      *    *  *     * *    *   *     *        *        *  *    *   *         *  *      *    *  *     *    *",
            "*      *     *  *****   *****   *****  *****   *        *  *     * *           *  ***** *     * ***** *     *  **\n"

        };

        foreach (string line in projectTaskTracker)
        {
            Console.WriteLine(line);
            
            
        }

        
        

        
        Console.WriteLine("Welcome to Project Tracker! Project Tracker is a neat tool used to\nmaintain tasks for various projects you may be working on.Type any\ncommand to begin\n");

        Console.WriteLine("Enter \"help\" to view commands");
        string response = "";
        while(!response.Equals("quit"))
        {
            
            Console.Write(">>> ");
            response = Console.ReadLine();
            if (response == null)
            {
                break;
            }

            response = response.ToLower();

            switch (response)
            {
                case "help":
                    displayHomeMenu();
                    break;
                case "list":
                    foreach (KeyValuePair<int, Project> entry in projects)
                    {
                        Project project = entry.Value;  
                        Console.WriteLine(project.name + project.status + project.dueDate);
                    }
                    break;
                         
                case "list -a":
                    break; 
                case "list -i":
                    break;
                    
                case "quit": 
                    //do nothing
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;


            }
        } 

        
    }

    public void fail(string message, int code)
    {
        Console.WriteLine(message);
        Environment.Exit(code);
    }

    public void displayHomeMenu()
    {
        Console.WriteLine("Home Commands:\n");
        Console.WriteLine("Command                        Usage\n");
        Console.WriteLine("list                           Lists all projects");
        Console.WriteLine("list -a                        Lists all active projects");
        Console.WriteLine("list -i                        Lists all inactive projects\n");
        Console.WriteLine("enter [project-name]           Enters the specified project");
        Console.WriteLine("new [project-name]             Creates a new project");
        Console.WriteLine("del [project-name]             Deletes the specified project");
        Console.WriteLine("act [project-name]             Marks the specified project as active");
        Console.WriteLine("deact [project-name]           Marks the specified project as inactive\n");
        Console.WriteLine("quit                           Quits\n");

    }

    public void loadData()
    {
        projects = new Dictionary<int, Project>();

        string jsonContent = File.ReadAllText("../../../ProjectData.json");

        JsonDocument doc = JsonDocument.Parse(jsonContent);

        JsonElement rootElement = doc.RootElement;

        foreach (JsonElement projectElement in rootElement.EnumerateArray())
        {
            int id = projectElement.GetProperty("projectId").GetInt32();
            string name = projectElement.GetProperty("name").ToString();
            string description = projectElement.GetProperty("description").ToString();
            Boolean status = projectElement.GetProperty("status").GetBoolean();
            string dueDate = projectElement.GetProperty("dueDate").ToString();

            Project newProject = new Project(id, name, description, status, dueDate);

            JsonElement tasksElement = projectElement.GetProperty("tasks");

            foreach (JsonElement taskElement in tasksElement.EnumerateArray())
            {
                int taskId = taskElement.GetProperty("id").GetInt32();
                int projectId = taskElement.GetProperty("projectId").GetInt32();

                if (id != projectId) { fail("Invalid Json", 1); };

                string taskName = taskElement.GetProperty("name").ToString();
                string taskDescription = taskElement.GetProperty("description").ToString();
                Boolean taskStatus = taskElement.GetProperty("status").GetBoolean();
                string taskDueDate = projectElement.GetProperty("dueDate").ToString();

                ProjectTask newTask = new ProjectTask(taskId, projectId, taskName, taskStatus, taskDescription, taskDueDate);
                newProject.tasks.Add(newTask.id, newTask);


            }

            projects.Add(newProject.projectId, newProject);
        }

    }

    public void saveData()
    {
        
        var jsonBuilder = new StringBuilder();
        jsonBuilder.AppendLine("[");
        Dictionary<int, Project>.Enumerator projectEnum = projects.GetEnumerator();
        projectEnum.MoveNext();
        for (int i = 0; i < projects.Count; i++)
        {

            Project myProject = projectEnum.Current.Value;
            
            Dictionary<int, ProjectTask> tasks = myProject.tasks;
            jsonBuilder.AppendLine("  {");
            jsonBuilder.AppendLine($"    \"projectId\": {myProject.projectId},");
            jsonBuilder.AppendLine($"    \"name\": \"{myProject.name}\",");
            jsonBuilder.AppendLine($"    \"description\": \"{myProject.description}\",");
            jsonBuilder.AppendLine($"    \"status\": {myProject.status.ToString().ToLower()},");
            jsonBuilder.AppendLine($"    \"dueDate\": \"{myProject.dueDate}\",");
            jsonBuilder.AppendLine($"    \"tasks\": [");

            Dictionary<int, ProjectTask>.Enumerator taskEnum = myProject.tasks.GetEnumerator();
            taskEnum.MoveNext();
            for (int j = 0; j < myProject.tasks.Count; j++)
            {
                ProjectTask myTask = taskEnum.Current.Value;
                jsonBuilder.AppendLine("      {");
                jsonBuilder.AppendLine($"        \"id\": {myTask.id},");
                jsonBuilder.AppendLine($"        \"projectId\": {myTask.projectId},");
                jsonBuilder.AppendLine($"        \"status\": {myTask.status.ToString().ToLower()},");
                jsonBuilder.AppendLine($"        \"name\": \"{myTask.name}\",");
                jsonBuilder.AppendLine($"        \"description\": \"{myTask.description}\",");
                jsonBuilder.AppendLine($"        \"dueDate\": \"{myTask.dueDate}\"");


                if (j == myProject.tasks.Count - 1)
                {
                    jsonBuilder.AppendLine("      }");
                }
                else
                {
                    jsonBuilder.AppendLine("      },");
                }


                taskEnum.MoveNext();
            }

            jsonBuilder.AppendLine("    ]");

            if (i == projects.Count - 1)
            {
                jsonBuilder.AppendLine("   }");
            }
            else
            {
                jsonBuilder.AppendLine("   },");
            }

            projectEnum.MoveNext();
        }

        jsonBuilder.AppendLine("]");

        string jsonString = jsonBuilder.ToString();

        File.WriteAllText("../../../ProjectData.json", jsonString);
        
        Console.WriteLine("Data saved successfully");
    }

     void OnProcessExit (object sender, EventArgs e)
    {
        saveData();
    }

}






