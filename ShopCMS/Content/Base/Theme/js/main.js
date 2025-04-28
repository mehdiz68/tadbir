var mod_pagespeed_WeC_GqZ9y2 = "(function($) {\n\n	\"use strict\";\n\n	$(window).stellar({\n    responsive: true,\n    parallaxBackgrounds: true,\n    parallaxElements: true,\n    horizontalScrolling: false,\n    hideDistantElements: false,\n    scrollProperty: 'scroll'\n  });\n\n\n	var fullHeight = function() {\n\n		$('.js-fullheight').css('height', $(window).height());\n		$(window).resize(function(){\n			$('.js-fullheight').css('height', $(window).height());\n		});\n\n	};\n	fullHeight();\n\n	// loader\n	var loader = function() {\n		setTimeout(function() { \n			if($('#ftco-loader').length > 0) {\n				$('#ftco-loader').removeClass('show');\n			}\n		}, 1);\n	};\n	loader();\n\n	// Scrollax\n  $.Scrollax();\n	\n\n	var carousel = function() {\n		$('.carousel-testimony').owlCarousel({\n			autoplay: true,\n			autoHeight: true,\n			center: true,\n			loop: true,\n			items:1,\n			margin: 30,\n			stagePadding: 0,\n			nav: false,\n			dots: true,\n			navText: ['<span class=\"ion-ios-arrow-back\">', '<span class=\"ion-ios-arrow-forward\">'],\n			responsive:{\n				0:{\n					items: 1\n				},\n				600:{\n					items: 1\n				},\n				1000:{\n					items: 1\n				}\n			}\n		});\n\n	};\n	carousel();\n\n	$('nav .dropdown').hover(function(){\n		var $this = $(this);\n		// 	 timer;\n		// clearTimeout(timer);\n		$this.addClass('show');\n		$this.find('> a').attr('aria-expanded', true);\n		// $this.find('.dropdown-menu').addClass('animated-fast fadeInUp show');\n		$this.find('.dropdown-menu').addClass('show');\n	}, function(){\n		var $this = $(this);\n			// timer;\n		// timer = setTimeout(function(){\n			$this.removeClass('show');\n			$this.find('> a').attr('aria-expanded', false);\n			// $this.find('.dropdown-menu').removeClass('animated-fast fadeInUp show');\n			$this.find('.dropdown-menu').removeClass('show');\n		// }, 100);\n	});\n\n\n	$('#dropdown04').on('show.bs.dropdown', function () {\n	  console.log('show');\n	});\n\n	// scroll\n	var scrollWindow = function() {\n		$(window).scroll(function(){\n			var $w = $(this),\n					st = $w.scrollTop(),\n					navbar = $('.ftco_navbar'),\n					sd = $('.js-scroll-wrap');\n\n			if (st > 150) {\n				if ( !navbar.hasClass('scrolled') ) {\n					navbar.addClass('scrolled');	\n				}\n			} \n			if (st < 150) {\n				if ( navbar.hasClass('scrolled') ) {\n					navbar.removeClass('scrolled sleep');\n				}\n			} \n			if ( st > 350 ) {\n				if ( !navbar.hasClass('awake') ) {\n					navbar.addClass('awake');	\n				}\n				\n				if(sd.length > 0) {\n					sd.addClass('sleep');\n				}\n			}\n			if ( st < 350 ) {\n				if ( navbar.hasClass('awake') ) {\n					navbar.removeClass('awake');\n					navbar.addClass('sleep');\n				}\n				if(sd.length > 0) {\n					sd.removeClass('sleep');\n				}\n			}\n		});\n	};\n	scrollWindow();\n\n\n	\n\n	var counter = function() {\n		\n		$('.ftco-counter').waypoint( function( direction ) {\n\n			if( direction === 'down' && !$(this.element).hasClass('ftco-animated') ) {\n\n				var comma_separator_number_step = $.animateNumber.numberStepFactories.separator(',')\n				$('.number').each(function(){\n					var $this = $(this),\n						num = $this.data('number');\n						// console.log(num);\n					$this.animateNumber(\n					  {\n					    number: num,\n					    numberStep: comma_separator_number_step\n					  }, 7000\n					);\n				});\n				\n			}\n\n		} , { offset: '95%' } );\n\n	}\n	counter();\n\n	var contentWayPoint = function() {\n		var i = 0;\n		$('.ftco-animate').waypoint( function( direction ) {\n\n			if( direction === 'down' && !$(this.element).hasClass('ftco-animated') ) {\n				\n				i++;\n\n				$(this.element).addClass('item-animate');\n				setTimeout(function(){\n\n					$('body .ftco-animate.item-animate').each(function(k){\n						var el = $(this);\n						setTimeout( function () {\n							var effect = el.data('animate-effect');\n							if ( effect === 'fadeIn') {\n								el.addClass('fadeIn ftco-animated');\n							} else if ( effect === 'fadeInLeft') {\n								el.addClass('fadeInLeft ftco-animated');\n							} else if ( effect === 'fadeInRight') {\n								el.addClass('fadeInRight ftco-animated');\n							} else {\n								el.addClass('fadeInUp ftco-animated');\n							}\n							el.removeClass('item-animate');\n						},  k * 50, 'easeInOutExpo' );\n					});\n					\n				}, 100);\n				\n			}\n\n		} , { offset: '95%' } );\n	};\n	contentWayPoint();\n\n	// magnific popup\n	$('.image-popup').magnificPopup({\n    type: 'image',\n    closeOnContentClick: true,\n    closeBtnInside: false,\n    fixedContentPos: true,\n    mainClass: 'mfp-no-margins mfp-with-zoom', // class to remove default margin from left and right side\n     gallery: {\n      enabled: true,\n      navigateByImgClick: true,\n      preload: [0,1] // Will preload 0 - before current, and 1 after the current image\n    },\n    image: {\n      verticalFit: true\n    },\n    zoom: {\n      enabled: true,\n      duration: 300 // don't foget to change the duration also in CSS\n    }\n  });\n\n  $('.popup-youtube, .popup-vimeo, .popup-gmaps').magnificPopup({\n    disableOn: 700,\n    type: 'iframe',\n    mainClass: 'mfp-fade',\n    removalDelay: 160,\n    preloader: false,\n\n    fixedContentPos: false\n  });\n\n  $('.appointment_date').datepicker({\n	  'format': 'm/d/yyyy',\n	  'autoclose': true\n	});\n	$('.appointment_time').timepicker();\n\n\n})(jQuery);\n\n";
eval(mod_pagespeed_Udcyhb9c2D);
eval(mod_pagespeed_DFAGfkuvS2);
eval(mod_pagespeed_M3P0bw$3eF);
eval(mod_pagespeed_AxF3aNNdP8);
eval(mod_pagespeed_WSJkyIcJ0Y);
eval(mod_pagespeed_ZfhpzvtydJ);
eval(mod_pagespeed_GUluxpY0vO);
eval(mod_pagespeed_c$OxnWiaz5);
eval(mod_pagespeed_PHXrUG1pI_);
eval(mod_pagespeed_nFFJ_JbqfP);
eval(mod_pagespeed_1F8Ofbwzlo);
eval(mod_pagespeed_MiF2CyD9s2);
eval(mod_pagespeed_WeC_GqZ9y2);

$("#slideshow > section:gt(0)").hide();

setInterval(function () {
    $('#slideshow > section:first')
        .fadeOut(1000)
        .next()
        .fadeIn(1000)
        .end()
        .appendTo('#slideshow');
}, 4000);

$('.carousel-services').owlCarousel({ autoplay: false, autoHeight: true, center: true, loop: true, items: 4, margin: 30, stagePadding: 0, nav: false, dots: true, navText: ['<span class=\"ion-ios-arrow-back\">', '<span class=\"ion-ios-arrow-forward\">'], responsive: { 0: { items: 2 }, 600: { items: 2 }, 1000: { items: 2 } } });


$("body").on("click", ".appointment-form input[type='submit']", function () {
    if ($("#Ad-FirstName").val() && $("#Ad-LastName").val() && $("#Ad-Phone").val()) {
        $(".loading").show();

        $.ajax({
            type: "Post",
            url: "/Home/NewRequest",
            data: JSON.stringify({ Name: $("#Ad-FirstName").val(), Family: $("#Ad-LastName").val(), Mobile: $("#Ad-Phone").val(), CatId: parseInt($("#Ad-Cat").val()), Message: $("#Ad-Message").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    if ($("#mLangId").val() == "1") {
                        $(".appointment-form").append("<p style='color:#fff;background-color:green;padding:5px;text-align:right' class='catmsg'> درخواست شما ثبت شد. </p>").fadeIn(300);
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
                    }
                    else {

                        $(".appointment-form").append("<p style='color:#fff;background-color:green;padding:5px' class='catmsg'> Your Request Submited. </p>").fadeIn(300);
                        $(".catmsg").delay(3000).fadeOut(300, function () { $(this).remove(); });
                    }
                    $("#Ad-FirstName").val("");
                    $("#Ad-LastName").val("");
                    $("#Ad-Phone").val("");
                    $("#Ad-Message").val("");
                }
                else
                    alert(response.Message);
                $(".loading").hide();
            }
        });
    }
    else {
        if ($("#mLangId").val() == "1")
            alert("لطفا همه موارد را وارد نمایید !");
        else
            alert("Please Fill All Field !");
    }
});
