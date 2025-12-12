using Unity.VisualScripting;
using UnityEngine;

public class Leaf_Generator : MonoBehaviour
{
    public GameObject leaf;
    void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            Vector3 leafPos = new Vector3(gameObject.transform.position.x +i*2,gameObject.transform.position.y,gameObject.transform.position.z);
            Instantiate(leaf, leafPos, gameObject.transform.rotation);
        }
    }

    void Update()
    {
        
    }
}
