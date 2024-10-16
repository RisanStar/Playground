using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Header("Assets")]
    [SerializeField] private GameObject cam;

    [Header("Parallax")]
    [SerializeField] private float paraEffect;
    private float length, startpos;

    private void Start()
    {
        startpos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        //float temp = (cam.transform.position.x * (1 - paraEffect));
        float dist = (cam.transform.position.x * paraEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        /*
        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
        */
    }
}