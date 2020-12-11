using UnityEngine;

public class Hideable : MonoBehaviour, IHideable {

    public void OnFOVEnter() {
        if(GetComponent<Renderer>()!= null)
        {
            GetComponent<Renderer>().enabled = true;
        }
        else
        {
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
            GetComponentInChildren<Canvas>().enabled = true;
        }
        
    }
    public void OnFOVLeave() {
        if (GetComponent<Renderer>() != null)
        {
            GetComponent<Renderer>().enabled = false;
        }
        else
        {
            GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            GetComponentInChildren<Canvas>().enabled = false;
        }
    }
}
