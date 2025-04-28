$(document).ready(function () {
    $(".persian").text($.persianNumbers($(".persian").text()));

    $("#Product").on("click", "#link-copy", function (e) {
        var $this = $(this);
        e.preventDefault();
        var dummy = document.createElement("input");
        document.body.appendChild(dummy);
        dummy.setAttribute("id", "dummy_id");
        document.getElementById("dummy_id").value = $this.attr("href");
        dummy.select();
        document.execCommand("copy");
        document.body.removeChild(dummy);

        swal({title: "", text: "کپی شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن',timer: 3000 });
    });

    var image = $('.c-gallery__img .product_img');
    var $lg = $('#other-images-gallery');

    //گالری عکس
    $lg.lightSlider({
        gallery: true,
        item: 1,
        loop: true,
        thumbItem: 5,
        slideMargin: 0,
        rtl: true,
        enableDrag: true,
        currentPagerPosition: 'right',
        onSliderLoad: function (el) {
            el.lightGallery({
                selector: '#other-images-gallery .lslide',
            });
            //var transform = $("#other-images-gallery").css("transform").toString();
            //transform = transform.replace("matrix", "").replace("(", "").replace(")", "");
            //var nums = transform.split(',');
            //console.log(nums);
            //var val = parseInt(nums[0]);
            //for (var i = 1; i < nums.length; i++) {
            //    if (parseInt(nums[i]) < val)
            //        val =nums[i];
            //}
            //alert("translate3d(" + Math.abs(val) + "px, 0px, 0px)!important");
            //$("#other-images-gallery").css("transform", "translate3d(" + val + "px, 0px, 0px)!important");
            $(".lSPager li img").each(function () {
                // console.log($(this).attr("src"));
            });

        },
        onAfterSlide: function (el) {
            $.removeData($('img'), 'elevateZoom');
            $('.zoomContainer').remove();
            if (window.innerWidth > 1024) {
                $("#other-images-gallery .active img").elevateZoom({
                    cursor: "zoom-in",
                    easing: true,
                    constrainType: 'height',
                    constrainSize: 274,
                    zoomType: 'inner',
                    containLensZoom: true,
                    gallery: 'pr_item_gallery',
                    galleryActiveClass: "active",
                    zoomWindowWidth: 300,
                    zoomWindowHeight: 300,
                    zoomWindowOffetx: 0,
                    zoomWindowOffety: 0,
                    zoomWindowPosition: 9
                });
            }
        },
    });
    $('#goToPrevSlide').on('click', function () {
        $lg.goToPrevSlide();
    });
    $('#goToNextSlide').on('click', function () {
        $lg.goToNextSlide();
    });


    if (window.innerWidth > 1024) {
        $(image).elevateZoom({
            cursor: "zoom-in",
            easing: true,
            constrainType: 'height',
            constrainSize: 274,
            zoomType: 'inner',
            containLensZoom: true,
            gallery: 'pr_item_gallery',
            galleryActiveClass: "active",
            zoomWindowWidth: 300,
            zoomWindowHeight: 300,
            zoomWindowOffetx: 0,
            zoomWindowOffety: 0,
            zoomWindowPosition: 9
        });
    }

    if ($(".lSPager li").length < 6) {
        $("#goToNextSlide,#goToPrevSlide").hide();
    }

    //فیکس منو
    $(window).scroll(function () {

        var $this = $(this);
        $(".tab-style3 .tab-content .tab-pane").each(function () {
            var taboffset = $(this).offset().top;
            if ($this.scrollTop() + 200 > taboffset) {
                $(".tab-style3 li a").removeClass("active");
                $("#" + $(this).attr("aria-labelledby")).addClass("active");

            }
        });

        var screensize = document.documentElement.clientWidth;
        //if (screensize <= 768) {
        var aTop = $('.tab-style3').offset().top;
        if ($(this).scrollTop() + 10 > aTop) {
            $('.tab-style3 .nav-tabs').addClass("fixx");
            $("#add-to-basket-fix").show();
        }
        else {

            $('.tab-style3 .nav-tabs').removeClass("fixx");
            $("#add-to-basket-fix").hide();
        }
        //}

    });

    //تغییر تب
    $(".tab-style3 .nav-tabs li a").click(function (e) {
        e.preventDefault();
        var $this = $(this);
        var aTop = $($this.attr("href")).offset().top;
        $('html, body').animate({
            scrollTop: aTop - 80
        }, 800);
    });

    //تغییر مدل،سایز،رنگ و... تغییر تنوع
    $("#Product").on("click", ".pr_switch_wrap .select", function () {
        $(this).parent().find(".select").removeClass("active");
        $(this).addClass("active");
        var color = null;
        if ($("#Colors .product_color_switch .active").length > 0)
            color = $("#Colors .product_color_switch .active span").attr("data-id");
        var model = "";
        if ($("#Models .product_size_switch span.active").length > 0 != undefined)
            model = $("#Models .product_size_switch span.active").text();
        var size = "";
        if ($("#Sizes .product_size_switch span.active").length > 0 != undefined)
            size = $("#Sizes .product_size_switch span.active").text();
        var garanty = null;
        if ($("#Garanties .product_size_switch span.active").length > 0)
            garanty = $("#Garanties .product_size_switch span.active").attr("data-id");
        $(".loading").show();
        var selector = $(this).parent().parent().attr("Id");
        $.ajax({
            type: "POST",
            url: "/Product/GetProductPrice",
            data: '{selector:"' + selector + '", productId:"' + $("#pid").val() + '",color:"' + color + '",model:"' + model + '",size:"' + size + '",garanty:"' + garanty + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    if (response.ProductStateId < 3) {
                        $("#add-to-basket-fix .btn-addtocart").removeClass("hide");
                        $("#add-to-basket-fix #not-exist-extra").addClass("hide");
                    }
                    else {
                        $("#add-to-basket-fix .btn-addtocart").addClass("hide");
                        $("#add-to-basket-fix #not-exist-extra").removeClass("hide");;

                    }
                    $("#Main-Detail").html(response.NewRow);

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



                    $('.product_color_switch span').each(function () {
                        var get_color = $(this).attr('data-color');
                        $(this).css("background-color", get_color);
                    });


                    var image2 = $('.c-gallery__img .product_img');
                    var $lg2 = $('#other-images-gallery');
                    //گالری عکس
                    $lg2.lightSlider({
                        gallery: true,
                        item: 1,
                        loop: true,
                        rtl: true,
                        thumbItem: 5,
                        slideMargin: 0,
                        enableDrag: true,
                        currentPagerPosition: 'right',
                        onSliderLoad: function (el) {
                            el.lightGallery({
                                selector: '#other-images-gallery .lslide'
                            });
                        },
                        onAfterSlide: function (el) {
                            $.removeData($('img'), 'elevateZoom');
                            $('.zoomContainer').remove();
                            if (window.innerWidth > 1024) {
                                $("#other-images-gallery .active img").elevateZoom({
                                    cursor: "zoom-in",
                                    easing: true,
                                    constrainType: 'height',
                                    constrainSize: 274,
                                    zoomType: 'inner',
                                    containLensZoom: true,
                                    gallery: 'pr_item_gallery',
                                    galleryActiveClass: "active",
                                    zoomWindowWidth: 300,
                                    zoomWindowHeight: 300,
                                    zoomWindowOffetx: 0,
                                    zoomWindowOffety: 0,
                                    zoomWindowPosition: 9
                                });
                            }
                        },
                    });
                    $('#goToPrevSlide').on('click', function () {
                        $lg2.goToPrevSlide();
                    });
                    $('#goToNextSlide').on('click', function () {
                        $lg2.goToNextSlide();
                    });


                    $(".minus").css("cursor", "not-allowed");

                    if (window.innerWidth > 1024) {
                        $(image2).elevateZoom({
                            cursor: "zoom-in",
                            easing: true,
                            constrainType: 'height',
                            constrainSize: 274,
                            zoomType: 'inner',
                            containLensZoom: true,
                            gallery: 'pr_item_gallery',
                            galleryActiveClass: "active",
                            zoomWindowWidth: 300,
                            zoomWindowHeight: 300,
                            zoomWindowOffetx: 0,
                            zoomWindowOffety: 0,
                            zoomWindowPosition: 9
                        });
                    }
                    if ($(".lSPager li").length < 6) {
                        $("#goToNextSlide,#goToPrevSlide").hide();
                    }


                    ////تعداد افزودن به سبد
                    //$('.plus').on('click', function () {
                    //    if ($(this).prev().val()) {
                    //        if (parseInt($(this).prev().val()) < parseInt($(this).prev().attr("max"))) {
                    //            $(this).prev().val(+$(this).prev().val() + 1);
                    //        }
                    //        if (parseInt($(this).prev().val()) == parseInt($(this).prev().attr("max"))) {
                    //            $(this).css("cursor", "not-allowed");
                    //            $(this).addClass("full");
                    //        }
                    //        else {
                    //            $(this).css("cursor", "pointer");
                    //            $(this).removeClass("full");
                    //        }

                    //        if (parseInt($(this).prev().val()) > parseInt($(this).prev().attr("min"))) {
                    //            $(this).parent().find(".minus").css("cursor", "pointer");
                    //            $(this).parent().find(".minus").removeClass("full");
                    //        }

                    //    }
                    //});
                    //$('.minus').on('click', function () {
                    //    if ($(this).next().val()) {
                    //        if (parseInt($(this).next().val()) > parseInt($(this).next().attr("min"))) {
                    //            $(this).next().val(+$(this).next().val() - 1);
                    //        }
                    //        if (parseInt($(this).next().val()) == parseInt($(this).next().attr("min"))) {
                    //            $(this).css("cursor", "not-allowed");
                    //            $(this).addClass("full");
                    //        }
                    //        else {
                    //            $(this).css("cursor", "pointer");
                    //            $(this).removeClass("full");
                    //        }

                    //        if (parseInt($(this).next().val()) < parseInt($(this).next().attr("max"))) {
                    //            $(this).parent().find(".plus").css("cursor", "pointer");
                    //            $(this).parent().find(".plus").removeClass("full");
                    //        }


                    //    }
                    //});

                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    // صفحه بندی نظرات محصول
    $("#Reviews").on("click", ".pagination li a", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        //نظرات
        $.ajax({
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#CommentList").html(response.NewRow); updateProgress();
                    $('html, body').animate({
                        scrollTop: $("#CommentList").offset().top - 80
                    }, 800);

                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    //مرتب سازی نظرات محصول
    $("#CommentList").on("click", ".js-filter-items li a", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        //نظرات
        $.ajax({
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#CommentList").html(response.NewRow); updateProgress();
                    $('html, body').animate({
                        scrollTop: $("#CommentList").offset().top - 80
                    }, 800);

                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    //لایک کامنت
    $("#CommentList").on("click", ".js-comment-like", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/Usefulcomment?id=" + $this.attr("data-comment"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $this.attr("data-counter", parseInt($this.attr("data-counter")) + 1);
                }
                else {
                }

                $(".loading").hide();
            }
        });

    });
    //دیسلایک کامنت
    $("#CommentList").on("click", ".js-comment-dislike", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/Unusefulcomment?id=" + $this.attr("data-comment"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $this.attr("data-counter", parseInt($this.attr("data-counter")) + 1);
                }
                else {
                }

                $(".loading").hide();
            }
        });

    });

    // صفحه بندی پرسش و پاسخ محصول
    $("#questins").on("click", ".pagination li a", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        //نظرات
        $.ajax({
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#FAQList").html(response.NewRow);
                    $('html, body').animate({
                        scrollTop: $("#FAQList").offset().top - 80
                    }, 800);

                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    //مرتب سازی پرسش و پاسخ محصول
    $("#questins").on("click", ".js-filter-items li a", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: $this.attr("href"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#FAQList").html(response.NewRow);
                    $('html, body').animate({
                        scrollTop: $("#FAQList").offset().top - 80
                    }, 800);

                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    //لایک پرسش و پاسخ
    $("#FAQList").on("click", ".js-answer-like", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/UsefulQuestion?id=" + $this.attr("data-answer"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $this.attr("data-counter", parseInt($this.attr("data-counter")) + 1);
                }
                else {
                }

                $(".loading").hide();
            }
        });

    });

    //دیسلایک پرسش و پاسخ
    $("#FAQList").on("click", ".js-answer-dislike", function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/UnusefulQuestion?id=" + $this.attr("data-answer"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $this.attr("data-counter", parseInt($this.attr("data-counter")) + 1);
                }
                else {
                }

                $(".loading").hide();
            }
        });

    });

    //انتخاب برای پاسخ
    $("#FAQList").on("click", ".js-add-answer-btn", function (e) {
        e.preventDefault()
        var $this = $(this);
        $("#ProductQuestionId").val($this.attr("data-question-id"));
        $("#AddQuestion button").text("ثبت پاسخ");
        $('html, body').animate({
            scrollTop: $("#questins").offset().top - 80
        }, 800);

    });

    //افزودن به مورد علاقه
    $(".add_wishlist").click(function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/AddFavorate?id=" + $this.attr("data-id"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    swal({ title: "", text: "انجام شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                }
                else if (response.statusCode == 403) {
                    $("#ModalLogin").modal();

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }

                $(".loading").hide();
            }
        });
    });

    //به من اطلاع بده
    $(".add_letmeknow").click(function (e) {
        e.preventDefault()
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/GetLetmeKnow?id=" + $this.attr("data-id"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {

                    if (response.data != null) {

                        if (response.data.lid != null) {
                            if (response.data.Available == true && response.data.AmazingOffer == true) {
                                $("#notificationmodal .c-form-notification__status").text("موجود شدن و پیشنهاد شگفت انگیز");
                                $("#notificationmodal .c-form-notification__status").attr("data-type", "3");
                            }
                            else if (response.data.Available == true) {
                                $("#notificationmodal .c-form-notification__status").text("موجود شدن ");
                                $("#notificationmodal .c-form-notification__status").attr("data-type", "2");
                            }
                            else if (response.data.AmazingOffer == true) {
                                $("#notificationmodal .c-form-notification__status").text("پیشنهاد شگفت انگیز");
                                $("#notificationmodal .c-form-notification__status").attr("data-type", "1");
                            }

                            $("#email-noti .js-observed-user-email").text(response.data.Email);
                            $("#mobile-noti .js-observed-user-number").text(response.data.PhoneNumber);

                            if (response.data.NotificationType == 1 || response.data.NotificationType == 4 || response.data.NotificationType == 5 || response.data.NotificationType == 7) {
                                $("#email-noti input[type='checkbox']").prop("checked", true);
                            }
                            if (response.data.NotificationType == 2 || response.data.NotificationType == 4 || response.data.NotificationType == 6 || response.data.NotificationType == 7) {
                                $("#mobile-noti input[type='checkbox']").prop("checked", true);
                            }
                            if (response.data.NotificationType == 3 || response.data.NotificationType == 5 || response.data.NotificationType == 6 || response.data.NotificationType == 7)
                                $("#msg-noti input[type='checkbox']").prop("checked", true);
                        }
                        else {
                            if ($(".btn-addtocart").attr("data-status") != "1") {
                                $("#notificationmodal .c-form-notification__status").text("موجود شدن و پیشنهاد شگفت انگیز");
                                $("#notificationmodal .c-form-notification__status").attr("data-type", "3");
                            }
                            else {
                                $("#notificationmodal .c-form-notification__status").text("پیشنهاد شگفت انگیز");
                                $("#notificationmodal .c-form-notification__status").attr("data-type", "1");
                            }

                            if (response.data.Email != null)
                                $("#email-noti .js-observed-user-email").text(response.data.Email);
                            else
                                $("#email-noti").hide();
                            if (response.data.PhoneNumber != null)
                                $("#mobile-noti .js-observed-user-number").text(response.data.PhoneNumber);
                            else
                                $("#mobile-noti").hide();

                        }

                        $("#notificationmodal").modal();
                    }
                }
                else if (response.statusCode == 403) {
                    $("#ModalLogin").modal()

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }

                $(".loading").hide();
            }
        });
    });

    //افزودن به به من اطلاع بده
    $("#add-to-incredible-observed").click(function (e) {
        e.preventDefault()
        var $this = $(this);
        var notificationtype = 0;
        if ($("#email-noti input[type='checkbox']").is(":checked") && $("#mobile-noti input[type='checkbox']").is(":checked") && $("#msg-noti input[type='checkbox']").is(":checked"))
            notificationtype = 7;
        else if ($("#mobile-noti input[type='checkbox']").is(":checked") && $("#msg-noti input[type='checkbox']").is(":checked"))
            notificationtype = 6;
        else if ($("#email-noti input[type='checkbox']").is(":checked") && $("#msg-noti input[type='checkbox']").is(":checked"))
            notificationtype = 5;
        else if ($("#email-noti input[type='checkbox']").is(":checked") && $("#mobile-noti input[type='checkbox']").is(":checked"))
            notificationtype = 4;
        else if ($("#msg-noti input[type='checkbox']").is(":checked"))
            notificationtype = 3;
        else if ($("#mobile-noti input[type='checkbox']").is(":checked"))
            notificationtype = 2;
        else if ($("#email-noti input[type='checkbox']").is(":checked"))
            notificationtype = 1;

        var amazing = $("#notificationmodal .c-form-notification__status").attr("data-type") == 3 || $("#notificationmodal .c-form-notification__status").attr("data-type") == 1 ? true : false;
        var available = $("#notificationmodal .c-form-notification__status").attr("data-type") == 3 || $("#notificationmodal .c-form-notification__status").attr("data-type") == 2 ? true : false;

        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/AddLetmeKnow?id=" + $this.attr("data-id") + "&amazing=" + amazing + "&available=" + available + "&notificationType=" + notificationtype,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#notificationmodal").modal("hide");
                    swal({ title: "", text: "انجام شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-success', confirmButtonText: 'بستن' });
                }
                else if (response.statusCode == 403) {
                    $("#ModalLogin").modal();

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }

                $(".loading").hide();
            }
        });
    });

    //افزودن به سبد خرید
    $("#Product").on("click", ".btn-addtocart", function () {
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Product/AddBasketItem",
            data: '{productid:"' + $("#pid").val() + '",productpriceid:"' + $("#pprid").val() + '",quantity:"' + $("#Basket-quantity").val() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#addedBasketModal").remove();
                    $(".basket-item-list").html(response.Basket);
                    $("body").append(response.AddedBasket);
                    $("#basket_cart_count").text(response.basketCount);
                    $("#addedBasketModal").modal('show');
                    //swal({
                    //    title: "",
                    //    text: "انجام شد",
                    //    type: "success",
                    //    showCancelButton: true,
                    //    confirmButtonClass: "btn-success",
                    //    confirmButtonText: "تکمیل خرید و پرداخت",
                    //    cancelButtonText: "ادامه خرید",
                    //    cancelButtonClass: "btn-danger",
                    //    closeOnConfirm: false,
                    //    closeOnCancel: false
                    //},
                    //    function (isConfirm) {
                    //        if (isConfirm) {
                    //            $(".loading").show();
                    //            window.location = "/Cart";
                    //        } else {
                    //            swal.close();
                    //        }

                    //    });

                }
                else {
                    swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }

                $(".loading").hide();
            }
        });
    });


    //حذف از سبد
    $("body").on("click", ".remove-basket-item2", function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/Cart/RemoveBasketItem/" + $this.attr("data-id"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $(".basket-item-list").html(response.Basket);
                    $(".cart_count").text(response.Count);

                    $("#addedBasketModal").modal('hide');
                    $("#addedBasketModal").remove();
                    $("body").append(response.AddedBasket);
                    $("#basket_cart_count").text(response.Count);
                    $("#addedBasketModal").modal('show');
                }
                else if (response.statusCode == 404) {
                    window.location.href = "/";
                }
                else {
                    alert(response.message);
                }

                $(".loading").hide();
            }
        });
    });

    //بستن کادر سبد
    $('body').on('hidden.bs.modal', "#addedBasketModal", function (e) {

        $("#addedBasketModal").remove();
    });

    //تعداد افزودن به سبد

    //تعداد افزودن به سبد
    $('body').on('click', "#addedBasketModal .plus", function () {
        if (!$(this).hasClass("full")) {
            if ($(this).prev().val()) {
                if (parseInt($(this).prev().val()) < parseInt($(this).prev().attr("max"))) {
                    $(this).prev().val(+$(this).prev().val() + 1);
                }
                if (parseInt($(this).prev().val()) == parseInt($(this).prev().attr("max")))
                    $(this).css("cursor", "not-allowed");
                else
                    $(this).css("cursor", "pointer");

                if (parseInt($(this).prev().val()) > parseInt($(this).prev().attr("min")))
                    $(this).parent().find(".minus").css("cursor", "pointer");


                changeQuantity($(this).parent().find(".Basket-quantity").attr("data-id"), $(this).parent().find(".Basket-quantity").val());
            }
        }
    });
    $('body').on('click', "#addedBasketModal .minus", function () {

        if (!$(this).hasClass("full")) {
            if ($(this).next().val()) {
                if (parseInt($(this).next().val()) > parseInt($(this).next().attr("min"))) {
                    $(this).next().val(+$(this).next().val() - 1);
                }
                if (parseInt($(this).next().val()) == parseInt($(this).next().attr("min")))
                    $(this).css("cursor", "not-allowed");
                else
                    $(this).css("cursor", "pointer");

                if (parseInt($(this).next().val()) < parseInt($(this).next().attr("max")))
                    $(this).parent().find(".plus").css("cursor", "pointer");


                changeQuantity($(this).parent().find(".Basket-quantity").attr("data-id"), $(this).parent().find(".Basket-quantity").val());
            }
        }
    });


    //تعداد افزودن به سبد
    $('body').on('click', ".pr_detail .plus", function () {
        if ($(this).prev().val()) {
            if (parseInt($(this).prev().val()) < parseInt($(this).prev().attr("max"))) {
                $(this).prev().val(+$(this).prev().val() + 1);
            }
            if (parseInt($(this).prev().val()) == parseInt($(this).prev().attr("max"))) {
                $(this).css("cursor", "not-allowed");
                $(this).addClass("full");
            }
            else {
                $(this).css("cursor", "pointer");
                $(this).removeClass("full");
            }

            if (parseInt($(this).prev().val()) > parseInt($(this).prev().attr("min"))) {
                $(this).parent().find(".minus").css("cursor", "pointer");
                $(this).parent().find(".minus").removeClass("full");
            }

        }
    });
    $('body').on('click', ".pr_detail .minus", function () {
        if ($(this).next().val()) {
            if (parseInt($(this).next().val()) > parseInt($(this).next().attr("min"))) {
                $(this).next().val(+$(this).next().val() - 1);
            }
            if (parseInt($(this).next().val()) == parseInt($(this).next().attr("min"))) {
                $(this).css("cursor", "not-allowed");
                $(this).addClass("full");
            }
            else {
                $(this).css("cursor", "pointer");
                $(this).removeClass("full");
            }

            if (parseInt($(this).next().val()) < parseInt($(this).next().attr("max"))) {
                $(this).parent().find(".plus").css("cursor", "pointer");
                $(this).parent().find(".plus").removeClass("full");
            }


        }
    });

    //$('body').on('click', ".pr_detail .plus", function () {
    //    if ($(this).prev().val()) {
    //        if (parseInt($(this).prev().val()) < parseInt($(this).prev().attr("max"))) {
    //            $(this).prev().val(+$(this).prev().val() + 1);
    //        }
    //        if (parseInt($(this).prev().val()) == parseInt($(this).prev().attr("max")))
    //            $(this).css("cursor", "not-allowed");
    //        else
    //            $(this).css("cursor", "pointer");

    //        if (parseInt($(this).prev().val()) > parseInt($(this).prev().attr("min")))
    //            $(this).parent().find(".minus").css("cursor", "pointer");


    //    }
    //});
    //$('body').on('click', ".pr_detail .minus", function () {

    //    if ($(this).next().val()) {
    //        if (parseInt($(this).next().val()) > parseInt($(this).next().attr("min"))) {
    //            $(this).next().val(+$(this).next().val() - 1);
    //        }
    //        if (parseInt($(this).next().val()) == parseInt($(this).next().attr("min")))
    //            $(this).css("cursor", "not-allowed");
    //        else
    //            $(this).css("cursor", "pointer");

    //        if (parseInt($(this).next().val()) < parseInt($(this).next().attr("max")))
    //            $(this).parent().find(".plus").css("cursor", "pointer");
    //    }
    //});

    $("#Product").on("keyup", "#Basket-quantity", function () {
        if (parseInt($(this).val()) > parseInt($(this).attr("max")))
            $(this).val($(this).attr("max"));
        if (parseInt($(this).val()) < parseInt($(this).attr("min")))
            $(this).val($(this).attr("min"));
        if (!$(this).val())
            $(this).val("1");
    });

    $(".qty").focus(function () {
        $(this).css("border", "1px solid #FF8A00");
        $(this).prev().css("border", "1px solid #FF8A00");
        $(this).next().css("border", "1px solid #FF8A00");
    });

    $(".qty").focusout(function () {
        $(this).css("border", "1px solid #ddd");
        $(this).prev().css("border", "1px solid #ddd");
        $(this).next().css("border", "1px solid #ddd");
    });
});


function updateProgress() {
    $("#CommentList .progress-bar").each(function () {
        var percentage = $(this).attr("aria-valuenow");
        $(this).css('width', percentage + '%');
    });
}



function changeQuantity(id, quantity) {

    $(".loading").show();
    $.ajax({
        type: "POST",
        url: "/Cart/ChangeBasketItem?id=" + id + "&quantity=" + quantity,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.statusCode == 200) {

                $(".basket-item-list").html(response.Basket);
                $(".cart_count").text(response.Count);

                $("#addedBasketModal").modal('hide');
                $("#addedBasketModal").remove();
                $("body").append(response.AddedBasket);
                $("#basket_cart_count").text(response.Count);
                $("#addedBasketModal").modal('show');
            }
            else if (response.statusCode == 404) {
                window.location.href = "/";
            }
            else {
                alert(response.message);
            }

            $(".loading").hide();
        }
    });
}