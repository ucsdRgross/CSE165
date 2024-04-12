using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class wall : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject brick = GameObject.Find("Brick");
        for (int h = 0; h < 10; h++)
        {
            float r = 7;
            int o = 40;
            for (int i = 0; i < o; i++)
            {
                GameObject new_brick = Instantiate(brick);
                float theta = 2.0f * Mathf.PI * (((float)i + (0.5f * (float)(h % 2))) / o);
                new_brick.transform.position = new Vector3(r * Mathf.Cos(theta), 
                    0.5f + (float)h,
                    r * Mathf.Sin(theta));
                new_brick.transform.LookAt(new Vector3(0, 0.5f + (float)h, 0));
                //new_brick.transform.eulerAngles = new Vector3(0, (2f * Mathf.PI * (float)i / o) * Mathf.Rad2Deg, 0);
                //new_brick.SetActive(true);
                //Debug.Log(i.ToString());
            }
        }
        
        brick.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
