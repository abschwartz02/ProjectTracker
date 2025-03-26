using ProjectTracker.Models;
using System.Text.Json;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;



class ProjectTrackerApplication
{
    
    Dictionary<string, Project> projects;
    int currentProject;
    int maxProjectLength;
    int maxActiveProjectLength;
    int maxInactiveProjectLength;
    
    static void Main()
    {
        ProjectTrackerApplication app = null;
        try
        {
            app = new ProjectTrackerApplication();
            app.ProjectsUserInterface();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            Environment.Exit(1);
        }

        
        
    }

    public ProjectTrackerApplication()
    {
        maxProjectLength = 0;
        maxActiveProjectLength = 0;
        maxInactiveProjectLength = 0;
        loadData();
        currentProject = -1;
       
    }
    void ProjectsUserInterface()
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

            response = "";
            while (response != null && response.Equals(""))
            {
                Console.Write(">>> ");
                response = Console.ReadLine();
                
            }

            if (response == null)
            {
                break;
            }

            string[] words = response.Split(" ");
            string command = words[0].ToLower();

            switch (command)
            {
                case "help":
                    if (words.Length != 1)
                    {
                        Console.WriteLine("Invalid command");
                        break;
                    }
                    displayHomeMenu();
                    break;
                case "list":
                    if (words.Length == 1)
                    {
                        if (projects.Count == 0)
                        {
                            Console.WriteLine("\nNo projects in the system\n");
                        }
                        else
                        {
                            displayProjectList(true, true);
                            foreach (KeyValuePair<string, Project> entry in projects)
                            {
                                Project project = entry.Value;
                                string activeStatus = project.status ? "Complete" : "Incomplete";
                                string result = String.Format("{0} {1} {2}", project.name.PadRight(maxProjectLength + 2), activeStatus.PadRight(11), project.dueDate.PadRight(0));
                                Console.WriteLine(result);
                            }
                            Console.WriteLine();
                        }
                    }
                    else if (words.Length == 2 && words[1].Equals("-c"))
                    {
                        bool menuPrinted = false;

                        foreach (KeyValuePair<string, Project> entry in projects)
                        {
                            Project project = entry.Value;
                            if (project.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayProjectList(false, true);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2}", project.name.PadRight(maxActiveProjectLength + 2), "Complete".PadRight(9), project.dueDate.PadRight(0));
                                Console.WriteLine(result);
                            }

                        }

                        if (!menuPrinted)
                        {
                            Console.WriteLine("\nNo complete projects in the system\n");
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                        break;
                    }
                    else if (words.Length == 2 && words[1].Equals("-i"))
                    {
                        bool menuPrinted = false;


                        foreach (KeyValuePair<string, Project> entry in projects)
                        {
                            Project project = entry.Value;
                            if (!project.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayProjectList(false, false);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2}", project.name.PadRight(maxInactiveProjectLength + 2), "Incomplete".PadRight(11), project.dueDate.PadRight(0));
                                Console.WriteLine(result);
                            }

                        }


                        if (!menuPrinted)
                        {
                            Console.WriteLine("\nNo incomplete projects in the system\n");
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid command");
                    }
                    break;

                case "del":
                    {
                        string projectName = "";
                        for (int i = 1; i < words.Length; i++)
                        {
                            projectName += words[i];
                            if (i != words.Length - 1)
                            {
                                projectName += " ";
                            }
                        }

                        if (projectName.Equals(""))
                        {
                            Console.WriteLine("No project specified. Usage: del [project-name]");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"No project with name \"{projectName}\"");
                        }
                        else
                        {
                            Console.Write($"Are you sure you wish to delete {projectName}? (y/n): ");
                            string deleteResponse = Console.ReadLine().ToLower();
                            while (!deleteResponse.Equals("y") && !deleteResponse.Equals("yes") && !deleteResponse.Equals("n") && !deleteResponse.Equals("no"))
                            {
                                Console.WriteLine("Invalid input");
                                Console.Write($"Are you sure you wish to delete {projectName}? (y/n): ");
                                deleteResponse = Console.ReadLine().ToLower();
                            }

                            if (deleteResponse.Contains("y"))
                            {
                                //no current accomodation to adjust if it is the max project name
                                projects.Remove(projectName);
                                Console.WriteLine($"{projectName} has been delete\n");
                            }

                        }
                        break;
                    }
                case "new":
                    {
                        string projectName = "";
                        for (int i = 1; i < words.Length; i++)
                        {
                            projectName += words[i];
                            if (i != words.Length - 1)
                            {
                                projectName += " ";
                            }
                        }

                        if (projectName.Equals(""))
                        {
                            Console.WriteLine("No project specified. Usage: new [project-name]");
                        }
                        else if (nameExists(projectName))
                        {
                            Console.WriteLine($"Project with name \"{projectName}\" already exists");
                        }
                        else
                        {
                            

                            Console.Write("Project description (Optional. Enter to skip): ");
                            string newDescription = Console.ReadLine();

                            string newDueDate = getValidDate();

                            Project newProject = new Project(projectName, newDescription, false, newDueDate);

                            projects.Add(newProject.name, newProject);

                            Console.WriteLine($"\n{newProject.name} has been created!\n");

                        }

                        break;
                    }
                case "quit": 
                    //do nothing
                    break;
                default:
                    Console.WriteLine("Invalid command");
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
        Console.WriteLine("\nHome Commands:\n");
        Console.WriteLine("Command                        Usage\n");
        Console.WriteLine("list                           Lists all projects");
        Console.WriteLine("list -c                        Lists all complete projects");
        Console.WriteLine("list -i                        Lists all incomplete projects\n");
        
        Console.WriteLine("new [project-name]             Creates a new project");
        Console.WriteLine("del [project-name]             Deletes the specified project\n");
        Console.WriteLine("finished [project-name]        Marks the specified project as complete");
        Console.WriteLine("unfinished [project-name]      Marks the specified project as incomplete\n");

        Console.WriteLine("enter [project-name]           Enters the specified project");
        Console.WriteLine("quit                           Quits\n");

    }

    public void loadData()
    {
        projects = new Dictionary<string, Project>();

        string jsonContent = File.ReadAllText("../../../ProjectData.json");

        JsonDocument doc = JsonDocument.Parse(jsonContent);

        JsonElement rootElement = doc.RootElement;

        foreach (JsonElement projectElement in rootElement.EnumerateArray())
        {
            
            string name = projectElement.GetProperty("name").ToString();
            string description = projectElement.GetProperty("description").ToString();
            Boolean status = projectElement.GetProperty("status").GetBoolean();
            string dueDate = projectElement.GetProperty("dueDate").ToString();

            if (nameExists(name)) { throw new Exception("Duplicate project names in ProjectData.json"); }
            checkLength(name, status);

            Project newProject = new Project(name, description, status, dueDate);

            JsonElement tasksElement = projectElement.GetProperty("tasks");

            foreach (JsonElement taskElement in tasksElement.EnumerateArray())
            {
                int taskId = taskElement.GetProperty("id").GetInt32();
                string projectName = taskElement.GetProperty("projectName").ToString();

                

                string taskName = taskElement.GetProperty("name").ToString();
                string taskDescription = taskElement.GetProperty("description").ToString();
                Boolean taskStatus = taskElement.GetProperty("status").GetBoolean();
                string taskDueDate = projectElement.GetProperty("dueDate").ToString();

                ProjectTask newTask = new ProjectTask(taskId, projectName, taskName, taskStatus, taskDescription, taskDueDate);
                newProject.tasks.Add(newTask.id, newTask);


            }

            projects.Add(newProject.name, newProject);
        }

    }

    public void saveData()
    {
        
        var jsonBuilder = new StringBuilder();
        jsonBuilder.AppendLine("[");
        Dictionary<string, Project>.Enumerator projectEnum = projects.GetEnumerator();
        projectEnum.MoveNext();
        for (int i = 0; i < projects.Count; i++)
        {

            Project myProject = projectEnum.Current.Value;
            
            Dictionary<int, ProjectTask> tasks = myProject.tasks;
            jsonBuilder.AppendLine("  {");
           
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
                jsonBuilder.AppendLine($"        \"projectName\": \"{myProject.name}\",");
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

     void displayProjectList(bool all, bool active)
     {
        string result = "";

        if (all)
        {
           result += String.Format("{0} {1} {2}", "\nName".PadRight(maxProjectLength + 3), "Status".PadRight(11), "Due-Date\n".PadRight(0));
        }
        
        else if (active)
        {
            result += String.Format("{0} {1} {2}", "\nName".PadRight(maxActiveProjectLength + 3), "Status".PadRight(9), "Due-Date\n".PadRight(0));
        }
        else
        {
            result += result += String.Format("{0} {1} {2}", "\nName".PadRight(maxInactiveProjectLength + 3), "Status".PadRight(11), "Due-Date\n".PadRight(0));
        }
        
        Console.WriteLine(result);
     }

    void checkLength(string name, bool status)
    {
        if (name.Length > maxProjectLength)
        {
            maxProjectLength = name.Length;
        }

        if (status && name.Length > maxActiveProjectLength)
        {
            maxActiveProjectLength = name.Length;
        }

        if (!status && name.Length > maxInactiveProjectLength)
        {
            maxInactiveProjectLength = name.Length;
        }
    }

    public bool nameExists(string name)
    {
        return projects.ContainsKey(name);
    }

    /*
     * promts user until date is valid or ""
     */
    public string getValidDate()
    {
        bool validDate = false;
        string[] formats = { "MM/dd/yyyy", "M/dd/yyyy", "M/d/yyyy" };
        Console.Write("Project due-date mm/dd/yyyy (Optional. Enter to skip): ");
  
        string newDueDate = Console.ReadLine();

        if (!newDueDate.Equals(""))
        {
            try
            {
                var dateTime = DateTime.ParseExact(newDueDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
                if (!(dateTime.Date >= DateTime.Today))
                {
                    Console.WriteLine("Due-date cannot be in the past");
                }
                else
                {
                    validDate = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid date");
            }
        }

        while (!newDueDate.Equals("") && !validDate)
        {
            
            Console.Write("Project due-date mm/dd/yyyy (Optional. Enter to skip): ");
            newDueDate = Console.ReadLine();

            try
            {
                var dateTime = DateTime.ParseExact(newDueDate, formats, new CultureInfo("en-US"), DateTimeStyles.None);
                if (!(dateTime.Date >= DateTime.Today))
                {
                    Console.WriteLine("Due-date cannot be in the past");
                }
                else
                {
                    validDate = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Invalid date");
            }
        }

        if (newDueDate.Equals(""))
        {
            return "None";
        }
        else
        {
            return newDueDate;
        }
        
    }
}

