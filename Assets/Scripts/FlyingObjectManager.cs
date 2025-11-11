using UnityEngine;

public class FlyingObjectManager : MonoBehaviour
{
    public void DestroyAllFlyingObjects()
    {
        FlyeingObjectScript[] flyeingObjects = 
            Object.FindObjectsByType<FlyeingObjectScript>(
                FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach(FlyeingObjectScript obj in flyeingObjects)
        {
            if(obj == null)
                continue;

            if (obj.CompareTag("bomb"))
            {
                obj.TriggerExplosion();
            }
            else
            {
                obj.StartToDestroy(Color.cyan);
            }
        }
        Debug.Log("Atgrieas no metodes!");
    }
}
