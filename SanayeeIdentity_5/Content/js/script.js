(function($) {
	
	"use strict";
	
	//Take Dimensions of Several Things
	function adjustMents() {
		
		var windowWidth =	$(window).width();
		var windowHeight =	$(window).height();
		var docHeight = $(document).height();
		
		$('.preloader').css('width',windowWidth).css('height',windowHeight);
			
	}
	
	
	//Hide Loading Box (Preloader)
	function handlePreloader() {
		$('.preloader').delay(2000).fadeOut(500);
	}
	
	
	//Clients Testimonial Slider
	if($('.client-testimonials').length){
		$('.client-testimonials .slider').bxSlider({
			adaptiveHeight: true,
			auto:true,
			mode:'fade',
			controls: false,
			pause: 5000,
			speed: 1000,
			pager:true
		});
	}
	
	
	
	//Services Slider
	if($('.services-slider').length){
		$('.services-slider .slider').bxSlider({
			adaptiveHeight: true,
			auto:true,
			controls: false,
			pause: 5000,
			speed: 1000,
			pager:true
		});
	}
	
	
	
	//Accordion
	if($('.accordion-box').length){
		$(".accordion-box").on('click', '.acc-btn', function() {
		
			if($(this).hasClass('active')!=true){
			$('.accordion .acc-btn').removeClass('active');
			
			}
			
			if ($(this).next('.acc-content').is(':visible')){
				$(this).removeClass('active');
				$(this).next('.acc-content').slideUp(700);
			}else{
				$(this).addClass('active');
				$('.accordion .acc-content').slideUp(700);
				$(this).next('.acc-content').slideDown(700);	
			}
		});
	}
	
	
	
	//Tabs
	if($('.tabs-box').length){
		$('.tabs-box .tab-btn').on('click', function(e) {
			var target = $($(this).attr('href'));
			e.preventDefault();
			$('.tabs-box .tab-btn').removeClass('active');
			$(this).addClass('active');
			$('.tabs-box .tab').fadeOut(0);
			$(target).fadeIn(150);
		});
	}
	
	
	//Jquery Knob animation -- Circular Chart
	if($('.dial').length){
		$('.dial').appear(function(){
          var elm = $(this);
          var color = elm.attr("data-fgColor");  
          var perc = elm.attr("value");  
 
          elm.knob({ 
               'value': 0, 
                'min':0,
                'max':100,
                "skin":"tron",
                "readOnly":true,
                "thickness":.1,
				'dynamicDraw': true,
				"displayInput":false
          });

          $({value: 0}).animate({ value: perc }, {
			  duration: 1500,
              easing: 'swing',
              progress: function () {                  elm.val(Math.ceil(this.value)).trigger('change')
              }
          });

          //circular progress bar color
          $(this).append(function() {
              elm.parent().parent().find('.circular-bar-content').css('color',color);
              elm.parent().parent().find('.circular-bar-content label').text(perc+'%');
          });

          },{accY: 50});
    }
	
	
	
	//Mobile Menu / Visible on Small Screens
	function mobileNav() {
		//Toggle Mobile Menu
		$('.main-header .nav-container .toggle-icon').on('click', function(e) {
			$(this).next('.mobile-menu').toggleClass('appeared');
		});
		
		//Toggle Submenu
		$('.main-header .nav-box .toggle-icon').on('click', function(e) {
			$(this).next('ul').slideToggle(500);
		});
	}
	
	
	// Elements Animation
	function elementsAnimations() {
		$('.animated').appear(function(){
			var el = $(this);
			var anim = el.data('animation');
			var animDelay = el.data('delay');
			if (animDelay) {
	
				setTimeout(function(){
					el.addClass( anim + " in" );
					el.removeClass('out');
				}, animDelay);
	
			}
	
			else {
				el.addClass( anim + " in" );
				el.removeClass('out');
			}    
		},{accY: -150});
	}
	
	
	// Google Map Settings
	if($('#location-map').length){
		var map;
		 map = new GMaps({
			el: '#location-map',
			zoom: 18,
			scrollwheel:false,
			//Set Latitude and Longitude Here
			lat: -37.817085,
			lng: 144.955631
		  });
		  
		  //Add map Marker
		  map.addMarker({
			lat: -37.817085,
			lng: 144.955631,
			infoWindow: {
			  content: '<p><b>Envato</b> <br> Melbourne VIC 3000, Australia</p>'
			}
		 
		});
	}
	
	
	//Form Validation
	if($('#contact-form').length){
		$('#contact-form').validate({ // initialize the plugin
			rules: {
				subject: {
					required: true
				},
				name: {
					required: true
				},
				email: {
					required: true,
					email: true
				},
				ptype: {
					required: true
				},
				priority: {
					required: true
				},
				message: {
					required: true
				}
			},
			submitHandler: function (form) { 
				alert('Form Submitted');
				return true; 
			}
		});
	}
	// gallery filter activation
    function GalleryFilter () {
    	$('#image-gallery').mixItUp();
    }
    // custom gallery image activer
    function GalleryImgActivator () {
    	if($('#gallery-page').length) {
			$('#gallery-page a').fancybox();
    	}    	
    }
	// custom tab for what we do section 
    function sideTab () {
        if ($('.side-tab').length) {
            var tabWrap = $('.side-tab .col-lg-9');
            var tabClicker = $('.side-tab .col-lg-3 ul li a');
            
            tabWrap.find('div').hide();
            tabWrap.find('div').eq(0).show();
            tabClicker.on('click', function() {
                var tabName = $(this).data('tab-name');
                tabClicker.parent().removeClass('active');
                $(this).parent().addClass('active');
                var id = '#'+ tabName;
                tabWrap.find('div').not(id).hide();
                tabWrap.find('div'+id).fadeIn('800');
                return false;
            });
            tabClicker.each(function () {
            	$(this).addClass('hvr-bounce-to-right');
            });
        }
    }
    // faq page toggler
	if($('.toggle').length){
		$('.toggle').find('.toggle-content').hide();
		$('.toggle-title').on('click', function () {
			$(this).find('i').toggleClass('fa-plus fa-minus');
			$(this).parent().find('.toggle-content').slideToggle();
		});
	}
	
/* ==========================================================================
   When document is ready, do
   ========================================================================== */
   
	$(document).on('ready', function() {
		mobileNav();
		elementsAnimations();
		GalleryFilter();
		GalleryImgActivator();
		sideTab();
	});

/* ==========================================================================
   When document is loading, do
   ========================================================================== */
	
	$(window).on('load', function() {
		handlePreloader();
	});
	


////////////////////////////////////////
/////////// 2. HEADER STICK ///////////
//////////////////////////////////////

$(window).on('scroll', function(){
if($(this).scrollTop() > 10) {
$('.nav-container').addClass('header_stick'); }
else if($(this).scrollTop() <= 10) {
$('.nav-container').removeClass('header_stick'); }
});



})(window.jQuery);
