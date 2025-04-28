$(document).ready(function () {

    $('#barbariNumber3').simplyCountable({ counter: '.BarbariTitle', maxCount: 24, safeClass: 'safe', overClass: 'over' });

    $(".SendWay").each(function () {
        var $this = $(this);
        $this.val($this.attr("data-id"));
    });

    $(".IconId").change(function () {
        if ($(this).is(":checked")) {
            $(this).parent().parent().append("<input name='IconId' type='hidden' value='" + $(this).attr("data-id") + "' /> ");
        }
        else {
            $(this).parent().parent().find("input[value='" + $(this).attr("data-id") + "'] ").remove();

        }
    });

    $("body").on("click change keyup keydown", ".Price,.Quantity", function () {

     
            if ($(this).val() != $(this).attr("data-old-value")) {
                if ($(this).val())
                    $(this).parent().attr("data-version", 1);
                else
                    $(this).parent().attr("data-version", 0);
            }
            else {
                $(this).parent().attr("data-version", 0);
            }
        


        if ($(this).parent().parent().find(".Price").parent().attr("data-version") != "0"  || $(this).parent().parent().find(".Quantity").parent().attr("data-version") != "0" ) {
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
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Admin/Orders/UpdateOrderRow',
                data: '{OrderRowId:"' + $this.attr("data-id") + '",Price:"' + removeComma($this.parent().parent().find(".Price").val()) + '",Quantity:"' + $this.parent().parent().find(".Quantity").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $this.removeClass("btn-primary");
                        $this.removeClass("ok");
                        $this.addClass("btn-default");
                        $(".PriceContainer,.PriceContainer,.MaxBasketCountContainer,.DeliveryTimeoutContainer,.QuantityContainer,.IsActiveContainer,.ProductStateIdContainer,.IsDefaultContainer").attr("data-version", "0");
                        $(".table-responsive tbody tr").each(function () {
                            $(this).find(".Price").attr("data-old-value", $(this).find(".Price").val());
                            $(this).find(".Quantity").attr("data-old-value", $(this).find(".Quantity").val());

                        });

                        swal({ title: "", text: "تغیر یافت", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        location.reload();
                    }
                    else {
                        swal({ title: "", text: response.Message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
    });


    //Confrirm Payment
    $("#Confirm-Variz-Payment").click(function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            $(".loading").show();
            var dataToSend = {
                'WalletId': $this.attr("data-id"),
                'OrderId':$this.attr("data-order-id")
            };
            jQuery.ajax({
                type: "POST",
                url: "/Orders/ConfirmVariz/",
                data: JSON.stringify(dataToSend),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (response) {
                    if (response.statusCode == 200) {
                        window.location.href = $this.attr("data-href");
                    }
                    else {
                        alert(response.Message);
                    }
                    $(".loading").hide();
                },
                failure: function (errMsg) {
                    $(".loading").hide();
                    alert(errMsg.Message);
                }
            });
        }

    });

    $("#Confirm-Card-To-Card-Payment").click(function () {
        var $this = $(this);
        if (confirm("آیا مطمئن هستید؟")) {
            $(".loading").show();
            var dataToSend = {
                'WalletId': $this.attr("data-id"),
                'OrderId': $this.attr("data-order-id")
            };
            jQuery.ajax({
                type: "POST",
                url: "/Orders/ConfirmCardToCard/",
                data: JSON.stringify(dataToSend),
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                cache: false,
                success: function (response) {
                    if (response.statusCode == 200) {
                        window.location.href = $this.attr("data-href");
                    }
                    else {
                        alert(response.Message);
                    }
                    $(".loading").hide();
                },
                failure: function (errMsg) {
                    $(".loading").hide();
                    alert(errMsg.Message);
                }
            });
        }

    });

    $("select#OrderStateId").change(function () {
        if ($(this).val() == "5") {
                $("div#trackingCode").removeClass("hide");
            $("div#BarbariNumber,#BarbariNumber2,#BarbariNumber3").removeClass("hide");
            $("div#ConfirmEstelamcc").addClass("hide");
        }
        else if ($(this).val() == "2" && ($("#ConfirmCashPeikDiv").attr("data-paymenttype") == 4 || $("#ConfirmPosPeikDiv").attr("data-paymenttype") == 5))
        {
            $("div#trackingCode,div#BarbariNumber,#BarbariNumber2,#BarbariNumber3,#ConfirmEstelamcc").addClass("hide");
            if($("#ConfirmCashPeikDiv").attr("data-paymenttype") == 5)
                $("div#ConfirmCashPeikDiv").removeClass("hide");
            else if($("#ConfirmPosPeikDiv").attr("data-paymenttype") == 4)
                $("div#ConfirmPosPeikDiv").removeClass("hide");

        }
        else if ($(this).val() == "1") {
            $("div#ConfirmEstelamcc").removeClass("hide");
            $("div#trackingCode").addClass("hide");
            $("div#BarbariNumber,#BarbariNumber2,#BarbariNumber3").addClass("hide");
            $("div#ConfirmCashPeikDiv").addClass("hide");
            $("div#ConfirmPosPeikDiv").addClass("hide");
        }

        else {
            $("div#trackingCode,div#BarbariNumber,#BarbariNumber2,#BarbariNumber3,div#ConfirmEstelamcc").addClass("hide");
        }
    });

    if ($("select#OrderStateId").val() == "5") {
            $("div#trackingCode").removeClass("hide");
            $("div#BarbariNumber,#BarbariNumber2,#BarbariNumber3").removeClass("hide");
    }
    else if ($("select#OrderStateId").val() == "2" && ($("#ConfirmCashPeikDiv").attr("data-paymenttype") == 4 || $("#ConfirmPosPeikDiv").attr("data-paymenttype") == 5)) {
        $("div#trackingCode,div#BarbariNumber,#BarbariNumber2,#BarbariNumber3,#ConfirmEstelamcc").addClass("hide");
        if ($("#ConfirmCashPeikDiv").attr("data-paymenttype") == 5)
            $("div#ConfirmCashPeikDiv").removeClass("hide");
        else if ($("#ConfirmPosPeikDiv").attr("data-paymenttype") == 4)
            $("div#ConfirmPosPeikDiv").removeClass("hide");

    }
    else if ($("select#OrderStateId").val()  == "1") {
        $("div#ConfirmEstelamcc").removeClass("hide");
        $("div#trackingCode").addClass("hide");
        $("div#BarbariNumber,#BarbariNumber2,#BarbariNumber3").addClass("hide");
        $("div#ConfirmCashPeikDiv").addClass("hide");
        $("div#ConfirmPosPeikDiv").addClass("hide");
    }
    else {
        $("div#trackingCode,div#BarbariNumber,#BarbariNumber2,#BarbariNumber3,div#ConfirmEstelamcc").addClass("hide");
    }

    $("#btnSearch").click(function () {
        var $this = $(this);
        $(".loading").show();
        var dataToSend = {
            'productCode': $("#ProductCode").val(),
            'productName': $("#ProductName").val()
        };

        jQuery.ajax({
            type: "POST",
            url: "/Orders/ProductSearch",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dataToSend),
            traditional: true,
            cache: false,
            success: function (response) {
                if (response.statusCode == 200) {
                    if (response.data.length > 0) {
                        var html = "<option id='0' value='0'>--انتخاب نمایید--</option>";
                        for (var i = 0; i < response.data.length; i++) {

                            html += "<option id='Cat" + response.data[i].id + "' value='" + response.data[i].id + "'>" + response.data[i].name + "</option>";
                        }
                        $(".productSelect > select").html(html);
                    }
                    else {
                        $(".productSelect > select").html("<option id='0' value='0'>--انتخاب نمایید--</option>");
                    }
                }
                $(".loading").hide();
            },
            failure: function (errMsg) {
                $(".loading").hide();
                alert(errMsg.data);
            }
        });
    });

    $("#btnAddProduct").click(function () {
        if ($("#selectProduct").val() != "0") {
            var $this = $(this);
            $(".loading").show();
            var dataToSend = {
                'OrderId': $("input#Id").val(),
                'productCode': $("#selectProduct").val(),
                'quantity': $("#txtquantity").val()
            };

            jQuery.ajax({
                type: "POST",
                url: "/Orders/AddProductToList",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dataToSend),
                traditional: true,
                cache: false,
                success: function (response) {
                    if (response.statusCode == 200) {
                        var price = parseInt(response.data.price);
                        var quantity = parseInt($("#txtquantity").val());
                        var totalPrice = price * quantity;
                        var content = '<tr class="ContentRow" data-rowid="@item.Id"><td>' + response.data.id + '</td>' +
                        '<td>' + response.data.name + '</td>' +
                        '<td><input class="text-box single-line" data-val="true" data-val-number="The field تعداد must be a number." data-val-required="The تعداد field is required." id="item_Quantity" name="item.Quantity" type="number" value="' + $("#txtquantity").val() + '"></td>' +
                        '<td>' + response.data.price.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</td>' +
                        '<td>' + totalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",") + '</td>' +
                        '<td>' +
                        '<a class="btn btn-info updateQuantity"><span class="glyphicon glyphicon-edit"></span> به روز رسانی </a>' +
                            '<a class="deleteProduct btn btn-danger"><span class="glyphicon glyphicon-remove"></span> حذف </a>' +
                        '</td></tr>';
                        $("#tblProducts > tbody").append(content);
                    }
                    else {
                        alert(response.data);
                    }
                    $(".loading").hide();
                },
                failure: function (errMsg) {
                    $(".loading").hide();
                    alert(errMsg.data);
                }
            });
        }
    });

    $("table").on("click", ".updateQuantity", function () {
        var $this = $(this);
        if ($("#item_Quantity").val() != "0") {
            $(".loading").show();
            var dataToSend = {
                'OrderRowId': $this.parents("tr:first").attr("data-rowid"),
                'quantity': $("#item_Quantity").val()
            };

            jQuery.ajax({
                type: "POST",
                url: "/Orders/AddProductQuantity",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dataToSend),
                traditional: true,
                cache: false,
                success: function (response) {
                    if (response.statusCode == 200) {
                        var price = parseInt($this.parents("tr:first").find("span#price").text().replace(",", ""));
                        var quantity = parseInt($("#item_Quantity").val());
                        var totalPrice = price * quantity;
                        $this.parents("tr:first").find("span#totalPrice").text(totalPrice.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ","));
                    }
                    else {
                        alert(response.data);
                    }
                    $(".loading").hide();
                },
                failure: function (errMsg) {
                    $(".loading").hide();
                    alert(errMsg.data);
                }
            });
        }
    });

    $("table").on("click", ".deleteProduct", function () {
        if (confirm("آیا شما مطمئن به حذف محصول مورد نظر هستید؟!") == true) {
            var $this = $(this);
            $(".loading").show();
            var dataToSend = {
                'OrderRowId': $this.parents("tr:first").attr("data-rowid")
            };

            jQuery.ajax({
                type: "POST",
                url: "/Orders/DeleteProduct",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(dataToSend),
                traditional: true,
                cache: false,
                success: function (response) {
                    if (response.statusCode == 200) {
                        $this.parents("tr:first").remove();
                    }
                    else {
                        alert(response.data);
                    }
                    $(".loading").hide();
                },
                failure: function (errMsg) {
                    $(".loading").hide();
                    alert(errMsg.data);
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