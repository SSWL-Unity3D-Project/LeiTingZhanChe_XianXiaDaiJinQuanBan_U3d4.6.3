using UnityEngine;

public class TestSpawnBoss : MonoBehaviour
{
    public XKSpawnNpcPoint m_BossPoint;
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            if (m_BossPoint != null)
            {
                m_BossPoint.SpawnPointAllNpc();
            }
        }
    }
}