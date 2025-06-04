using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MemorySongDao : SongStatsDao
{
    Song song;

    Dictionary<string, Song> storedSongs;

    private void Start()
    {
        storedSongs = new Dictionary<string, Song>();
    }

    public void NewSong()
    {
        Guid guid = Guid.NewGuid();

        song = new Song();
        song.id = guid.ToString();
        song.title = "";
        song.author = "";
        song.events = new List<SongEvent>();

    }

    public void SetSongInfo(string _title, string _author)
    {
        song.title = _title;
        song.author = _author;
    }

    public void AddSongEvent(float _time, int _data)
    {
        SongEvent e = new SongEvent();

        e.time = _time;
        e.data = _data;

        song.events.Add(e);
    }

    public void SaveSong()
    {
        storedSongs.Add(song.id, song);
    }

    public void LoadSong(string id)
    {
        song = storedSongs[id];
    }

    public void DeleteSong(string id)
    {
        storedSongs.Remove(id);
    }

    public void GetSongInfo(out string _title, out string _author)
    {
        _title = song.title;
        _author = song.author;
    }


    public List<SongEvent> GetSongEvents()
    {
        return song.events;
    }

    public void GetSongInfo(string id, out string _title, out string _author)
    {
        _title = storedSongs[id].title;
        _author = storedSongs[id].author;
    }

    public List<string> GetSongIds()
    {
        string[] idArray = new string[storedSongs.Keys.Count];
        storedSongs.Keys.CopyTo(idArray, 0);
        List<string> idList = new List<string>();
        idList.AddRange(idArray);

        return idList;
    }
}
