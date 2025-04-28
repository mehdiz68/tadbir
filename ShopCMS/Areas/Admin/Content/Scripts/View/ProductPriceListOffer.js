$(document).ready(function () {
    $(".Price").keyup(function () {
        $(this).val(numberWithCommas(parseInt(removeComma($(this).val()))));

    });

    $(".PageSizeSelector").change(function () {
        $("#PageSize").val($(this).val());
        $("#SearchType").val("1");
        $("#Search-Form").submit();
    });
    $(".PageSizeSelector2").change(function () {
        $("#PageSize2").val($(this).val());
        $("#SearchType").val("4");
        $("#Search-Form").submit();
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

    $("#Add-ProductOffer-Group").click(function () {
        if ($("#CatOffer #ProductCatId").val() != "" && $(".ValueCat").val() != "") {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductPrice/AddProductofferGroup',
                data: '{Id:"' + $("#CatOffer #ProductCatId").val() + '",OfferId:"' + $("#Add-ProductOffer-Group").attr("data-Offer-Id") + '",codeType:"' + $(".CodeTypeCat option:selected").val() + '",valuee:"' + $(".ValueCat").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        swal({ title: "", text: "ثبت شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        window.location.href = "/Admin/Offers/Detail/" + $("#Add-ProductOffer-Group").attr("data-Offer-Id");
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
        else
            swal({ title: "", text: "گروه و میزان تخفیف خود را انتخاب نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
    });


    $("#Add-ProductOffer-Brand").click(function () {
        if ($("#BrandId").val() != "" && $(".ValueBrand").val() != "") {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/ProductPrice/AddProductofferBrand',
                data: '{Id:"' + $("#BrandId").val() + '",OfferId:"' + $("#Add-ProductOffer-Brand").attr("data-Offer-Id") + '",codeType:"' + $(".CodeTypeBrand option:selected").val() + '",valuee:"' + $(".ValueBrand").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        swal({ title: "", text: "ثبت شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        window.location.href = "/Admin/Offers/Detail/" + $("#Add-ProductOffer-Brand").attr("data-Offer-Id");
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
        else
            swal({ title: "", text: "گروه و میزان تخفیف خود را انتخاب نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
    });



    $("body").on("change", "#ProductTypeId", function () {
        var typeid = $("#ProductTypeId").val();
        $.ajax({
            type: "GET",
            url: '/Admin/Products/GetCategory/?ProductTypeId=' + typeid,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#ProductCatId").val(null);

                    var r = $(".CategoryManager select").filter(function () {
                        return parseInt($(this).attr("data-level")) > 1;
                    });
                    r.remove();
                    $(".CategoryManager select option").remove();
                    var html = "<option id='0' value='Null'>هیچکدام</option>";
                    $(".CategoryManager select").append(html);
                    for (var i = 0; i < response.data.length; i++) {
                        html = "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        $(".CategoryManager select").append(html);
                    }
                }
            }

        });
    });
    $("body").on("change", "#OfferProductTypeId", function () {
        var typeid = $("#OfferProductTypeId").val();
        $.ajax({
            type: "GET",
            url: '/Admin/Products/GetOfferCategory/?ProductTypeId=' + typeid + "&OfferId=" + $("#OfferId").val(),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 200) {
                    $("#ProductCatId").val(null);

                    var r = $(".CategoryManager select").filter(function () {
                        return parseInt($(this).attr("data-level")) > 1;
                    });
                    r.remove();
                    $(".CategoryManager select option").remove();
                    var html = "<option id='0' value='Null'>هیچکدام</option>";
                    $(".CategoryManager select").append(html);
                    for (var i = 0; i < response.data.length; i++) {
                        html = "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        $(".CategoryManager select").append(html);
                    }
                }
            }

        });
    });
    if ($("#ProductOffer .CategoryManager #ProductCatId").val() != "") {
        var CurrentCatId = parseInt($("#ProductOffer .CategoryManager #ProductCatId").val());
        if (CurrentCatId > 0) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/Products/GetParentCategory/',
                data: '{CategoryId:"' + CurrentCatId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 201) {
                        var ProductTypeId = 0;
                        ProductTypeId = $("#OfferProductTypeId").val();
                        $("#ProductOffer .CategoryManager").html("<input type='hidden' name='ProductCatId' id='ProductCatId' value='null' />");
                        for (var i = 0; i < response.data.length; i++) {
                            var id = "";
                            if (response.data[i].ParrentId != null)
                                id = response.data[i].ParrentId;
                            var html = "<select data-level='" + i + "' data-id='" + id + "' id='Category" + id + "' name='Category" + id + "' class='form-control '><option id='0' value='0'>--انتخاب نمایید--</option></select>";
                            $("#ProductOffer .CategoryManager").append(html);
                            LoadChildCategory($("#ProductOffer .CategoryManager select").last(), id, "option", ProductTypeId);
                        }
                        $("#ProductOffer .CategoryManager select").each(function () {
                            $("#ProductOffer .CategoryManager select option[value='" + $(this).attr("data-id") + "']").prop("selected", true);
                        });
                        $("#ProductOffer .CategoryManager select option[value='" + CurrentCatId + "']").prop("selected", true);
                        $("#ProductCatId").val(CurrentCatId);
                    }
                    else if (response.statusCode == 200) {
                        $("#ProductOffer .CategoryManager select option[value='" + response.data + "']").prop("selected", true);
                        LoadChildCategory($("#ProductOffer .CategoryManager select").first(), response.data, "select", ProductTypeId);
                    }
                    $(".loading").hide();
                }
            });
        }
        else {
            $("#ProductOffer .CategoryManager select option:eq(0)").prop("selected", true);
        }
    }


    if ($("#CatOffer .CategoryManager #ProductCatId").val() != "") {
        var CurrentCatId = parseInt($("#CatOffer .CategoryManager #ProductCatId").val());
        if (CurrentCatId > 0) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/Products/GetParentCategory/',
                data: '{CategoryId:"' + CurrentCatId + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 201) {
                        var ProductTypeId = 0;
                        ProductTypeId = $("#OfferProductTypeId2").val();
                        $("#CatOffer.CategoryManager").html("<input type='hidden' name='ProductCatId' id='ProductCatId' value='null' />");
                        for (var i = 0; i < response.data.length; i++) {
                            var id = "";
                            if (response.data[i].ParrentId != null)
                                id = response.data[i].ParrentId;
                            var html = "<select data-level='" + i + "' data-id='" + id + "' id='Category" + id + "' name='Category" + id + "' class='form-control '><option id='0' value='0'>--انتخاب نمایید--</option></select>";
                            $("#CatOffer .CategoryManager").append(html);
                            LoadChildCategory($("#CatOffer .CategoryManager select").last(), id, "option", ProductTypeId);
                        }
                        $("#CatOffer .CategoryManager select").each(function () {
                            $("#CatOffer .CategoryManager select option[value='" + $(this).attr("data-id") + "']").prop("selected", true);
                        });
                        $("#CatOffer .CategoryManager select option[value='" + CurrentCatId + "']").prop("selected", true);
                        $("#ProductCatId").val(CurrentCatId);
                    }
                    else if (response.statusCode == 200) {
                        $("#CatOffer .CategoryManager select option[value='" + response.data + "']").prop("selected", true);
                        LoadChildCategory($("#CatOffer .CategoryManager select").first(), response.data, "select", ProductTypeId);
                    }
                    $(".loading").hide();
                }
            });
        }
        else {
            $("#CatOffer .CategoryManager select option:eq(0)").prop("selected", true);
        }
    }

    $(".CategoryManager").on("change", "select", function () {
        var $this = $(this);
        if ($this.val() > 0) {
            var ProductTypeId = 0;
            if ($("#ProductTypeId").val() != undefined)
                ProductTypeId = $("#ProductTypeId").val();
            else
                ProductTypeId = $("#OfferProductTypeId").val();
            LoadChildCategory($this, $this.val(), "select", ProductTypeId);
            $(".CategoryManager #ProductCatId").val($this.val());
        }
        else {
            $this.parent().find("select").filter(function () { return $(this).attr("data-level") > (parseInt($this.attr("data-level"))); }).remove();
            $(".CategoryManager #ProductCatId").val($this.prev().find("option:selected").val());
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


function LoadChildCategory($this, value, type, productTypeId) {
    value == "" ? value = null : value;
    $(".loading").show();
    $.ajax({
        async: false,
        type: "POST",
        url: '/Admin/Products/GetChildCategory/',
        data: '{CategoryId:"' + value + '",ProductTypeId:"' + productTypeId + '"}',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            if (response.statusCode == 200) {
                if (type == "select")
                    $this.parent().find("select").filter(function () { return $(this).attr("data-level") > (parseInt($this.attr("data-level"))); }).remove();
                if (response.data.length > 0) {
                    var html = "";
                    if (type == "select")
                        html += "<select data-level='" + (parseInt($this.attr("data-level")) + 1) + "' id='Category" + value + "' name='Category" + value + "' class='form-control'><option id='0' value='0'>--انتخاب نمایید--</option>";
                    for (var i = 0; i < response.data.length; i++) {
                        if (type == "select")
                            html += "<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>";
                        else {
                            $this.append("<option id='Cat" + response.data[i].Id + "' value='" + response.data[i].Id + "'>" + response.data[i].Title + "</option>");
                        }
                    }
                    if (type == "select")
                        html += "</select>";
                    $this.parent().append(html);
                }

                $("#ProductCatId").val(value);
            }
            $(".loading").hide();
        }
    });

}