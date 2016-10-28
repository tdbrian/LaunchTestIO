import { Directive, OnInit, ElementRef, Renderer } from '@angular/core';

@Directive({ selector: '[appVerticalAlignMiddle]' })
export class VerticalAlignMiddleDirective implements OnInit {
  bodyHeight: number;
  formHeight: number;
  marginTop: number;

  constructor(private el: ElementRef, private renderer: Renderer) { }

  ngOnInit() {
    this.setMarginTop();
    window.onresize = this.windowResize;
  }

  private windowResize = () => {
    this.setMarginTop();
  }

  private setMarginTop() {
    this.formHeight =  this.el.nativeElement.clientHeight;
    this.bodyHeight = window.innerHeight;
    const marginTop = (this.bodyHeight / 2) - (this.formHeight / 2);
    if (marginTop > 0) {
      this.renderer.setElementStyle(this.el.nativeElement, 'margin-top', `${marginTop}px`);
    }
  }
}
