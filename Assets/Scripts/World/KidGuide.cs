using UnityEngine;

public class KidGuide : MonoBehaviour
{
    public Transform[] path;
    public float speed = 2f;
    public float waitDistance = 6f;
    public Transform player;

    private int index = 0;
    private bool guiding = false;

    public void StartGuiding()
    {
        guiding = true;
    }

    void Update()
    {
        if (!guiding || index >= path.Length) return;

        if (Vector3.Distance(player.position, transform.position) > waitDistance)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            path[index].position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, path[index].position) < 0.3f)
            index++;
    }
}
