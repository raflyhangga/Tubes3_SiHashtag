using MySql.Data.MySqlClient;
using DotNetEnv;
using System;

Env.Load();

string serverIp = Environment.GetEnvironmentVariable("SERVER_IP");
string dbUser = Environment.GetEnvironmentVariable("DB_USER");
string dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
string dbName = Environment.GetEnvironmentVariable("DB_NAME");

// See https://aka.ms/new-console-template for more information
string connstring = "server="+serverIp+";uid="+dbUser+";pwd="+dbPassword+";database="+dbName;
Console.WriteLine(connstring);

try {
    MySqlConnection con = new();
    con.ConnectionString = connstring;
    con.Open();

    // Nanti querynya ganti
    string query = "SELECT * FROM mahasiswa";
    MySqlCommand cmd = new(query,con);

    MySqlDataReader reader = cmd.ExecuteReader();

    while (reader.Read()){
        Console.WriteLine(reader["username"]);
    }
} catch(MySqlException ex) {
    Console.Write(ex.ToString());
}   