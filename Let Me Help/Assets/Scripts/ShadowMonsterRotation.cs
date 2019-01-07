using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowMonsterRotation : MonoBehaviour
{
    [SerializeField]
    private float RotationRange;
    [SerializeField]
    private float RotationSpeed;
    [SerializeField] [Range(0,1.00f)]
    private float RotationFlow;

    public float timer = 0;
    public float direction = 0;
    public int locDir = 1;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        //    if(timer>=RotationRange)
        //    {
        //        direction -= RotationFlow;
        //        direction = Mathf.Max(-1.0f, direction);

        //        if(direction == -1.0f)
        //        {
        //            timer = 0;
        //        }
        //    }

        //    if(timer<= - RotationRange)
        //    {
        //        direction += RotationFlow;
        //        direction = Mathf.Min(1.0f, direction);

        //        if(direction == 1.0f)
        //        {
        //            timer = 0;
        //        }
        //    }

        if(timer>=RotationRange)
        {
            locDir *= -1;
            timer = -RotationRange;
        }

        direction += (locDir * RotationFlow);

        direction = Mathf.Clamp(direction, -1.0f, 1.0f);

        transform.Rotate(Vector3.right, direction * RotationSpeed);

        timer += Time.deltaTime;
    }
}
