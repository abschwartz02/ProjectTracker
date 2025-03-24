using System.Text.Json;
using MySqlConnector;

class ProjectTrackerApplication
{
    static async Task Main()
    {
        ProjectTrackerApplication app = new ProjectTrackerApplication();
        app.UserInterface();
        
    }

    public ProjectTrackerApplication()
    {
        //set future field values
    }
    void UserInterface()
    {
        Console.WriteLine("Welcome!!!!.");

        //get connection info from appsetting.json
        string jsonText = File.ReadAllText(@"../../../appsetting.json");

        //add try catch later
        string connectionString = JsonDocument.Parse(jsonText).RootElement.GetProperty("databaseConnection").GetString();
        

        var connection = new MySqlConnection(connectionString);
        
        try
        {
            Console.WriteLine("Attempting to open connection");
            connection.Open(); // Try opening the connection

            if (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("Connection to MySQL database was successful!");
            }
            else
            {
                Console.WriteLine("Connection to MySQL database failed.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error connecting to MySQL: {ex.Message}");
        }

        

    }
}


//MySqlConnection connection = new MySqlConnection(); 

//;




