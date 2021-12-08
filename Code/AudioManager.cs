using UnityEngine;

/**
 * <summary>
 * AudioManagers contain references to <see cref="AudioClip"/>s
 * and have methods that can play these sounds.
 * <br/>
 * <b>Author:</b> Josi Scarlett Whitlock
 * <br/>
 * <b>Version:</b> 1.0
 * </summary>
 */
public class AudioManager : MonoBehaviour {
    #region Fields
    /// <summary><see cref="AudioClip"/> to be played when units finish merging.</summary>
    [SerializeField]
    private AudioClip merge;
    /// <summary><see cref="AudioClip"/> to be played when a bottom button is pressed.</summary>
    [SerializeField]
    private AudioClip buttonClick;
    /// <summary><see cref="AudioClip"/> to be played when units split.</summary>
    [SerializeField]
    private AudioClip split;
    /// <summary><see cref="AudioSource"/> used to play <see cref="AudioClip"/>s.</summary>
    [SerializeField]
    private AudioSource audioSource;
    #endregion

    #region Properties
    /// <summary>Boolean controlled by a toggle that controls whether sound should be played.</summary>
    public bool SoundOn { get; set; } = true;
    #endregion

    #region Methods
    /// <summary>
    /// Plays the sound stored in <see cref="merge"/>.
    /// </summary>
    public void Merged() {
        audioSource.clip = merge;
        if (SoundOn && !audioSource.isPlaying)
            audioSource.Play();
    }

    /// <summary>
    /// Plays the sound stored in <see cref="split"/>.
    /// </summary>
    public void Split() {
        audioSource.clip = split;
        if (SoundOn)
            audioSource.Play();
    }

    /// <summary>
    /// Plays the sound stored in <see cref="buttonClick"/>.
    /// </summary>
    public void ButtonPressed() {
        audioSource.clip = buttonClick;
        if (SoundOn)
            audioSource.Play();
    }
    #endregion
}
