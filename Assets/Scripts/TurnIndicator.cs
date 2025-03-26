using UnityEngine;

public class TurnIndicator : MonoBehaviour
{
    [SerializeField]
    private GameObject light1;
    SpriteRenderer light1Renderer;
    [SerializeField]
    private GameObject light2;
    SpriteRenderer light2Renderer;
    private int turnsRemaining;

    void Start()
    {
        turnsRemaining = 2;
        light1Renderer = light1.GetComponent<SpriteRenderer>();
        light2Renderer = light2.GetComponent<SpriteRenderer>();
        AllLightsOn();
    }

    public void AllLightsOn()
    {
        turnsRemaining = 2;
        light1Renderer.color = new Color(1f, 0f, 0f);
        light2Renderer.color = new Color(1f, 0f, 0f);
    }

    public void DecrementTurn()
    {
        if (turnsRemaining == 2)
        {
            turnsRemaining -= 1;
            light1Renderer.color = new Color(0.3f, 0.3f, 0.3f);
        }
        else if (turnsRemaining == 1)
        {
            turnsRemaining -= 1;
            light2Renderer.color = new Color(0.3f, 0.3f, 0.3f);
        }
    }
}
