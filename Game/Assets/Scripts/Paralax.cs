using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
    [SerializeField] Vector2 paralax;
    [SerializeField] new GameObject camera;
    Vector2 initPosition;

    private void Start() => initPosition = transform.position;

    void Update()
    {
        var distance = camera.transform.position * paralax;
        transform.position = initPosition + distance;
    }
}
