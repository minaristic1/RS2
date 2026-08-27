import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';
import { SessionService } from './shared/session/session.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'grizgo-web';
  guestIdShort: string;

  constructor(private sessionService: SessionService) {
    this.guestIdShort = this.sessionService.getUserId().slice(0, 8);
  }
}
