using UnityEngine;

public class MusicController : MonoBehaviour
{
    public AudioSource musicSource;
    private bool isMuted = false;

    public void ToggleMusic()
    {
        isMuted = !isMuted;
        musicSource.mute = isMuted;
    }
}
