using UnityEngine;
using UnityEngine.SceneManagement;

public class GoalScript : MonoBehaviour
{
    public GameObject goal;
    bool goalActive = false;

    public void EnableGoal() {
        goal.SetActive(true);
        goalActive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player") && goalActive) {
            goalActive = false;
            Debug.Log("Won Level!");
            MenuScript.ChangeData(1, PlayerManagerScript.coinsCollected);
            SceneManager.LoadScene(collision.gameObject.GetComponent<PlayerManagerScript>().nextLevel);
        }
    }
}
