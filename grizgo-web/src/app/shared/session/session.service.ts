import { Injectable } from '@angular/core';
import { AuthService } from '../../auth/services/auth.service';

@Injectable({ providedIn: 'root' })
export class SessionService {
  private readonly storageKey = 'grizgo-guest-user-id';

  constructor(private authService: AuthService) {}

  /**
   * Vraća ID ulogovanog korisnika ako postoji, inače privremeni gost ID
   * (za ekrane koji rade i bez naloga, npr. anonimno pregledanje).
   */
  getUserId(): string {
    const loggedInUserId = this.authService.currentUser()?.id;

    if (loggedInUserId) {
      return loggedInUserId;
    }

    let userId = localStorage.getItem(this.storageKey);

    if (!userId) {
      userId = crypto.randomUUID();
      localStorage.setItem(this.storageKey, userId);
    }

    return userId;
  }
}
