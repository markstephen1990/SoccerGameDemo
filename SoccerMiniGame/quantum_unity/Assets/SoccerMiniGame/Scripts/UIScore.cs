using Quantum;
using UnityEngine;
using UnityEngine.UI;

public unsafe class UIScore : MonoBehaviour
{
  public GameObject Panel;
  public Text Score;

  private void Start()
  {
    QuantumEvent.Subscribe<EventOnGoal>(this, OnGoal);
  }

  private void OnGoal(EventOnGoal e)
  {
    Panel.SetActive(true);
    UpdateScoreText(GetGameScore(QuantumRunner.Default.Game));
  }

  private int[] GetGameScore(QuantumGame game)
  {
    var f = game.Frames.Verified;
    int[] score = new int[Constants.MAX_PLAYERS];


  for (int i = 0; i < Constants.MAX_PLAYERS; i++)
  {
    PlayerFields playerFields = f.Global->Players[i];

    if (playerFields.ControlledCharacter != EntityRef.None)
    {
        score[i] = playerFields.PlayerScore;
    }
  }
    return score;
  }

  private void UpdateScoreText(int[] score)
  {
    Score.text = "";
    for (int i = 0; i < score.Length; i++)
    {
      if (i > 0)
      {
        Score.text += "\n";
      }
      Score.text += "Player " + (i + 1) + " : " + score[i];
    }
  }

  private void OnDestroy()
  {
    QuantumEvent.UnsubscribeListener(this);
  }

}