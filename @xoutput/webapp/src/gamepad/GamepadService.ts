import { EventEmitter, EventListener } from '../utils/EventEmitter';
import { GamepadReader } from './GamepadReader';

class GamepadService {
  private gamepads: Record<string, GamepadReader> = {};
  private changeEmitter: EventEmitter<GamepadReader[]> = new EventEmitter();

  isAvailable(): boolean {
    return typeof navigator.getGamepads === 'function';
  }

  start() {
    if (!this.isAvailable()) {
      return;
    }
    window.addEventListener('gamepadconnected', (e: GamepadEvent) => {
      this.gamepads[e.gamepad.id] = new GamepadReader(e.gamepad);
      this.changeEmitter.emit(this.getGamepads());
    });
    window.addEventListener('gamepaddisconnected', (e: GamepadEvent) => {
      delete this.gamepads[e.gamepad.id];
      this.changeEmitter.emit(this.getGamepads());
    });
  }

  addChangeListener(listener: EventListener<GamepadReader[]>) {
    this.changeEmitter.addListener(listener);
  }

  removeChangeListener(listener: EventListener<GamepadReader[]>) {
    this.changeEmitter.removeListener(listener);
  }

  getGamepads(): GamepadReader[] {
    return Object.values(this.gamepads).filter((gamepad) => gamepad.gamepad.connected);
  }

  getGamepad(id: string): GamepadReader {
    return this.gamepads[id];
  }
}

export const gamepadService = new GamepadService();
