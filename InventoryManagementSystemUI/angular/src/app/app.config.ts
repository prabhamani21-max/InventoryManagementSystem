/// <reference types="@angular/localize" />

import {
  ApplicationConfig,
  importProvidersFrom,
  isDevMode,
  provideZoneChangeDetection
} from '@angular/core';

import {
  provideRouter,
  withInMemoryScrolling
} from '@angular/router';

import {
  provideHttpClient,
  withFetch,
  withInterceptorsFromDi
} from '@angular/common/http';

import { provideAnimations } from '@angular/platform-browser/animations';
import { provideToastr } from 'ngx-toastr';

import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';

import { routes } from './app-routing.module';
import { CoreModule } from './core/core.module';

export const appConfig: ApplicationConfig = {
  providers: [

    // ⚡ Optimized change detection
    provideZoneChangeDetection({
      eventCoalescing: true
    }),

    // 🌐 Router
    provideRouter(
      routes,
      withInMemoryScrolling({
        scrollPositionRestoration: 'top',
        anchorScrolling: 'enabled'
      })
    ),

    // 🌍 HTTP Client
    provideHttpClient(
      withFetch(),
      withInterceptorsFromDi()
    ),

    // 🧠 NgRx State
    provideStore(),
    provideEffects(),

    // 🧪 Redux DevTools (dev only)
    provideStoreDevtools({
      maxAge: 25,
      logOnly: !isDevMode()
    }),

    // 🎨 UI Utilities
    provideAnimations(),
    provideToastr({
      timeOut: 3000,
      positionClass: 'toast-top-right',
      preventDuplicates: true,
      closeButton: true
    }),

    // 🧩 Singleton services
    importProvidersFrom(CoreModule),
  ]
};
