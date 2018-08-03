using UnityEngine;

public class TestNpcAni : MonoBehaviour
{
    public Animator AnimatorCom;
	void OnTriggerFireAnimation()
	{
		Debug.Log("Unity:"+"OnTriggerFireAnimation...");
        PlayRun3();
    }

    void PlayRun3()
    {
        AnimatorCom.SetBool("Root1", false);
        AnimatorCom.SetBool("Run1", false);
        AnimatorCom.SetBool("Run2", false);
        AnimatorCom.SetBool("Run3", false);
        AnimatorCom.SetBool("Run4", false);
        AnimatorCom.SetBool("Fire1", false);
        AnimatorCom.SetBool("Fire2", false);
        AnimatorCom.SetBool("Run3", true);
    }
}