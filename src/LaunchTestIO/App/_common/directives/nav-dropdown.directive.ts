import { Directive, HostListener, HostBinding } from '@angular/core';

@Directive({ selector: '[appNavDropdown]' })
export class NavDropdownDirective {
    @HostBinding('class.open') _open = false;

    isOpen() {
        return this._open;
    }

    open() {
        this._open = true;
    }

    close() {
        this._open = false;
    }

    toggle() {
        if (this.isOpen()) {
            this.close();
        } else {
            this.open();
        }
    }
}

/**
* Allows the dropdown to be toggled via click.
*/
@Directive({ selector: '[appNavDropdownToggle]' })
export class NavDropdownToggleDirective {
    constructor(private dropdown: NavDropdownDirective) {}

    @HostListener('click', ['$event'])
    toggleOpen($event: any) {
        $event.preventDefault();
        this.dropdown.toggle();
    }
}

export const NAV_DROPDOWN_DIRECTIVES = [NavDropdownDirective, NavDropdownToggleDirective];
