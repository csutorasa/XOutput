class GamepadService {
  private gamepads: Record<string, Gamepad>;

  start() {
    window.addEventListener('gamepadconnected', (e: GamepadEvent) => {
      this.gamepads[e.gamepad.id] = e.gamepad;
    });
    window.addEventListener('gamepaddisconnected', (e: GamepadEvent) => {
      delete this.gamepads[e.gamepad.id];
    });
  }

  getGamepads(): Gamepad[] {
    return Object.values(this.gamepads);
  }

  getGamePad(id: string): Gamepad {
    return this.gamepads[id];
  }
}
