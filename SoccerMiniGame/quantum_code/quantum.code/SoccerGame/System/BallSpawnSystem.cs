using Photon.Deterministic;

namespace Quantum.SoccerGame.System
{
    // This system handles spawning and destroying soccer balls in the game
    unsafe class BallSpawnSystem : SystemSignalsOnly, ISignalOnSpawnBall, ISignalOnDestroyBall
    {
        // Commented out: a variable to store a reference to the ball entity
        // private EntityRef ballEntity = EntityRef.None;

        // Method called when a ball needs to be spawned
        public void OnSpawnBall(Frame f)
        {
            // Check if the ball is not already valid (i.e., it hasn't been created yet)
            if (!f.Global->Ball.IsValid)
            {
                // Find the soccer ball prototype asset
                var prototype = f.FindAsset<EntityPrototype>("Resources/DB/SoccerBall|EntityPrototype");
                // Create the ball entity from the prototype
                f.Global->Ball = f.Create(prototype);

                // Get a random position near the center of the field
                var randomPosition = GetRandomPositionNearCenter(f);

                // If the ball has a Transform3D component, set its position to the random position
                if (f.Unsafe.TryGetPointer<Transform3D>(f.Global->Ball, out var transform))
                {
                    transform->Position = randomPosition;
                }
            }
        }

        // Method called when a ball needs to be destroyed
        public void OnDestroyBall(Frame f)
        {
            // Check if the ball is valid (i.e., it exists)
            if (f.Global->Ball.IsValid)
            {
                // Destroy the ball entity
                f.Destroy(f.Global->Ball);
                // Set the ball reference to None, indicating it no longer exists
                f.Global->Ball = EntityRef.None;
            }
        }

        // Method to get a random position near the center of the field
        private FPVector3 GetRandomPositionNearCenter(Frame frame)
        {
            FP centerX = FP.FromFloat_UNSAFE(0); // Center X position
            FP centerZ = FP.FromFloat_UNSAFE(0); // Center Z position

            // Generate random values between -1 and 1 for X and Z offsets
            FP randomOffsetX = (frame.RNG->Next() * 2 - 1);
            FP randomOffsetZ = (frame.RNG->Next() * 2 - 1);

            // Return a new vector with the random offsets applied to the center position
            return new FPVector3(centerX + randomOffsetX, 0, centerZ + randomOffsetZ);
        }
    }
}
