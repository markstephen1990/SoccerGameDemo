using Quantum;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks {

    public RuntimePlayer runtimePlayer;
  public override void OnGameStart(Quantum.QuantumGame game) {
    // paused on Start means waiting for Snapshot
    Debug.Log("OnGameStart");
    //if (game.Session.IsPaused) return;

    Debug.Log("game.GetLocalPlayers():" + game.GetLocalPlayers().Length);
    foreach (var lp in game.GetLocalPlayers()) {
      Debug.Log("CustomCallbacks - sending player: " + lp);
      game.SendPlayerData(lp, runtimePlayer);
    }
  }

  public override void OnGameResync(Quantum.QuantumGame game)
  {
    Debug.Log("Detected Resync. Verified tick: " + game.Frames.Verified.Number);
  }
}

