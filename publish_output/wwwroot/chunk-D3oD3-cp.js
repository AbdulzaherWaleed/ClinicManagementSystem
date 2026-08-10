import{A as Fp,Hn as ju,J as LD,Kt as Wl,Nt as UL,S as Ee,Tr as tC,Y as LL,dr as oe,en as Ym,h as DC,kt as TE,lt as PL,pn as b,q as Kp,rt as NE,u as C$1,x as EE}from"./chunk-wnS9qIQM.js";import{E as Rt,p as It,z as ce}from"./main-II3MM3G3.js";import{l as p}from"./chunk-B05-dV72.js";import{N as xt,u as Je}from"./chunk-ByLXf5qN.js";import{n as U}from"./chunk-ByL6LUeh.js";var R=`
    .p-textarea {
        font-weight: dt('textarea.font.weight');
        font-size: dt('textarea.font.size');
        color: dt('textarea.color');
        background: dt('textarea.background');
        padding-block: dt('textarea.padding.y');
        padding-inline: dt('textarea.padding.x');
        border: 1px solid dt('textarea.border.color');
        transition:
            background dt('textarea.transition.duration'),
            color dt('textarea.transition.duration'),
            border-color dt('textarea.transition.duration'),
            outline-color dt('textarea.transition.duration'),
            box-shadow dt('textarea.transition.duration');
        appearance: none;
        border-radius: dt('textarea.border.radius');
        outline-color: transparent;
        box-shadow: dt('textarea.shadow');
    }

    .p-textarea:enabled:hover {
        border-color: dt('textarea.hover.border.color');
    }

    .p-textarea:enabled:focus {
        border-color: dt('textarea.focus.border.color');
        box-shadow: dt('textarea.focus.ring.shadow');
        outline: dt('textarea.focus.ring.width') dt('textarea.focus.ring.style') dt('textarea.focus.ring.color');
        outline-offset: dt('textarea.focus.ring.offset');
    }

    .p-textarea.p-invalid {
        border-color: dt('textarea.invalid.border.color');
    }

    .p-textarea.p-variant-filled {
        background: dt('textarea.filled.background');
    }

    .p-textarea.p-variant-filled:enabled:hover {
        background: dt('textarea.filled.hover.background');
    }

    .p-textarea.p-variant-filled:enabled:focus {
        background: dt('textarea.filled.focus.background');
    }

    .p-textarea:disabled {
        opacity: 1;
        background: dt('textarea.disabled.background');
        color: dt('textarea.disabled.color');
    }

    .p-textarea::placeholder {
        color: dt('textarea.placeholder.color');
    }

    .p-textarea.p-invalid::placeholder {
        color: dt('textarea.invalid.placeholder.color');
    }

    .p-textarea-fluid {
        width: 100%;
    }

    .p-textarea-resizable {
        overflow: hidden;
        resize: none;
    }

    .p-textarea-sm {
        font-size: dt('textarea.sm.font.size');
        padding-block: dt('textarea.sm.padding.y');
        padding-inline: dt('textarea.sm.padding.x');
    }

    .p-textarea-lg {
        font-size: dt('textarea.lg.font.size');
        padding-block: dt('textarea.lg.padding.y');
        padding-inline: dt('textarea.lg.padding.x');
    }
`;var C={root:({instance:e})=>[`p-textarea p-component`,{"p-filled":e.$filled(),"p-textarea-resizable ":e.autoResize(),"p-variant-filled":e.$variant()===`filled`,"p-textarea-fluid":e.hasFluid,"p-inputfield-sm p-textarea-sm":e.pSize()===`small`,"p-textarea-lg p-inputfield-lg":e.pSize()===`large`,"p-invalid":e.invalid()}]};var I=(()=>{class e extends ce{name=`textarea`;style=R;classes=C;static ɵfac=(()=>{let t;return function(r){return(t||(t=Ym(e)))(r||e)}})();static ɵprov=oe({token:e,factory:e.ɵfac})}return e})();var S=new b(`TEXTAREA_INSTANCE`);var Q=(()=>{class e extends xt{componentName=`Textarea`;bindDirectiveInstance=C$1(Rt,{self:!0});$pcTextarea=C$1(S,{optional:!0,skipSelf:!0})??void 0;pTextareaPT=PL();pTextareaUnstyled=PL();autoResize=PL(!1,{transform:UL});pSize=PL();variant=PL();fluid=PL(!1,{transform:UL});invalid=PL(!1,{transform:UL});$variant=DC(()=>this.variant()||this.config.inputVariant());onResize=LL();get hasFluid(){return this.fluid()??!!this.pcFluid}_componentStyle=C$1(I);ngControl=C$1(p,{optional:!0,self:!0});pcFluid=C$1(Je,{optional:!0,host:!0,skipSelf:!0});destroyRef=C$1(Ee);constructor(){super(),ju(()=>{let t=this.pTextareaPT();t&&this.directivePT.set(t)}),ju(()=>{this.pTextareaUnstyled()&&this.directiveUnstyled.set(this.pTextareaUnstyled())})}onInit(){this.ngControl&&this.ngControl.valueChanges&&this.ngControl.valueChanges.pipe(U(this.destroyRef)).subscribe(()=>{this.updateState()})}onAfterViewInit(){this.autoResize()&&this.resize(),this.cd.detectChanges()}onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms([`host`,`root`])),this.autoResize()&&this.resize(),this.writeModelValue(this.ngControl?.value??this.el.nativeElement.value)}onInput(t){this.writeModelValue(t.target?.value),this.updateState()}resize(t){this.el.nativeElement.style.height=`auto`,this.el.nativeElement.style.height=this.el.nativeElement.scrollHeight+`px`,parseFloat(this.el.nativeElement.style.height)>=parseFloat(this.el.nativeElement.style.maxHeight)?(this.el.nativeElement.style.overflowY=`scroll`,this.el.nativeElement.style.height=this.el.nativeElement.style.maxHeight):this.el.nativeElement.style.overflow=`hidden`,this.onResize.emit(t||{})}updateState(){this.autoResize()&&this.resize()}static ɵfac=function(n){return new(n||e)};static ɵdir=TE({type:e,selectors:[[``,`pTextarea`,``],[``,`pInputTextarea`,``]],hostVars:2,hostBindings:function(n,r){n&1&&Kp(`input`,function(F){return r.onInput(F)}),n&2&&LD(r.cx(`root`))},inputs:{pTextareaPT:[1,`pTextareaPT`],pTextareaUnstyled:[1,`pTextareaUnstyled`],autoResize:[1,`autoResize`],pSize:[1,`pSize`],variant:[1,`variant`],fluid:[1,`fluid`],invalid:[1,`invalid`]},outputs:{onResize:`onResize`},features:[tC([I,{provide:S,useExisting:e},{provide:It,useExisting:e}]),NE([Rt]),Fp]})}return e})();var W=(()=>{class e{static ɵfac=function(n){return new(n||e)};static ɵmod=EE({type:e});static ɵinj=Wl({})}return e})();export{W as n,Q as t};