namespace Quantum.SoccerGame.System
{
    // This system handles goal detection and related actions in the soccer game
    public unsafe class GoalSystem : SystemSignalsOnly, ISignalOnTriggerEnter3D
    {
        // Method called when a trigger is entered in 3D space
        public void OnTriggerEnter3D(Frame f, TriggerInfo3D info)
        {
            // Check if the entity that entered the trigger is a ball
            if (f.TryGet<BallFields>(info.Entity, out var ball))
            {
                // Increment the score of the player who last touched the ball
                f.Global->Players[ball.LastTouchedPlayerId].PlayerScore++;

                // Update the player's controlled character state
                f.Set(f.Global->Players[ball.LastTouchedPlayerId].ControlledCharacter, f.Global->Players[ball.LastTouchedPlayerId]);

                // Refresh the block status for all players
                RefreshPlayerBlockStatus(f);

                // Signal to destroy and respawn the ball
                f.Signals.OnDestroyBall();
                f.Signals.OnSpawnBall();

                // Signal that a goal has been scored
                f.Signals.OnGoal();

                // Trigger the goal event
                f.Events.OnGoal();
            }
        }

        // Method to refresh the block status for all players
        private void RefreshPlayerBlockStatus(Frame f)
        {
            // Iterate through all players
            for (var i = 0; i < f.Global->Players.Length; i++)
            {
                // Check if the player has a controlled character
                if (f.Global->Players[i].ControlledCharacter != EntityRef.None)
                {
                    // Reset the block status for the player
                    f.Global->Players[i].HasAlreadyBlocked = false;

                    // Update the player's controlled character state
                    f.Set(f.Global->Players[i].ControlledCharacter, f.Global->Players[i]);
                }
            }
        }
    }
}
