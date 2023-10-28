
using UnityEngine;
using UnityEngine.AI;

public class PlayerChaser : MonoBehaviour
{
    private NavMeshAgent navigation;
    public GameObject player;
    public Material shaderMaterial;
    // Start is called before the first frame update
    void Start()
    {

        navigation = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navigation.destination = player.transform.position;
        float strength = 2f / ((gameObject.transform.position - player.transform.position).magnitude + .1f);
        shaderMaterial.SetFloat("_Vignette_Strength" , strength); 
    }

    void OnApplicationQuit()
    {
        //removes merge conflicts in the material params
        shaderMaterial.SetFloat("_Vignette_Strength", 0);
    }
}
