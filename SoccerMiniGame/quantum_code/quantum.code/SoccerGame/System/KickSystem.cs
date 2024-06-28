using Photon.Deterministic;

namespace Quantum.SoccerGame.System
{
    // This system handles the kick mechanics when a collision occurs in the soccer game
    public unsafe class KickSystem : SystemSignalsOnly, ISignalOnCollisionEnter3D
    {
        // Method called when a collision occurs in 3D space
        public void OnCollisionEnter3D(Frame f, CollisionInfo3D info)
        {
            // Check if both entities involved in the collision exist
            if (f.Exists(info.Entity) && f.Exists(info.Other))
            {
                // Try to get pointers to PlayerLink and BallFields components
                f.Unsafe.TryGetPointer<PlayerLink>(info.Entity, out var playerFields);
                f.Unsafe.TryGetPointer<BallFields>(info.Other, out var ballFields);

                // Check if both player and ball components are valid
                if (playerFields != null && ballFields != null)
                {
                    // Update the last touched player ID for the ball
                    ballFields->LastTouchedPlayerId = playerFields->Player;

                    // Apply the kick force to the ball
                    ApplyKickForce(f, info.Entity, info.Other, info.ContactPoints.Average, info.ContactNormal, 70);
                    return;
                }
            }
        }

        // Method to apply kick force to the ball
        private void ApplyKickForce(Frame frame, EntityRef playerEntity, EntityRef ballEntity, FPVector3 contactPoint, FPVector3 contactNormal, FP forceMagnitude)
        {
            // Check if both entities have PhysicsBody3D components
            if (frame.Has<PhysicsBody3D>(ballEntity) && frame.Has<PhysicsBody3D>(playerEntity))
            {
                var ballPhysics = frame.Get<PhysicsBody3D>(ballEntity);

                // Calculate the kick direction from player to ball
                FPVector3 kickDirection = (frame.Get<Transform3D>(ballEntity).Position - frame.Get<Transform3D>(playerEntity).Position).Normalized;

                // Apply force in the direction of the kick
                var force = kickDirection * forceMagnitude;

                // Update the ball's velocity with the applied force
                ballPhysics.Velocity += force;

                // Set the updated physics body back to the ball entity
                frame.Set(ballEntity, ballPhysics);
            }
        }
    }
}
