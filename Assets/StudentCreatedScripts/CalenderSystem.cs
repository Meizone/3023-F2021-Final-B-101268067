using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CalenderSystem : MonoBehaviour
{

    [SerializeField] public List<Season> Seasons;
    [SerializeField] private int Season = 0;
    [SerializeField] private int Day = 0;
    [SerializeField] private Text DateTextBox;
    [SerializeField] private Text EventInformationBox;


    // Start is called before the first frame update
    void Start()
    {
        // Set the Current day to the Base Starting day, Default: Season 0, Day 0.
        SetCurrentDay(Season,Day);

        // Go through all the Seasons and Each day of the season and run through the probability set by each Date for
        // X Event to occur.
        for(int i = 0; i != Seasons.Count; i++)
        {
            for(int j = 0; j != Seasons[i].Days.Count; j++)
            {
                if(Random.Range(0,100) < Seasons[i].Days[j].Probability)
                {
                    Seasons[i].Days[j].EventNotes =  Seasons[i].Days[j].EventHappenedNote;
                    Seasons[i].Days[j].DidEventOccur = true;
                }
                else
                {
                    Seasons[i].Days[j].EventNotes = Seasons[i].Days[j].EventDidntHappenNote;
                    Seasons[i].Days[j].DidEventOccur = false; 
                }
            }
        }

        // Set EventVisual Target for each Season after setting the First one.
        for(int i = 1; i != Seasons.Count; i++)
        {
            for(int j = 0; j != Seasons[i].Days.Count; j++)
            {
                // Check if the Days in Season[i] has the same number days as Season[0]
                if(Seasons[i].Days.Count == Seasons[0].Days.Count)
                    Seasons[i].Days[j].EventVisualTarget = Seasons[0].Days[j].EventVisualTarget;
            }
        }
        

        // Update All the Visuals to correspond with the Starting Month.
        UpdateVisuals(Season);
    }


    // Sets the Day and runs the Event for that Day if the Event did occur based off Probability.
    public void SetCurrentDay(int currentSeason, int currentDay)
    {
        // Set the Current Date Text to the Current Date.
        DateTextBox.text = "Season: " + Seasons[currentSeason].SeasonName + " Day: "+  (Day+1);

        // Run the Daily Event
        if(Seasons[currentSeason].Days[currentDay].DidEventOccur)
        {
            Seasons[Season].Days[currentDay].InvokeDailyEvent();
        }
    }

    // Increment the Day and set the current do to the Incremented Day.
    public void NextDay()
    {

        // If it is the End of the week Set the Day back to the beginning
        // and Increment the Season
        if(Day == (Seasons[Season].Days.Count-1)) 
        {
            // Check if by Incrementing the Season we go out of Range of our Seasons
            if(Season == Seasons.Count-1)
            {
                Season = 0;
                UpdateVisuals(Season);
            }
            else
                Season += 1;


            Day = 0;
            // Run the Update Visual Function if Incrementing Seasons
            UpdateVisuals(Season);
        }
        else
            Day += 1;

        // Set the current day to the new date.
        SetCurrentDay(Season, Day);
    }

    public void NextSeason()
    {
        // Increment the Season, If 4 Seasons have passed set the Season back to 0.
        if(Season == (Seasons.Count-1))
        {
            Season = 0;
            UpdateVisuals(Season);
        }
        else
        {
            Season += 1;
            UpdateVisuals(Season);
        }
        

        // Set the current day to the new date.
        SetCurrentDay(Season, Day);
    }


    public void UpdateVisuals(int Season)
    {
        // Go through all the days in the Season
        for(int i = 0; i != Seasons[Season].Days.Count; i++)
        {
            // Make sure EventVisualTarget and EventVisual are both not Null
            if(Seasons[Season].Days[i].EventVisualTarget != null && Seasons[Season].Days[i].EventVisual != null)
            {
                // Check if the Event Occured
                if(Seasons[Season].Days[i].DidEventOccur)
                {
                    // Turn on the SpriteRenderer and set the EventVisualTarget sprite to the Event Visual
                    Seasons[Season].Days[i].EventVisualTarget.sprite = Seasons[Season].Days[i].EventVisual;
                    Seasons[Season].Days[i].EventVisualTarget.enabled = true;
                }
                else
                {
                    // Disable the SpriteRenderer. Event has not occured.
                    Seasons[Season].Days[i].EventVisualTarget.enabled = false;
                }

            }
        }
    }





    // Listener for Buttons, When button is clicked set the EventInformationBox text to the event information.
    public void CheckDateEvent(int CheckingDate)
    {
        EventInformationBox.text = 
        "Season: " + (Seasons[Season].SeasonName) + "\n" + 
        " Day: " + CheckingDate + "\n" + 
        (Seasons[Season].Days[CheckingDate].EventName) + "\n" + 
        Seasons[Season].Days[CheckingDate].EventNotes;
        
    }
}


// Season Class, Holds Information Generally about the Season
[System.Serializable]
public class Season
{
    [Header("Basic Season Information")]
    public string SeasonName;
    // List of Each Day in said season
    public List<DayEffects> Days;

}

[System.Serializable]
public class DayEffects
{

    [Header("Event Notes")]
    [SerializeField] public string EventNotes;
    [SerializeField] public string EventName;
    [SerializeField] public int Probability;
    [SerializeField] public string EventHappenedNote;
    [SerializeField] public string EventDidntHappenNote;
    [SerializeField] public bool DidEventOccur;

    [Header("Visual Event Notifier")]
    [SerializeField] public Image EventVisualTarget;
    [SerializeField] public Sprite EventVisual;


    [Header("Events Notifier")]
    public UnityEvent functionEvent;
    public void InvokeDailyEvent()
    {
        if(functionEvent != null)
        {
            functionEvent.Invoke();
        }
    }



}


