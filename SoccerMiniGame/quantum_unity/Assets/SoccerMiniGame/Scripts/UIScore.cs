using Quantum;
using UnityEngine;
using UnityEngine.UI;

public unsafe class UIScore : MonoBehaviour
{
    public Text Score; // UI text component to display the score

    // Subscribe to the OnGoal event when the script starts
    private void Start()
    {
        QuantumEvent.Subscribe<EventOnGoal>(this, OnGoal);
    }

    // Event handler for the OnGoal event
    private void OnGoal(EventOnGoal e)
    {
        UpdateScoreText(GetGameScore(QuantumRunner.Default.Game)); // Update the score text with the current game score
    }

    // Get the current game score
    private int[] GetGameScore(QuantumGame game)
    {
        var f = game.Frames.Verified; // Get the verified frame
        int[] score = new int[Constants.MAX_PLAYERS]; // Initialize the score array

        // Iterate through all players to get their scores
        for (int i = 0; i < Constants.MAX_PLAYERS; i++)
        {
            PlayerFields playerFields = f.Global->Players[i];

            // If the player controls a character, get their score
            if (playerFields.ControlledCharacter != EntityRef.None)
            {
                score[i] = playerFields.PlayerScore;
            }
        }

        return score; // Return the score array
    }

    // Update the score text UI
    private void UpdateScoreText(int[] score)
    {
        Score.text = ""; // Clear the score text
        for (int i = 0; i < score.Length; i++)
        {
            // Add a newline character for each player except the first
            if (i > 0)
            {
                Score.text += "\n";
            }
            // Append the score for each player
            Score.text += "Player " + (i + 1) + " : " + score[i];
        }
    }

    // Unsubscribe from the OnGoal event when the script is destroyed
    private void OnDestroy()
    {
        QuantumEvent.UnsubscribeListener(this);
    }
}
