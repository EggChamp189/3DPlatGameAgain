using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public TMP_Text text;
    public static string toDisplay = "Controls: WASD to move, Left Click to attack, Space to Jump";
    public static int screen = 0;
    public static int highscore = 0;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        text.text = toDisplay;
    }

    public static void ChangeData(int reason, int coins) {
        screen = reason;

        if (highscore < coins)
            highscore = coins;

        toDisplay = screen switch
        {
            // won
            1 => "You Won! Your highscore is " + highscore + " Coins. Good Job!",
            // lost
            2 => "You Lost... Your highscore is " + highscore + " Coins.",
            // default
            _ => "Controls: WASD to move, Left Click to attack, Space to Jump",
        };
    }

    public void LoadScene(string scene) { SceneManager.LoadScene(scene); }
    public void QuitGame() { Application.Quit(); }
}
