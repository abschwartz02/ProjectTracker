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
	                    id INT AUTO_INCREMENT,
                        name VARCHAR(20) NOT NULL,
                        active BOOLEAN NOT NULL DEFAULT TRUE,
                        description VARCHAR(500),
                        deadline DATE,
	                    PRIMARY KEY(id)
                    );", connection);


                    commandExecutor.ExecuteReader().Close();
                    
                    
                    commandExecutor.CommandText = @"CREATE TABLE tasks(
	                    id INT AUTO_INCREMENT,
                        projectId INT NOT NULL,
                        name VARCHAR(20) NOT NULL,
                        active BOOLEAN NOT NULL DEFAULT TRUE,
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
        
        
        string response = "";
        while(!response.Equals("quit"))
        {
            displayHomeMenu();

            Console.Write(">>> ");
            response = Console.ReadLine().ToLower();

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


//MySqlConnection connection = new MySqlConnection(); 

//;




