using System;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region Actions

    public static event Action OnStartGameButtonPressedAction;

    #endregion
    

    #region Methods

    public void StartGameButtonPressed ( ) => OnStartGameButtonPressedAction?.Invoke ( );

    public void ExitGameButtonPressed ( ) => Application.Quit ( );

    #endregion
}
