// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using Microsoft.VisualBasic.FileIO;
using System.Text.RegularExpressions;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;


Console.WriteLine("Hello, World!");

var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

string connString = config.GetConnectionString("DefaultConnection");
IDbConnection conn = new MySqlConnection(connString);

/*
var weatherURL = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=f43a843d9a3f633f4eeceac37eca061a";
var client = new HttpClient();
var weatherResponse = client.GetStringAsync(weatherURL).Result;
var weather = JObject.Parse(weatherResponse).GetValue("weather").ToArray();
Console.WriteLine(weather[0]["main"]);
*/


var path = @"C:\Users\freeb\Desktop\repos\WeatherLocationAPI\us_cities.csv"; 
using (TextFieldParser csvParser = new TextFieldParser(path))
{
    csvParser.CommentTokens = new string[] { "#" };
    csvParser.SetDelimiters(new string[] { "," });
    csvParser.HasFieldsEnclosedInQuotes = true;

    // Skip the row with the column names
    csvParser.ReadLine();

    while (!csvParser.EndOfData)
    {
        // Read current line fields, pointer moves to the next line.
        string[] fields = csvParser.ReadFields();
        string city = fields[1].ToString().Replace(","," "); 
        Console.WriteLine(city); 
        decimal lat = Convert.ToDecimal(fields[2]);
        decimal lon = Convert.ToDecimal(fields[3]);
        var weatherURL = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid=f43a843d9a3f633f4eeceac37eca061a";
        var client = new HttpClient();
        var weatherResponse = client.GetStringAsync(weatherURL).Result;
        var weather = JObject.Parse(weatherResponse).GetValue("weather").ToArray();
        //Console.WriteLine(fields[1] + "-" + weather[0]["main"]);
        conn.Execute($"INSERT INTO weather_locations.us_cities (id,city,latitude,longitude,weather) VALUES ({fields[0]},'{city}',{fields[2]},{fields[3]},'{weather[0]["main"]}');");
        //Console.WriteLine($"INSERT INTO weather_locations.us_cities (id,city,latitude,longitude,weather) VALUES ({fields[0]},'{city}',{fields[2]},{fields[3]},'{weather[0]["main"]}');");
    }
}







