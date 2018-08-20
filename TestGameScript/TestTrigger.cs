using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    void OnTriggerExit(Collider other)
    {
        Debug.Log("************** Collider == " + other.gameObject.name + " ***********name ======== " + gameObject.name);
    }
}