import{A as Fp,Ar as vE,Dn as gi,En as gD,Er as th,Gn as kc,Hn as ju,I as ID,J as LD,Kt as Wl,Lt as VL,N as Gp,Nt as UL,Pt as Up,Sn as fD,Tr as tC,Ut as WD,dr as oe$1,en as Ym,h as DC,hr as pr,jn as hD,kt as TE,l as Bv,lt as PL,mt as QE,nn as ZE,nr as mh,nt as Mr,pn as b,qt as Wp,rt as NE,u as C,vt as Qp,x as EE,xt as Ri}from"./chunk-wnS9qIQM.js";import{s as Vn}from"./chunk-pDNCOkjP.js";import{E as Rt,bt as p,i as En,p as It,r as Ea,xt as p$1,z as ce}from"./main-II3MM3G3.js";var L=`
    .p-tag {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        background: dt('tag.primary.background');
        color: dt('tag.primary.color');
        font-size: dt('tag.font.size');
        font-weight: dt('tag.font.weight');
        padding: dt('tag.padding');
        border-radius: dt('tag.border.radius');
        gap: dt('tag.gap');
    }

    .p-tag-icon {
        font-size: dt('tag.icon.size');
        width: dt('tag.icon.size');
        height: dt('tag.icon.size');
    }

    .p-tag-rounded {
        border-radius: dt('tag.rounded.border.radius');
    }

    .p-tag-success {
        background: dt('tag.success.background');
        color: dt('tag.success.color');
    }

    .p-tag-info {
        background: dt('tag.info.background');
        color: dt('tag.info.color');
    }

    .p-tag-warn {
        background: dt('tag.warn.background');
        color: dt('tag.warn.color');
    }

    .p-tag-danger {
        background: dt('tag.danger.background');
        color: dt('tag.danger.color');
    }

    .p-tag-secondary {
        background: dt('tag.secondary.background');
        color: dt('tag.secondary.color');
    }

    .p-tag-contrast {
        background: dt('tag.contrast.background');
        color: dt('tag.contrast.color');
    }
`;var te=[`icon`];var ne=[`*`];function ie(e,n){if(e&1&&Wp(0,`span`,1),e&2){let t=fD(2);LD(t.cn(t.cx(`icon`),t.icon())),Gp(`pBind`,t.ptm(`icon`))}}function oe(e,n){if(e&1&&QE(0,ie,1,3,`span`,0),e&2)ZE(fD().icon()?0:-1)}function ae(e,n){if(e&1&&(gi(0,`span`,1),Qp(1,2),kc()),e&2){let t=fD();LD(t.cx(`icon`)),Gp(`pBind`,t.ptm(`icon`)),Bv(),Gp(`ngTemplateOutlet`,t.iconTemplate())}}var re={root:({instance:e})=>{let n=e.severity(),t=e.rounded();return[`p-tag p-component`,{"p-tag-info":n===`info`,"p-tag-success":n===`success`,"p-tag-warn":n===`warn`,"p-tag-danger":n===`danger`,"p-tag-secondary":n===`secondary`,"p-tag-contrast":n===`contrast`,"p-tag-rounded":t}]},icon:`p-tag-icon`,label:`p-tag-label`};var W=(()=>{class e extends ce{name=`tag`;style=L;classes=re;static ɵfac=(()=>{let t;return function(o){return(t||(t=Ym(e)))(o||e)}})();static ɵprov=oe$1({token:e,factory:e.ɵfac})}return e})();var X=new b(`TAG_INSTANCE`);var se=(()=>{class e extends En{componentName=`Tag`;$pcTag=C(X,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=C(Rt,{self:!0});severity=PL();value=PL();icon=PL();rounded=PL(!1,{transform:UL});iconTemplate=VL(`icon`,{descendants:!1});_componentStyle=C(W);dataP=DC(()=>{let t=this.severity(),i=this.rounded();return this.cn({rounded:i,[t]:t})});onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms([`host`,`root`]))}static ɵfac=(()=>{let t;return function(o){return(t||(t=Ym(e)))(o||e)}})();static ɵcmp=vE({type:e,selectors:[[`p-tag`]],contentQueries:function(i,o,Z){i&1&&th(Z,o.iconTemplate,te,4),i&2&&ID()},hostVars:3,hostBindings:function(i,o){i&2&&(Up(`data-p`,o.dataP()),LD(o.cx(`root`)))},inputs:{severity:[1,`severity`],value:[1,`value`],icon:[1,`icon`],rounded:[1,`rounded`]},features:[tC([W,{provide:X,useExisting:e},{provide:It,useExisting:e}]),NE([Rt]),Fp],ngContentSelectors:ne,decls:5,vars:5,consts:[[3,`class`,`pBind`],[3,`pBind`],[3,`ngTemplateOutlet`]],template:function(i,o){i&1&&(hD(),gD(0),QE(1,oe,1,1)(2,ae,2,4,`span`,0),gi(3,`span`,1),WD(4),kc()),i&2&&(Bv(),ZE(o.iconTemplate()?2:1),Bv(2),LD(o.cx(`label`)),Gp(`pBind`,o.ptm(`label`)),Bv(),mh(o.value()))},dependencies:[Vn,Ea,Rt],encapsulation:2})}return e})();var Ce=(()=>{class e{static ɵfac=function(i){return new(i||e)};static ɵmod=EE({type:e});static ɵinj=Wl({imports:[se,Ea,Ea]})}return e})();var Y=class e{constructor(n,t,i,o){this.templateRef=n;this.viewContainer=t;this.permissionService=i;this.authService=o;ju(()=>{this.authService.currentUser(),this.updateView()})}templateRef;viewContainer;permissionService;authService;permission=``;hasView=!1;set appHasPermission(n){this.permission=n,this.updateView()}updateView(){if(!this.permission)return;let n=this.permissionService.hasPermission(this.permission);n&&!this.hasView?(this.viewContainer.createEmbeddedView(this.templateRef),this.hasView=!0):!n&&this.hasView&&(this.viewContainer.clear(),this.hasView=!1)}static ɵfac=function(t){return new(t||e)(Mr(pr),Mr(Ri),Mr(p),Mr(p$1))};static ɵdir=TE({type:e,selectors:[[``,`appHasPermission`,``]],inputs:{appHasPermission:`appHasPermission`}})};export{Y as n,se as r,Ce as t};