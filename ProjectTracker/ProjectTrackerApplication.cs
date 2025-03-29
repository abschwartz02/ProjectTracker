using ProjectTracker.Models;
using System.Text.Json;
using System.Text;
using System.Globalization;
using System.ComponentModel.DataAnnotations;




class ProjectTrackerApplication
{
    
    Dictionary<string, Project> projects;
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

        Console.WriteLine("Enter \"help\" to view home commands");
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

            response = response.Trim();
            string[] words = response.Split(" ");
            
            
            
            string command = words[0].ToLower();

            switch (command)
            {
                case "help":
                    if (words.Length != 1)
                    {
                        Console.WriteLine("\nInvalid command\n");
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
                            displayProjectListHeaders(true, true);
                            foreach (KeyValuePair<string, Project> entry in projects)
                            {
                                Project project = entry.Value;
                                string activeStatus = project.status ? "Complete" : "Incomplete";
                                string result = String.Format("{0} {1} {2} {3}", project.name.PadRight(maxProjectLength + 2), activeStatus.PadRight(11), project.dueDate.PadRight(0), isLate(project.status, project.dueDate));
                                Console.WriteLine(result);
                            }
                            Console.WriteLine();
                        }
                    }
                    else if (words.Length == 2 && words[1].ToLower().Equals("-c"))
                    {
                        bool menuPrinted = false;

                        foreach (KeyValuePair<string, Project> entry in projects)
                        {
                            Project project = entry.Value;
                            if (project.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayProjectListHeaders(false, true);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2} {3}", project.name.PadRight(maxActiveProjectLength + 2), "Complete".PadRight(9), project.dueDate.PadRight(0), isLate(project.status, project.dueDate));
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
                    else if (words.Length == 2 && words[1].ToLower().Equals("-i"))
                    {
                        bool menuPrinted = false;


                        foreach (KeyValuePair<string, Project> entry in projects)
                        {
                            Project project = entry.Value;
                            if (!project.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayProjectListHeaders(false, false);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2} {3}", project.name.PadRight(maxInactiveProjectLength + 2), "Incomplete".PadRight(11), project.dueDate.PadRight(0), isLate(project.status, project.dueDate));
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
                            Console.WriteLine("\nNo project specified. Usage: del [project-name]\n");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                        }
                        else
                        {
                            Console.Write($"Are you sure you wish to delete {projectName}? (y/n): ");
                            string deleteResponse = Console.ReadLine();
                            if (deleteResponse == null)
                            {
                                Environment.Exit(0);
                                break;
                            }

                            deleteResponse = deleteResponse.ToLower().Trim();
                            while (!(deleteResponse == null) &&!deleteResponse.Equals("y") && !deleteResponse.Equals("yes") && !deleteResponse.Equals("n") && !deleteResponse.Equals("no"))
                            {
                                deleteResponse = deleteResponse.ToLower().Trim();
                                Console.WriteLine("Invalid input");
                                Console.Write($"Are you sure you wish to delete {projectName}? (y/n): ");
                                deleteResponse = Console.ReadLine();
                            }

                            if (deleteResponse == null)
                            {
                                Environment.Exit(0);
                                break;
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
                            Console.WriteLine("\nNo project specified. Usage: new [project-name]\n");
                        }
                        else if (nameExists(projectName))
                        {
                            Console.WriteLine($"\nProject with name \"{projectName}\" already exsits\n");
                        }
                        else
                        {
                            

                            Console.Write("Project description (Optional. Enter to skip): ");
                            string newDescription = Console.ReadLine();

                            if (newDescription == null)
                            {
                                Environment.Exit(0);
                                break;
                            }
                            newDescription = newDescription.Trim();

                            if (newDescription == "") { newDescription = "None"; }

                            string newDueDate = getValidDate();

                            Project newProject = new Project(projectName, newDescription, false, newDueDate, 0);

                            projects.Add(newProject.name, newProject);
                            checkLength(newProject.name, newProject.status);

                            Console.WriteLine($"\n{newProject.name} has been created!\n");

                        }

                        break;
                    }
                case "finish":
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
                            Console.WriteLine("\nNo project specified. Usage: finish [project-name]\n");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                        }
                        else
                        {
                            projects.GetValueOrDefault(projectName).status = true;
                            checkLength(projectName, true);
                            Console.WriteLine($"\n{projectName} has been marked as complete\n");
                        }
                        break;
                    }
                case "unfinish":
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
                            Console.WriteLine("\nNo project specified. Usage: unfinish [project-name]\n");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                        }
                        else
                        {
                            projects.GetValueOrDefault(projectName).status = false;
                            checkLength(projectName, false);
                            Console.WriteLine($"\n{projectName} has been marked as incomplete\n");
                        }
                        break;
                    }
                case "edit":
                    {
                        if (words.Length == 1)
                        {
                            Console.WriteLine("No edit command specified\n\nUsage:\nedit name [project-name]\nedit date [project-date]\n");
                        }
                        else if (words[1].ToLower() == "name" || words[1].ToLower() == "date")
                        {
                            string projectName = "";
                            for (int i = 2; i < words.Length; i++)
                            {
                                projectName += words[i];
                                if (i != words.Length - 1)
                                {
                                    projectName += " ";
                                }
                            }

                            if (words[1].ToLower() == "name" && projectName.Equals(""))
                            {
                                Console.WriteLine("\nNo project specified. Usage: edit name [project-name]\n");
                            }
                            else if (words[1].ToLower() == "date" && projectName.Equals(""))
                            {
                                Console.WriteLine("\nNo project specified. Usage: edit date [project-name]\n");
                            }
                            else if (!nameExists(projectName))
                            {
                                Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                            }
                            else
                            {
                                if (words[1] == "date")
                                {
                                    string newDate = getValidDate();

                                    projects.GetValueOrDefault(projectName).dueDate = newDate;

                                    Console.WriteLine($"\nDate for {projectName} has been updated to {newDate}\n");
                                }
                                else
                                {
                                    string newName = "";
                                    Console.Write($"New name for {projectName}: ");
                                    newName = Console.ReadLine();

                                    if (newName == null)
                                    {
                                        Environment.Exit(0);
                                    }

                                    newName = newName.Trim();
                                    while (newName == "" || nameExists(newName))
                                    {
                                        if (newName == "")
                                        {
                                            Console.WriteLine("Name cannot be empty");
                                        }
                                        else
                                        {
                                            Console.WriteLine($"\nProject with the name {newName} already exists\n");
                                        }

                                        Console.Write($"New name for {projectName}: ");
                                        newName = Console.ReadLine();

                                        if (newName == null)
                                        {
                                            Environment.Exit(0);
                                        }
                                        newName = newName.Trim();
                                    }


                                    Project p = projects.GetValueOrDefault(projectName);
                                    p.name = newName;
                                    foreach (KeyValuePair<int, ProjectTask> taskEntry in p.tasks)
                                    {
                                        ProjectTask task = taskEntry.Value;
                                        task.projectName = newName;
                                    }

                                    checkLength(newName, p.status);

                                    projects.Add(newName, p);
                                    projects.Remove(projectName);

                                    Console.WriteLine($"\n{projectName} successfuly renamed to {newName}\n");

                                }
                                
                                
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid command");
                        }
                        break;
                    }
                case "enter":
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
                            Console.WriteLine("\nNo project specified. Usage: enter [project-name]\n");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                        }
                        else
                        {
                           
                            TaskUserInterface(projectName);
                        }
                        break;
                    }
                case "view":
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
                            Console.WriteLine("\nNo project specified. Usage: view [project-name]\n");
                        }
                        else if (!nameExists(projectName))
                        {
                            Console.WriteLine($"\nNo project with name \"{projectName}\"\n");
                        }
                        else
                        {
                            DisplayProjectInfo(projectName);
                        }
                        break;
                    } 
                case "quit": 
                    //do nothing
                    break;
                default:
                    Console.WriteLine("\nInvalid command\nEnter \"help\" to view home commands\n");
                    break;


            }
        } 
    }

    void TaskUserInterface(string projectName)
    {
        Console.WriteLine("Entered " + projectName +"\n");

        Console.WriteLine("Enter \"help\" to view task commands\n");
        string response = "";
        Project currentProject = projects.GetValueOrDefault(projectName);
        while (!response.Equals("exit"))
        {

            response = "";
            while (response != null && response.Equals(""))
            {
                Console.Write($"{projectName}>>> ");
                response = Console.ReadLine();

            }

            if (response == null)
            {
                Environment.Exit(0);
            }

            response = response.Trim();
            string[] words = response.Split(" ");
            string command = words[0].ToLower();

            switch (command)
            {
                case "help":
                    displayTaskMenu();
                    break;
                case "list":
                    if (words.Length == 1)
                    {
                        if (currentProject.tasks.Count == 0)
                        {
                            Console.WriteLine("\nNo tasks for this project\n");
                        }
                        else
                        {
                            displayTaskListHeaders(true, true, currentProject);
                            foreach (KeyValuePair<int, ProjectTask> entry in currentProject.tasks)
                            {
                                ProjectTask projectTask = entry.Value;
                                string activeStatus = projectTask.status ? "Complete" : "Incomplete";
                                string result = String.Format("{0} {1} {2} {3} {4}",projectTask.id.ToString().PadRight(4), projectTask.name.PadRight(currentProject.maxTaskLength + 2), activeStatus.PadRight(11), projectTask.dueDate.PadRight(0), isLate(projectTask.status, projectTask.dueDate));
                                Console.WriteLine(result);
                            }
                            Console.WriteLine();
                        }
                    }
                    else if (words.Length == 2 && words[1].ToLower().Equals("-c"))
                    {
                        bool menuPrinted = false;

                        foreach (KeyValuePair<int, ProjectTask> entry in currentProject.tasks)
                        {
                            ProjectTask projectTask = entry.Value;
                            if (projectTask.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayTaskListHeaders(false, true, currentProject);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2} {3} {4}", projectTask.id.ToString().PadRight(4), projectTask.name.PadRight(currentProject.maxActiveTaskLength + 2), "Complete".PadRight(9), projectTask.dueDate.PadRight(0), isLate(projectTask.status, projectTask.dueDate));
                                Console.WriteLine(result);
                            }

                        }

                        if (!menuPrinted)
                        {
                            Console.WriteLine("\nNo complete tasks for this project\n");
                        }
                        else
                        {
                            Console.WriteLine();
                        }
                        break;
                    }
                    else if (words.Length == 2 && words[1].ToLower().Equals("-i"))
                    {
                        bool menuPrinted = false;


                        foreach (KeyValuePair<int, ProjectTask> entry in currentProject.tasks)
                        {
                            ProjectTask projectTask = entry.Value;
                            if (!projectTask.status)
                            {
                                if (!menuPrinted)
                                {
                                    displayTaskListHeaders(false, false, currentProject);
                                    menuPrinted = true;
                                }
                                string result = String.Format("{0} {1} {2} {3} {4}", projectTask.id.ToString().PadRight(4), projectTask.name.PadRight(currentProject.maxInactiveTaskLength + 2), "Incomplete".PadRight(11), projectTask.dueDate.PadRight(0), isLate(projectTask.status, projectTask.dueDate));
                                Console.WriteLine(result);
                            }

                        }


                        if (!menuPrinted)
                        {
                            Console.WriteLine("\nNo incomplete tasks for this project\n");
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
                case "finish":
                    {
                        if (words.Length != 2)
                        {
                            Console.WriteLine("\nInvalid finsih command.\nUsage: finish [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[1]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("\nInvalid finsih command.\nUsage: finish [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }

                        ProjectTask currentTask = currentProject.tasks.GetValueOrDefault(taskId);
                        currentProject.checkLength(currentTask.name, true);
                        currentTask.status = true;

                        Console.WriteLine($"\nTask {taskId} has been marked as complete\n");
                        break;
                    }
                   
                case "unfinish":
                    {
                        if (words.Length != 2)
                        {
                            Console.WriteLine("\nInvalid unfinsih command.\nUsage: unfinish [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[1]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("\nInvalid unfinsih command.\nUsage: unfinish [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }

                        ProjectTask currentTask = currentProject.tasks.GetValueOrDefault(taskId);
                        currentProject.checkLength(currentTask.name, false);
                        currentTask.status = false;

                        Console.WriteLine($"\nTask {taskId} has been marked as incomplete\n");
                        break;
                    }
                case "del":
                    {
                        if (words.Length != 2)
                        {
                            Console.WriteLine("\nInvalid del command.\nUsage: del [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[1]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("\nInvalid del command.\nUsage: del [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }

                        Console.Write($"Are you sure you wish to delete task {taskId}? (y/n): ");
                        string deleteResponse = Console.ReadLine();
                        if (deleteResponse == null)
                        {
                            Environment.Exit(0);
                            break;
                        }

                        deleteResponse = deleteResponse.ToLower().Trim();
                        while (!(deleteResponse == null) && !deleteResponse.Equals("y") && !deleteResponse.Equals("yes") && !deleteResponse.Equals("n") && !deleteResponse.Equals("no"))
                        {
                            deleteResponse = deleteResponse.ToLower().Trim();
                            Console.WriteLine("Invalid input");
                            Console.Write($"Are you sure you wish to delete task {taskId}? (y/n): ");
                            deleteResponse = Console.ReadLine();
                        }

                        if (deleteResponse == null)
                        {
                            Environment.Exit(0);
                            break;
                        }

                        if (deleteResponse.Contains("y"))
                        {
                            //no current accomodation to adjust if it is the max project name
                            currentProject.tasks.Remove(taskId);
                            Console.WriteLine($"\nTask {taskId} has been deleted\n");
                        }
                        break;
                    }
                case "new":
                    {
                        string taskName = "";
                        for (int i = 1; i < words.Length; i++)
                        {
                            taskName += words[i];
                            if (i != words.Length - 1)
                            {
                                taskName += " ";
                            }
                        }

                        if (taskName.Equals(""))
                        {
                            Console.WriteLine("\nNo project specified. Usage: new [task-name]\n");
                        }
                        else
                        {


                            Console.Write("Task description (Optional. Enter to skip): ");
                            string taskDescription = Console.ReadLine();

                            if (taskDescription == null)
                            {
                                Environment.Exit(0);
                                break;
                            }
                            taskDescription = taskDescription.Trim();

                            if (taskDescription == "") { taskDescription = "None"; }

                            string newDueDate = getValidDate();

                            currentProject.taskCount++;
                            ProjectTask newTask = new ProjectTask(currentProject.taskCount, currentProject.name, taskName, false, taskDescription, newDueDate);

        
                            currentProject.tasks.Add(newTask.id, newTask);
                            currentProject.checkLength(newTask.name, newTask.status);

                            Console.WriteLine($"\nTask \"{newTask.name}\" has been created with id {newTask.id}!\n");

                        }

                        break;
                    }
                case "edit":
                    {
                        if (words.Length != 3 || ((words[1] != "name") && (words[1] != "date")))
                        {
                            Console.WriteLine("Invalid edit command\n\nUsage:\nedit name [task-id]\nedit date [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[2]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("Invalid edit command\n\nUsage:\nedit name [task-id]\nedit date [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }
                        ProjectTask currentTask = currentProject.tasks.GetValueOrDefault(taskId);

                        if (words[1] == "date")
                        {
                            string newDueDate = getValidDate();
                            currentTask.dueDate = newDueDate;

                            Console.WriteLine($"\nDate for task {taskId} has been updated to {newDueDate}\n");
                        }
                        else
                        {
                            string newName = "";
                            Console.Write($"New name for task {taskId}: ");
                            newName = Console.ReadLine();

                            if (newName == null)
                            {
                                Environment.Exit(0);
                            }

                            while (newName == "")
                            {
                                Console.WriteLine("Task name cannot be empty\n");
                                Console.Write($"New name for task {taskId}: ");
                                newName = Console.ReadLine();

                                if (newName == null)
                                {
                                    Environment.Exit(0);
                                }
                            }

                            
                            currentTask.name = newName;
                            currentProject.checkLength(newName, currentTask.status);

                            Console.WriteLine($"\nTask {taskId} successfuly renamed to {newName}\n");
                        }
                        break;
                    }
                case "view":
                    {
                        if (words.Length != 2)
                        {
                            Console.WriteLine("\nInvalid view command.\nUsage: view [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[1]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("\nInvalid view command.\nUsage: view [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }

                        ProjectTask currentTask = currentProject.tasks.GetValueOrDefault(taskId);
                        Console.WriteLine("\nTask Id #: " + taskId + "\n");
                        Console.WriteLine("Task: " + currentTask.name);
                        Console.WriteLine("Description: " + currentTask.description);
                        string status = currentTask.status ? "Complete" : "Incomplete";
                        Console.WriteLine("Status: " + status);
                        Console.WriteLine("Due-Date: " + currentTask.dueDate + " " + isLate(currentTask.status, currentTask.dueDate));

                        Console.WriteLine("\nNotes:\n");

                        if (currentTask.notes.Count == 0)
                        {
                            Console.WriteLine("none");
                        }
                        else
                        {
                            for (int i = 0; i < currentTask.notes.Count; i++)
                            {
                                Console.Write(" * ");
                                
                                string[] noteLines = currentTask.notes[i].Split("\n");
                                for (int j = 0; j < noteLines.Length; j++)
                                {
                                    if (j != 0)
                                    {
                                        Console.Write("   ");
                                    }
                                    if (noteLines[j] != "\n")
                                    {
                                        Console.WriteLine(noteLines[j]);
                                    }
                                    
                                }
                                Console.WriteLine();

                            }
                            
                        }

                        
                        break;
                    }
                case "note":
                    {
                        if (words.Length != 2)
                        {
                            Console.WriteLine("\nInvalid view command.\nUsage: view [task-id]\n");
                            break;
                        }

                        int taskId = -1;
                        try
                        {
                            taskId = Int32.Parse(words[1]);
                        }
                        catch (System.FormatException e)
                        {
                            Console.WriteLine("\nInvalid view command.\nUsage: view [task-id]\n");
                            break;
                        }

                        if (!(currentProject.tasks.ContainsKey(taskId)))
                        {
                            Console.WriteLine($"\nNo task with id {taskId}\n");
                            break;
                        }

                        ProjectTask currentTask = currentProject.tasks.GetValueOrDefault(taskId);

                        Console.WriteLine("\nNew Note (Double enter to confirm):\n");

                        string note = "";

                        note = Console.ReadLine();
                        if (note == null)
                        {
                            Environment.Exit(0);
                        }
                        note = note + "\n";
                        // keep reading request until a double line brake is detected
                        
                        while (!note.Contains("\n\n"))
                        {
                            string nextLine = Console.ReadLine();
                            if (nextLine == null)
                            {
                                Environment.Exit(0);
                            }
                            note = note + nextLine + "\n";
                        }

                        note = note.Trim();
                        if (note == "")
                        {
                            Console.WriteLine("Note cannot be empty\n");
                        }
                        else
                        {
                            currentTask.notes.Add(note);
                            Console.WriteLine("Note has been added to task " + taskId);
                        }
                        
                        break;


                    }
                case "quit":
                    Environment.Exit(0);
                    break;
                case "exit":
                    Console.WriteLine();
                    break;
                default:
                    Console.WriteLine("\nInvalid command\nEnter \"help\" to view task commands\n");
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
        Console.WriteLine("____________________________________________________________________________");
        Console.WriteLine("| Command                      | Usage                                     |");
        Console.WriteLine("|______________________________|___________________________________________|");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| list                         | Lists all projects                        |");
        Console.WriteLine("| list -c                      | Lists all complete projects               |");
        Console.WriteLine("| list -i                      | Lists all incomplete projects             |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| view [project-name]          | View a summary of the specified proejct   |");
        Console.WriteLine("| new [project-name]           | Creates a new project                     |");
        Console.WriteLine("| del [project-name]           | Deletes the specified project             |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| finish [project-name]        | Marks the specified project as complete   |");
        Console.WriteLine("| unfinish [project-name]      | Marks the specified project as incomplete |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| edit name [project-name]     | Edit the name of an existing project      |");
        Console.WriteLine("| edit date [project-name]     | Edit the date of an existing project      |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| enter [project-name]         | Enters the specified project              |");
        Console.WriteLine("| quit                         | Quits                                     |");
        Console.WriteLine("|______________________________|___________________________________________|\n");


    }

    public void displayTaskMenu()
    {

        Console.WriteLine("\nTask Commands:\n");
        Console.WriteLine("____________________________________________________________________________");
        Console.WriteLine("| Command                      | Usage                                     |");
        Console.WriteLine("|______________________________|___________________________________________|");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| list                         | Lists all projects tasks                  |");
        Console.WriteLine("| list -c                      | Lists all complete projects tasks         |");
        Console.WriteLine("| list -i                      | Lists all incomplete projects tasks       |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| view [task-id]               | View details/notes of the specified task  |");
        Console.WriteLine("| note [task-id]               | Add a note to the specified task          |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| new [task-name]              | Creates a new task                        |");
        Console.WriteLine("| del [task-id]                | Deletes the specified task                |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| finish [task-id]             | Marks the specified task as complete      |");
        Console.WriteLine("| unfinish [task-id]           | Marks the specified task as incomplete    |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| edit name [task-id]          | Edit the name of an existing task         |");
        Console.WriteLine("| edit date [task-id]          | Edit the date of an existing task         |");
        Console.WriteLine("|                              |                                           |");
        Console.WriteLine("| exit                         | Exits the current project                 |");
        Console.WriteLine("| quit                         | Quits                                     |");
        Console.WriteLine("|______________________________|___________________________________________|\n");


       

    }

    public void loadData()
    {
        projects = new Dictionary<string, Project>();

        string jsonContent = "";
        try
        {
            jsonContent = File.ReadAllText("../../../ProjectData.json");
        }
        catch (System.IO.FileNotFoundException e)
        {
            //no file no problem, just app with no loaded data
            return;
        }
        JsonDocument doc = JsonDocument.Parse(jsonContent);

        JsonElement rootElement = doc.RootElement;

        foreach (JsonElement projectElement in rootElement.EnumerateArray())
        {
            
            string name = projectElement.GetProperty("name").ToString();
            string description = projectElement.GetProperty("description").ToString();
            Boolean status = projectElement.GetProperty("status").GetBoolean();
            string dueDate = projectElement.GetProperty("dueDate").ToString();
            int taskCount = projectElement.GetProperty("taskCount").GetInt32();

            if (nameExists(name)) { throw new Exception("Duplicate project names in ProjectData.json"); }
            checkLength(name, status);

            Project newProject = new Project(name, description, status, dueDate, taskCount);

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

                JsonElement notesElement = taskElement.GetProperty("notes");

                foreach (JsonElement note in notesElement.EnumerateArray())
                {
                    newTask.notes.Add(note.GetString());
                   
                }

                newProject.addTask(newTask);
                


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
            jsonBuilder.AppendLine($"    \"taskCount\": {myProject.taskCount},");
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
                jsonBuilder.AppendLine($"        \"dueDate\": \"{myTask.dueDate}\",");
                jsonBuilder.AppendLine("        \"notes\": [");
                for (int k = 0; k < myTask.notes.Count; k++)
                {
                    jsonBuilder.Append($"            \"{ myTask.notes[k]}\"");
                    if (k != myTask.notes.Count - 1)
                    {
                        jsonBuilder.AppendLine(",");
                    }
                }
                jsonBuilder.AppendLine();
                jsonBuilder.AppendLine("        ]");

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

     void displayProjectListHeaders(bool all, bool active)
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
            result += String.Format("{0} {1} {2}", "\nName".PadRight(maxInactiveProjectLength + 3), "Status".PadRight(11), "Due-Date\n".PadRight(0));
        }
        
        Console.WriteLine(result);
     }

    void displayTaskListHeaders(bool all, bool active, Project current)
    {
        string result = "";

        if (all)
        {
            result += String.Format("{0} {1} {2} {3}", "\nID#".PadRight(5), "Name".PadRight(current.maxTaskLength + 2), "Status".PadRight(11), "Due-Date\n".PadRight(0));
        }

        else if (active)
        {
            result += String.Format("{0} {1} {2} {3}", "\nID#".PadRight(5), "Name".PadRight(current.maxActiveTaskLength + 2), "Status".PadRight(9), "Due-Date\n".PadRight(0));
        }
        else
        {
            result += String.Format("{0} {1} {2} {3}", "\nID#".PadRight(5), "Name".PadRight(current.maxInactiveTaskLength + 2), "Status".PadRight(11), "Due-Date\n".PadRight(0));
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

        if (newDueDate == null)
        {
            Environment.Exit(0);
        }

        newDueDate = newDueDate.Trim();

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

            if (newDueDate == null)
            {
                Environment.Exit(0);
            }

            newDueDate = newDueDate.Trim();
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

    public string isLate(bool finished, string date)
    {
        if (finished || date == "None")
        {
            return "";
        }
        string[] formats = { "MM/dd/yyyy", "M/dd/yyyy", "M/d/yyyy" };

        try
        {
            var dateTime = DateTime.ParseExact(date, formats, new CultureInfo("en-US"), DateTimeStyles.None);
            if (!(dateTime.Date >= DateTime.Today))
            {
                return "(Past Due)";
            }
            else
            {
                return "";
            }
        }
        catch (Exception e)
        {
            return "invalid date";
        }
    }

    public void DisplayProjectInfo(string projectName)
    {
        Console.WriteLine("\nProject: " + projectName);

        Project p = projects.GetValueOrDefault(projectName);

        Console.WriteLine("Description: " + p.description);
        string status = p.status ? "Complete" : "Incomplete";
        Console.WriteLine("Status: " + status);
        Console.WriteLine("Due-Date: " + p.dueDate + " " + isLate(p.status, p.dueDate));
        Console.WriteLine("Total Project Tasks: " + p.tasks.Count());
        Console.WriteLine($"    Complete: { p.numComplete()}");
        Console.WriteLine($"    Incomplete: { p.numIncomplete()}\n");

    }
}

