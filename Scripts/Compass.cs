using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    public float speed = 10f;
    private bool localitationActivated = false;

    void Start()
    {
        StartCoroutine(StartAsync());
    }
    IEnumerator StartAsync()
    {
        Debug.Log("entra");
        
        //yield return new WaitForSeconds(4);   Esto se usa para Unity Remote 5

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Sin permisos");
            yield break;
        }

        Input.location.Start();

        int maxWait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(1);
            maxWait--;
        }

        if (maxWait < 1)
        {
            print("Timed out");
            yield break;
        }
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            print("Unable to determine device location");
            yield break;
        }
        //yield return new WaitForSeconds(2);    Esto se usa para Unity Remote 5

        Debug.Log("Running");
        Text text = GameObject.FindGameObjectWithTag("Text").GetComponent<Text>();
        text.text += "    " + Input.location.lastData.latitude + "  " + Input.location.lastData.longitude + "  " + Input.location.lastData.altitude;
        print("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

        localitationActivated = true;
        Input.compass.enabled = true;
    }

    void Update()
    {

        Vector3 dir = Vector3.zero;

        dir.x = Input.acceleration.x;
        dir.y = Input.acceleration.y;

        dir *= Time.deltaTime;

        transform.Translate(dir * speed);

        if (localitationActivated)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, Input.compass.trueHeading, transform.eulerAngles.z), Time.deltaTime * 7);
        }
    }

     void OnDestroy()
    {
        Input.location.Stop();
    }
}

