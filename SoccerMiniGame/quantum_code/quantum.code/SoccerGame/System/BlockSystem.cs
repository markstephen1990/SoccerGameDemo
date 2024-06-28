using Photon.Deterministic;

namespace Quantum.SoccerGame.System
{
    // This system handles player blocking mechanics in the soccer game
    public unsafe class BlockSystem : SystemMainThreadFilter<BlockSystem.Filter>
    {
        public struct Filter
        {
            public EntityRef Entity;
            public CharacterController3D* CharacterController;
            public Transform3D* Transform;
            public PlayerLink* Link;
        }

        public override void Update(Frame f, ref Filter filter)
        {
            // Get player input for the current player
            var input = f.GetPlayerInput(filter.Link->Player);

            // Check if the jump button was pressed
            if (input->Jump.WasPressed)
            {
                // Check if the player has already blocked
                if (!f.Global->Players[filter.Link->Player].HasAlreadyBlocked)
                {
                    // Mark the player as having blocked
                    f.Global->Players[filter.Link->Player].HasAlreadyBlocked = true;

                    // Get the physics body of the ball
                    var ballPhysics = f.Get<PhysicsBody3D>(f.Global->Ball);

                    // Set the ball's velocity to a new value, effectively blocking it
                    ballPhysics.Velocity = new FPVector3(ballPhysics.Velocity.X, ballPhysics.Velocity.Y, FP.FromFloat_UNSAFE(-100));

                    // Update the ball's physics in the frame
                    f.Set(f.Global->Ball, ballPhysics);

                    // Update the player's state in the frame
                    f.Set(filter.Entity, f.Global->Players[filter.Link->Player]);
                }
            }
        }
    }
}
