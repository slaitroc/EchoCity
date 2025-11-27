using TMPro;
using UnityEngine;

public class Attraction : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float intensity = 1f;
    [SerializeField] private float intensityFactor = 1f;
    [SerializeField] private float rangeFactor = 1f;
    [SerializeField] private float decay = 1f;

    [SerializeField] private TextMeshPro attractionText;
    [SerializeField] private GameObject player;
    [SerializeField] private float distance;
    [SerializeField] private float attraction;
    private float duration;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
        attraction = 0f;
        attractionText.text = GetComponentInChildren<TextMeshPro>().text;
        attractionText.text = attraction.ToString("F2");
    }

    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);

        if (!AttractionManager.calculateAttraction)
        {
            if (attraction > 0f)
                attraction -= decay * 0.08f * Time.deltaTime;
            else
                attraction = 0f;
        }
        else
        {
            attraction += intensity
                          * intensityFactor
                          / Mathf.Pow((distance + 0.01f) * rangeFactor, decay)
                          * Time.deltaTime;
        }
        attractionText.text = attraction.ToString("F2");

    }

}
