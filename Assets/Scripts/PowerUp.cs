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
                player.GetComponent<Player>().Grow();
                break;
            case Type.Starpower:
                Debug.Log("Collected Star Power");
                player.GetComponent<Player>().Starpower();
                break;
        }
        
        Destroy(gameObject);
    }   
    
}
