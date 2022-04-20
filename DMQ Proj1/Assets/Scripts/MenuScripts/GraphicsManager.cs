using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsManager : MonoBehaviour
{
    //Create dropdown variable
    public Dropdown screenResolution;
    //public TMPro.TMP_Dropdown dropdown;
    public static float brightness;
    public Dropdown quality;

    //Create Resolutions array
    Resolution[] resolutions;
    void Start()
    {
        //var dropdown = transform.GetComponent<Dropdown>();
        screenResolution.options.Clear();
        //Populate resolutions array with system available resolutions
        resolutions = Screen.resolutions;
        //Create a string list
        List<string> items = new List<string>();
        //Create Resolution index variable
        int currentResoluionIndex = 0;
        //Iterate through resolutions array
        for(int i = 0; i < resolutions.Length; i++)
        {
            //Create string from eash resolution in array
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate;
            //Add string to list
            items.Add(option);
            //Test if current resolution matches string
            if(resolutions[i].width == Screen.currentResolution.width 
                && resolutions[i].height == Screen.currentResolution.height)
            {
                //if Succeeds update resolution index value
                currentResoluionIndex = i;
            }
        }
        //Iterate through string list
        foreach(var item in items)
        {
            //add eash item to dropdown menu
            screenResolution.options.Add(new Dropdown.OptionData() { text = item });
        }
        //update current dropdown menu index value
        screenResolution.value = currentResoluionIndex;
        //Refresh dropdown to reflect correct selection
        screenResolution.RefreshShownValue();
        //Add listener to dropdown
        screenResolution.onValueChanged.AddListener(delegate { setGameResolution(screenResolution.value); });
        quality.value = QualitySettings.GetQualityLevel();
        Debug.Log(quality.value);
        quality.RefreshShownValue();
    }
    //Function to set Game Resolution
    public void setGameResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    //Function to set Full Screen
    public void setFullScreen( bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    //Function to set Graphic Quality Level
    public void setGraphicsQuality(int index)
    {
        QualitySettings.SetQualityLevel(index + 1);
    }
}
