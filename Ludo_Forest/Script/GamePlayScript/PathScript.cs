using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    public Transform pathtransfrom;
    public Transform layoutTransform;

    public void Check()
    {
        Debug.Log("Moving..");
    }

    public void SetMultiPosition()
    {
        if (pathtransfrom.childCount > 0)
        {
            // Create a temporary list to avoid modifying the hierarchy while iterating
            List<Transform> children = new List<Transform>();

            for (int i = 0; i < pathtransfrom.childCount; i++)
            {
                children.Add(pathtransfrom.GetChild(i));
            }

            foreach (Transform child in children)
            {
                child.SetParent(layoutTransform);
                child.SetAsLastSibling(); // Optional: bring to front
            }
        }
    }
}
