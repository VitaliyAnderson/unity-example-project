using System.Linq;
using UnityEngine;

public class DisplayService : IServiceSingleton
{
    public DisplayService()
    {
    }

    public Resolution[] GetResolutions()
    {
        return Screen.resolutions;
    }

    public Resolution GetResolutionDefault()
    {
        return GetResolutions()
            .Where(x => x.width == Screen.currentResolution.width &&
                x.height == Screen.currentResolution.height &&
                x.refreshRateRatio.value == Screen.currentResolution.refreshRateRatio.value)
            .FirstOrDefault();
    }

    public void SetResolution(Resolution Resolution)
    {
        if (Resolution.width != Screen.currentResolution.width ||
            Resolution.height != Screen.currentResolution.height ||
            Resolution.refreshRateRatio.value != Screen.currentResolution.refreshRateRatio.value)
        {
            Screen.SetResolution(
                Resolution.width,
                Resolution.height,
                Screen.fullScreenMode,
                Resolution.refreshRateRatio);
        }
    }

    public bool GetFullscreenDefault()
    {
        return true;
    }

    public void SetFullscreen(bool value)
    {
        if (value != Screen.fullScreen)
        {
            Screen.fullScreen = value;
        }
    }
}
