using UnityEngine;

/// <summary>
/// VocalReverb works with the Steam Phonon API to create realistic voal reverb
/// via the users microphone. Place this script where the users mouth would be.
/// </summary>
[RequireComponent(typeof(Phonon.PhononSource), typeof(AudioSource))]
public class VocalReverb : MonoBehaviour
{
    [SerializeField]
    private bool playOnStart = true;

    // changing these starts / stops the effect
    public bool stopMicPlayback = false;
    public bool startMicPlayBack = false;

    private bool microphoneListenerOn = false;

    private AudioSource audioSource;

    [SerializeField]
    private int playBackLatency = 10; // prevents latency issues, 10 works.

    private float timeSinceEnable = 0f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //start the microphone listener
        if (playOnStart)
        {
            StartMicListener();
        }
    }

    void Update()
    {
        // use as buttons in the inspector or call remotely.
        if (stopMicPlayback)
        {
            StopMicListener();
        }
        if (startMicPlayBack)
        {
            StartMicListener();
        }

        stopMicPlayback = false;
        startMicPlayBack = false;

        PlayMic();  // must be in update
    }

    private void StopMicListener()
    {
        microphoneListenerOn = false;
        audioSource.Stop();
        audioSource.clip = null;

        Microphone.End(null);
    }

    private void StartMicListener()
    {
        microphoneListenerOn = true;
        audioSource.clip = null;
        timeSinceEnable = Time.time;
    }

    // syncronises the mic with the audiosource playback and plays it.
    void PlayMic()
    {
        if (!microphoneListenerOn)
            return;

        //pause a little before setting clip to avoid lag and bugginess
        if (Time.time - timeSinceEnable > 0.5f && !Microphone.IsRecording(null))
        {
            audioSource.clip = Microphone.Start(null, true, playBackLatency, 44100);

            //ensure we have a valid mic position
            while (!(Microphone.GetPosition(null) > 0))
                continue;

            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        StopMicListener();
    }

    private void OnEnable()
    {
        StartMicListener();
    }

    private void OnValidate()
    {
        if (GetComponent<Phonon.PhononSource>().directMixFraction > 0f)
        {
            Debug.LogWarning("Voice Reverb requires Phonon directMixFraction to be zero");
        }
    }
}