/*===================================
Author       : Bestwebcreator.
Template Name: Shopwise - eCommerce Bootstrap 4 HTML Template
Version      : 1.0
===================================*/

/*===================================*
PAGE JS
*===================================*/



(function ($) {
    'use strict';
    var instance = $("img.lazy").lazyload({
        effect: "fadeIn"
    });

    /*===================================*
    01. LOADING JS
    /*===================================*/
    $(window).on('load', function () {
        setTimeout(function () {
            $(".preloader").delay(700).fadeOut(700).addClass('loaded');
        }, 800);
        $('#PopupMain').modal('show');
    });

    /*===================================*
    02. BACKGROUND IMAGE JS
    *===================================*/
    /*data image src*/
    $(".background_bg").each(function () {
        var attr = $(this).attr('data-img-src');
        if (typeof attr !== typeof undefined && attr !== false) {
            $(this).css('background-image', 'url(' + attr + ')');
        }
    });

    /*===================================*
    03. ANIMATION JS
    *===================================*/
    //$(function () {

    //    function ckScrollInit(items, trigger) {
    //        items.each(function () {
    //            var ckElement = $(this),
    //                AnimationClass = ckElement.attr('data-animation'),
    //                AnimationDelay = ckElement.attr('data-animation-delay');

    //            ckElement.css({
    //                '-webkit-animation-delay': AnimationDelay,
    //                '-moz-animation-delay': AnimationDelay,
    //                'animation-delay': AnimationDelay,
    //                opacity: 0
    //            });

    //            var ckTrigger = (trigger) ? trigger : ckElement;

    //            ckTrigger.waypoint(function () {
    //                ckElement.addClass("animated").css("opacity", "1");
    //                ckElement.addClass('animated').addClass(AnimationClass);
    //            }, {
    //                triggerOnce: true,
    //                offset: '90%',
    //            });
    //        });
    //    }

    //    ckScrollInit($('.animation'));
    //    ckScrollInit($('.staggered-animation'), $('.staggered-animation-wrap'));

    //});

    /*===================================*
    04. MENU JS
    *===================================*/
    //Main navigation scroll spy for shadow
    $(window).on('scroll', function () {
        var scroll = $(window).scrollTop();

        if (scroll >= 150) {
            $('header.fixed-top').addClass('nav-fixed');
        } else {
            $('header.fixed-top').removeClass('nav-fixed');
        }

    });

    //Show Hide dropdown-menu Main navigation 
    $(document).on('ready', function () {
        $(document).ajaxComplete(function () {
            $("img.lazy").lazyload({
                effect: "fadeIn"
            });
        });
        //$(".cart_trigger").click(function () {
        //    window.location.href = "/cart";
        //});

        $("#Load-More").click(function () {
            $(".loading").show();
            var t = $(this).offset().top;
            var $this = $(this);
            $.ajax({
                type: "POST",
                url: "/Home/LoadMorePr",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    if (response.statusCode == 200) {
                        $("#Random-Products").append(response.NewRow);

                        $('html, body').animate({
                            scrollTop: t + 40
                        }, 600);
                    }
                    else if (response.statusCode == 404) {
                        window.location.href = "/";
                    }
                    else {
                        alert(response.message);
                    }

                    $(".loading").hide();
                    $('.product_color_switch span').each(function () {
                        var get_color = $(this).attr('data-color');
                        $(this).css("background-color", get_color);
                    });
                    if ($("#Random-Products .item").length > 3) {
                        $("#Load-More").remove();
                    }
                }

            });
        });
        $('.dropdown-menu a.dropdown-toggler').on('click', function () {
            //var $el = $( this );
            //var $parent = $( this ).offsetParent( ".dropdown-menu" );
            if (!$(this).next().hasClass('show')) {
                $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
            }
            var $subMenu = $(this).next(".dropdown-menu");
            $subMenu.toggleClass('show');

            $(this).parent("li").toggleClass('show');

            $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function () {
                $('.dropdown-menu .show').removeClass("show");
            });

            return false;
        });
    });

    //Hide Navbar Dropdown After Click On Links
    var navBar = $(".header_wrap");
    var navbarLinks = navBar.find(".navbar-collapse ul li a.page-scroll");

    $.each(navbarLinks, function () {

        var navbarLink = $(this);

        navbarLink.on('click', function () {
            navBar.find(".navbar-collapse").collapse('hide');
            $("header").removeClass("active");
        });

    });

    //Main navigation Active Class Add Remove
    $('.navbar-toggler').on('click', function () {
        $("header").toggleClass("active");
        if ($('.search-overlay').hasClass('open')) {
            $(".search-overlay").removeClass('open');
            $(".search_trigger").removeClass('open');
        }
    });

    $(document).on('ready', function () {
        if ($('.header_wrap').hasClass("fixed-top") && !$('.header_wrap').hasClass("transparent_header") && !$('.header_wrap').hasClass("no-sticky")) {
            $(".header_wrap").before('<div class="header_sticky_bar d-none"></div>');
        }
    });

    $(window).on('scroll', function () {
        var scroll = $(window).scrollTop();

        if (scroll >= 150) {

            if (parseInt($(window).width()) <= 768) {
                $("header .product_search_form").css("position", "fixed");
            }

            $('.header_sticky_bar').removeClass('d-none');
            $('header.no-sticky').removeClass('nav-fixed');

        }
        else {
            $('.header_sticky_bar').addClass('d-none');

            if (parseInt($(window).width()) <= 768) {
                $("header .product_search_form").css("position", "unset");
            }
        }

    });

    var setHeight = function () {
        var height_header = $(".header_wrap").height();
        $('.header_sticky_bar').css({ 'height': height_header });
    };

    $(window).on('load', function () {
        setHeight();
    });

    $(window).on('resize', function () {
        setHeight();
    });

    $('.sidetoggle').on('click', function () {
        $(this).addClass('open');
        $('body').addClass('sidetoggle_active');
        $('.sidebar_menu').addClass('active');
        $("body").append('<div id="header-overlay" class="header-overlay"></div>');
    });

    $(document).on('click', '#header-overlay, .sidemenu_close', function () {
        $('.sidetoggle').removeClass('open');
        $('body').removeClass('sidetoggle_active');
        $('.sidebar_menu').removeClass('active');
        $('#header-overlay').fadeOut('3000', function () {
            $('#header-overlay').remove();
        });
        return false;
    });

    $(".categories_btn").on('click', function () {
        $('.side_navbar_toggler').attr('aria-expanded', 'false');
        $('#navbarSidetoggle').removeClass('show');
    });

    $(".side_navbar_toggler").on('click', function () {
        $('.categories_btn').attr('aria-expanded', 'false');
        $('#navCatContent').removeClass('show');
    });

    $(".pr_search_trigger").on('click', function () {
        $(this).toggleClass('show');
        $('.product_search_form').toggleClass('show');
    });

    var rclass = true;

    $("html").on('click', function () {
        if (rclass) {
            $('.categories_btn').addClass('collapsed');
            $('.categories_btn,.side_navbar_toggler').attr('aria-expanded', 'false');
            $('#navCatContent,#navbarSidetoggle').removeClass('show');
        }
        rclass = true;
    });

    $(".categories_btn,#navCatContent,#navbarSidetoggle .navbar-nav,.side_navbar_toggler").on('click', function () {
        rclass = false;
    });

    /*===================================*
    05. SMOOTH SCROLLING JS
    *===================================*/
    // Select all links with hashes

    var topheaderHeight = $(".top-header").innerHeight();
    var mainheaderHeight = $(".header_wrap").innerHeight();
    var headerHeight = mainheaderHeight - topheaderHeight - 20;
    //$('a.page-scroll[href*="#"]:not([href="#"])').on('click', function () {
    //    $('a.page-scroll.active').removeClass('active');
    //    $(this).closest('.page-scroll').addClass('active');
    //     On-page links
    //    if (location.pathname.replace(/^\//, '') === this.pathname.replace(/^\//, '') && location.hostname === this.hostname) {
    //         Figure out element to scroll to
    //        var target = $(this.hash),
    //            speed = $(this).data("speed") || 800;
    //        target = target.length ? target : $('[name=' + this.hash.slice(1) + ']');

    //         Does a scroll target exist?
    //        if (target.length) {
    //             Only prevent default if animation is actually gonna happen
    //            event.preventDefault();
    //            $('html, body').animate({
    //                scrollTop: target.offset().top - headerHeight
    //            }, speed);
    //        }
    //    }
    //});
    $(window).on('scroll', function () {
        var lastId,
            // All list items
            menuItems = $(".header_wrap").find("a.page-scroll"),
            topMenuHeight = $(".header_wrap").innerHeight() + 20,
            // Anchors corresponding to menu items
            scrollItems = menuItems.map(function () {
                var items = $($(this).attr("href"));
                if (items.length) { return items; }
            });
        var fromTop = $(this).scrollTop() + topMenuHeight;

        // Get id of current scroll item
        var cur = scrollItems.map(function () {
            if ($(this).offset().top < fromTop)
                return this;
        });
        // Get the id of the current element
        cur = cur[cur.length - 1];
        var id = cur && cur.length ? cur[0].id : "";

        if (lastId !== id) {
            lastId = id;
            // Set/remove active class
            menuItems.closest('.page-scroll').removeClass("active").end().filter("[href='#" + id + "']").closest('.page-scroll').addClass("active");
        }

    });

    $('.more_slide_open').slideUp();
    $('.more_categories').on('click', function () {
        $(this).toggleClass('show');
        $('.more_slide_open').slideToggle();
    });

    /*===================================*
    06. SEARCH JS
    *===================================*/

    $(".close-search").on("click", function () {
        $(".search_wrap,.search_overlay").removeClass('open');
        $("body").removeClass('search_open');
    });

    var removeClass = true;
    $(".search_wrap").after('<div class="search_overlay"></div>');
    $(".search_trigger").on('click', function () {
        $(".search_wrap,.search_overlay").toggleClass('open');
        $("body").toggleClass('search_open');
        removeClass = false;
        if ($('.navbar-collapse').hasClass('show')) {
            $(".navbar-collapse").removeClass('show');
            $(".navbar-toggler").addClass('collapsed');
            $(".navbar-toggler").attr("aria-expanded", false);
        }
    });
    $(".search_wrap form").on('click', function () {
        removeClass = false;
    });
    $("html").on('click', function () {
        if (removeClass) {
            $("body").removeClass('open');
            $(".search_wrap,.search_overlay").removeClass('open');
            $("body").removeClass('search_open');
        }
        removeClass = true;
    });

    /*===================================*
    07. SCROLLUP JS
    *===================================*/
    $(window).on('scroll', function () {
        if (!$(".siteAside").hasClass("not-show-all")) {
            if ($(this).scrollTop() > 150) {
                $('.siteAside').fadeIn();
            } else {
                $('.siteAside').fadeOut();
            }
        }
    });

    $(".js-toTop").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: 0
        }, 600);
        return false;
    });
    $(".js-asideLink-shop").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $("#Recommends").offset().top - 50
        }, 600);
        return false;
    });
    $(".js-asideLink-off").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $(".OFF-Products").offset().top - 50
        }, 600);
        return false;
    });
    $(".js-asideLink-new").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $("#New-Products").offset().top - 50
        }, 600);
        return false;
    });
    $(".js-asideLink-brand").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $("#Top-Brands").offset().top - 50
        }, 600);
        return false;
    });
    $(".js-asideLink-cat").on('click', function (e) {
        e.preventDefault();
        $('html, body').animate({
            scrollTop: $("#Top-Categories").offset().top - 50
        }, 600);
        return false;
    });
    /*===================================*
    08. Hover Parallax Js
    *===================================*/
    if ($(".scene").length > 0) {
        var sceneElements = document.querySelectorAll('.scene');
        var parallaxScenes = [];
        for (var i = 0; i < sceneElements.length; i++) {
            parallaxScenes.push(new Parallax(sceneElements[i]));
        }
        var scene = $(".scene")[0];
        var parallax = new Parallax(scene, {
            scalarX: 5,
            scalarY: 5
        });
    }

    /*===================================*
    09. PARALLAX JS
    *===================================*/
    $(window).on('load', function () {
        $('.parallax_bg').parallaxBackground();
    });

    /*===================================*
    10. COUNTER JS
    *===================================*/
    var timer = $('.counter');
    if (timer.length) {
        timer.appear(function () {
            timer.countTo();
        });
    }

    /*===================================*
    11. MASONRY JS
    *===================================*/
    $(window).on("load", function () {
        var $grid_selectors = $(".grid_container");
        //var filter_selectors = ".grid_filter > li > a";
        //if ($grid_selectors.length > 0) {
        //    $grid_selectors.imagesLoaded(function () {
        //        if ($grid_selectors.hasClass("masonry")) {
        //            $grid_selectors.isotope({
        //                itemSelector: '.grid_item',
        //                percentPosition: true,
        //                layoutMode: "masonry",
        //                masonry: {
        //                    columnWidth: '.grid-sizer'
        //                },
        //            });
        //        }
        //        else {
        //            $grid_selectors.isotope({
        //                itemSelector: '.grid_item',
        //                percentPosition: true,
        //                layoutMode: "fitRows",
        //            });
        //        }
        //    });
        //}

        ////isotope filter
        //$(document).on("click", filter_selectors, function () {
        //    $(filter_selectors).removeClass("current");
        //    $(this).addClass("current");
        //    var dfselector = $(this).data('filter');
        //    if ($grid_selectors.hasClass("masonry")) {
        //        $grid_selectors.isotope({
        //            itemSelector: '.grid_item',
        //            layoutMode: "masonry",
        //            masonry: {
        //                columnWidth: '.grid_item'
        //            },
        //            filter: dfselector
        //        });
        //    }
        //    else {
        //        $grid_selectors.isotope({
        //            itemSelector: '.grid_item',
        //            layoutMode: "fitRows",
        //            filter: dfselector
        //        });
        //    }
        //    return false;
        //});

        //$('.portfolio_filter').on('change', function () {
        //    $grid_selectors.isotope({
        //        filter: this.value
        //    });
        //});

        //$(window).on("resize", function () {
        //    setTimeout(function () {
        //        $grid_selectors.find('.grid_item').removeClass('animation').removeClass('animated'); // avoid problem to filter after window resize
        //        $grid_selectors.isotope('layout');
        //    }, 300);
        //});
    });

    //$('.link_container').each(function () {
    //    $(this).magnificPopup({
    //        delegate: '.image_popup',
    //        type: 'image',
    //        mainClass: 'mfp-zoom-in',
    //        removalDelay: 500,
    //        gallery: {
    //            enabled: true
    //        }
    //    });
    //});

    /*===================================*
    12. SLIDER JS
    *===================================*/
    function carousel_slider() {
        $('.carousel_slider').each(function () {
            var $carousel = $(this);
            $carousel.owlCarousel({
                dots: $carousel.data("dots"),
                loop: $carousel.data("loop"),
                items: $carousel.data("items"),
                margin: $carousel.data("margin"),
                mouseDrag: $carousel.data("mouse-drag"),
                touchDrag: $carousel.data("touch-drag"),
                autoHeight: $carousel.data("autoheight"),
                center: $carousel.data("center"),
                nav: $carousel.data("nav"),
                rewind: $carousel.data("rewind"),
                navText: ['<i class="ion-ios-arrow-left"></i>', '<i class="ion-ios-arrow-right"></i>'],
                autoplay: $carousel.data("autoplay"),
                animateIn: $carousel.data("animate-in"),
                animateOut: $carousel.data("animate-out"),
                autoplayTimeout: $carousel.data("autoplay-timeout"),
                smartSpeed: $carousel.data("smart-speed"),
                responsive: $carousel.data("responsive")
            });
            $carousel.on("change.owl.carousel", function (e) {
                var t = $(e.target).parent().offset().top - 150;
                $('html, body').animate({
                    scrollTop: t - 1
                }, 600);
                $('html, body').animate({
                    scrollTop: t + 1
                }, 600);
            });
            if ($carousel.hasClass("NewPr")) {
                $("#nextPr").click(function () {
                    //var t = $(this).offset().top;
                    // $('html, body').animate({
                    //    scrollTop: t-1 
                    //}, 600);
                    //$('html, body').animate({
                    //    scrollTop: t +1
                    //}, 600);
                    $carousel.trigger('next.owl.carousel');

                });
                $("#prevPr").click(function () {
                    $carousel.trigger('prev.owl.carousel');
                });

            }
            else if ($carousel.hasClass("SalePr")) {
                $("#nextSale").click(function () {
                    $carousel.trigger('next.owl.carousel');
                });
                $("#prevSale").click(function () {
                    $carousel.trigger('prev.owl.carousel');
                });
            }

        });
    }
    function slick_slider() {
        $('.slick_slider').each(function () {
            var $slick_carousel = $(this);
            $slick_carousel.slick({
                arrows: $slick_carousel.data("arrows"),
                dots: $slick_carousel.data("dots"),
                infinite: $slick_carousel.data("infinite"),
                centerMode: $slick_carousel.data("center-mode"),
                vertical: $slick_carousel.data("vertical"),
                fade: $slick_carousel.data("fade"),
                cssEase: $slick_carousel.data("css-ease"),
                autoplay: $slick_carousel.data("autoplay"),
                verticalSwiping: $slick_carousel.data("vertical-swiping"),
                autoplaySpeed: $slick_carousel.data("autoplay-speed"),
                speed: $slick_carousel.data("speed"),
                pauseOnHover: $slick_carousel.data("pause-on-hover"),
                draggable: $slick_carousel.data("draggable"),
                slidesToShow: $slick_carousel.data("slides-to-show"),
                slidesToScroll: $slick_carousel.data("slides-to-scroll"),
                asNavFor: $slick_carousel.data("as-nav-for"),
                focusOnSelect: $slick_carousel.data("focus-on-select"),
                responsive: $slick_carousel.data("responsive")
            });
        });
    }


    $(document).on("ready", function () {
        carousel_slider();
        slick_slider();

        //حذف از سبد
        $("body").on("click", ".remove-basket-item", function () {
            $(".loading").show();
            var $this = $(this);
            $.ajax({
                type: "POST",
                url: "/Cart/RemoveBasketItem/" + $this.attr("data-id"),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $this.parent().parent().remove();
                    }
                    else if (response.statusCode == 404) {
                        window.location.href = "/";
                    }
                    else {
                        alert(response.message);
                    }
                    $(".cart_count").text(response.Count);

                    $(".loading").hide();
                }
            });
        });

        $("body").on("click", ".remove-all-basket", function () {
            $(".loading").show();
            var $this = $(this);
            $.ajax({
                type: "POST",
                url: "/Cart/RemoveAllBasketItem/",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {

                    window.location.href = "/";

                    $(".cart_count").text("0");

                    $(".loading").hide();
                }
            });
        });

        $("#navCatContent ul li .dropdown-menu").css("min-width", $("#navbarSidetoggle .navbar-nav").width());

        var host = "http://" + window.location.host + "/";

        (function ($) {
            $.fn.donetyping = function (callback) {
                var _this = $(this);
                var x_timer;
                _this.keyup(function () {
                    clearTimeout(x_timer);
                    x_timer = setTimeout(clear_timer, 1000);
                });

                function clear_timer() {
                    clearTimeout(x_timer);
                    callback.call(_this);
                }
            }
        })(jQuery);


        if (window.innerWidth > 768) {
            $("#navCatContent li a").click(function (e) {
                e.stopPropagation();
                window.location.href = $(this).attr("href");
            });
        }

        $('#search_input').donetyping(function () {
            $("#SearchList > div .content").html("");
            var keyword = $("#search_input").val().toString().trim();
            if (keyword.length > 1) {
                $.ajax({
                    type: "POST",
                    url: "/Home/MainSearch?keyword=" + keyword + "",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (result) {
                        if (result.statusCode == 200) {
                            $("#SearchList").fadeIn();
                            $("#SearchList").addClass("flex");
                            //Render Result
                            var resultAsJson = JSON.parse(JSON.stringify(result.data));
                            if (resultAsJson.length > 0) {
                                for (var i in resultAsJson) {
                                    var Sr = resultAsJson[i];
                                    var Title = Sr.Title;
                                    var PageAddress = Sr.PageAddress;
                                    var Name = Sr.Name;
                                    var ID = Sr.Id;
                                    var Value = Sr.Value;
                                    var ElementType = Sr.TypeId;

                                    switch (true) {
                                        case (ElementType == 1):
                                            //برند
                                            $("#SearchList .ProductList .content").append("<div class='Brand'><a href='/TFB/" + ID + "/" + PageAddress + "' target='_blank'>همه کالاها برند <label>" + Name + "</label></a></div>"); break;
                                        case (ElementType == 2):
                                            //گروه محصول
                                            $("#SearchList .ProductList .content").append("<div class='Category'><a href='/TFS/" + ID + "/" + PageAddress + "' target='_blank'>همه کالاها گروه <label>" + Name + "</label></a></div>"); break;
                                        case (ElementType == 3):
                                            {
                                                //محصول
                                                if (Value != null)
                                                    $("#SearchList .ProductList .content").append("<div class='Product'><a href='/TFS/" + ID + "/" + PageAddress + "?q=" + Title + ' ' + Value + "' target='_blank'>" + Title + " در دسته <label>" + Name + "</label></a></div>");
                                                else
                                                    $("#SearchList .ProductList .content").append("<div class='Product'><a href='/TFS/" + ID + "/" + PageAddress + "?q=" + Title + "' target='_blank'>" + Title + " در دسته <label>" + Name + "</label></a></div>");
                                                break;
                                            }
                                    }
                                }
                            }
                            else {
                                $("#SearchList .ProductList .content").append("<div class='Category'><a href='/Search?q=" + keyword + "' target='_blank'> نمایش همه نتایج برای <label>" + keyword + "</label></a></div>");

                            }

                        }


                        $("#search_input").css("background-image", "");

                    }//Success
                });
            }
            else {
                $("#SearchList").fadeOut();
                $("#SearchList").removeClass("flex");
            }
        });
        $("#search_input").keyup(function (e) {
            var keyword = $("#search_input").val().toString().trim();
            if (keyword.length > 1 && e.keyCode != 13 && e.keyCode != 16 && e.keyCode != 17 && e.keyCode != 18)
                $("#search_input").css("background-image", "url(" + host + "content/Default/images/ajax-loader.gif)").css("background-repeat", "no-repeat");
            else
                $("#search_input").css("background-image", "");
        });
        $("*").keydown(function (e) {
            if (e.which == 27) {
                $("#search_input").val("");
                $("#SearchList").fadeOut();
                $("#SearchList").removeClass("flex");
                $("#search_input").css("background-image", "");
            }
        });
        $(document).mouseup(function (e) {
            var container = $("#SearchList");

            if (!container.is(e.target) // if the target of the click isn't the container...
                && container.has(e.target).length === 0) // ... nor a descendant of the container
            {
                container.slideUp();
                $("#search_input").val("");
                $("#search_input").css("background-image", "");
                $("#SearchList").removeClass("flex");
            }
        });
    });
    /*===================================*
    13. CONTACT FORM JS
    *===================================*/
    $("#submitButton").on("click", function (event) {
        event.preventDefault();
        var mydata = $("form").serialize();
        $.ajax({
            type: "POST",
            dataType: "json",
            url: "contact.php",
            data: mydata,
            success: function (data) {
                if (data.type === "error") {
                    $("#alert-msg").removeClass("alert, alert-success");
                    $("#alert-msg").addClass("alert, alert-danger");
                } else {
                    $("#alert-msg").addClass("alert, alert-success");
                    $("#alert-msg").removeClass("alert, alert-danger");
                    $("#first-name").val("Enter Name");
                    $("#email").val("Enter Email");
                    $("#phone").val("Enter Phone Number");
                    $("#subject").val("Enter Subject");
                    $("#description").val("Enter Message");

                }
                $("#alert-msg").html(data.msg);
                $("#alert-msg").show();
            },
            error: function (xhr, textStatus) {
                alert(textStatus);
            }
        });
    });

    /*===================================*
    14. POPUP JS
    *===================================*/
    //$('.content-popup').magnificPopup({
    //    type: 'inline',
    //    preloader: true,
    //    mainClass: 'mfp-zoom-in',
    //});

    //$('.image_gallery').each(function () { // the containers for all your galleries
    //    $(this).magnificPopup({
    //        delegate: 'a', // the selector for gallery item
    //        type: 'image',
    //        gallery: {
    //            enabled: true,
    //        },
    //    });
    //});

    //$('.popup-ajax').magnificPopup({
    //    type: 'ajax',
    //    callbacks: {
    //        ajaxContentAdded: function () {
    //            carousel_slider();
    //            slick_slider();
    //        }
    //    }
    //});

    //$('.video_popup, .iframe_popup').magnificPopup({
    //    type: 'iframe',
    //    removalDelay: 160,
    //    mainClass: 'mfp-zoom-in',
    //    preloader: false,
    //    fixedContentPos: false
    //});

    /*===================================*
    15. Select dropdowns
    *===================================*/

    if ($('select').length) {
        // Traverse through all dropdowns
        $.each($('select'), function (i, val) {
            var $el = $(val);

            if ($el.val() === "") {
                $el.addClass('first_null');
            }

            if (!$el.val()) {
                $el.addClass('not_chosen');
            }

            $el.on('change', function () {
                if (!$el.val())
                    $el.addClass('not_chosen');
                else
                    $el.removeClass('not_chosen');
            });

        });
    }

    /*==============================================================
    16. FIT VIDEO JS
    ==============================================================*/
    if ($(".fit-videos").length > 0) {
        $(".fit-videos").fitVids({
            customSelector: "iframe[src^='https://w.soundcloud.com']"
        });
    }

    /*==============================================================
    17. DROPDOWN JS
    ==============================================================*/
    if ($(".custome_select").length > 0) {
        $(document).on('ready', function () {
            $(".custome_select").msDropdown();
        });
    }

    /*===================================*
    18. PROGRESS BAR JS
    *===================================*/
    $('.progress-bar').each(function () {
        var width = $(this).attr('aria-valuenow');
        $(this).appear(function () {
            $(this).css('width', width + '%');
            $(this).children('.count_pr').css('left', width + '%');
            $(this).find('.count').countTo({
                from: 0,
                to: width,
                time: 3000,
                refreshInterval: 50,
            });
        });
    });

    /*===================================*
    19.MAP JS
    *===================================*/
    if ($("#map").length > 0) {
        google.maps.event.addDomListener(window, 'load', init);
    }

    var map_selector = $('#map');
    function init() {

        var mapOptions = {
            zoom: map_selector.data("zoom"),
            mapTypeControl: false,
            center: new google.maps.LatLng(map_selector.data("latitude"), map_selector.data("longitude")), // New York
        };
        var mapElement = document.getElementById('map');
        var map = new google.maps.Map(mapElement, mapOptions);
        var marker = new google.maps.Marker({
            position: new google.maps.LatLng(map_selector.data("latitude"), map_selector.data("longitude")),
            map: map,
            icon: map_selector.data("icon"),

            title: map_selector.data("title"),
        });
        marker.setAnimation(google.maps.Animation.BOUNCE);
    }


    /*===================================*
    20. COUNTDOWN JS
    *===================================*/
    $('.countdown_time').each(function () {
        var endTime = $(this).data('time');
        $(this).countdown(endTime, function (tm) {
            if (parseInt(tm.strftime('%D')) == 0) {
                $(this).html(tm.strftime('<div class="countdown_box"><div class="countdown-wrap"><span class="countdown hours">%H</span><span class="cd_text">ساعت</span></div></div><div class="countdown_box"><div class="countdown-wrap"><span class="countdown minutes">%M</span><span class="cd_text">دقیقه</span></div></div><div class="countdown_box"><div class="countdown-wrap"><span class="countdown seconds">%S</span><span class="cd_text">ثانیه</span></div></div>'));
            }
            else {
                $(this).html(tm.strftime('<div class="countdown_box"><div class="countdown-wrap"><span class="countdown days">%D </span><span class="cd_text">روز</span></div></div><div class="countdown_box"><div class="countdown-wrap"><span class="countdown hours">%H</span><span class="cd_text">ساعت</span></div></div><div class="countdown_box"><div class="countdown-wrap"><span class="countdown minutes">%M</span><span class="cd_text">دقیقه</span></div></div><div class="countdown_box"><div class="countdown-wrap"><span class="countdown seconds">%S</span><span class="cd_text">ثانیه</span></div></div>'));
            }
        });
    });

    /*===================================*
    21. List Grid JS
    *===================================*/
    $('.shorting_icon').on('click', function () {
        if ($(this).hasClass('grid')) {
            $('.shop_container').removeClass('list').addClass('grid');
            $(this).addClass('active').siblings().removeClass('active');
        }
        else if ($(this).hasClass('list')) {
            $('.shop_container').removeClass('grid').addClass('list');
            $(this).addClass('active').siblings().removeClass('active');
        }
        $(".shop_container").append('<div class="loading_pr"><div class="mfp-preloader"></div></div>');
        setTimeout(function () {
            $('.loading_pr').remove();
        }, 800);
    });

    /*===================================*
    22. TOOLTIP JS
    *===================================*/
    $(function () {
        $('[data-toggle="tooltip"]').tooltip({
            trigger: 'hover',
        });
    });
    $(function () {
        $('[data-toggle="popover"]').popover();
    });

    /*===================================*
    23. PRODUCT COLOR JS
    *===================================*/
    $('.product_color_switch span').each(function () {
        var get_color = $(this).attr('data-color');
        $(this).css("background-color", get_color);
    });

    $('.product_color_switch .select,.product_size_switch .select').on("click", function () {
        $(this).siblings(this).removeClass('active').end().addClass('active');
    });

    /*===================================*
    24. QUICKVIEW POPUP + ZOOM IMAGE + PRODUCT SLIDER JS
    *===================================*/




    /*===================================*
    26. RATING STAR JS
    *===================================*/
    $(document).on("ready", function () {
        $('.star_rating span').on('click', function () {
            var onStar = parseFloat($(this).data('value'), 10); // The star currently selected
            var stars = $(this).parent().children('.star_rating span');
            for (var i = 0; i < stars.length; i++) {
                $(stars[i]).removeClass('selected');
            }
            for (i = 0; i < onStar; i++) {
                $(stars[i]).addClass('selected');
            }
        });
    });

    /*===================================*
    27. CHECKBOX CHECK THEN ADD CLASS JS
    *===================================*/
    $('.create-account,.different_address').hide();
    $('#createaccount:checkbox').on('change', function () {
        if ($(this).is(":checked")) {
            $('.create-account').slideDown();
        } else {
            $('.create-account').slideUp();
        }
    });
    $('#differentaddress:checkbox').on('change', function () {
        if ($(this).is(":checked")) {
            $('.different_address').slideDown();
        } else {
            $('.different_address').slideUp();
        }
    });

    /*===================================*
    28. Cart Page Payment option
    *===================================*/
    $(document).on('ready', function () {
        $('[name="payment_option"]').on('change', function () {
            var $value = $(this).attr('value');
            $('.payment-text').slideUp();
            $('[data-method="' + $value + '"]').slideDown();
        });
    });

    /*===================================*
    29. ONLOAD POPUP JS
    *===================================*/

    $(window).on('load', function () {
        setTimeout(function () {
            $("#onload-popup").modal('show', {}, 500);
        }, 3000);

    });

    /*===================================*
    30. SHOW HIDE PASSWORD
    *===================================*/

    $(".toggle-password").on('click', function () {

        $(this).toggleClass("fas fa-eye-slash fas fa-eye");
        var input = $($(this).attr("data-toggle"));
        if (input.attr("type") == "password") {
            input.attr("type", "text");
        } else {
            input.attr("type", "password");
        }
    });


    /*===================================*
    *===================================*/
    //$( window ).on( "load", function() {
    //		document.onkeydown = function(e) {
    //			if(e.keyCode == 123) {
    //			 return false;
    //			}
    //			if(e.ctrlKey && e.shiftKey && e.keyCode == 'I'.charCodeAt(0)){
    //			 return false;
    //			}
    //			if(e.ctrlKey && e.shiftKey && e.keyCode == 'J'.charCodeAt(0)){
    //			 return false;
    //			}
    //			if(e.ctrlKey && e.keyCode == 'U'.charCodeAt(0)){
    //			 return false;
    //			}
    //		
    //			if(e.ctrlKey && e.shiftKey && e.keyCode == 'C'.charCodeAt(0)){
    //			 return false;
    //			}      
    //		 };
    //		 
    //		$("html").on("contextmenu",function(){
    //			return false;
    //		});
    //	});


})(jQuery);

var mediaElements = document.querySelectorAll('video, audio');
var mediaElements2 = [];
for (var i = 0; i < mediaElements.length; i++) {
    if (mediaElements[i].className.indexOf("no-videojs") < 0)
        mediaElements2.push(mediaElements[i]);
}

for (var i = 0, total = mediaElements2.length; i < total; i++) {
    var features = ['playpause', 'current', 'progress', 'duration', 'volume', 'skipback', 'jumpforward', 'speed', 'fullscreen'];

    new MediaElementPlayer(mediaElements2[i], {
        autoRewind: false,
        features: features,
    });

}

function just_persian(str, keyCode) {
    var specialKeys = new Array();
    specialKeys.push(8); //Backspace
    specialKeys.push(18); //ALT
    specialKeys.push(16); //SHIFT
    specialKeys.push(17); //CTRL
    specialKeys.push(27); //ESC
    specialKeys.push(32); //ESC
    specialKeys.push(9); //Tab
    specialKeys.push(46); //Delete
    specialKeys.push(36); //Home
    specialKeys.push(35); //End
    specialKeys.push(37); //Left
    specialKeys.push(39); //Right
    if (specialKeys.indexOf(keyCode) != -1)
        return true;
    else {
        var p = /^[\u0600-\u06FF\s]+$/;
        if (!p.test(str)) {
            return false
        }
        var arregex = /[\u0600-\u06ff]|[\u0750-\u077f]|[\ufb50-\ufc3f]|[\ufe70-\ufefc]/;

        if (!arregex.test(str)) {
            return false
        }
        return true;
    }
}

