using UnityEngine;

   public class LinkedMovement
{
    public LinkedMovement(float movement, float mass)
    {
        this.movement = movement;
        this.mass = mass;
    }

    public float movement { get; set; }
    public float mass { get; set; }
}

