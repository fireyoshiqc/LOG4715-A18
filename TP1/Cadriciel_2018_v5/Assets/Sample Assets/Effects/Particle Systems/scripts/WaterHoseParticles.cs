using System.Collections.Generic;
using UnityEngine;

public class WaterHoseParticles : MonoBehaviour {
	
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>(16);
	
	public static float lastSoundTime;
	public float force = 1;
	
    void OnParticleCollision(GameObject other) {
		
        int safeLength = GetComponent<ParticleSystem>().GetSafeCollisionEventSize();

        if (collisionEvents.Count < safeLength) 
		{
            collisionEvents = new List<ParticleCollisionEvent>(safeLength);
		}
        
        int numCollisionEvents = GetComponent<ParticleSystem>().GetCollisionEvents(other, collisionEvents);
        int i = 0;

        while (i < numCollisionEvents)
		{
		
			if (Time.time > lastSoundTime + 0.2f)
			{
				lastSoundTime = Time.time;
			}
			
			var col = collisionEvents[i].colliderComponent;

			if (col.GetComponent<Rigidbody>() != null)
			{
                Vector3 vel = collisionEvents[i].velocity;
                col.GetComponent<Rigidbody>().AddForce(vel*force,ForceMode.Impulse);
            }

			other.BroadcastMessage("Extinguish",SendMessageOptions.DontRequireReceiver);

            i++;
        }
    }
}