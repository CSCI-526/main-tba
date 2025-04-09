using UnityEngine;

public class PingPong : MonoBehaviour
{
    public float minScale;
    public float maxScale;
    private float elapsedTime = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime * 1.0f;
        float scale = Mathf.Lerp(minScale, maxScale, Mathf.PingPong(elapsedTime, 1.0f));
        transform.localScale = new Vector3(scale, scale, 1.0f);
    }
}
