using UnityEngine;
using UnityEngine.Events;

namespace ModernMalick.Huntwave.Components
{
    public class Impact : MonoBehaviour
    {
        public UnityEvent<Vector3> onImpact;

        public void CreateImpact(Vector3 position)
        {
            onImpact.Invoke(position);
        }

        public static void TryCreateImpact(GameObject other, Vector3 position)
        {
            var impact = other.GetComponent<Impact>();
            if (impact == null) return;
            impact.CreateImpact(position);
        }
    }
}