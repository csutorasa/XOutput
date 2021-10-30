export type EventListener<T> = (event: T) => void;

export class EventEmitter<T> {
  private listeners: EventListener<T>[] = [];

  addListener(listener: EventListener<T>): void {
    this.listeners.push(listener);
  }

  removeListener(listener: EventListener<T>): void {
    this.listeners = this.listeners.filter((l) => l !== listener);
  }

  emit(event: T): void {
    this.listeners.forEach((listener) => listener(event));
  }
}
