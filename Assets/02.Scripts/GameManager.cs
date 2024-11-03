using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PitchType
{ Do, Re, Mi, Fa, So, La, Ti }
public enum ColorType
{ Red, Orange, Yellow, Green, Blue, Indigo, Violet }

public class GameManager : MonoBehaviour
{
    // GameManager itself (Singleton)
    public static GameManager gm { get; private set; }
    
    // DontDestroyOnLoad Managers
    public static SoundManager sm;
    
    // Other Managers
    public static EffectManager em;

    public Transform Player;
    
    public static readonly Color[] colors = {
        Color.red,
        new Color(1f, 0.5f, 0f),  // Orange
        Color.yellow,
        Color.green,
        Color.blue,
        new Color(0.29f, 0f, 0.51f),  // Indigo
        new Color(0.56f, 0f, 1f)      // Violet
    };
    
    
    // Singleton pattern & Find SoundManager
    private void Awake()
    {
        if (gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
            
            sm = GetComponentInChildren<SoundManager>();
            em = GetComponentInChildren<EffectManager>();
        }
        else if (gm != this)
        {
            Destroy(gameObject);
        }
    }
    
    // Called every time when some scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        em = FindObjectOfType<EffectManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    # region Test
    // Update is called once per frame
    void Update()
    {
        
    }
    # endregion
}
