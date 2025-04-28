$(document).ready(function () {

    $(".InputColor").spectrum({
        preferredFormat: "hex",
        showInput: true,
        showPalette: true,
        allowEmpty: true,
        palette: [["red", "rgba(0, 255, 0, .5)", "rgb(0, 0, 255)"]]
    });

    //******************************************نمایش لیست مقادیر خصوصیت لیستی*********************************************
    if ($("#DataType").val() == 8 || $("#DataType").val() == 15)
        $('#ItemModal').modal('show');

    if ($("#DataType").val() == 11 || $("#DataType").val() == 12)
        $('#ItemColorModal').modal('show');

    $("#DataType").change(function () {
        if ($(this).val() == 8 || $(this).val() == 15) {
            $('#ItemModal').modal('show');
        }
        if ($(this).val() == 11 || $(this).val() == 12) {
            $('#ItemColorModal').modal('show');
        }
    });

    //******************************************افزودن خصوصیت لیستی**********************************************************
    $("#ProductAttributeItemAdd #Add-Item").click(function () {
        if ($("#AddedValue").val()) {
            $("#AddedValue").removeClass("input-validation-error");
            $(".loading").show(); NProgress.start();

            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Admin/ProductAttributes/AddAttributeItem/',
                data: '{Value:"' + $("#AddedValue").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $(".loading").hide(); NProgress.done();

                        $("#ProductAttributeItemList table tbody #empty").remove();
                        $("#ProductAttributeItemList table tbody").prepend("<tr><td>" + $("#AddedValue").val() + "</td><td><span class='btn btn-danger'>حذف</span><span class='btn btn-primary'>ویرایش</span></td></tr>");
                        $("#AddedValue").val("");
                    }
                    else if (response.statusCode == 501) {

                        $(".loading").hide(); NProgress.done();
                        swal({ title: "", text: "رکورد وارد شده تکراری است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    else {
                        $(".loading").hide(); NProgress.done();
                        swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }

                }
            });
        }
        else {

            $("#AddedValue").addClass("input-validation-error");
        }
    });

    //******************************************نمایش مقادیر وارد شده ی خصوصیت لیستی***************************************


    //******************************************نمایش برای ویرایش مقدار وارد شده ی خصوصیت لیستی***************************
    $("#ProductAttributeItemList table tbody").on("click", ".btn-primary", function () {
        $("#Edit-Item").css("display", "inline-block");
        $("#AddedValue").val($(this).parent().prev().text().trim());
        $("#ProductAttributeItemList table tbody tr").removeClass("selected");
        $(this).parent().parent().addClass("selected");

    });

    //******************************************ویرایش خصوصیت لیستی**********************************************************
    $("#Edit-Item").click(function () {
        $(".loading").show(); NProgress.start();
        var $this = $(this);
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/ProductAttributes/EditAttributeItem/',
            data: '{Value:"' +  $("#AddedValue").val() + '",OldValue:"' + $("#ProductAttributeItemList table tbody .selected td:first").text().trim() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();
                    $("#ProductAttributeItemList table tbody .selected td:first").text($("#AddedValue").val());
                    $("#Edit-Item").css("display", "none");
                    $("#AddedValue").val("");
                }
                else if (response.statusCode == 501) {

                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "رکورد وارد شده تکراری است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });

    });
    

    //******************************************حذف خصوصیت لیستی**********************************************************
    $("#ProductAttributeItemList table tbody").on("click", ".btn-danger", function () {
        $(".loading").show(); NProgress.start();
        var $this = $(this);
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/ProductAttributes/RemoveAttributeItem/',
            data: '{Value:"' + $this.parent().prev().text().trim() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();

                    $this.parent().parent().remove();
                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });

    });

    
    //************************************************************************************************************************************************************************//


    //******************************************افزودن خصوصیت رنگی**********************************************************
    $("#ProductAttributeItemColorAdd #Add-Item-Color").click(function () {
        var valid = true;
        if ($("#AddedValueColor").val())
            $("#AddedValueColor").removeClass("input-validation-error");
        else {
            valid = false;
            $("#AddedValueColor").addClass("input-validation-error");
        }
        if ($("#AddedValueValue").val())
            $("#AddedValueValue").removeClass("input-validation-error");
        else {
            valid = false;
            $("#AddedValueValue").addClass("input-validation-error");
        }
        if (valid) {
            $(".loading").show(); NProgress.start();

            $.ajax({
                crossDomain: true,
                type: "POST",
                url: '/Admin/ProductAttributes/AddAttributeItemColor/',
                data: '{Value:"' + $("#AddedValueValue").val() + '",Color:"' + $("#AddedValueColor").val() + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    if (response.statusCode == 400) {
                        $(".loading").hide(); NProgress.done();

                        $("#ProductAttributeItemColorList table tbody #empty").remove();
                        $("#ProductAttributeItemColorList table tbody").prepend("<tr><td>" + $("#AddedValueColor").val() + "</td><td>" + $("#AddedValueValue").val() + " <br /> <span style='display:inline-block;background-color:"+$("#AddedValueValue").val()+";width:25px;height:25px;border:1px solid #aaa'></span></td><td><span class='btn btn-danger'>حذف</span><span class='btn btn-primary'>ویرایش</span></td></tr>");
                        $("#AddedValueValue").val("");
                        $("#AddedValueColor").val("");
                    }
                    else if (response.statusCode == 501)
                    {

                        $(".loading").hide(); NProgress.done();
                        swal({ title: "", text: "رکورد وارد شده تکراری است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                    }
                    else {
                        $(".loading").hide(); NProgress.done();
                        swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                    }

                }
            });
        }
        else {
            swal({ title: "", text: "عنوان رنگ و خود رنگ را وارد نمایید !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

        }
    });

    //******************************************نمایش مقادیر وارد شده ی خصوصیت رنگی***************************************


    //******************************************نمایش برای ویرایش مقدار وارد شده ی خصوصیت رنگی***************************
    $("#ProductAttributeItemColorList table tbody").on("click", ".btn-primary", function () {
        $("#Edit-Item-Color").css("display", "inline-block");
        $("#AddedValueValue").val($(this).parent().prev().text().trim());
        $("#AddedValueColor").val($(this).parent().prev().prev().text().trim());
        $("#ProductAttributeItemColorAdd table tbody tr").removeClass("selected");
        $(this).parent().parent().addClass("selected");

    });

    //******************************************ویرایش خصوصیت رنگی**********************************************************
    $("#Edit-Item-Color").click(function () {
        $(".loading").show(); NProgress.start();
        var $this = $(this);
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/ProductAttributes/EditAttributeItemColor/',
            data: '{Color:"' + $("#AddedValueColor").val() + '",OldColor:"' + $("#ProductAttributeItemColorList table tbody .selected td:first").text().trim() + '",Value:"' + $("#AddedValueValue").val() + '",OldValue:"' + $("#ProductAttributeItemColorList table tbody .selected td:nth-child(2)").text().trim() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();
                    $("#ProductAttributeItemColorList table tbody .selected td:first").text($("#AddedValueColor").val());
                    $("#ProductAttributeItemColorList table tbody .selected td:nth-child(2)").text($("#AddedValueValue").val());
                    $("#Edit-Item-Color").css("display", "none");
                    $("#AddedValueColor").val("");
                    $("#AddedValueValue").val("");
                }
                else if (response.statusCode == 501) {

                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "رکورد وارد شده تکراری است !", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });
                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });

    });


    //******************************************حذف خصوصیت رنگی**********************************************************
    $("#ProductAttributeItemColorList table tbody").on("click", ".btn-danger", function () {
        $(".loading").show(); NProgress.start();
        var $this = $(this);
        $.ajax({
            crossDomain: true,
            type: "POST",
            url: '/Admin/ProductAttributes/RemoveAttributeItemColor/',
            data: '{Value:"' + $this.parent().prev().text().trim() + '",Color:"' + $this.parent().prev().prev().text().trim() + '"}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.statusCode == 400) {
                    $(".loading").hide(); NProgress.done();

                    $this.parent().parent().remove();
                }
                else {
                    $(".loading").hide(); NProgress.done();
                    swal({ title: "", text: "خطایی رخ داد.", type: "error", showCancelButton: false, confirmButtonClass: 'btn-error', confirmButtonText: 'بستن' });

                }

            }
        });

    });

});