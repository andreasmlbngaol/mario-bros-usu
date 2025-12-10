using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{

    public enum Type
    {
        MagicMushroom,
        Starpower,
    }

    public Type type;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Collect(other.gameObject);
        }
    }

    private void Collect(GameObject player)
    {
        switch (type)
        {
            case Type.MagicMushroom:
                // Implement magic mushroom effect
                break;
            case Type.Starpower:
                // Implement starpower effect
                break;
        }
    }
    // Start is called before the first frame update
    
}
