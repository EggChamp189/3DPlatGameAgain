using UnityEngine;

public class MovingPlatformScript : MonoBehaviour
{
    public Vector3 moveDir;
    public float speed;
    public SpriteRenderer arrow;
    public Color activeColor;
    public Color idleColor;

    private void OnCollisionEnter(Collision collision)
    {
        arrow.color = activeColor;
        collision.transform.SetParent(gameObject.transform);
    }

    private void OnCollisionStay(Collision collision)
    {
        transform.Translate(moveDir * Time.deltaTime * speed);
    }

    private void OnCollisionExit(Collision collision)
    {
        arrow.color = idleColor;
        collision.transform.SetParent(gameObject.transform.parent);
    }
}
