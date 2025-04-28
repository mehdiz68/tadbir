// JavaScript Document

var $mainWrapper = $('#mainWrapper');
$mainWrapper.on('show.bs.collapse', '.collapse', function () {
    $mainWrapper.find('.collapse.in').collapse('hide');
});

$(document).ready(function () {
    $(".dropdown").hover(
    function () {
        $('.dropdown-menu', this).stop(true, true).fadeToggle("500");
        $(this).toggleClass('open');
    },
    function () {
        $('.dropdown-menu', this).stop(true, true).fadeToggle("500");
        $(this).toggleClass('open');
    }
    );


});


var api;
$(document).ready(function () {
    api = $(".fullwidthbanner").apexSlider({
        startWidth: 1170,
        startHeight: 893,
        transition: "random",
        thumbWidth: 100,
        thumbHeight: 47,
        thumbAmount: 0,
        navType: "both",
        navStyle: "round",
        navArrow: "visible",
        showNavOnHover: true,
        timerAlign: "bottom",
        shadow: 0,
        fullWidth: true
    });
});


$("#menu-close").click(function (e) {
    e.preventDefault();
    $("#sidebar-wrapper").toggleClass("active");
});
// Opens the sidebar menu
$("#menu-toggle").click(function (e) {
    e.preventDefault();
    $("#sidebar-wrapper").toggleClass("active");
});


(function ($) {
    $.fn.blueMobileMenu = function (options) {
        var settings = $.extend({
            color: "#556b2f",
            backgroundColor: "white"
        }, options);
        var id = this.attr("id");
        var mobile = true;
        $("a[href$=" + id + "]").addClass("blueMobileMenuIcon").on("click", function () {
            $("#" + id).slideToggle();
        });
        this.addClass("blueMobileMenu");
        this.find("li").attr("class", "firstLevel");
        this.find("li ul li").attr("class", "secondLevel");
        this.find("li ul li ul li").attr("class", "thirdLevel");
        this.find("li ul li ul li ul li").attr("class", "fourthLevel");
        this.find("li").has("ul").addClass("closed").prepend("<div class='icon_menu'></div>");
        if (mobile === true) {
            this.on("click", ".icon_menu", function (e) {
                $(this).parent().find("ul").first().slideToggle();
                if ($(this).parent().hasClass("closed")) {
                    //$(this).attr("src", "fa fa-chevron-down");
                    $(this).parent().removeClass("closed").addClass("open");
                }
                else {
                    $(this).parent().removeClass("open").addClass("closed");
                }
                e.stopPropagation();
            });
        }
        return this;
    }
}(jQuery));

$(document).ready(function () {
    var owl = $('.workCarousel');
    owl.owlCarousel({
        margin: 0,
        nav: true,
        rtl: true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 3 },
            992: { items: 4 },
            1200: { items: 5 }
        }
    })
})

$(document).ready(function () {
    var owl = $('.categoryCarousel');
    owl.owlCarousel({
        margin: 0,
            rtl:true,
        nav: true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 3 },
            992: { items: 4 },
            1200: { items: 4 }
        }
    })
})

$(document).ready(function () {
    var owl = $('.ProductScroll');
    owl.owlCarousel({
        margin: 30,
        nav: true,
            rtl:true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 2 },
            992: { items: 3 },
            1200: { items: 4 }
        }
    })
})

$(document).ready(function () {
    var owl = $('.ProductScrollTab');
    owl.owlCarousel({
        margin: 30,
        nav: true,
            rtl:true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 2 },
            992: { items: 2 },
            1200: { items: 3 }
        }
    })
})


$(document).ready(function () {
    var owl = $('.ProductScrollMedia');
    owl.owlCarousel({
        margin: 30,
        nav: true,
            rtl:true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 2 },
            992: { items: 1 },
            1200: { items: 1 }
        }
    })
})

$(document).ready(function () {
    var owl = $('.OfferCarousel');
    owl.owlCarousel({
        margin: 30,
        nav: true,
            rtl:true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 1 },
            768: { items: 1 },
            992: { items: 2 },
            1200: { items: 2 }
        }
    })
})

$(document).ready(function () {
    var owl = $('.CatCarousel');
    owl.owlCarousel({
        margin: 30,
        nav: true,
            rtl:true,
        loop: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 4 },
            992: { items: 5 },
            1200: { items: 6 }
        }
    })
})


$(document).ready(function () {
    $('.bxslider').bxSlider({
        pagerCustom: '#bx-pager',
        auto: false,
    });
})

$(document).ready(function () {
    var owl = $('#bx-pager');
    owl.owlCarousel({
        margin: 10,
        nav: true,
            rtl:true,
        loop: false,
        autoplay: false,
        responsive: {
            0: { items: 3 },
            640: { items: 4 },
            768: { items: 3 },
            992: { items: 4 },
        }
    })
})


$('.carousel-inner').each(function () {
    if ($(this).children('div').length === 1) $(this).siblings('.carousel-control, .carousel-indicators').hide();
});

$(document).ready(function () {
    $('#list').click(function (event) { event.preventDefault(); $('#products .item').addClass('list-item'); });
    $('#grid').click(function (event) { event.preventDefault(); $('#products .item').removeClass('list-item'); $('#products .item').addClass('grid-group-item'); });
});



$(document).ready(function () {
    $('.ProductTab').easyResponsiveTabs({
        type: 'default', //Types: default, vertical, accordion
        width: 'auto', //auto or any width like 600px
        fit: true, // 100% fit in a container
        tabidentify: 'hor_1', // The tab groups identifier
        activate: function (event) { // Callback function if tab is switched
            var $tab = $(this);
            //var $info = $('#nested-tabInfo');
            var $name = $('span', $info);
            $name.text($tab.text());
            $info.show();
        }
    });
});