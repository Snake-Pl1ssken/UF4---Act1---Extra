using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System;

public class MySQLSongDao : SongStatsDao
{
    //Se trata de un caso de 2 tablas la tabla songEvents va a la tabla song 
    //el indice empieza en 0 y cada que se recibe un evento se a de sumar en 1 el indice una
    //vez recibidios todos los eventos y pasados todos los indices correspondientes al evento es que se a 
    //de poner el indice otra vez a 0 para esperar el proximo evento
    // Datos de conexión
    public string server = "127.0.0.1";
    public string database = "drumhero";
    public string user = "root";
    public string password = "";
    public int port = 3307;
    private MySqlConnection connection;
    Song song;

    public MySQLSongDao()
    {
        try
        {
            string connectionString = "Server="+ server +"; Port="+ port +"; Database="+database+"; Uid="+ user +"; Pwd="+ password +";";
            Debug.Log(connectionString);
            connection = new MySqlConnection(connectionString);

            connection.Open();
            Debug.Log("Connected 2 MySQL");

        }
        catch(Exception ex) 
        {
            Debug.Log("Not connected 2 MySQL: " + ex);
        }
    }

    public void AddSongEvent(float _time, int _data)
    {
        SongEvent eventos = new SongEvent
        {
            time = _time,
            data = _data
        };

        song.events.Add(eventos);
    }

    public void DeleteSong(string id)
    {
        string query = "DELETE FROM songs WHERE id = '" + id + "'";
        MySqlCommand command = new MySqlCommand(query, connection);

        command.ExecuteNonQuery();
    }

    public List<SongEvent> GetSongEvents()
    {
        List<SongEvent> events = new List<SongEvent>();

        string query = "SELECT data, time FROM SongEvents"; 
        MySqlCommand command = new MySqlCommand(query, connection);

        MySqlDataReader reader = command.ExecuteReader();

        while (reader.Read())
        {
            SongEvent songEvent = new SongEvent
            {
                data = reader.GetFloat("data"),
                time = reader.GetFloat("time")
            };

            events.Add(songEvent);
        }

        return events;
    }

    public List<string> GetSongIds()
    {
        List<string> ids = new List<string>();

        string query = "SELECT id FROM Songs";
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();
        
        while (reader.Read())
        {
            string id = reader.GetString("id");
            ids.Add(id);
        }
        
        return ids;
    }

    public void GetSongInfo(out string _title, out string _author)
    {
        _title = song.title;
        _author = song.author;
    }

    public void GetSongInfo(string id, out string _title, out string _author)
    {
        _title = "";
        _author = "";

        string query = "SELECT title, author FROM Songs WHERE id = '" + id + "'";
        MySqlCommand command = new MySqlCommand(query, connection);
        MySqlDataReader reader = command.ExecuteReader();
        
        if (reader.Read())
        {
            _title = reader.GetString("title");
            _author = reader.GetString("author");
        }
        else
        {
            Debug.Log("No esta cancion con id: " + id);
        }        
    }

    public void LoadSong(string id)
    {
        throw new System.NotImplementedException();
    }

    public void NewSong()
    {
        song = new Song();
        song.id = Guid.NewGuid().ToString();
        song.title = "";
        song.author = "";
        song.events = new List<SongEvent>();
    }

    public void SaveSong()
    {
        try
        {
            string songId = song.id;
            string insertSongQuery =
                "INSERT INTO Songs (id, title, author) VALUES ('"
                + song.id + "', '"
                + song.title + "', '"
                + song.author + "')";

            MySqlCommand command = new MySqlCommand(insertSongQuery, connection);
            command.ExecuteNonQuery();

            for (int i = 0; i < song.events.Count; i++)
            {
                SongEvent e = song.events[i];
                string insertEventQuery =
                    $"INSERT INTO SongEvents (song_id, event_index, event_data, event_time) VALUES ('{song.id}', {i}, {e.data.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {e.time.ToString(System.Globalization.CultureInfo.InvariantCulture)})";


                command = new MySqlCommand(insertEventQuery, connection);
                command.ExecuteNonQuery(); //ExecuteNonQuery para no devolver resultado
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Error por esto: " + ex);
        }

    }

    public void SetSongInfo(string _title, string _author)
    {
        song.title = _title;
        song.author = _author;
    }
}