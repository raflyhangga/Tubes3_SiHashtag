using MySql.Data.MySqlClient;
using DotNetEnv;
using System;

public class Database{
    static MySqlConnection _connection;
    static bool _initialized = false;
    public static void Initialize(){
        Env.Load();
        string serverIp = Environment.GetEnvironmentVariable("SERVER_IP");      if(serverIp == null || serverIp == "") serverIp = "localhost";
        string dbUser = Environment.GetEnvironmentVariable("DB_USER");          if(dbUser == null || dbUser == "") dbUser = "root";
        string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");  if(dbPassword == null || dbPassword == "") dbPassword = "password";
        string dbName = Environment.GetEnvironmentVariable("DB_NAME");          if(dbName == null || dbName == "") dbName = "finger";
        string port = Environment.GetEnvironmentVariable("DB_PORT");            if(port == null || port == "") port = "3306";
        string connstring = "server="+serverIp+";uid="+dbUser+";pwd="+dbPassword+";database="+dbName+";port="+port;

        try {
            _connection = new MySqlConnection(connstring);
            _connection.Open();
            _initialized = true;
        } catch(MySqlException ex) {
            Console.Write(ex.ToString());
        }   

    }

    public static MySqlDataReader Execute(string query) {
        if(!_initialized) Initialize();
        MySqlCommand cmd = new MySqlCommand(query, _connection);
        return cmd.ExecuteReader();
    }
    public static MySqlCommand ExecuteCommand(string query) {
        if(!_initialized) Initialize();
        MySqlCommand cmd = new MySqlCommand(query, _connection);
        return cmd;
    }
    public static MySqlDataReader Execute(string query, params (string, object)[] parameters) {
        if(!_initialized) Initialize();
        MySqlCommand cmd = new MySqlCommand(query, _connection);
        foreach(var tuple in parameters){
            cmd.Parameters.AddWithValue(tuple.Item1, tuple.Item2);
        }
        return cmd.ExecuteReader();
    }

    public static int ExecuteNonQuery(string query, params (string, object)[] parameters) {
        if(!_initialized) Initialize();
        MySqlCommand cmd = new MySqlCommand(query, _connection);
        foreach(var tuple in parameters){
            cmd.Parameters.AddWithValue(tuple.Item1, tuple.Item2);
        }
        return cmd.ExecuteNonQuery();
    }

    public static MySqlDataReader Execute(string query, params (string, string[])[] parameters) {
        if(!_initialized) Initialize();
        MySqlCommand cmd = new MySqlCommand(query, _connection);
        foreach(var tuple in parameters){
            foreach(string val in tuple.Item2){
                cmd.Parameters.AddWithValue(tuple.Item1, val);
            }
        }
        return cmd.ExecuteReader();
    }


    public static void Execute(string fullQuery, MySqlParameter[] parameters){
        var cmd = new MySqlCommand(fullQuery.ToString(), _connection);
        cmd.Parameters.AddRange(parameters);
        cmd.ExecuteNonQuery();
    }

}

