$(document).ready(function () {
    $(".Value").keyup(function () {
        $(this).val(numberWithCommas(parseInt(removeComma($(this).val()))));

    });
    $(".MaxValue").keyup(function () {
        $(this).val(numberWithCommas(parseInt(removeComma($(this).val()))));

    });
    $(".PageSizeSelector").change(function () {
        $("#PageSize").val($(this).val());
        $("#Search-Form").submit();
    });
    $("body").on("click change keyup keydown", ".Code,.Value,.CodeType,.MaxValue,.IsActive,.CountUse", function () {

        if ($(this).hasClass("IsActive")) {
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


        if ($(this).parent().parent().find(".Value").parent().attr("data-version") != "0" || $(this).parent().parent().find(".MaxValue").parent().attr("data-version") != "0" || $(this).parent().parent().find(".Code").parent().attr("data-version") != "0" || $(this).parent().parent().find(".CodeType").parent().attr("data-version") != "0" || $(this).parent().parent().find(".IsActive").parent().attr("data-version") != "0" || $(this).parent().parent().find(".CountUse").parent().attr("data-version") != "0") {
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
        if ($(this).hasClass("ok") && $(this).parent().parent().find(".Value").val()) {
            $(".loading").show();
            var $this = $(this);
            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Admin/Offers/UpdateCodeGift',
                data: '{OfferId:"' + $this.attr("data-offer-id") + '",UserCodeGiftId:"' + $this.attr("data-UserCodeGift-id") + '",Value:"' + removeComma($this.parent().parent().find(".Value").val()) + '",Code:"' + $this.parent().parent().find(".Code").val() + '",MaxValue:"' + removeComma($this.parent().parent().find(".MaxValue").val()) + '",CountUse:"' + $this.parent().parent().find(".CountUse").val() + '",IsActive:"' + $this.parent().parent().find(".IsActive").is(":checked") + '",CodeType:"' + $this.parent().parent().find(".CodeType option:selected").val() + '",UserId:"' + $this.attr("data-id") + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        $this.parent().parent().find(".CountUse").val(response.CountUse);
                        $this.removeClass("btn-primary");
                        $this.removeClass("ok");
                        $this.addClass("btn-default");
                        $(".CodeContainer,.ValueContainer,.CodeTypeContainer,.MaxValueContainer,.CountUseContainer,.IsActiveContainer").attr("data-version", "0");
                        $(".table-responsive tbody tr").each(function () {
                            $(this).find(".Value").attr("data-old-value", $(this).find(".Value").val());
                            $(this).find(".MaxValue").attr("data-old-value", $(this).find(".MaxValue").val());
                            $(this).find(".Code").attr("data-old-value", $(this).find(".Code").val());
                            $(this).find(".CodeType").attr("data-old-value", $(this).find(".CodeType").val());
                            $(this).find(".IsActive").attr("data-old-value", $(this).find(".IsActive").is(":checked"));
                            $(this).find(".CountUse").attr("data-old-value", $(this).find(".CountUse").val());

                        });


                        swal({ title: "", text: response.message, type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
    });


    $("#Add-userCodeGift-Group").click(function () {
        if ($("#UserGroupIdCAT").val() != "" && $(".ValueCat").val() != "" && $(".MaxValueCat").val() != "" && $(".CountUseCat").val() != "" ) {
            $(".loading").show();
            $.ajax({
                type: "POST",
                url: '/Admin/Offers/AddUserCodeGiftGroup',
                data: '{OfferId:"' + $("#Add-userCodeGift-Group").attr("data-Offer-Id") + '",codeType:"' + $(".CodeTypeCat option:selected").val() + '",UserGroupId:"' + $("#UserGroupIdCAT option:selected").val() + '",codeValueCat :"' + $(".codeValueCat ").val() + '",ValueCat :"' + $(".ValueCat").val() + '",MaxValueCat :"' + $(".MaxValueCat").val() + '",CountUseCat :"' + $(".CountUseCat").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 200) {
                        swal({ title: "", text: "ثبت شد", type: "success", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                        window.location.href = "/Admin/Offers/CodeDetail/" + $("#Add-userCodeGift-Group").attr("data-Offer-Id");
                    }
                    else {
                        swal({ title: "", text: response.message, type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }
                    $(".loading").hide();
                }
            });
        }
        else
            swal({ title: "", text: "گروه کاربری و تنظیمات تخفیف خود را انتخاب نمایید", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
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