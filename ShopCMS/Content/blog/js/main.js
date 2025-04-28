/*  ---------------------------------------------------
    Template Name: Foodeiblog
    Description:  Foodeiblog Blog HTML Template
    Author: Colorlib
    Author URI: https://colorlib.com
    Version: 1.0
    Created: Colorlib
---------------------------------------------------------  */

'use strict';



(function ($) {
    /*------------------
        Preloader
    --------------------*/
    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(200).fadeOut("slow");


		var host = "http://" + window.location.host + "/";
		; (function ($) {
			$.fn.extend({
				donetyping: function (callback, timeout) {
					timeout = timeout || 1e3; // 1 second default timeout
					var timeoutReference,
						doneTyping = function (el) {
							if (!timeoutReference) return;
							timeoutReference = null;
							callback.call(el);
						};
					return this.each(function (i, el) {
						var $el = $(el);
						// Chrome Fix (Use keyup over keypress to detect backspace)
						// thank you @palerdot
						$el.is(':input') && $el.on('keyup keypress', function (e) {
							// This catches the backspace button in chrome, but also prevents
							// the event from triggering too premptively. Without this line,
							// using tab/shift+tab will make the focused element fire the callback.
							if (e.type == 'keyup' && e.keyCode != 8) return;

							// Check if timeout has been set. If it has, "reset" the clock and
							// start over again.
							if (timeoutReference) clearTimeout(timeoutReference);
							timeoutReference = setTimeout(function () {
								// if we made it here, our timeout has elapsed. Fire the
								// callback
								doneTyping(el);
							}, timeout);
						}).on('blur', function () {
							// If we can, fire the event since we're leaving the field
							doneTyping(el);
						});
					});
				}
			});
		})(jQuery);
		$('#search_input').donetyping(function () {
			$("#SearchList > div .content").html("");
			var keyword = $("#search_input").val().toString().trim();
			if (keyword.length > 1) {
				$.ajax({
					type: "POST",
					url: "/Home/Search?keyword=" + keyword + "",
					contentType: "application/json; charset=utf-8",
					dataType: "json",

                    success: function (result) {
                        if (result.statusCode == 200) {
                            $("#SearchList").fadeIn();
                            $("#SearchList").addClass("flex");
                            var DisplayType = result.DisplayType;
                            //Render Result
                            var resultAsJson = JSON.parse(JSON.stringify(result.data));
                            for (var i in resultAsJson) {
                                var Sr = resultAsJson[i];
                                var Title = Sr.Title;
                                var PageAddress = Sr.PagaAdress;
                                var ID = Sr.Id;
                                var Img = Sr.Img;
                                var ElementType = Sr.TypeId;


                                switch (true) {
                                    case (ElementType > 0):
                                        //content
                                        $("#SearchList #ContentTypeList" + ElementType + " .content").append("<div><div class='FloatRight'><a href='" + host + PageAddress + "' target='_blank'><img class='img-thumbnail' src='" + Img + "'/></a></div><div class='FloatRight text'><a href='" + host + PageAddress + "' target='_blank'>" + Title + "</a></div><div class='clearfix'></div></div>"); break;
                                    //case (ElementType == -1):
                                    //    //tag
                                    //    $("#SearchList .TagList .content").append("<div class='FloatRight'><a href='" + host + PageAddress + "' target='_blank'>" + Title + "</a></div>"); break;
                                    case (ElementType == 0):
                                        //category
                                        $("#SearchList .CategoryList .content").append("<p class='text-right'><a href='" + host + PageAddress + "' target='_blank'>" + Title + "</a></p>"); break;

                                    case (ElementType == -3):
                                        //Product List
                                        $("#SearchList .ProductCategoryList .content").append("<div class='FloatRight'><a href='" + host + PageAddress + "' target='_blank'>" + Title + "</a></div>"); break;
                                    case (ElementType == -2):
                                        //Product
                                        $("#SearchList .ProductList .content").append("<div><div class='FloatRight text'><a href='" + host + PageAddress + "' target='_blank'>" + Title + "</a></div><div class='clearfix'></div></div>"); break;
                                }



                            }
                            //check No Result
                            if ($("#SearchList .TagList .content").children().length == 0)
                                $("#SearchList .TagList").hide();
                            else
                                $("#SearchList .TagList").show();

                            if ($("#SearchList .CategoryList .content").children().length == 0) {
                                $("#SearchList .CategoryList").hide();
                                $("#SearchList .CategoryList .continue a").remove("");
                            }
                            else {
                                $("#SearchList .CategoryList").show();
                                $("#SearchList .CategoryList .continue").html("<a target='_blank' href='" + host + "SearchResult/0/" + keyword + "'>نتایج بیشتر...</a>");
                            }

                            $("#SearchList .ContentTypeList").each(function () {
                                if ($(this).find(".content").children().length == 0) {
                                    $(this).hide();
                                }
                                else
                                    $(this).show();
                            });

                            if ($("#SearchList .ProductCategoryList .content").children().length == 0) {
                                $(".ProductCategoryList").hide();
                            }
                            else {
                                $(".ProductCategoryList").show();
                                $("#SearchList .ProductCategoryList .continue").html("<a target='_blank' href='" + host + "SearchResult/-3/" + keyword + "'>نتایج بیشتر...</a>");
                            }

                            if ($("#SearchList .ProductList .content").children().length == 0) {
                                $("#SearchList .ProductList").hide();
                                $("#SearchList .ProductList .continue a").remove("");
                            }
                            else {
                                $("#SearchList .ProductList").show();
                                $("#SearchList .ProductList .continue").html("<a target='_blank' href='" + host + "SearchResult/-2/" + keyword + "'>نتایج بیشتر...</a>");
                            }


                            $("#SearchList .ContentTypeList").each(function () {
                                if ($(this).find(".content").children().length == 0) {
                                    $(this).hide();
                                }
                                else {
                                    $(this).show();
                                    $(this).find(".continue").html("<a target='_blank' href='" + host + "SearchResult/" + $(this).attr("data-id") + "/" + keyword + "'>نتایج بیشتر...</a>");
                                }
                            });


                            //check No Result

                            if ($("#SearchList .ProductCategoryList .content").children().length == 0) {
                                $(".ProductCategoryList").hide();
                            }
                            else {
                                $(".ProductCategoryList").show();
                            }

                            if ($("#SearchList .ProductList .content").children().length == 0) {
                                $("#SearchList .ProductList").hide();
                                $("#SearchList .ProductList .continue a").remove("");
                            }
                            else
                                $("#SearchList .ProductList").show();

                            $("#search_input").css("background-image", "");
                        }
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

    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    //Humberger Menu
    $(".humberger__open").on('click', function () {
        $(".humberger__menu__wrapper").addClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").addClass("active");
    });

    $(".humberger__menu__overlay").on('click', function () {
        $(".humberger__menu__wrapper").removeClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").removeClass("active");
    });

    //Search Switch
    $('.search-switch').on('click',function() {
        $('.search-model').fadeIn(400);
    });

    $('.search-close-switch').on('click',function() {
        $('.search-model').fadeOut(400,function() {
            $('#search-input').val('');
        });
    });

    /*------------------
		Navigation
	--------------------*/
    $(".mobile-menu").slicknav({
        prependTo: '#mobile-menu-wrap',
        allowParentLinks: true
    });

    /*------------------
        Carousel Slider
    --------------------*/
    var hero_s = $(".hero__slider");
    hero_s.owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        nav: true,
        navText: ["<span class='arrow_carrot-left'><span/>", "<span class='arrow_carrot-right'><span/>"],
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

})(jQuery);