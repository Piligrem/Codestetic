; (function ($) {
    var defaults = {
    	enable: true,
    	header: '',
    	footer: '',
    	delayHiding: 10000,
    	template: '<div id="tooltip"></div>',
    	templateHeader: '<div class="tip-header"></div>',
		templateBody: '<div class="tip-body"></div>',
		trmplateFooter: '<div class="tip-footer"></div>'
    };

    var TitleTooltip = function (element, options) {
    	this.element = $(element); //.addClass('colorpicker-element');
    	this.options = $.extend(true, {}, defaults, this.element.data(), options);

    	this.tip = $(this.options.template);
    	this.tipHeader = $(this.options.templateHeader);
    	this.tipBody = $(this.options.templateBody);
    	this.tipFooter = $(this.options.trmplateFooter);

    	this.header = this.options.header;
    	this.title = '';
    	this.footer = this.options.footer;

    	this.tip.appendTo('body');

    	this.currentElement = {};
    	this._visible = false;

		//this.tip.find('.tip-body').html(this.element.data('title'));

		////Remove the title attribute's to avoid the native tooltip from the browser
		//this.element.data('title', '');

    	//this.element.on('mouseover.titletooltip touchstart.titletooltip', $.proxy(this.mouseover, this));
    	//this.element.on('mousemove.titletooltip touchstart.titletooltip', $.proxy(this.mousemove, this));
    	this.element.on('mousemove.titletooltip', $.proxy(this.mousemove, this));
    	this.element.on('mouseout.titletooltip touchend.titletooltip', $.proxy(this.mouseout, this));
    };

    TitleTooltip.prototype = {
    	constructor: TitleTooltip,
    	mouseover: function (e) {
    		var target = $(e.target)
    		if (target.data('title')) {
    			this.tip.find('.tip-body').html(target.data('title'));
    		}

    		//Remove the title attribute's to avoid the native tooltip from the browser
    		//this.element.data('title', '');

    		//Append the tooltip template and its value
    		this.tip.appendTo('body');

    		//Show the tooltip with faceIn effect
    		$('#tooltip').fadeIn('500');
    		$('#tooltip').fadeTo('10', 0.9);
    	},
    	mousemove: function (e) {
    		//Keep changing the X and Y axis for the tooltip, thus, the tooltip move along with the mouse
    		var element = $(document.elementFromPoint(e.pageX, e.pageY)),
				title = element.data('title');
			
    		if (title == undefined) {
    			title = element.closest('[data-title]').data('title');
    			element = element.closest('[data-title]');
    		}

    		if (title && title != this.title && !this._visible) {
    			this.title = title;
    			this.currentElement = element
    			this.show();
    			console.log('%cShow', 'color: blue');
    		} else if (title && title == this.title) {
    			this.tip.css('top', e.pageY + 10);
    			this.tip.css('left', e.pageX + 20);
    			//console.log('%cMove', 'color: green');
    		}
    		else if (title == undefined && this.currentElement != element) {
    			this.hide();
    			console.log('%cHide', 'color: red');
    		}

    		//this.tip.css('top', e.pageY + 10);
    		//this.tip.css('left', e.pageX + 20);
    		//$('#tooltip').fadeIn('500');
    		//$('#tooltip').fadeTo('10', 0.8);
    		return false;
    	},
    	mouseout: function (e) {
    		this.hide();
    		//Put back the title attribute's value
    		//this.element.data('title',$('.tipBody').html());
	
    		//Remove the appended tooltip template
    		//this.tip.detach();

    		////Append the tooltip template and its value
    		//this.tip.appendTo(this.element);

    		////Show the tooltip with faceIn effect
    		//$('#tooltip').fadeIn('500');
    		//$('#tooltip').fadeTo('10', 0.9);
    	},
    	isDisabled: function () {},
    	enable: function () {},
    	disable: function () { },
    	show: function () {
    		this._createTip();
    		this.tip.show(); //'fade', {}, 300);
    		this._visible = true;
    		//setTimeout(function (e) {
    		//	e.hide();
    		//}, this.options.delayHiding, this);
    	},
    	hide: function () {
    		this.tip.hide(); //'fade', {}, 300);
    		this.title = '';
    		this._visible = false;
    	},
    	_createTip: function () {
    		this.tip.html('');
    		this.tipHeader.html(this.header).appendTo(this.tip);
    		this.tipBody.html(this.title).appendTo(this.tip);
    		this.tipFooter.html(this.footer).appendTo(this.tip);
    		return true;
    	},
    	checkTitle: function (element) {
    		
    		return false;
    	}
    };

    $.titletooltip = TitleTooltip;
    $.fn.titletooltip = function (option) {
    	var markerArgs = arguments,
          rv;

    	var $returnValue = this.each(function () {
    		var $this = $(this),
              inst = $this.data('titletooltip'),
              options = ((typeof option === 'object') ? option : {});
    		if ((!inst) && (typeof option !== 'string')) {
    			$this.data('titletooltip', new TitleTooltip(this, options));
    		} else {
    			if (typeof option === 'string') {
    				rv = inst[option].apply(inst, Array.prototype.slice.call(markerArgs, 1));
    			}
    		}
    	});
    	if (option === 'getValue') {
    		return rv;
    	}
    	return $returnValue;
    };
    $.fn.titletooltip.constructor = TitleTooltip;
})(jQuery);