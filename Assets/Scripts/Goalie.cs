using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goalie : MonoBehaviour
{

    float speed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 1.76)
            speed = -1;
        else if (transform.position.x < -1.87)
            speed = 1;
        transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
    }
}
