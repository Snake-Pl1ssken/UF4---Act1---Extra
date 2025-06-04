using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SongManager : MonoBehaviour
{
    public enum StorageType
    { 
        memory,
        mySQL
    }

    public StorageType storagetype;
    SongStatsDao songstatsDao;
    Song song;

    string id;

    void Start()
    {
        switch (storagetype)
        { 
            case StorageType.memory:
                songstatsDao = new MemorySongDao();
                break;
            case StorageType.mySQL:
                songstatsDao = new MySQLSongDao();
                break;
        }
    }

    public void NewSong()
    {
        songstatsDao.NewSong();
    }

    public void SetSongInfo(string _title, string _author)
    {
        songstatsDao.SetSongInfo(_title, _author);
    }

    public void AddSongEvent(float _time, int _data)
    {
        songstatsDao.AddSongEvent(_time, _data);
    }

    public void SaveSong()
    {
        songstatsDao.SaveSong();
    }

    public void LoadSong(string id)
    {
        songstatsDao.LoadSong(id);
    }

    public void DeleteSong(string id)
    {
        songstatsDao.DeleteSong(id);
    }

    public void GetSongInfo(out string _title, out string _author)
    {
        songstatsDao.GetSongInfo(out _title, out _author);
    }

    
    public List<SongEvent> GetSongEvents()
    {
        return songstatsDao.GetSongEvents();
    }

    public void GetSongInfo(string id, out string _title, out string _author)
    {
        songstatsDao.GetSongInfo(id, out _title, out _author);
    }

    public List<string> GetSongIds()
    {
        return songstatsDao.GetSongIds();
    }
}
