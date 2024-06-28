namespace Quantum.SoccerGame.System
{
    // This system handles the end of the game when a goal is scored
    public unsafe class GameEndSystem : SystemSignalsOnly, ISignalOnGoal
    {
        // Method called when a goal is scored
        public void OnGoal(Frame f)
        {
            CheckEndGame(f); // Check if the game should end
        }

        // Method to check if the game should end based on player scores
        private void CheckEndGame(Frame f)
        {
            // Iterate through all players
            for (int i = 0; i < f.Global->Players.Length; i++)
            {
                // Check if the player's score has reached or exceeded the maximum goal limit
                if (f.Global->Players[i].PlayerScore >= Constants.MAX_GOAL)
                {
                    f.Events.GameplayEnded(); // Trigger the event to end the game
                    return; // Exit the loop and method since the game has ended
                }
            }
        }
    }
}
