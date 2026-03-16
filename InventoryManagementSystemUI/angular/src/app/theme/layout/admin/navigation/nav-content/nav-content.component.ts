// angular import
import { Component, OnInit, inject, output } from '@angular/core';
import { Location, LocationStrategy } from '@angular/common';

// project import
import { environment } from 'src/environments/environment';
import { NavigationItem, NavigationItems } from '../navigation';
import { SharedModule } from 'src/app/theme/shared/shared.module';
import { NavGroupComponent } from './nav-group/nav-group.component';
import { AuthenticationService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-nav-content',
  imports: [SharedModule, NavGroupComponent],
  templateUrl: './nav-content.component.html',
  styleUrls: ['./nav-content.component.scss']
})
export class NavContentComponent implements OnInit {
  private location = inject(Location);
  private locationStrategy = inject(LocationStrategy);
  private authService = inject(AuthenticationService);

  // version
  title = 'Demo application for version numbering';
  currentApplicationVersion = environment.appVersion;

  // public pops
  navigation: NavigationItem[];
  contentWidth: number;
  wrapperWidth!: number;
  scrollWidth: number;
  windowWidth: number;

  NavMobCollapse = output();

  // constructor
  constructor() {
    this.navigation = this.getNavigationByRole();
    this.windowWidth = window.innerWidth;
    this.scrollWidth = 0;
    this.contentWidth = 0;
  }

  private getNavigationByRole(): NavigationItem[] {
    const roleId = this.getCurrentUserRoleId();
    return this.filterNavigationByRole(NavigationItems, roleId);
  }

  private getCurrentUserRoleId(): number | null {
    const roleId = this.authService.getUserInformation()?.roleId;
    const parsedRoleId = Number(roleId);
    return Number.isNaN(parsedRoleId) ? null : parsedRoleId;
  }

  private filterNavigationByRole(items: NavigationItem[], roleId: number | null): NavigationItem[] {
    return items.reduce((filteredItems: NavigationItem[], item: NavigationItem) => {
      const isAllowed = !item.roleId || (roleId !== null && item.roleId.includes(roleId));
      if (!isAllowed) {
        return filteredItems;
      }

      const filteredItem: NavigationItem = { ...item };

      if (item.children?.length) {
        filteredItem.children = this.filterNavigationByRole(item.children, roleId);
      }

      const isContainerItem = filteredItem.type === 'group' || filteredItem.type === 'collapse';
      if (isContainerItem && (!filteredItem.children || filteredItem.children.length === 0)) {
        return filteredItems;
      }

      filteredItems.push(filteredItem);
      return filteredItems;
    }, []);
  }

  // life cycle event
  ngOnInit() {
    if (this.windowWidth < 992) {
      setTimeout(() => {
        document.querySelector('.pcoded-navbar')?.classList.add('menupos-static');
        const navPs = document.querySelector('#nav-ps-gradient-able') as HTMLElement;
        if (navPs) {
          navPs.style.height = '100%';
        }
      }, 500);
    }
  }

  fireLeave() {
    const sections = document.querySelectorAll('.pcoded-hasmenu');
    for (let i = 0; i < sections.length; i++) {
      sections[i].classList.remove('active');
      sections[i].classList.remove('pcoded-trigger');
    }

    let current_url = this.location.path();
    const baseHref = this.locationStrategy.getBaseHref();
    if (baseHref) {
      current_url = baseHref + this.location.path();
    }
    const link = "a.nav-link[ href='" + current_url + "' ]";
    const ele = document.querySelector(link);
    if (ele !== null && ele !== undefined) {
      const parent = ele.parentElement;
      const up_parent = parent?.parentElement?.parentElement;
      const last_parent = up_parent?.parentElement;
      if (parent?.classList.contains('pcoded-hasmenu')) {
        parent.classList.add('active');
      } else if (up_parent?.classList.contains('pcoded-hasmenu')) {
        up_parent.classList.add('active');
      } else if (last_parent?.classList.contains('pcoded-hasmenu')) {
        last_parent.classList.add('active');
      }
    }
  }

  navMob() {
    if (this.windowWidth < 992 && document.querySelector('app-navigation.pcoded-navbar')?.classList.contains('mob-open')) {
      this.NavMobCollapse.emit();
    }
  }

  fireOutClick() {
    let current_url = this.location.path();
    const baseHref = this.locationStrategy.getBaseHref();
    if (baseHref) {
      current_url = baseHref + this.location.path();
    }
    const link = "a.nav-link[ href='" + current_url + "' ]";
    const ele = document.querySelector(link);
    if (ele !== null && ele !== undefined) {
      const parent = ele.parentElement;
      const up_parent = parent?.parentElement?.parentElement;
      const last_parent = up_parent?.parentElement;
      if (parent?.classList.contains('pcoded-hasmenu')) {
        parent.classList.add('pcoded-trigger');
        parent.classList.add('active');
      } else if (up_parent?.classList.contains('pcoded-hasmenu')) {
        up_parent.classList.add('pcoded-trigger');
        up_parent.classList.add('active');
      } else if (last_parent?.classList.contains('pcoded-hasmenu')) {
        last_parent.classList.add('pcoded-trigger');
        last_parent.classList.add('active');
      }
    }
  }
}
