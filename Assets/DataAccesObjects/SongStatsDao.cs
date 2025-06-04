using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SongEvent
{
    public float time;
    public float data;
};

public struct Song
{
    public string id;
    public string title;
    public string author;
    public List<SongEvent> events;
}

public interface SongStatsDao 
{
    public void NewSong();
    public void SetSongInfo(string _title, string _author);
    public void AddSongEvent(float _time, int _data);
    public void SaveSong();
    public void LoadSong(string id);
    public void DeleteSong(string id);
    public void GetSongInfo(out string _title, out string _author);
    public List<SongEvent> GetSongEvents();
    public void GetSongInfo(string id, out string _title, out string _author);
    public List<string> GetSongIds();
}
