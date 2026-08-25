import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly storageKey = 'grizgo-guest-user-id';

  getUserId(): string {
    let userId = localStorage.getItem(this.storageKey);

    if (!userId) {
      userId = crypto.randomUUID();
      localStorage.setItem(this.storageKey, userId);
    }

    return userId;
  }
}
