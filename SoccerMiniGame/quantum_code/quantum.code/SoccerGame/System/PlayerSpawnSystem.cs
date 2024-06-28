namespace Quantum.SoccerGame
{
    // This system handles the spawning and disconnection of players in the soccer game
    unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerDataSet, ISignalOnPlayerDisconnected
    {
        // Method called when player data is set
        public void OnPlayerDataSet(Frame frame, PlayerRef player)
        {
            // Increment the global player count
            frame.Global->PlayerCount++;

            // Get the player data for the specified player
            var data = frame.GetPlayerData(player);

            // Resolve the reference to the avatar prototype
            var prototype = frame.FindAsset<EntityPrototype>(data.CharacterPrototype.Id);

            // Create a new entity for the player based on the prototype
            var entity = frame.Create(prototype);

            // Create and initialize a PlayerLink component, then add it to the player entity
            if (frame.Unsafe.TryGetPointer<PlayerLink>(entity, out var playerLink))
            {
                playerLink->Player = player;
            }

            // Add the player to the global player list
            if (frame.Unsafe.TryGetPointer<PlayerFields>(entity, out var playerFields))
            {
                frame.Global->Players[frame.Global->PlayerCount - 1] = playerFields[0];
            }

            // Offset the instantiated object in the world based on its ID
            if (frame.Unsafe.TryGetPointer<Transform3D>(entity, out var transform))
            {
                transform->Position.X = (int)player;
            }

            // Spawn the ball if there are two players
            if (frame.Global->PlayerCount == 2)
            {
                frame.Signals.OnSpawnBall();
            }
        }

        // Method called when a player disconnects
        public void OnPlayerDisconnected(Frame f, PlayerRef player)
        {
            // Decrement the global player count
            f.Global->PlayerCount--;

            // Destroy the player's controlled character entity
            f.Destroy(f.Global->Players[player._index - 1].ControlledCharacter);

            // Set the player's controlled character reference to None
            f.Global->Players[player._index - 1].ControlledCharacter = EntityRef.None;

            // Destroy the ball if there is one or fewer players left
            if (f.Global->PlayerCount <= 1)
            {
                f.Signals.OnDestroyBall();
            }
        }
    }
}
