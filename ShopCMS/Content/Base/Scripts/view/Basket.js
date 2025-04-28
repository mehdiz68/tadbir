$(document).ready(function () {


    //تعداد افزودن به سبد
    $('body').on('click', ".plus", function () {
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
    $('body').on('click', ".minus", function () {

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


    $("body").on("keyup", ".Basket-quantity", function () {
        if (parseInt($(this).val()) > parseInt($(this).attr("max")))
            $(this).val($(this).attr("max"));
        if (parseInt($(this).val()) < parseInt($(this).attr("min")))
            $(this).val($(this).attr("min"));
        if (!$(this).val())
            $(this).val("1");
    });
    $("body").on("focus", ".qty", function () {
        $(this).css("border", "1px solid #FF8A00");
        $(this).prev().css("border", "1px solid #FF8A00");
        $(this).next().css("border", "1px solid #FF8A00");
    });

    $("body").on("focusout", ".qty", function () {
        $(this).css("border", "1px solid #ddd");
        $(this).prev().css("border", "1px solid #ddd");
        $(this).next().css("border", "1px solid #ddd");
    });

    //حذف از سبد
    $("body").on("click", ".js-remove-from-cart", function () {
        $(".loading").show();
        var $this = $(this);
        $.ajax({
            type: "POST",
            url: "/Cart/RemoveBasketItem/" + $this.attr("data-id"),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#cart-data").html(response.NewRow);
                    $(".loading").hide();
                    $('html, body').animate({
                        scrollTop: $this.offset().top - 80
                    }, 800);
                }
                else if (response.statusCode == 404) {
                    $(".loading").show();
                    window.location.href = "/";
                }
                else {
                    alert(response.message);
                    $(".loading").hide();
                }

            }
        });
    });


    //تغییر تعداد در سبد
    $("body").on("change", ".Basket-quantity", function () {
        var $this = $(this);
        changeQuantity($this.attr("data-id"), $this.val());
    });

    //step2-----Shipping--------------------------

    //باز کردنِ تغییر آدرس
    $("#change-address-btn").click(function () {
        $("#user-default-address-container").hide();
        $("#user-address-list-container").show();
    });

    $("#cancel-change-address-btn").click(function () {
        $("#user-default-address-container").show();
        $("#user-address-list-container").hide();
    });

    //انتخاب آدرس
    $("#Basket .c-checkout-address__item").click(function () {
        $("#Basket .c-checkout-address__item").removeClass("is-selected");
        $("#Basket .c-checkout-address__item input[type=radio]").prop("checked", false);
        $(this).addClass("is-selected");
        $(this).find("input[type=radio]").prop("checked", true);


        var extraprice = false;
        var sendwayselecteddataId = $(".c-checkout-pack__row .active").attr("data-id");
        var sendwayselected = $(".c-checkout-pack__shipping-type-item[data-id=" + sendwayselecteddataId + "]");
        if (sendwayselected.find(".Extra-price").length > 0)
            extraprice = sendwayselected.find(".select-extra-price").is(":checked");

        var CityId = 0;
        CityId = $(".c-checkout-address__content .is-selected").attr("data-city-id");

        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/changeCity?extraprice=" + extraprice + "&CityId=" + CityId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#SendWays").html(response.NewRow).promise().done(function () {
                        $(".loading").hide();
                    });

                }
                else if (response.statusCode == 404) {
                    window.location.href = "/";
                }
                else {
                    alert(response.message);
                    $(".loading").hide();
                }

            }
        });

    });

    //انتخاب روش ارسال
    $("body").on("click","input.js-shipping-type-selector", function () {
        var $this = $(this);
        $this.closest(".js-checkout-pack").find(".js-shipment-submit-type").addClass("u-hidden");
        $(".js-shipment-submit-type[data-id='" + $this.attr("data-id") + "']").removeClass("u-hidden");
    });

    //تغییر روش ارسال
    $("body").on("click", ".c-checkout-pack__shipping-type", function () {
        var $this = $(this).parent();
        console.log($this.attr("data-s-id"));
        var sid = $this.attr("data-s-id");
        var packageid = $this.find("input").attr("data-package-id");
        //اگر روش ارسالی دارای انتخاب زمان تحویل نیست 
        if ($this.attr("data-type") == "False") {
            $this.parent().attr("data-active", "True");
            //اگر بقیه مرسوله ها هم معتبر هستند
            if (CheckSendWayValid()) {
                $("#Stay-Shipping").addClass("u-hidden");
                $("#Go-to-payment").removeClass("u-hidden");
            }
            else {
                $("#Stay-Shipping").removeClass("u-hidden");
                $("#Go-to-payment").addClass("u-hidden");
            }
        }
        else {
            var validselect = false;
            $(".js-shipment-submit-type[data-id='" + $this.attr("data-id") + "'] input[type='radio']").each(function () {
                if ($(this).is(":checked"))
                    validselect = true;
            });
            if (validselect) {

                $this.parent().attr("data-active", "True");
                //اگر بقیه مرسوله ها هم معتبر هستند
                if (CheckSendWayValid()) {
                    $("#Stay-Shipping").addClass("u-hidden");
                    $("#Go-to-payment").removeClass("u-hidden");
                }
                else {
                    $("#Stay-Shipping").removeClass("u-hidden");
                    $("#Go-to-payment").addClass("u-hidden");
                }
            }
            else {
                $this.parent().attr("data-active", "False");
                $("#Stay-Shipping").removeClass("u-hidden");
                $("#Go-to-payment").addClass("u-hidden");

            }
        }
        var extraprice = false;
        if ($this.find(".Extra-price").length > 0)
            extraprice = $this.find(".select-extra-price").is(":checked");

        var CityId = 0;
        CityId = $(".c-checkout-address__content .is-selected").attr("data-city-id");

        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/changeAside?id=" + sid + "&ValidSendWaySelect=" + $(".c-checkout-pack__shipping-type-row[data-active='True']").length + "&productPackageTypeId=" + packageid + "&extraprice=" + extraprice + "&CityId=" + CityId,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $(".o-page__aside").html(response.NewRow).promise().done(function () {
                        $(".Extra-price").hide();
                        $this.find(".Extra-price").show();                        
                        $(".loading").hide();
                    });

                    $('html, body').animate({
                        scrollTop: $(".js-checkout-pack").offset().top - 80
                    }, 800);

                }
                else if (response.statusCode == 404) {
                    window.location.href = "/";
                }
                else {
                    alert(response.message);
                }

            }
        });

    });

    $("body").on("change", ".select-extra-price", function () {
        var $this = $(this);
        if ($this.is(":checked"))
            $this.parent().parent().addClass("checked");
        else
            $this.parent().parent().removeClass("checked");
        $this.parent().parent().parent().find(".c-checkout-pack__shipping-type").trigger("click");

        $('html, body').animate({
            scrollTop: $(".js-checkout-pack").offset().top - 80
        }, 800);


    });
    $("body").on("click", ".js-shipment-submit-type input[type='radio']", function () {
        if ($(this).is(":checked")) {
            $(this).closest(".js-shipment-submit-type").parent().find(".c-checkout-pack__shipping-type-row").attr("data-active", "True");
        }
        //اگر بقیه مرسوله ها هم معتبر هستند
        if (CheckSendWayValid()) {
            $("#Stay-Shipping").addClass("u-hidden");
            $("#Go-to-payment").removeClass("u-hidden");
        }
        else {
            $("#Stay-Shipping").removeClass("u-hidden");
            $("#Go-to-payment").addClass("u-hidden");
        }

    });

    //ارسال به مرحله 3
    $("body").on("click", "#Go-to-payment", function () {
        var shipping = [];
        $(".js-checkout-pack").each(function () {
            var obj = {
                ProductPackageType: $(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").attr("data-package"),
                addressId: parseInt($(".c-checkout-address__content .is-selected").attr("data-id")),
                sendwayId: parseInt($(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").attr("data-send-way-id")),
                sendwayBoxId: parseInt($(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").attr("data-box-id")),
                deliveryDate: $(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").parent().attr("data-type") == "True" ? $(this).find(".js-shipment-submit-type[data-id='" + $(this).find(".c-checkout-pack__shipping-type-row input:checked").parent().attr("data-id") + "']").find("input[type='radio']:checked").attr("data-date") : null,
                sendwayWorktimeId: parseInt($(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").parent().attr("data-type") == "True" ? $(this).find(".js-shipment-submit-type[data-id='" + $(this).find(".c-checkout-pack__shipping-type-row input:checked").parent().attr("data-id") + "']").find("input[type='radio']:checked").attr("data-time-id") : 0),
                extraprice: $(this).find(".c-checkout-pack__shipping-type-row input[type='radio']:checked").parent().find(".select-extra-price").is(":checked")
            };
            shipping.push(obj);
        });

        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/GotoStep3",
            data: JSON.stringify({ shippings: shipping }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    window.location.href = "/Cart/Payment";
                }
                else if (response.statusCode == 404) {

                    window.location.href = "/";
                }
                else {
                    $("#shipping-data").prepend("<p class='c-shipment-page__shared-address-message'>" + response.message + "</p>").fadeIn(300);
                    $(".c-shipment-page__shared-address-message").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Shipping"; });

                    $(".loading").hide();
                }

            }
        });
    });



    //step3-----payment--------------------------
    $(".c-payment__voucher-header").click(function () {
        $(this).toggleClass("c-payment__voucher-header--open");
        $(this).parent().find(".js-voucher-list").toggleClass("u-hidden");
    });
    $(".js-checkout-order-summary__header").click(function () {
        $(this).toggleClass("is-active");
        $(this).parent().find(".c-swiper--order-summary").toggleClass("u-hidden");


        var slider2 =
            new Swiper("#" + $(this).parent().find(".c-swiper--order-summary").attr("id"), {
                spaceBetween: 5,
                slidesPerView: 5,

                // Navigation arrows
                navigation: {
                    nextEl: '.swiper-button-next',
                    prevEl: '.swiper-button-prev',
                },

                // And if we need scrollbar
                scrollbar: {
                    el: '.swiper-scrollbar',
                },
                breakpoints: {
                    768: {
                        slidesPerView: 2,
                        spaceBetween: 10,
                    }
                }

            });

    });
    //Code Gift
    $("#payment-voucher-input").keyup(function () {
        var $this = $(this);
        if ($this.val()) {
            $this.next().removeClass("c-payment__serial-input-icon--disabled");
        }
        else {
            $this.next().addClass("c-payment__serial-input-icon--disabled");
        }
    });
    $(".js-apply-voucher").click(function () {
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/CheckGiftCode",
            data: JSON.stringify({ code: $("#payment-voucher-input").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $(".loading a").append("<p class='alert alert-info msg-gift'>کد تخفیف وارد شده معتبر است و تخفیف اعمال شد </p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 404) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>کد تخفیف وارد شده معتبر نیست</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 405) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>زمان استفاده از کد تخفیف وارد شده نرسیده است.</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 406) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>زمان استفاده از کد تخفیف گذشته است.</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 407) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>سقف استفاده از کد تخفیف پر شده است !</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 408) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>این کد تخفیف برای اولین خرید قابل استفاده است. شما قبلا خرید انجام داده اید !</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 409) {
                    $(".loading a").append("<p class='alert alert-danger msg-gift'>این کد تخفیف برای کالاهایی که انتخاب کردید مجاز نیست !</p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else {
                    $(".loading").show();
                    alert(response.message);
                    window.location.href = "/Cart/Payment";
                }

            }
        });
    });
    $(".js-decline-voucher").click(function () {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/RemoveGiftCode",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var aTop = $(".js-voucher-header").offset().top;
                $('html, body').animate({
                    scrollTop: aTop - 80
                }, 800);

                if (response.statusCode == 200) {
                    $(".loading a").append("<p class='alert alert-info msg-gift'>کد تخفیف حذف شد. </p>").fadeIn(300);
                    $(".loading").show();
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else {
                    $(".loading").show();
                    alert(response.message);
                    window.location.href = "/Cart/Payment";
                }


            }
        });
    });
    //Bon Gift
    $("body").on("mouseup change keyup", "#payment-gift-card-input", function () {
        var $this = $(this);
        if ($this.val()) {
            if (parseInt($this.val()) > 0)
                $this.next().removeClass("c-payment__serial-input-icon--disabled");
            else
                $this.next().addClass("c-payment__serial-input-icon--disabled");
        }
        else {
            $this.next().addClass("c-payment__serial-input-icon--disabled");
        }
    });
    $(".js-apply-gift-card").click(function () {
        var $this = $(this);
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/CheckBon",
            data: JSON.stringify({ count: $("#payment-gift-card-input").val() }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var aTop = $(".js-voucher-header").offset().top;
                $('html, body').animate({
                    scrollTop: aTop - 80
                }, 800);

                if (response.statusCode == 200) {
                    $(".c-payment__voucher").prepend("<p class='alert alert-info msg-gift'>بن تخفیف وارد شده اعمال شد. </p>").fadeIn(300);
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 405) {
                    $(".c-payment__voucher").prepend("<p class='alert alert-danger msg-gift'>شما بن تخفیف ندارید.</p>").fadeIn(300);
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else if (response.statusCode == 406) {
                    $(".c-payment__voucher").prepend("<p class='alert alert-danger msg-gift'>تعداد بن تخفیف نا معتبر است.</p>").fadeIn(300);
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }

                else {
                    alert(response.message);
                    window.location.href = "/Cart/Payment";
                }


                $(".loading").hide();
            }
        });
    });
    $(".js-decline-gift-card").click(function () {
        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/Removebon",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                var aTop = $(".js-voucher-header").offset().top;
                $('html, body').animate({
                    scrollTop: aTop - 80
                }, 800);

                if (response.statusCode == 200) {
                    $(".c-payment__voucher").prepend("<p class='alert alert-info msg-gift'>بن تخفیف حذف شد. </p>").fadeIn(300);
                    $(".msg-gift").delay(3000).fadeOut(300, function () { window.location.href = "/Cart/Payment"; });
                }
                else {
                    alert(response.message);
                    window.location.href = "/Cart/Payment";
                }


                $(".loading").hide();
            }
        });
    });
    $("#Basket").on("change",".c-checkout-time-table__hour-container",function () {
        var $this = $(this).parent();
        $('html, body').animate({
            scrollTop: $this.offset().top - 80
        }, 800);
    });
    $("#Basket").on("click", "#sendFactor1", function () {
        $('html, body').animate({
            scrollTop: $("#factor-select").offset().top - 80
        }, 800);
    });
    //pay
    $("#Pay-final").click(function () {
        var paymentway = $(".c-payment__paymethod input[type='radio']:checked");


        $(".loading").show();
        $.ajax({
            type: "POST",
            url: "/Cart/AddOrder",
            data: JSON.stringify({ payId: parseInt(paymentway.attr("data-type-payment")), bankid: parseInt(paymentway.attr("data-bank-id")), userDescription: $("#userDescription").val(), sendFactor: $("#sendFactor1").is(":checked") ? true : false, codeGift: $("#payment-voucher-input").val() ? $("#payment-voucher-input").val() : "", BonCount: $("#payment-gift-card-input").val() ? $("#payment-gift-card-input").val() : 0, CustomerOrderId: parseInt($("#Pay-final").attr("data-CustomerOrderId")) > 0 ? parseInt($("#Pay-final").attr("data-CustomerOrderId")) : null }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.IsEstelam == false) {
                    if (response.statusCode == 201) {
                        window.location.href = "/Cart/OnlinePay/" + response.orderid + "?bankid=" + parseInt(response.bankid);
                    }
                    else if (response.statusCode == 202) {
                        window.location.href = "/Cart/GetPay/" + response.orderid;
                    }
                    else if (response.statusCode == 203) {
                        window.location.href = "/Cart/CardPay/" + response.orderid;
                    }
                    else if (response.statusCode == 204) {
                        window.location.href = "/Cart/Pos/" + response.orderid;
                    }
                    else if (response.statusCode == 205) {
                        window.location.href = "/Cart/Cash/" + response.orderid;
                    }
                    else if (response.statusCode == 206) {
                        window.location.href = "/Cart/Receipt/" + response.orderid;
                    }
                    else if (response.statusCode == 404) {

                        window.location.href = "/";
                    }
                    else if (response.statusCode == 4043) {
                        $(".c-payment").prepend("<p class='c-shipment-page__shared-address-message'>برخی از کالاهای شما تغییر موجودی داشته اند. پس از بررسی و در صورت تمایل اقدام به ادامه فرآیند خرید نمایید.</p>").fadeIn(300);
                        $(".c-shipment-page__shared-address-message").delay(3000).fadeOut(300);
                        window.location.href = "/Cart?m=3";
                        $(".loading").hide();
                    }
                    else if (response.statusCode == 4044) {
                        $(".c-payment").prepend("<p class='c-shipment-page__shared-address-message'>کد تخفیف معتبر نمی باشد !</p>").fadeIn(300);
                        $(".c-shipment-page__shared-address-message").delay(3000).fadeOut(300);
                        window.location.href = "/Cart?m=4";
                        $(".loading").hide();
                    }
                    else {
                        $(".c-payment").prepend("<p class='c-shipment-page__shared-address-message'>" + response.message + "</p>").fadeIn(300);
                        $(".c-shipment-page__shared-address-message").delay(3000).fadeOut(300);
                        $(".loading").hide();
                    }
                }
                else {

                    window.location.href = "/Cart/WaitingForConfirmation/" + response.orderid;
                }
            }

        });
    });
});

function CheckSendWayValid() {
    if ($(".c-checkout-pack__shipping-type-row").length == $(".c-checkout-pack__shipping-type-row[data-active='True']").length)
        return true;
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
                $("#cart-data").html(response.NewRow);

                $(".c-checkout__items .c-checkout__item").each(function () {
                    var $this = $(this).find(".quantity");

                    if ($this.find(".Basket-quantity").val() == $this.find(".Basket-quantity").attr("max")) {
                        $this.find(".plus").css("cursor", "not-allowed");
                        $this.find(".plus").addClass("full");
                    }
                    else {
                        $this.find(".plus").css("cursor", "pointer");
                        $this.find(".plus").removeClass("full");
                    }

                    if ($this.find(".Basket-quantity").val() == $this.find(".Basket-quantity").attr("min")) {
                        $this.find(".minus").css("cursor", "not-allowed");
                        $this.find(".minus").addClass("full");
                    }
                    else {
                        $this.find(".minus").css("cursor", "pointer");
                        $this.find(".minus").removeClass("full");
                    }
                });

                $('html, body').animate({
                    scrollTop: $(".js-checkout").offset().top - 80
                }, 800);

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

