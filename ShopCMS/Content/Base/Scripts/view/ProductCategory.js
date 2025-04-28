$(document).ready(function () {

    $('#child-cat').owlCarousel({
        rtl: true,
        loop: false,
        margin: 10,
        nav: false,
        dots: true,
        responsive: {
            0: {
                items: 4
            },
            600: {
                items: 4
            },
            1000: {
                items: 5
            }
        }
    });

    $('.carousel_slider').owlCarousel({
        rtl: true,
        loop: false,
        margin: 10,
        nav: false,
        dots: true,
        responsive: {
            0: {
                items: 2
            },
            600: {
                items: 2
            },
            1000: {
                items: 5
            }
        }
    });

});
