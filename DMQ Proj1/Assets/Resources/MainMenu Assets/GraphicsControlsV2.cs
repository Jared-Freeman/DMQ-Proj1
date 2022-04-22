using System;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DuloGames.UI.Tweens;


public class GraphicsControlsV2 : MonoBehaviour
{
    [SerializeField] private DuloGames.UI.UISelectField ScreenResolutions;

    [SerializeField] private DuloGames.UI.UISelectField GraphicQualtiy;

    [SerializeField] private Toggle FSToggle;

    //Create Resolutions array
    Resolution[] resolutions;

    // Start is called before the first frame update
    protected void Start()
    {
        setupResolutionsSelectField();
        setupGraphicsQualitySelectField();
        setupFullScreenToggle();
    }

    protected void OnEnable()
    {
        addGraphicsResolutionListener();
        addGraphicsQualityListener();
    }

    protected void OnDisable()
    {
        removeGraphicsResolutionsListener();
        removeGraphicsQualityListener();
    }

    /// <summary>
    /// Function to change in game screen resolution
    /// </summary>
    /// <param name="resolutionIndex"></param>
    /// <param name="option"></param>
    public void setGameResolution(int resolutionIndex, string option)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
    }

    /// <summary>
    /// Function to toggle on/off full screen setting
    /// </summary>
    /// <param name="isFullScreen"></param>
    public void setFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }

    /// <summary>
    /// Funtction to change graphics quality setting
    /// </summary>
    /// <param name="index"></param>
    public void setGraphicsQuality(int index, string option)
    {
        QualitySettings.SetQualityLevel(index);
    }

    /// <summary>
    /// Creates a event listener to change screen resolution
    /// </summary>
    void addGraphicsResolutionListener()
    {
        if (this.ScreenResolutions == null)
        {
            return;
        }

        this.ScreenResolutions.onChange.AddListener(setGameResolution);
    }

    /// <summary>
    /// Creates an event listener to change graphics quality
    /// </summary>
    void addGraphicsQualityListener()
    {
        if (this.GraphicQualtiy == null)
        {
            return;
        }

        this.GraphicQualtiy.onChange.AddListener(setGraphicsQuality);
    }

    /// <summary>
    /// Removes event listener for screen resolution changes
    /// </summary>
    void removeGraphicsResolutionsListener()
    {
        if (this.ScreenResolutions == null)
        {
            return;
        }
        this.ScreenResolutions.onChange.RemoveListener(setGameResolution);
    }

    /// <summary>
    /// Removes event listener for graphics quality changes
    /// </summary>
    void removeGraphicsQualityListener()
    {
        if (this.GraphicQualtiy == null)
        {
            return;
        }
        this.GraphicQualtiy.onChange.RemoveListener(setGameResolution);
    }


    /// <summary>
    /// Sets up the resolution SelectField
    /// </summary>
    void setupResolutionsSelectField()
    {
        if (this.ScreenResolutions == null)
        {
            Debug.Log("SelectField for Resolutions not found");
            return;
        }

        //clears Select Field Options
        this.ScreenResolutions.ClearOptions();

        //Populate resolutions array with system available resolutions
        resolutions = Screen.resolutions;

        //Adds resolutions to SelectField options
        foreach (Resolution res in resolutions)
        {
            this.ScreenResolutions.AddOption(res.width + "x" + res.height + " @ " + res.refreshRate + "Hz");
        }

        //Gets current resolution
        Resolution currentResolution = Screen.currentResolution;

        //Sets SelectField to current resolution
        this.ScreenResolutions.SelectOption(currentResolution.width + "x" + currentResolution.height + " @ "
            + currentResolution.refreshRate + "Hz");
    }

    /// <summary>
    /// Sets up the graphics quality SelectField
    /// </summary>
    void setupGraphicsQualitySelectField()
    {
        if (this.GraphicQualtiy == null)
        {
            Debug.Log("SelectField for Graphics Qualtiy not found");
            return;
        }

        this.GraphicQualtiy.SelectOptionByIndex(QualitySettings.GetQualityLevel());
    }

    /// <summary>
    /// Setup toggle button for full screen mode
    /// </summary>
    void setupFullScreenToggle()
    {
        if(FSToggle == null)
        {
            return ;
        }
        FSToggle.isOn = Screen.fullScreen;
    }
}
