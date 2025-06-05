using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using TMPro;

public class LevelManager : MonoBehaviour
{
    public Transform mainCamera;

    public Transform songManager;

    [Header("Menus")]

    public Transform menuMain;
    public Transform menuPlay;
    public Transform menuRecord;
    public Transform menuDelete;
    public Transform menuMessage;
    public Transform hudRecording;
    public Transform hudPlaying;

    [Header("Menu recording")]

    public Transform inputAuthor;
    public Transform inputTitle;

    [Header("Menu play")]

    public Transform playDropdownSong;

    [Header("Menu delete")]

    public Transform deleteDropdownSong;

    [Header("Menu message")]

    public Transform messageText;

    [Header("Help")]

    public Transform textHelp;

    [Header("Lights")]

    public Transform rotatingLights;

    [Header("Drums")]

    public Transform drum1;
    public Transform drum2;
    public Transform drum3;
    public Transform drum4;
    public Transform drum5;
    public Transform drum6;

    Drum drum1C;
    Drum drum2C;
    Drum drum3C;
    Drum drum4C;
    Drum drum5C;
    Drum drum6C;

    InputField inputAuthorC;
    InputField inputTitleC;

    TMP_Dropdown playDropdownSongC;

    TMP_Dropdown deleteDropdownSongC;

    List<string> songIds;

    TextMeshProUGUI messageTextC;
    State messageNextState;

    Text textHelpC;

    FileStream infoFile;
    StreamWriter tWriter;
    StreamReader tReader;

    Camera mainCameraC;

    enum State
    {
        menuMain,
        menuRecord,
        menuPlay,
        menuDelete,
        menuMessage,
        recording,
        playing
    }

    State state;

    bool mainPlayPressed;
    bool mainRecordPressed;
    bool mainDeletePressed;
    bool mainExitPressed;
    bool playPlayPressed;
    bool playBackPressed;
    bool recordRecordPressed;
    bool recordBackPressed;
    bool deleteDeletePressed;
    bool deleteBackPressed;
    bool recordingEndPressed;
    bool playingStopPressed;
    bool messageAcceptPressed;

    SongManager songManagerC;

    float playTimer;
    int playLastEventIndex;

    float recordTimer;


    void Awake()
    {
        drum1C = drum1.GetComponent<Drum>();
        drum2C = drum2.GetComponent<Drum>();
        drum3C = drum3.GetComponent<Drum>();
        drum4C = drum4.GetComponent<Drum>();
        drum5C = drum5.GetComponent<Drum>();
        drum6C = drum6.GetComponent<Drum>();

        inputAuthorC = inputAuthor.GetComponent<InputField>();
        inputTitleC = inputTitle.GetComponent<InputField>();
        deleteDropdownSongC = deleteDropdownSong.GetComponent<TMP_Dropdown>();
        playDropdownSongC = playDropdownSong.GetComponent<TMP_Dropdown>();
        messageTextC = messageText.GetComponent<TextMeshProUGUI>();
        textHelpC = textHelp.GetComponent<Text>();

        mainCameraC = mainCamera.GetComponent<Camera>();
        songManagerC = songManager.GetComponent<SongManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetState(State.menuMain);
    }

    void SetState(State s)
    {
        menuMain.gameObject.SetActive(s == State.menuMain);
        menuPlay.gameObject.SetActive(s == State.menuPlay);
        menuRecord.gameObject.SetActive(s == State.menuRecord);
        menuDelete.gameObject.SetActive(s == State.menuDelete);
        menuMessage.gameObject.SetActive(s == State.menuMessage);
        hudRecording.gameObject.SetActive(s == State.recording);
        hudPlaying.gameObject.SetActive(s == State.playing);
        rotatingLights.gameObject.SetActive(s == State.playing);

        if(s == State.menuMain) { textHelpC.text = "Choose an option"; }
        else if(s == State.menuPlay) { textHelpC.text = "Choose the song you want to play"; }
        else if(s == State.menuDelete) { textHelpC.text = "Choose the song you want to delete"; }
        else if(s == State.menuRecord) { textHelpC.text = "Fill the song info to start recording"; }
        else if(s == State.menuMessage) { textHelpC.text = "Press accept to continue"; }
        else if(s == State.playing)
        {
            string title;
            string author;
            songManagerC.GetSongInfo(out title, out author);
            textHelpC.text = "Playing \"" + title + "\" by " + author;
        }
        else // s == State.recording
        {
            string title;
            string author;
            songManagerC.GetSongInfo(out title, out author);
            textHelpC.text = "Recording \"" + title + "\" by " + author;
        }

        state = s;
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.menuMain)
        {
            if(mainRecordPressed)
            {
                    inputTitleC.text = "My new song";
                    inputAuthorC.text = "Me";
                    SetState(State.menuRecord);
            }
            else if(mainPlayPressed)
            {
                if(songManagerC.GetSongIds().Count == 0)
                {   messageTextC.text = "You haven't recorded any songs yet";
                    messageNextState = State.menuMain;
                    textHelpC.text = "";
                    SetState(State.menuMessage);
                }
                else
                {
                    playDropdownSongC.ClearOptions();

                    songIds = songManagerC.GetSongIds();
                    List<string> infos = new List<string>();
                    foreach(string id in songIds)
                    {
                        string title;
                        string author;
                        songManagerC.GetSongInfo(id, out title, out author);
                        infos.Add("\"" + title + "\" by " + author);
                    }

                    playDropdownSongC.AddOptions(infos);

                    SetState(State.menuPlay);
                }
            }
            else if(mainDeletePressed)
            {
                if(songManagerC.GetSongIds().Count == 0)
                {   messageTextC.text = "You haven't recorded any songs yet";
                    messageNextState = State.menuMain;
                    SetState(State.menuMessage);
                }
                else
                {
                    deleteDropdownSongC.ClearOptions();

                    songIds = songManagerC.GetSongIds();
                    List<string> infos = new List<string>();
                    foreach(string id in songIds)
                    {
                        string title;
                        string author;
                        songManagerC.GetSongInfo(id, out title, out author);
                        infos.Add("\"" + title + "\" by " + author);
                    }

                    deleteDropdownSongC.AddOptions(infos);
                    SetState(State.menuDelete);
                }
            }
            else if(mainExitPressed) { Application.Quit(); }
        }
        else if(state == State.menuRecord)
        {
            if(recordRecordPressed)
            {
                if(inputTitleC.text.Trim().Length == 0)
                {   messageTextC.text = "You must write a title for your song";
                    messageNextState = State.menuRecord;
                    SetState(State.menuMessage);
                }
                else if(inputAuthorC.text.Trim().Length == 0)
                {
                    messageTextC.text = "You must write your author's name";
                    messageNextState = State.menuRecord;
                    SetState(State.menuMessage);
                }
                else
                {
                    songManagerC.NewSong();
                    songManagerC.SetSongInfo(inputTitleC.text, inputAuthorC.text);
                    recordTimer = 0;
                    SetState(State.recording);
                }

            }
            else if(recordBackPressed) { SetState(State.menuMain); }
        }
        else if(state == State.menuPlay)
        {
            if(playPlayPressed)
            {
                string id = songIds[playDropdownSongC.value];
                Debug.Log(id);
                songManagerC.LoadSong(id);
                playTimer = 0;
                playLastEventIndex = -1;
                SetState(State.playing);
            }
            else if(playBackPressed)
            {
                SetState(State.menuMain);
            }
        }
        else if(state == State.menuDelete)
        {
            if(deleteDeletePressed)
            {
                string id = songIds[deleteDropdownSongC.value];
                songManagerC.DeleteSong(id);

                SetState(State.menuMain);
            }
            else if(deleteBackPressed) { SetState(State.menuMain); }

        }
        else if(state == State.playing)
        {           
            List<SongEvent> events = songManagerC.GetSongEvents();

            playTimer += Time.deltaTime;

            int i = playLastEventIndex + 1;
            bool foundFutureEvent = false;
            bool foundStopEvent = false;

            while(i < events.Count && !foundFutureEvent && !foundStopEvent)
            {
                if (events[i].time <= playTimer)
                {
                    if (events[i].data < 0) { foundStopEvent = true; }
                    else if (events[i].data == 0) { drum1C.Hit(); }
                    else if (events[i].data == 1) { drum2C.Hit(); }
                    else if (events[i].data == 2) { drum3C.Hit(); }
                    else if (events[i].data == 3) { drum4C.Hit(); }
                    else if (events[i].data == 4) { drum5C.Hit(); }
                    else // events[i].data == 5
                    { drum6C.Hit(); }

                    playLastEventIndex = i;

                    i ++;
                }
                else
                {
                    foundFutureEvent = true;
                }
            }


            if(playingStopPressed || foundStopEvent) { SetState(State.menuMain); }
        }
        else if(state == State.recording)
        {
            recordTimer += Time.deltaTime;

            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = mainCameraC.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.transform == drum1) { songManagerC.AddSongEvent(recordTimer, 0); drum1C.Hit(); }
                    else if (hit.transform == drum2) { songManagerC.AddSongEvent(recordTimer, 1); drum2C.Hit(); }
                    else if (hit.transform == drum3) { songManagerC.AddSongEvent(recordTimer, 2); drum3C.Hit(); }
                    else if (hit.transform == drum4) { songManagerC.AddSongEvent(recordTimer, 3); drum4C.Hit(); }
                    else if (hit.transform == drum5) { songManagerC.AddSongEvent(recordTimer, 4); drum5C.Hit(); }
                    else if (hit.transform == drum6) { songManagerC.AddSongEvent(recordTimer, 5); drum6C.Hit(); }
                }
            }

            if (recordingEndPressed)
            {
                songManagerC.AddSongEvent(recordTimer, -1);
                songManagerC.SaveSong();
                SetState(State.menuMain);
            }
        }
        else // state == State.menuMessage
        {
            if(messageAcceptPressed) { SetState(messageNextState); }
        }        

        mainPlayPressed = false;
        mainRecordPressed = false;
        mainDeletePressed = false;
        mainExitPressed = false;
        playPlayPressed = false;
        playBackPressed = false;
        recordRecordPressed = false;
        recordBackPressed = false;
        deleteDeletePressed = false;
        deleteBackPressed = false;
        recordingEndPressed = false;
        playingStopPressed = false;
        messageAcceptPressed = false;


    }

    public void OnMenuMainPlayPressed()
    {
        mainPlayPressed = true;
    }

    public void OnMenuMainRecordPressed()
    {
        mainRecordPressed = true;
    }

    public void OnMenuMainDeletePressed()
    {
        mainDeletePressed = true;
    }

    public void OnMenuMainExitPressed()
    {
        mainExitPressed = true;
    }

    public void OnMenuPlayPlayPressed()
    {
        playPlayPressed = true;
    }

    public void OnMenuPlayBackPressed()
    {
        playBackPressed = true;
    }

    public void OnMenuRecordRecordPressed()
    {
        recordRecordPressed = true;
    }

    public void OnMenuRecordBackPressed()
    {
        recordBackPressed = true;
    }

    public void OnMenuDeleteDeletePressed()
    {
        deleteDeletePressed = true;
    }

    public void OnMenuDeleteBackPressed()
    {
        deleteBackPressed = true;
    }

    public void OnHudRecordingEndPressed()
    {
        recordingEndPressed = true;
    }

    public void OnHudPlayingStopPressed()
    {
        playingStopPressed = true;
    }

    public void OnMenuMessageAcceptPressed()
    {
        messageAcceptPressed = true;
    }
}

