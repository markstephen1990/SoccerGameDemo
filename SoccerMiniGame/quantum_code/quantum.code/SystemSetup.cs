using Photon.Deterministic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quantum.SoccerGame;
using Quantum.SoccerGame.System;

namespace Quantum {
  public static class SystemSetup {
    public static SystemBase[] CreateSystems(RuntimeConfig gameConfig, SimulationConfig simulationConfig) {
            return new SystemBase[] {
        // pre-defined core systems
        //  new Core.CullingSystem2D(),
        new Core.CullingSystem3D(),

        //new Core.PhysicsSystem2D(),
        new Core.PhysicsSystem3D(),

        Core.DebugCommand.CreateSystem(),

        //new Core.NavigationSystem(),
        new Core.EntityPrototypeSystem(),
        new Core.PlayerConnectedSystem(),

        // user systems go here B
        new MovementSystem(),
        new PlayerSpawnSystem(),
        new GoalSystem(),
        new KickSystem(),
        new BallSpawnSystem(),
        new BlockSystem(),
        new GameEndSystem(),
      };
    }
  }
}
