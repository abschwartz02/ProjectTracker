using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Text.Json;
using MySqlConnector;

class ProjectTrackerApplication
{

    MySqlCommand commandExecutor;
    string currentProject;
    static void Main()
    {
        ProjectTrackerApplication app = new ProjectTrackerApplication();
        app.UserInterface();
        
    }

    public ProjectTrackerApplication()
    {
        commandExecutor = null;
        currentProject = null;
    }
    void UserInterface()
    {


        string[] projectTaskTracker = new string[]
        {
           " ****    ****     *****      *****  *****  *****  *****    *****  ****         *        *****  *     *  *****   ****     ** ",
            "*    *  *    *   *     *       *    *     *         *        *   *    *       * *      *       *    *   *      *    *    **" ,
            "*    *  *    *   *     *       *    *     *         *        *   *    *      *   *     *       *   *    *      *    *    **",
            "*****   *****    *     *       *    ***   *         *        *   *****      *******    *       ****     ***    *****     **",
            "*       *   *    *     *  *    *    *     *         *        *   *   *     *       *   *       *   *    *      *   *     **      ",
            "*       *    *   *     *  *    *    *     *         *        *   *    *   *         *  *       *    *   *      *    *",
            "*       *     *   *****    *****    *****  *****    *        *   *     * *           *  *****  *     *  *****  *     *   **\n"

        };

        foreach (string line in projectTaskTracker)
        {
            Console.WriteLine(line);
        }

        //get connection info from appsetting.json
        string jsonText = File.ReadAllText(@"../../../appsetting.json");

        //add try catch later
        string connectionString = JsonDocument.Parse(jsonText).RootElement.GetProperty("databaseConnection").GetString();


        var connection = new MySqlConnection(connectionString);
        
        try
        {
            Console.WriteLine("Attempting to connect to database...");
            connection.Open(); // Try opening the connection

            if (connection.State == System.Data.ConnectionState.Open)
            {
                if (connection.Database == "")
                {
                    fail("No database specified in appsetting.json", 1);
                }
                
            }
            else
            {
                fail("Connection to MySQL database failed.", 1);
            }
        }
        catch (MySqlException ex)
        {
            if (ex.ErrorCode.ToString().Equals("UnknownDatabase"))
            {
                //initialize database
                string databaseName = connection.Database.ToString();
                connectionString = connectionString.Replace($"Database={connection.Database};", "");

                
                connection = new MySqlConnection(connectionString);

                try
                {
                    connection.Open();

                    Console.WriteLine("New database, initializing Data...");

                    string createDBQuery = "CREATE DATABASE " + databaseName + ";";


                    new MySqlCommand(createDBQuery, connection).ExecuteNonQuery();

                    connection.ChangeDatabase(databaseName);

                    var commandExecutor = new MySqlCommand(@"CREATE TABLE projects(
                        name VARCHAR(20) NOT NULL,
                        status VARCHAR(8) NOT NULL DEFAULT 'ACTIVE',
                        description VARCHAR(500),
                        deadline VARCHAR(10) DEFAULT 'NONE',
	                    PRIMARY KEY(name)
                    );", connection);


                    commandExecutor.ExecuteReader().Close();
                    
                    
                    commandExecutor.CommandText = @"CREATE TABLE tasks(
	                    id INT AUTO_INCREMENT,
                        projectId INT NOT NULL,
                        name VARCHAR(20) NOT NULL,
                        status BOOLEAN NOT NULL DEFAULT TRUE,
                        description VARCHAR(500),
                        deadline DATE,
	                    PRIMARY KEY(id)
                    );";

                    commandExecutor.ExecuteReader().Close();

                    


                }
                catch(MySqlException ex2)
                {
                    fail($"Error connecting to MySQL2: {ex2.Message}", 1);
                }

            }
            else
            {
                fail($"Error connecting to MySQL: {ex.Message}", 1);
            }
            
        }

        if (commandExecutor == null)
        {
            commandExecutor = new MySqlCommand("", connection);
        }

        Console.WriteLine("Connection to MySQL database was successful!\n");
        Console.WriteLine("Welcome to Project Tracker! Project Tracker is a neat tool used to\nmaintain tasks for various projects you may be working on.Type any\ncommand to begin\n");

        Console.WriteLine("Enter \"help\" to view commands");
        string response = "";
        while(!response.Equals("quit"))
        {
            
            Console.Write(">>> ");
            response = Console.ReadLine().ToLower();

            switch (response)
            {
                case "help":
                    displayHomeMenu();
                    break;
                case "list":
                {
                    commandExecutor.CommandText = "SELECT COUNT(*) FROM projects";
                    var reader = commandExecutor.ExecuteReader();
                    reader.Read();

                    long numProjects = (long)reader.GetValue(0);
                    reader.Close();
                    if (numProjects == 0)
                    {
                        Console.WriteLine("No projects in the system");
                    }
                    else
                    {
                        commandExecutor.CommandText = "SELECT * FROM projects";
                        reader = commandExecutor.ExecuteReader();
                        Console.WriteLine("Project          Status         Due-Date\n");
                        while (reader.Read())
                        {
                            //format whitespace later
                            Console.WriteLine(reader.GetValue(0).ToString() + reader.GetValue(1).ToString() + reader.GetValue(3).ToString());
                        }
                        reader.Close();
                        Console.WriteLine();
                    }
                    break;
                }
                         
                case "list -a":
                {
                    commandExecutor.CommandText = "SELECT COUNT(*) FROM projects WHERE status = 'ACTIVE'";
                    var reader = commandExecutor.ExecuteReader();
                    reader.Read();

                    long numProjects = (long)reader.GetValue(0);
                    reader.Close();
                    if (numProjects == 0)
                    {
                        Console.WriteLine("No active projects in the system");
                    }
                    else
                    {
                        commandExecutor.CommandText = "SELECT * FROM projects WHERE status = 'ACTIVE'";
                        reader = commandExecutor.ExecuteReader();
                        Console.WriteLine("Project          Status         Due-Date\n");
                        while (reader.Read())
                        {
                            //format whitespace later
                            Console.WriteLine(reader.GetValue(0).ToString() + reader.GetValue(1).ToString() + reader.GetValue(3).ToString());
                        }
                        reader.Close();
                        Console.WriteLine();
                    }
                    break;
                }
                    
                    
                case "list -i":
                    {
                        commandExecutor.CommandText = "SELECT COUNT(*) FROM projects WHERE status = 'INACTIVE'";
                        var reader = commandExecutor.ExecuteReader();
                        reader.Read();

                        long numProjects = (long)reader.GetValue(0);
                        reader.Close();
                        if (numProjects == 0)
                        {
                            Console.WriteLine("No inactive projects in the system");
                        }
                        else
                        {
                            commandExecutor.CommandText = "SELECT * FROM projects WHERE status = 'INACTIVE'";
                            reader = commandExecutor.ExecuteReader();
                            Console.WriteLine("Project          Status         Due-Date\n");
                            while (reader.Read())
                            {
                                //format whitespace later
                                Console.WriteLine(reader.GetValue(0).ToString() + reader.GetValue(1).ToString() + reader.GetValue(3).ToString());
                            }
                            reader.Close();
                            Console.WriteLine();
                        }
                        break;
                    }
                    
                case "quit":
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
        Console.WriteLine("enter    [project-name]        Enters the specified project");
        Console.WriteLine("new      [project-name]        Creates a new project");
        Console.WriteLine("del      [project-name]        Deletes the specified project");
        Console.WriteLine("act      [project-name]        Marks the specified project as active");
        Console.WriteLine("deact    [project-name]        Marks the specified project as inactive\n");
        Console.WriteLine("quit                           quits\n");

    }

    
}






