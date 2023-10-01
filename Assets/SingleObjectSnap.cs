// Written by Ben Baeyens - https://www.benbaeyens.com/

// <summary>
// This script automatically snaps any component it's attached to to their corresponding values.
// </summary>

using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Ben's Script Library/Level Building/Single Object Snap")]
public class SingleObjectSnap : MonoBehaviour
{

    [SerializeField] private bool isEnabled = true;

    [SerializeField] private float snapValueX = 0.5f;
    [SerializeField] private float snapValueY = 0.5f;
    [SerializeField] private float snapValueZ = 0.5f;

    void Update()
    {
        if (!Application.isPlaying && isEnabled)
        {
            if (snapValueX != 0)
                transform.position = new Vector3(Mathf.Round(transform.position.x * (1 / snapValueX)) / (1 / snapValueX), transform.position.y, transform.position.z);

            if (snapValueY != 0)
                transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y * (1 / snapValueY)) / (1 / snapValueY), transform.position.z);

            if (snapValueZ != 0)
                transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.Round(transform.position.z * (1 / snapValueZ)) / (1 / snapValueZ));
        }
    }
}