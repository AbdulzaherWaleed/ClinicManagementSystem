import{A as Fp,Ar as vE,Bt as Vp,Dn as gi,En as gD,Er as th,Gn as kc,I as ID,J as LD,Kt as Wl,Lt as VL,N as Gp,Sn as fD,Tr as tC,Ut as WD,dr as oe$1,en as Ym,h as DC,jn as hD,l as Bv,lt as PL,mt as QE,nn as ZE,pn as b,rt as NE,u as C,vt as Qp,x as EE,zn as jc}from"./chunk-wnS9qIQM.js";import{s as Vn}from"./chunk-pDNCOkjP.js";import{E as Rt,L as ba,et as lr,i as En,p as It,pt as va,r as Ea,z as ce$1}from"./main-II3MM3G3.js";var $=`
    .p-card {
        display: block;
        background: dt('card.background');
        color: dt('card.color');
        box-shadow: dt('card.shadow');
        border-radius: dt('card.border.radius');
        display: flex;
        flex-direction: column;
    }

    .p-card-caption {
        display: flex;
        flex-direction: column;
        gap: dt('card.caption.gap');
    }

    .p-card-body {
        padding: dt('card.body.padding');
        display: flex;
        flex-direction: column;
        gap: dt('card.body.gap');
    }

    .p-card-title {
        font-size: dt('card.title.font.size');
        font-weight: dt('card.title.font.weight');
    }

    .p-card-subtitle {
        color: dt('card.subtitle.color');
        font-size: dt('card.subtitle.font.size');
        font-weight: dt('card.subtitle.font.weight');
    }
`;var K=[`header`];var L=[`title`];var U=[`subtitle`];var W=[`content`];var X=[`footer`];var Y=[`*`,[[`p-header`]],[[`p-footer`]]];var Z=[`*`,`p-header`,`p-footer`];function ee(e,d){e&1&&Qp(0)}function te(e,d){if(e&1&&(gi(0,`div`,1),gD(1,1),Vp(2,ee,1,0,`ng-container`,2),kc()),e&2){let t=fD();LD(t.cx(`header`)),Gp(`pBind`,t.ptm(`header`)),Bv(2),Gp(`ngTemplateOutlet`,t.headerTemplate())}}function ne(e,d){if(e&1&&WD(0),e&2)jc(` `,fD(2).header(),` `)}function ie(e,d){e&1&&Qp(0)}function ae(e,d){if(e&1&&(gi(0,`div`,1),QE(1,ne,1,1),Vp(2,ie,1,0,`ng-container`,2),kc()),e&2){let t=fD();LD(t.cx(`title`)),Gp(`pBind`,t.ptm(`title`)),Bv(),ZE(t.showHeaderText()?1:-1),Bv(),Gp(`ngTemplateOutlet`,t.titleTemplate())}}function oe(e,d){if(e&1&&WD(0),e&2)jc(` `,fD(2).subheader(),` `)}function de(e,d){e&1&&Qp(0)}function re(e,d){if(e&1&&(gi(0,`div`,1),QE(1,oe,1,1),Vp(2,de,1,0,`ng-container`,2),kc()),e&2){let t=fD();LD(t.cx(`subtitle`)),Gp(`pBind`,t.ptm(`subtitle`)),Bv(),ZE(t.showSubheaderText()?1:-1),Bv(),Gp(`ngTemplateOutlet`,t.subtitleTemplate())}}function ce(e,d){e&1&&Qp(0)}function le(e,d){e&1&&Qp(0)}function pe(e,d){if(e&1&&(gi(0,`div`,1),gD(1,2),Vp(2,le,1,0,`ng-container`,2),kc()),e&2){let t=fD();LD(t.cx(`footer`)),Gp(`pBind`,t.ptm(`footer`)),Bv(2),Gp(`ngTemplateOutlet`,t.footerTemplate())}}var se={root:`p-card p-component`,header:`p-card-header`,body:`p-card-body`,caption:`p-card-caption`,title:`p-card-title`,subtitle:`p-card-subtitle`,content:`p-card-content`,footer:`p-card-footer`};var G=(()=>{class e extends ce$1{name=`card`;style=$;classes=se;static ɵfac=(()=>{let t;return function(n){return(t||(t=Ym(e)))(n||e)}})();static ɵprov=oe$1({token:e,factory:e.ɵfac})}return e})();var J=new b(`CARD_INSTANCE`);var me=(()=>{class e extends En{componentName=`Card`;$pcCard=C(J,{optional:!0,skipSelf:!0})??void 0;bindDirectiveInstance=C(Rt,{self:!0});_componentStyle=C(G);header=PL();subheader=PL();headerFacet=VL(ba,{descendants:!1});footerFacet=VL(va,{descendants:!1});headerTemplate=VL(`header`,{descendants:!1});titleTemplate=VL(`title`,{descendants:!1});subtitleTemplate=VL(`subtitle`,{descendants:!1});contentTemplate=VL(`content`,{descendants:!1});footerTemplate=VL(`footer`,{descendants:!1});hasHeader=DC(()=>!!(this.headerFacet()||this.headerTemplate()));hasTitle=DC(()=>!!(this.header()||this.titleTemplate()));hasSubtitle=DC(()=>!!(this.subheader()||this.subtitleTemplate()));hasFooter=DC(()=>!!(this.footerFacet()||this.footerTemplate()));showHeaderText=DC(()=>this.header()&&!this.titleTemplate());showSubheaderText=DC(()=>this.subheader()&&!this.subtitleTemplate());onAfterViewChecked(){this.bindDirectiveInstance.setAttrs(this.ptms([`host`,`root`]))}getBlockableElement(){return this.el.nativeElement}static ɵfac=(()=>{let t;return function(n){return(t||(t=Ym(e)))(n||e)}})();static ɵcmp=vE({type:e,selectors:[[`p-card`]],contentQueries:function(o,n,l){o&1&&th(l,n.headerFacet,ba,4)(l,n.footerFacet,va,4)(l,n.headerTemplate,K,4)(l,n.titleTemplate,L,4)(l,n.subtitleTemplate,U,4)(l,n.contentTemplate,W,4)(l,n.footerTemplate,X,4),o&2&&ID(7)},hostVars:2,hostBindings:function(o,n){o&2&&LD(n.cx(`root`))},inputs:{header:[1,`header`],subheader:[1,`subheader`]},features:[tC([G,{provide:J,useExisting:e},{provide:It,useExisting:e}]),NE([Rt]),Fp],ngContentSelectors:Z,decls:8,vars:11,consts:[[3,`pBind`,`class`],[3,`pBind`],[4,`ngTemplateOutlet`]],template:function(o,n){o&1&&(hD(Y),QE(0,te,3,4,`div`,0),gi(1,`div`,1),QE(2,ae,3,5,`div`,0),QE(3,re,3,5,`div`,0),gi(4,`div`,1),gD(5),Vp(6,ce,1,0,`ng-container`,2),kc(),QE(7,pe,3,4,`div`,0),kc()),o&2&&(ZE(n.hasHeader()?0:-1),Bv(),LD(n.cx(`body`)),Gp(`pBind`,n.ptm(`body`)),Bv(),ZE(n.hasTitle()?2:-1),Bv(),ZE(n.hasSubtitle()?3:-1),Bv(),LD(n.cx(`content`)),Gp(`pBind`,n.ptm(`content`)),Bv(2),Gp(`ngTemplateOutlet`,n.contentTemplate()),Bv(),ZE(n.hasFooter()?7:-1))},dependencies:[Vn,Ea,lr,Rt],encapsulation:2})}return e})();var Se=(()=>{class e{static ɵfac=function(o){return new(o||e)};static ɵmod=EE({type:e});static ɵinj=Wl({imports:[me,Ea,lr,Ea,lr]})}return e})();export{me as n,Se as t};