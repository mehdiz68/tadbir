$(document).ready(function () {
    var tellNumbers = [];
    $("input#chbAll").change(function () {
        var thisElement = $(this);
        if (thisElement.is(":checked")) {
            tellNumbers = $.map($('input[name="chbSelect"]'), function (el) {
                $(el).prop("checked", true);
                return $(el).closest('tr').children('td').eq(4).find("span").text();
            })
        }
        else {
            tellNumbers = [];
            $('input[name="chbSelect"]').attr("checked",false);
        }

        if (tellNumbers.length>0) {
            if ($("#TextString").val() != "" && ($("select[name='DestinationSms']").val() == "2"
            || ($("select[name='DestinationSms']").val() != "" && $("select[name='DestinationSms']").val() != "2"))) {
                $("input#sendSms").attr("disabled", false);
            }
            else {
                $("input#sendSms").attr("disabled", true);
            }
            //if (tellNumbers.substring(tellNumbers.length - 1) == ",")
            //    tellNumbers = tellNumbers.slice(0, -1);
        }
        else {
            $("input#sendSms").attr("disabled", true);
        }
    });

    $('input[name="chbSelect"]').change(function () {
        if ($(this).is(":checked")) {
            tellNumbers.push($(this).closest('tr').children('td').eq(4).find("span").text());
        }
        else {
            var tellText = $(this).closest('tr').children('td').eq(4).find("span").text();
            var FindIndex = tellNumbers.indexOf(tellText);
            tellNumbers.splice(FindIndex,1);
        }
        
        if (tellNumbers.length>0) {
            if ($("#TextString").val() != "") {
                $("input#sendSms").attr("disabled", false);
            }
            else {
                $("input#sendSms").attr("disabled", true);
            }
            //if (tellNumbers.substring(tellNumbers.length - 1) == ",")
            //    tellNumbers = tellNumbers.slice(0, -1);
        }
        else {
            $("input#sendSms").attr("disabled", true);
        }
    });
    $('#TextString').on('input propertychange paste', function () {
        // do your stuff
        if ($(this).val() != "" && ((tellNumbers.length>0 && $("select[name='DestinationSms']").val() == "2")
            || ($("select[name='DestinationSms']").val() != "" && $("select[name='DestinationSms']").val() != "2")))
            $("input#sendSms").attr("disabled", false);
        else
            $("input#sendSms").attr("disabled", true);
    });

    $("select[name='DestinationSms']").change(function () {
        if ($('#TextString').val() != "" && ((tellNumbers.length > 0 && $(this).val() == "2")
            || ($(this).val() != "" && $(this).val() != "2")))
            $("input#sendSms").attr("disabled", false);
        else
            $("input#sendSms").attr("disabled", true);
    });

    //$("ul.views > li > a").click(function () {
    //    var sortOrder = getUrlVars()["sortOrder"];
    //    if ($(this).parent("li").attr("id") == "#view2") {
    //        //document.location.href = document.location.href + "?Type=Users"
    //        history.pushState({}, document.title, "?Type=Users&&sortOrder=" + sortOrder)
    //    }
    //    else {
    //        history.pushState({}, document.title, "?Type=PhoneGroup&&sortOrder=" + sortOrder)
    //    }
    //});

    //$("a.sortOrder").click(function () {
    //    var Type = getUrlVars()["Type"];
    //    if (Type != undefined) {
    //        var sortOrder = getUrlVars()["sortOrder"];
    //        history.pushState({}, document.location.href, "?Type=" + Type + "&&sortOrder=" + sortOrder)
    //        $("input[name='Activeli']").val(Type);
    //    }
    //});

    $("#sendSms").click(function () {
        $(".loading").show();
        var dataToSend = {
            'TellsNumber': tellNumbers,
            'TextString': $("#TextString").val(),
            'DestinationSms': $("select[name='DestinationSms']").val()
        };

        jQuery.ajax({
            type: "POST",
            url: "/Sms/SendSms",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(dataToSend),
            traditional: true,
            cache: false,
            success: function (data) {
                if (data.success == 1) {
                    $("span#result").css("color", "green");
                    $("span#result").text("پیامک مورد نظر ارسال شد");
                }
                else if (success = 2) {
                    $("span#result").css("color", "red");
                    $("span#result").text("هیچ شماره موبایل معتبری انتخاب نشده است");
                }
                else {
                    $("span#result").css("color", "red");
                    $("span#result").text("ارسال پیامک با مشکل روبه رو شده است");
                }
                $(".loading").hide();
            },
            failure: function (errMsg) {
                $(".loading").hide();
                $("span#result").css("color", "red");
                $("span#result").text("ارسال پیامک با مشکل روبه رو شده است");
            }
        });
    });
});

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

