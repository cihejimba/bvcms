/*! http://mths.be/smoothscroll v1.5.2 by @mathias */
function AddSelected(t){switch(t.from){case"RelatedFamily":$("#related-families-div").loadWith("/Person2/RelatedFamilies/"+t.pid,function(){$(t.key).click()});break;case"Family":$("#family-div").loadWith("/Person2/FamilyMembers/"+t.pid);break;case"MergeTo":window.location="/Merge/{0}/{1}".format(t.pid,t.pid2)}}function RebindUserInfoGrid(){$.updateTable($("#user-tab form")),$("#memberDialog").dialog("close")}function dialogError(t){return t}!function(t,e){var o=function(){var o,n=e(t.documentElement),i=e(t.body);return n.scrollTop()?n:(o=i.scrollTop(),i.scrollTop(o+1).scrollTop()==o?n:i.scrollTop(o))}();e.fn.smoothScroll=function(t){return t=~~t||400,this.find('a[href*="#"]').click(function(n){var i=this.hash,r=e(i);location.pathname.replace(/^\//,"")===this.pathname.replace(/^\//,"")&&location.hostname===this.hostname&&r.length&&(n.preventDefault(),o.stop().animate({scrollTop:r.offset().top},t,function(){location.hash=i}))}).end()}}(document,jQuery),function(t,e){var o,n;return n=e.document,o=function(){function o(o){this._options=t.extend({name:"tour",container:"body",keyboard:!0,storage:e.localStorage,debug:!1,backdrop:!1,redirect:!0,orphan:!1,duration:!1,basePath:"",template:"<div class='popover'>          <div class='arrow'></div>          <h3 class='popover-title'></h3>          <div class='popover-content'></div>          <div class='popover-navigation'>            <div class='btn-group'>              <button class='btn btn-sm btn-default' data-role='prev'>&laquo; Prev</button>              <button class='btn btn-sm btn-default' data-role='next'>Next &raquo;</button>              <button class='btn btn-sm btn-default' data-role='pause-resume'                data-pause-text='Pause'                data-resume-text='Resume'              >Pause</button>            </div>            <button class='btn btn-sm btn-default' data-role='end'>End tour</button>          </div>        </div>",afterSetState:function(t,e){},afterGetState:function(t,e){},afterRemoveState:function(t){},onStart:function(t){},onEnd:function(t){},onShow:function(t){},onShown:function(t){},onHide:function(t){},onHidden:function(t){},onNext:function(t){},onPrev:function(t){},onPause:function(t,e){},onResume:function(t,e){}},o),this._force=!1,this._inited=!1,this._steps=[],this.backdrop={overlay:null,$element:null,$background:null,backgroundShown:!1,overlayElementShown:!1}}return o.prototype.setState=function(t,e){var o,n;if(this._options.storage){n=""+this._options.name+"_"+t;try{this._options.storage.setItem(n,e)}catch(i){o=i,o.code===DOMException.QUOTA_EXCEEDED_ERR&&this.debug("LocalStorage quota exceeded. setState failed.")}return this._options.afterSetState(n,e)}return null==this._state&&(this._state={}),this._state[t]=e},o.prototype.removeState=function(t){var e;return this._options.storage?(e=""+this._options.name+"_"+t,this._options.storage.removeItem(e),this._options.afterRemoveState(e)):null!=this._state?delete this._state[t]:void 0},o.prototype.getState=function(t){var e,o;return this._options.storage?(e=""+this._options.name+"_"+t,o=this._options.storage.getItem(e)):null!=this._state&&(o=this._state[t]),(void 0===o||"null"===o)&&(o=null),this._options.afterGetState(t,o),o},o.prototype.addSteps=function(t){var e,o,n,i;for(i=[],o=0,n=t.length;n>o;o++)e=t[o],i.push(this.addStep(e));return i},o.prototype.addStep=function(t){return this._steps.push(t)},o.prototype.getStep=function(e){return null!=this._steps[e]?t.extend({id:"step-"+e,path:"",placement:"right",title:"",content:"<p></p>",next:e===this._steps.length-1?-1:e+1,prev:e-1,animation:!0,container:this._options.container,backdrop:this._options.backdrop,redirect:this._options.redirect,orphan:this._options.orphan,duration:this._options.duration,template:this._options.template,onShow:this._options.onShow,onShown:this._options.onShown,onHide:this._options.onHide,onHidden:this._options.onHidden,onNext:this._options.onNext,onPrev:this._options.onPrev,onPause:this._options.onPause,onResume:this._options.onResume},this._steps[e]):void 0},o.prototype.init=function(t){var e=this;return this._force=t,this.ended()?this._debug("Tour ended, init prevented."):(this.setCurrentStep(),this._setupMouseNavigation(),this._setupKeyboardNavigation(),this._onResize(function(){return e.showStep(e._current)}),null!==this._current&&this.showStep(this._current),this._inited=!0,this)},o.prototype.start=function(t){var e;return null==t&&(t=!1),this._inited||this.init(t),null===this._current?(e=this._makePromise(null!=this._options.onStart?this._options.onStart(this):void 0),this._callOnPromiseDone(e,this.showStep,0)):void 0},o.prototype.next=function(){var t;return this.ended()?this._debug("Tour ended, next prevented."):(t=this.hideStep(this._current),this._callOnPromiseDone(t,this._showNextStep))},o.prototype.prev=function(){var t;return this.ended()?this._debug("Tour ended, prev prevented."):(t=this.hideStep(this._current),this._callOnPromiseDone(t,this._showPrevStep))},o.prototype.goTo=function(t){var e;return this.ended()?this._debug("Tour ended, goTo prevented."):(e=this.hideStep(this._current),this._callOnPromiseDone(e,this.showStep,t))},o.prototype.end=function(){var o,i,r=this;return o=function(o){return t(n).off("click.tour-"+r._options.name),t(n).off("keyup.tour-"+r._options.name),t(e).off("resize.tour-"+r._options.name),r.setState("end","yes"),r._inited=!1,r._force=!1,r._clearTimer(),null!=r._options.onEnd?r._options.onEnd(r):void 0},i=this.hideStep(this._current),this._callOnPromiseDone(i,o)},o.prototype.ended=function(){return!this._force&&!!this.getState("end")},o.prototype.restart=function(){return this.removeState("current_step"),this.removeState("end"),this.setCurrentStep(0),this.start()},o.prototype.pause=function(){var t;return t=this.getStep(this._current),t&&t.duration?(this._paused=!0,this._duration-=(new Date).getTime()-this._start,e.clearTimeout(this._timer),this._debug("Paused/Stopped step "+(this._current+1)+" timer ("+this._duration+" remaining)."),null!=t.onPause?t.onPause(this,this._duration):void 0):void 0},o.prototype.resume=function(){var t,o=this;return t=this.getStep(this._current),t&&t.duration?(this._paused=!1,this._start=(new Date).getTime(),this._duration=this._duration||t.duration,this._timer=e.setTimeout(function(){return o._isLast()?o.next():o.end()},this._duration),this._debug("Started step "+(this._current+1)+" timer with duration "+this._duration),null!=t.onResume&&this._duration!==t.duration?t.onResume(this,this._duration):void 0):void 0},o.prototype.hideStep=function(e){var o,n,i,r=this;return(i=this.getStep(e))?(this._clearTimer(),n=this._makePromise(null!=i.onHide?i.onHide(this,e):void 0),o=function(e){var o;return o=t(i.element),o.data("bs.popover")||o.data("popover")||(o=t("body")),o.popover("destroy"),i.reflex&&o.css("cursor","").off("click.tour-"+r._options.name),i.backdrop&&r._hideBackdrop(),null!=i.onHidden?i.onHidden(r):void 0},this._callOnPromiseDone(n,o),n):void 0},o.prototype.showStep=function(e){var o,i,r,a,s=this;return(a=this.getStep(e))?(r=e<this._current,o=this._makePromise(null!=a.onShow?a.onShow(this,e):void 0),i=function(o){var i,u;if(s.setCurrentStep(e),u=t.isFunction(a.path)?a.path.call():s._options.basePath+a.path,i=[n.location.pathname,n.location.hash].join(""),s._isRedirect(u,i))return void s._redirect(a,u);if(s._isOrphan(a)){if(!a.orphan)return s._debug("Skip the orphan step "+(s._current+1)+". Orphan option is false and the element doesn't exist or is hidden."),void(r?s._showPrevStep():s._showNextStep());s._debug("Show the orphan step "+(s._current+1)+". Orphans option is true.")}return a.backdrop&&s._showBackdrop(s._isOrphan(a)?void 0:a.element),s._scrollIntoView(a.element,function(){return null!=a.element&&a.backdrop&&s._showOverlayElement(a.element),s._showPopover(a,e),null!=a.onShown&&a.onShown(s),s._debug("Step "+(s._current+1)+" of "+s._steps.length)}),a.duration?s.resume():void 0},this._callOnPromiseDone(o,i),o):void 0},o.prototype.setCurrentStep=function(t){return null!=t?(this._current=t,this.setState("current_step",t)):(this._current=this.getState("current_step"),this._current=null===this._current?null:parseInt(this._current,10)),this},o.prototype._showNextStep=function(){var t,e,o,n=this;return o=this.getStep(this._current),e=function(t){return n.showStep(o.next)},t=this._makePromise(null!=o.onNext?o.onNext(this):void 0),this._callOnPromiseDone(t,e)},o.prototype._showPrevStep=function(){var t,e,o,n=this;return o=this.getStep(this._current),e=function(t){return n.showStep(o.prev)},t=this._makePromise(null!=o.onPrev?o.onPrev(this):void 0),this._callOnPromiseDone(t,e)},o.prototype._debug=function(t){return this._options.debug?e.console.log("Bootstrap Tour '"+this._options.name+"' | "+t):void 0},o.prototype._isRedirect=function(t,e){return null!=t&&""!==t&&t.replace(/\?.*$/,"").replace(/\/?$/,"")!==e.replace(/\/?$/,"")},o.prototype._redirect=function(e,o){return t.isFunction(e.redirect)?e.redirect.call(this,o):e.redirect===!0?(this._debug("Redirect to "+o),n.location.href=o):void 0},o.prototype._isOrphan=function(e){return null==e.element||!t(e.element).length||t(e.element).is(":hidden")&&"http://www.w3.org/2000/svg"!==t(e.element)[0].namespaceURI},o.prototype._isLast=function(){return this._current<this._steps.length-1},o.prototype._showPopover=function(e,o){var n,i,r,a,s,u,l=this;return u=t.extend({},this._options),r=t(t.isFunction(e.template)?e.template(o,e):e.template),i=r.find(".popover-navigation"),s=this._isOrphan(e),s&&(e.element="body",e.placement="top",r=r.addClass("orphan")),n=t(e.element),r.addClass("tour-"+this._options.name),e.options&&t.extend(u,e.options),e.reflex&&n.css("cursor","pointer").on("click.tour-"+this._options.name,function(){return l._isLast()?l.next():l.end()}),e.prev<0&&i.find("*[data-role=prev]").addClass("disabled"),e.next<0&&i.find("*[data-role=next]").addClass("disabled"),e.duration||i.find("*[data-role='pause-resume']").remove(),e.template=r.clone().wrap("<div>").parent().html(),n.popover({placement:e.placement,trigger:"manual",title:e.title,content:e.content,html:!0,animation:e.animation,container:e.container,template:e.template,selector:e.element}).popover("show"),a=n.data("bs.popover")?n.data("bs.popover").tip():n.data("popover").tip(),a.attr("id",e.id),this._reposition(a,e),s?this._center(a):void 0},o.prototype._reposition=function(e,o){var i,r,a,s,u,l,c;if(s=e[0].offsetWidth,r=e[0].offsetHeight,c=e.offset(),u=c.left,l=c.top,i=t(n).outerHeight()-c.top-e.outerHeight(),0>i&&(c.top=c.top+i),a=t("html").outerWidth()-c.left-e.outerWidth(),0>a&&(c.left=c.left+a),c.top<0&&(c.top=0),c.left<0&&(c.left=0),e.offset(c),"bottom"===o.placement||"top"===o.placement){if(u!==c.left)return this._replaceArrow(e,2*(c.left-u),s,"left")}else if(l!==c.top)return this._replaceArrow(e,2*(c.top-l),r,"top")},o.prototype._center=function(o){return o.css("top",t(e).outerHeight()/2-o.outerHeight()/2)},o.prototype._replaceArrow=function(t,e,o,n){return t.find(".arrow").css(n,e?50*(1-e/o)+"%":"")},o.prototype._scrollIntoView=function(o,n){var i,r,a,s,u,l=this;return o?(i=t(o),r=t(e),a=i.offset().top,u=r.height(),s=Math.max(0,a-u/2),this._debug("Scroll into view. ScrollTop: "+s+". Element offset: "+a+". Window height: "+u+"."),t("body").stop().animate({scrollTop:Math.ceil(s)},function(){return n(),l._debug("Scroll into view. Animation end element offset: "+i.offset().top+". Window height: "+r.height()+".")})):n()},o.prototype._onResize=function(o,n){return t(e).on("resize.tour-"+this._options.name,function(){return clearTimeout(n),n=setTimeout(o,100)})},o.prototype._setupMouseNavigation=function(){var e=this;return e=this,t(n).off("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=next]:not(.disabled)").on("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=next]:not(.disabled)",function(t){return t.preventDefault(),e.next()}),t(n).off("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=prev]:not(.disabled)").on("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=prev]:not(.disabled)",function(t){return t.preventDefault(),e.prev()}),t(n).off("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=end]").on("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=end]",function(t){return t.preventDefault(),e.end()}),t(n).off("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=pause-resume]").on("click.tour-"+this._options.name,".popover.tour-"+this._options.name+" *[data-role=pause-resume]",function(o){var n;return o.preventDefault(),n=t(this),n.text(n.data(e._paused?"pause-text":"resume-text")),e._paused?e.resume():e.pause()})},o.prototype._setupKeyboardNavigation=function(){var e=this;if(this._options.keyboard)return t(n).on("keyup.tour-"+this._options.name,function(t){if(t.which)switch(t.which){case 39:return t.preventDefault(),e._isLast()?e.next():e.end();case 37:if(t.preventDefault(),e._current>0)return e.prev();break;case 27:return t.preventDefault(),e.end()}})},o.prototype._makePromise=function(e){return e&&t.isFunction(e.then)?e:null},o.prototype._callOnPromiseDone=function(t,e,o){var n=this;return t?t.then(function(t){return e.call(n,o)}):e.call(this,o)},o.prototype._showBackdrop=function(e){return this.backdrop.backgroundShown?void 0:(this.backdrop=t("<div/>",{"class":"tour-backdrop"}),this.backdrop.backgroundShown=!0,t("body").append(this.backdrop))},o.prototype._hideBackdrop=function(){return this._hideOverlayElement(),this._hideBackground()},o.prototype._hideBackground=function(){return this.backdrop.remove(),this.backdrop.overlay=null,this.backdrop.backgroundShown=!1},o.prototype._showOverlayElement=function(e){var o,n,i;if(!this.backdrop.overlayElementShown)return this.backdrop.overlayElementShown=!0,n=t(e),o=t("<div/>"),i=n.offset(),i.top=i.top,i.left=i.left,o.width(n.innerWidth()).height(n.innerHeight()).addClass("tour-step-background").offset(i),n.addClass("tour-step-backdrop"),t("body").append(o),this.backdrop.$element=n,this.backdrop.$background=o},o.prototype._hideOverlayElement=function(){return this.backdrop.overlayElementShown?(this.backdrop.$element.removeClass("tour-step-backdrop"),this.backdrop.$background.remove(),this.backdrop.$element=null,this.backdrop.$background=null,this.backdrop.overlayElementShown=!1):void 0},o.prototype._clearTimer=function(){return e.clearTimeout(this._timer),this._timer=null,this._duration=null},o}(),e.Tour=o}(jQuery,window),$(function(){$("#split").live("click",function(t){t.preventDefault();var e=$(this).attr("href");bootbox.confirm("Are you sure you want to split this person into their own family?",function(t){t===!0&&$.post(e,{},function(t){window.location=t})})}),$("#deletePerson").live("click",function(t){t.preventDefault();var e=$(this).attr("href");bootbox.confirm("Are you sure you want to delete this record?",function(t){t===!0&&$.post(e,{},function(t){window.location=t})})}),$("a.editaddr").live("click",function(t){t.preventDefault(),$("<div />").load($(this).attr("href"),{},function(){var t=$(this),e=t.find("form");e.modal("show"),e.on("hidden",function(){t.remove(),e.remove()}),e.on("click","a.clear-address",function(){$("#AddressLineOne").val(""),$("#AddressLineTwo").val(""),$("#CityName").val(""),$("#ZipCode").val(""),$("#BadAddress").prop("checked",!1),$("#StateCode_Value").val(""),$("#StateCode_Value").trigger("chosen:updated"),$("#ResCode_Value").val("0"),$("#ResCode_Value").trigger("chosen:updated"),$("#Country_Value").val("United States"),$("#FromDt").val(""),$("#ToDt").val("")}),e.on("click","a.close-saved-address",function(){$.post($(this).attr("href"),{},function(t){$("#profile-header").html(t).ready(n)})})})}),$("a.personal-picture, a.family-picture").live("click",function(t){t.preventDefault(),$("<div />").load($(this).attr("href"),{},function(){var t=$(this),e=t.find("form");e.modal("show"),e.on("hidden",function(){t.remove(),e.remove()}),$("#delete-picture").click(function(t){t.preventDefault();var o=this;return bootbox.confirm("Are you sure you want to delete this picture?",function(t){t===!0&&(e.attr("action",o.href),e.submit())}),!1}),$("#refresh-thumbnail").click(function(t){t.preventDefault();var o=this;return e.attr("action",o.href),e.submit(),!1})})}),$("#family_related a.edit").live("click",function(t){t.preventDefault(),$("<div class='modal fade hide' />").load($(this).attr("href"),{},function(){var t=$(this);t.modal("show"),t.on("shown",function(){t.find("textarea").focus()}),t.on("hidden",function(){$(this).remove()}),t.on("click","a.save",function(e){e.preventDefault(),$.post($(this).attr("href"),{value:t.find("textarea").val()},function(e){$("#related-families-div").html(e),t.modal("hide")})}),t.on("click","a.delete",function(e){e.preventDefault();var o=$(this);return bootbox.confirm("Are you sure you want to remove this relationship?",function(e){e===!0&&$.post(o.attr("href"),{},function(e){$("#related-families-div").html(e),t.modal("hide")})}),!1})})}),$("a.membertype").live("click",function(t){t.preventDefault();$(this);$("<div />").load(this.href,{},function(){var t=$(this),e=t.find("form");e.modal("show"),e.on("hidden",function(){t.remove(),e.remove(),$.RebindMemberGrids()})})}),$('a[data-toggle="tab"]').on("shown",function(t){t.preventDefault();var e=$(t.target).attr("href").replace("#","#tab-");return window.location.hash=e,$.cookie("lasttab",e),!1});var t=$.cookie("lasttab");if(window.location.hash&&(t=window.location.hash),t){var e=$("a[href='"+t.replace("tab-","")+"']"),o=e.closest("ul").data("tabparent");o&&$("a[href='#"+o+"']").click().tab("show"),"#"!==e.attr("href")&&($.cookie("lasttab",e.attr("href")),e.click().tab("show"))}$("a[href='#enrollment']").on("shown",function(t){$("#current").length<2&&$("a[href='#current']").click().tab("show")}),$("a[href='#profile']").on("shown",function(t){var e="#memberstatus";$(e).length<2&&($("a[href='"+e+"']").click().tab("show"),$.cookie("lasttab",e))}),$("a[href='#ministry']").on("shown",function(t){var e="#contactsreceived";$(e).length<2&&($("a[href='"+e+"']").click().tab("show"),$.cookie("lasttab",e))}),$("a[href='#giving']").on("shown",function(t){var e="#contributions";$(e).length<2&&($("a[href='"+e+"']").click().tab("show"),$.cookie("lasttab",e))}),$("a[href='#emails']").on("shown",function(t){var e="#receivedemails";$(e).length<2&&($("a[href='"+e+"']").click().tab("show"),$.cookie("lasttab",e))}),$("a[href='#system']").on("shown",function(t){var e="#user";$(e).length<2&&($("a[href='"+e+"']").click().tab("show"),$.cookie("lasttab",e))}),$("#future").live("click",function(t){t.preventDefault();var e=$(this).closest("div.loaded"),o=e.find("form").serialize();$.post($("#FutureLink").val(),o,function(t){e.html(t)})}),$("#addrf").validate(),$("#addrp").validate(),$("#basic").validate(),$("body").on("change",".atck",function(t){var e=$(this);$.post("/Meeting/MarkAttendance/",{MeetingId:$(this).attr("mid"),PeopleId:$(this).attr("pid"),Present:e.is(":checked")},function(t){t.error&&(e.attr("checked",!e.is(":checked")),alert(t.error))})}),$("#vtab>ul>li").click(function(){$("#vtab>ul>li").removeClass("selected"),$(this).addClass("selected");var t=$("#vtab>ul>li").index($(this));$("#vtab>div").hide().eq(t).show()});var n=function(){$(".popover-map").dropdown(),$("#PositionInFamily").editable({source:[{value:10,text:"Primary Adult"},{value:20,text:"Secondary Adult"},{value:30,text:"Child"}],type:"select",url:"/Person2/PostData",name:"position"}),$("#Campus").editable({source:"/Person2/Campuses",type:"select",url:"/Person2/PostData",name:"campus"})};n(),$.InitFunctions.Editable=function(){$("a.editable").editable(),$("a.editable-bit").editable({type:"checklist",mode:"inline",source:{True:"True"},emptytext:"False"})},$("a.click-pencil").live("click",function(t){return t.stopPropagation(),$(this).prev().editable("toggle"),!1}),$("a.visibilityroles").live("click",function(t){return t.preventDefault(),$(this).editable("toggle"),!1}),$.InitFunctions.MemberDocsEditable=function(){$("#memberdocs-form a.editable").editable({placement:"right",showbuttons:"bottom"})},$("#failedemails a.unblock").live("click",function(t){confirm("are you sure?")&&$.post("/Manage/Emails/Unblock",{email:$(this).attr("email")},function(t){$.growlUI("email unblocked",t)})}),$("#failedemails a.unspam").live("click",function(t){confirm("are you sure?")&&$.post("/Manage/Emails/Unspam",{email:$(this).attr("email")},function(t){$.growlUI("email unspam",t)})}),$.RebindMemberGrids=function(){$("#refresh-current").click(),$("#refresh-pending").click(),$("#refresh-previous").click()}}),$(function(){$("#membergroups .ckbox").live("click",function(t){var e=$(this).closest("form"),o=e.serialize()+"&"+$.param({ck:$(this).is(":checked")});return $.post($(this).attr("href"),o),!0}),$("#membergroups .update-smallgroup").live("click",function(t){t.preventDefault();var e=$(this).attr("href"),o="This will add or remove everybody to/from this sub-group. Are you sure?";return bootbox.confirm(o,function(t){t&&$.post(e)}),!1}),$("#OrgSearch").live("keydown",function(t){13===t.keyCode&&(t.preventDefault(),$("#orgsearchbtn").click())}),$("a.movemember").live("click",function(t){t.preventDefault();var e=$(this).closest("form"),o=e.serialize(),n=$(this).attr("href");return bootbox.confirm("are you sure?",function(t){t&&$.post(n,o,function(t){e.modal("hide"),RebindMemberGrids()})}),!1})});