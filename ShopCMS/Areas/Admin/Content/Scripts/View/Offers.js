$(document).ready(function () {

    $(".Price").keyup(function () {
        $(this).val(numberWithCommas(parseInt(removeComma($(this).val()))));

    });
    $("#Offers .PageSizeSelector").change(function(){
     $("#PageSize").val($(this).val());
     $("#Search-Form").submit();
 });
    $("#Products .PageSizeSelector").change(function () {
        $("#pgsize").val($(this).val());
        $("#Search-Form2").submit();
    });

    $("body").on("click change keyup keydown", ".Price,.MaxBasketCount,.DeliveryTimeout,.Quantity,.IsActive,.ProductStateId,.IsDefault,.MaxBasketCountOffer,.QuantityOffer,.Value,.CodeType", function () {

        if ($(this).hasClass("IsActive") || $(this).hasClass("IsDefault")) {
            var oldvalue = $(this).attr("data-old-value") == "true" || $(this).attr("data-old-value") == "True" ? true : false;

            if ($(this).is(":checked") != oldvalue)
                $(this).parent().attr("data-version", 1);
            else {
                $(this).parent().attr("data-version", 0);
            }
        }
        else {
            if ($(this).val() != $(this).attr("data-old-value")) {
                if ($(this).val())
                    $(this).parent().attr("data-version", 1);
                else
                    $(this).parent().attr("data-version", 0);
            }
            else {
                $(this).parent().attr("data-version", 0);
            }
        }


        if ($(this).parent().parent().find(".Price").parent().attr("data-version") != "0" || $(this).parent().parent().find(".MaxBasketCount").parent().attr("data-version") != "0" || $(this).parent().parent().find(".DeliveryTimeout").parent().attr("data-version") != "0" || $(this).parent().parent().find(".Quantity").parent().attr("data-version") != "0" || $(this).parent().parent().find(".IsActive").parent().attr("data-version") != "0" || $(this).parent().parent().find(".ProductStateId").parent().attr("data-version") != "0" || $(this).parent().parent().find(".IsDefault").parent().attr("data-version") != "0" || $(this).parent().parent().find(".QuantityOffer").parent().attr("data-version") != "0" || $(this).parent().parent().find(".MaxBasketCountOffer").parent().attr("data-version") != "0" || $(this).parent().parent().find(".Value").parent().attr("data-version") != "0" || $(this).parent().parent().find(".CodeType").parent().attr("data-version") != "0") {
            $(this).parent().parent().find(".Edit-Now").removeClass("btn-default");
            $(this).parent().parent().find(".Edit-Now").addClass("btn-primary ok");

        }
        else {
            $(this).parent().parent().find(".Edit-Now").removeClass("btn-primary");
            $(this).parent().parent().find(".Edit-Now").removeClass("ok");
            $(this).parent().parent().find(".Edit-Now").addClass("btn-default");

        }

    });
    $(".Edit-Now").click(function (e) {
        e.preventDefault();
        if ($(this).hasClass("ok") && $(this).parent().parent().find(".Price").val()) {
            $(".loading").show();
            var $this = $(this);
            console.log('{PrId:"' + $this.attr("data-id") + '",Price:"' + removeComma($this.parent().parent().find(".Price").val()) + '",MaxBasketCount:"' + $this.parent().parent().find(".MaxBasketCount").val() + '",DeliveryTimeout:"' + $this.parent().parent().find(".DeliveryTimeout").val() + '",Quantity:"' + $this.parent().parent().find(".Quantity").val() + '",IsActive:"' + $this.parent().parent().find(".IsActive").is(":checked") + '",ProductStateId:"' + $this.parent().parent().find(".ProductStateId option:selected").val() + '",IsDefault:"' + $this.parent().parent().find(".IsDefault").is(":checked") + '"}');
            $.ajax({
                async: false,
                type: "POST",
                url: '/Admin/ProductPrice/UpdatePrPrice',
                data: '{PrId:"' + $this.attr("data-id") + '",Price:"' + removeComma($this.parent().parent().find(".Price").val()) + '",MaxBasketCount:"' + $this.parent().parent().find(".MaxBasketCount").val() + '",DeliveryTimeout:"' + $this.parent().parent().find(".DeliveryTimeout").val() + '",Quantity:"' + $this.parent().parent().find(".Quantity").val() + '",IsActive:"' + $this.parent().parent().find(".IsActive").is(":checked") + '",ProductStateId:"' + $this.parent().parent().find(".ProductStateId option:selected").val() + '",IsDefault:"' + $this.parent().parent().find(".IsDefault").is(":checked") + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        var prstateId = response.ProductStateId;
                        $.ajax({
                            async: false,
                            type: "POST",
                            url: '/Admin/ProductPrice/UpdateProductoffer',
                            data: '{offerId:"' + $this.attr("data-offer-id") + '",ProductOfferId:"' + $this.attr("data-product-offer-id") + '",ProductPriceId:"' + $this.attr("data-id") + '",codeType:"' + $this.parent().parent().find(".CodeType option:selected").val() + '",valuee:"' + $this.parent().parent().find(".Value").val() + '",Quantity:"' + $this.parent().parent().find(".QuantityOffer").val() + '",maxQuantity:"' + $this.parent().parent().find(".MaxBasketCountOffer").val() + '"}',
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (response) {
                                if (response.statusCode == 200) {
                                    $this.parent().parent().find(".ProductStateId").val(prstateId);
                                    $this.removeClass("btn-primary");
                                    $this.removeClass("ok");
                                    $this.addClass("btn-default");
                                    $(".PriceContainer,.PriceContainer,.MaxBasketCountContainer,.DeliveryTimeoutContainer,.QuantityContainer,.IsActiveContainer,.ProductStateIdContainer,.IsDefaultContainer,.MaxBasketCountOffer,.QuantityOffer,.Value,.Code").attr("data-version", "0");
                                    $(".table-responsive tbody tr").each(function () {
                                        $(this).find(".Price").attr("data-old-value", $(this).find(".Price").val());
                                        $(this).find(".MaxBasketCount").attr("data-old-value", $(this).find(".MaxBasketCount").val());
                                        $(this).find(".DeliveryTimeout").attr("data-old-value", $(this).find(".DeliveryTimeout").val());
                                        $(this).find(".Quantity").attr("data-old-value", $(this).find(".Quantity").val());
                                        $(this).find(".IsActive").attr("data-old-value", $(this).find(".IsActive").is(":checked"));
                                        $(this).find(".ProductStateId").attr("data-old-value", $(this).find(".ProductStateId").val());
                                        $(this).find(".IsDefault").attr("data-old-value", $(this).find(".IsDefault").is(":checked"));
                                        $(this).find(".QuantityOffer").attr("data-old-value", $(this).find(".QuantityOffer").val());
                                        $(this).find(".MaxBasketCountOffer").attr("data-old-value", $(this).find(".MaxBasketCountOffer").val());
                                        $(this).find(".CodeType").attr("data-old-value", $(this).find(".CodeType").val());
                                        $(this).find(".Value").attr("data-old-value", $(this).find(".Value").val());

                                    });

                                    if ($this.hasClass("IsDefaultChanger")) {
                                        $this.parent().parent().parent().find(".IsDefault").prop("checked", false);
                                        $this.parent().parent().find(".IsDefault").prop("checked", true);
                                    }
                                    swal({ title: "", text: response.message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                                }

                                else {
                                    swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                                }
                            },
                            error: function (e) {
                                console.log(e);
                            }
                        });



                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
    });

});
function removeComma(x) {
    while (x.indexOf(',') > -1) {
        x = x.replace(",", "");
    }
    return x;
}

function numberWithCommas(x) {
    return x.toLocaleString();
}
