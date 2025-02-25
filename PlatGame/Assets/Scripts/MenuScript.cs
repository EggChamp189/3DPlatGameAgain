using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public TMP_Text text;
    public static string toDisplay = "Controls: WASD to move, Left Click to attack, Left Shift to run, Space to Jump. Esc allows you to click out of the screen. Attack while moving to spin forward! Attack in midair to bounce on enemies and boxes! Jump on the red springs to get launched. You only have 3 lives for three levels. Good Luck!";
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
            _ => "Controls: WASD to move, Left Click to attack, Left Shift to run, Space to Jump. Esc allows you to click out of the screen. Attack while moving to spin forward! Attack in midair to bounce on enemies and boxes! Jump on the red springs to get launched. You only have 3 lives for three levels. Good Luck!",
        };
    }

    public void LoadScene(string scene) { SceneManager.LoadScene(scene); }
    public void QuitGame() { Application.Quit(); }
}
