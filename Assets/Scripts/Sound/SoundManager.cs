using System;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Fields

    [ SerializeField ] private Sound [ ] sounds;

    [ SerializeField ] private Music [ ] musics;

    public static SoundManager Instance { get { return _instance; } }
    private static SoundManager _instance;

    #endregion


    #region Methods

    private void Awake ( ) 
    {
        GameStateManager.OnGameStateChangeAction += OnGameStateChange;
        
        GameManager.StartBattleAction += BeginFightMusic;

        BoardManager.OnPlayerWinAction += StopFightMusic;
        BoardManager.OnPlayerLoseAction += StopFightMusic;

        if ( _instance != null && _instance != this ) 
            Destroy ( gameObject );
        else 
            _instance = this;

        sounds.ToList ( ).ForEach ( sound => sound.Init ( gameObject.AddComponent<AudioSource> ( ) ) );

        musics.ToList ( ).ForEach ( music => music.Init ( gameObject.AddComponent<AudioSource> ( ) ) );
    }

    private void OnDestroy ( ) 
    {
        GameStateManager.OnGameStateChangeAction -= OnGameStateChange;
        
        GameManager.StartBattleAction -= BeginFightMusic;
        
        BoardManager.OnPlayerWinAction -= StopFightMusic;
        BoardManager.OnPlayerLoseAction -= StopFightMusic;
    }


    #region Sound

    public void Play ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Play ( );

    public void Stop ( SoundType type ) => Array.Find ( sounds, s => s.Type == type )?.Stop ( );

    #endregion

    
    #region Music

    private void OnGameStateChange ( GameState state ) 
    {
        switch ( state ) 
        {
            case GameState.MainMenu:
                Play ( MusicType.MainMenu );
                Stop ( MusicType.Fight );
                break;

            case GameState.Cutscene:
                Stop ( MusicType.MainMenu );
                Stop ( MusicType.Fight );
                break;

            case GameState.InGame:
                Stop ( MusicType.MainMenu );
                Stop ( MusicType.Fight );
                break;
        }
    }

    private void BeginFightMusic ( Level _ ) => Play ( MusicType.Fight );

    private void StopFightMusic ( Level _ ) => Stop ( MusicType.Fight );

    private void Play ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.FadeInPlay ( );

    private void Stop ( MusicType type ) => Array.Find ( musics, m => m.Type == type )?.FadeOutStop ( );

    #endregion


    #endregion
}