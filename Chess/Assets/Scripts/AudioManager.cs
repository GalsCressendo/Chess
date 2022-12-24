using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AudioManager : MonoBehaviour
{
    public List<Sound> sounds;
    public static AudioManager instance;

    public const string BOARD_START_AUDIO = "BoardStart";
    public const string MOVE_PIECE_1 = "MovePiece1";
    public const string MOVE_PIECE_2 = "MovePiece2";
    public const string MOVE_PIECE_3 = "MovePiece3";
    public const string MOVE_PIECE_4 = "MovePiece4";
    public const string WIN_AUDIO = "Win";

    private void Awake()
    {

        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
        }
    }

    public void PlayAudio(string name)
    {
        Sound sound = sounds.Find(s => s.name == name);
        if(sound == null)
        {
            return;
        }

        sound.source.Play();

    }

    public void GetMovePieceAudio()
    {
        var moveAudio = (from s in sounds
                         where s.name.Contains("MovePiece")
                         select s.name).ToList();

        int random = UnityEngine.Random.Range(0, moveAudio.Count);

        PlayAudio(moveAudio[random]);
    }

}
