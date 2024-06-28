using Photon.Deterministic;

namespace Quantum.SoccerGame
{
    // This system handles the movement mechanics for players in the soccer game
    public unsafe class MovementSystem : SystemMainThreadFilter<MovementSystem.Filter>
    {
        // Structure to filter entities with specific components
        public struct Filter
        {
            public EntityRef Entity; // Reference to the entity
            public CharacterController3D* CharacterController; // Pointer to CharacterController3D component
            public Transform3D* Transform; // Pointer to Transform3D component
            public PlayerLink* Link; // Pointer to PlayerLink component
        }

        // Update method called each frame
        public override void Update(Frame f, ref Filter filter)
        {
            // Get player input for the current player
            var input = f.GetPlayerInput(filter.Link->Player);

            // Create an input vector based on player input, scaled down
            var inputVector = new FPVector2((FP)input->DirectionX / 10, (FP)input->DirectionY / 10);

            // If the input vector's magnitude squared is greater than 1, normalize it
            if (inputVector.SqrMagnitude > 1)
            {
                inputVector = inputVector.Normalized;
            }

            // Move the character based on the input vector
            filter.CharacterController->Move(f, filter.Entity, inputVector.XOY);

            // Set the Y position of the character's transform to 3
            filter.Transform->Position.Y = 3;
        }
    }
}
